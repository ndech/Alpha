using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Graphics.Shaders;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Color = SharpDX.Color;

namespace Alpha.Graphics
{
    enum VerticalAlignment { Top, Bottom, Middle }

    enum HorizontalAlignment { Left, Right, Center, Justify }

    class TextLine
    {
        public List<String> Words { get; set; }
        public int WordCount { get; set; }
        public int Width { get; set; }
        public bool WordWrapped { get; set; }

        public TextLine()
        {
            Words = new List<string>();
            WordCount = 0;
            Width = 0;
            WordWrapped = true;
        }
    }
    class Text
    {
        public VerticalAlignment VerticalAlignment { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public String Content { get; set; }
        public Color BaseColor { get; set; }
        public Font Font;
        public Vector2I Size;

        private int _numberOfLetters;
        private const int SpaceSize = 3;
        private const int LineSpacing = 3;

        private Buffer _indexBuffer;
        private Buffer _vertexBuffer;
        private FontShader.Vertex[] _vertices;
        private readonly FontShader _shader;

        public List<TextLine> Lines { get; set; } 

        public Text(IRenderer renderer, string content, Font font, Vector2I size, Color color)
        {
            Content = content;
            Font = font;
            Size = size;
            VerticalAlignment = VerticalAlignment.Middle;
            HorizontalAlignment = HorizontalAlignment.Center;
            BaseColor = color;
            _shader = renderer.FontShader;

            Lines = SplitInLines();
            CreateIndexBuffer(renderer);
            CreateVertexBuffer(renderer);
        }

        private void CreateVertexBuffer(IRenderer renderer)
        {
            _vertices = new FontShader.Vertex[_numberOfLetters * 4];
            int letterIndex = 0;
            Vector4 color = BaseColor.ToVector4();
            int lineHeight = Font.Characters.First().Value.height;
            float spaceSize = SpaceSize;

            float positionY = 0;
            if (VerticalAlignment == VerticalAlignment.Bottom)
                positionY = Size.Y - (lineHeight * Lines.Count + LineSpacing * (Lines.Count -1));
            if (VerticalAlignment == VerticalAlignment.Middle)
                positionY = (float)(Size.Y - (lineHeight * Lines.Count + LineSpacing * (Lines.Count - 1))) / 2;

            for(int i = 0; i<Lines.Count; i++)
            {
                TextLine line = Lines[i];
                line.Width -= SpaceSize + 1; //Remove last space and pixel padding after last character
                float positionX = 0;
                if (HorizontalAlignment == HorizontalAlignment.Right)
                    positionX = Size.X - line.Width;
                if(HorizontalAlignment == HorizontalAlignment.Center)
                    positionX = (float)(Size.X - line.Width) / 2;
                if (HorizontalAlignment == HorizontalAlignment.Justify && line.WordWrapped)
                    spaceSize = SpaceSize + (float)(Size.X - line.Width)/(line.WordCount - 1);
                for (int j = 0; j < line.Words.Count; j++)
                {
                    String word = line.Words[j];
                    for (int k = 0; k < word.Length; k ++)
                    {
                        char letter = word[k];
                        if (letter == '[')
                        {
                            if(word[k+1] == '[')
                                continue;
                            String token = word.Substring(k + 1, word.IndexOf(']', k + 1) - (k + 1));
                            if (token == "red")
                                color = Color.Red.ToVector4();
                            else if (token == "yellow")
                                color = Color.Yellow.ToVector4();
                            else if (token == "-")
                                color = BaseColor.ToVector4();
                            else
                                throw new InvalidOperationException("Unexpected token : " + token);
                            k = word.IndexOf(']', k + 1);
                            continue;
                        }
                        Font.Character c = Font.Characters[letter];
                        _vertices[letterIndex * 4] = new FontShader.Vertex { position = new Vector3(positionX, positionY, 0.0f), texture = new Vector2(c.uLeft, c.vTop), color = color }; //Top left
                        _vertices[letterIndex * 4 + 1] = new FontShader.Vertex { position = new Vector3(positionX + c.width, positionY + c.height, 0.0f), texture = new Vector2(c.uRight, c.vBottom), color = color }; //Right bottom
                        _vertices[letterIndex * 4 + 2] = new FontShader.Vertex { position = new Vector3(positionX, positionY + c.height, 0.0f), texture = new Vector2(c.uLeft, c.vBottom), color = color }; //Left bottom
                        _vertices[letterIndex * 4 + 3] = new FontShader.Vertex { position = new Vector3(positionX + c.width, positionY, 0.0f), texture = new Vector2(c.uRight, c.vTop), color = color }; //Top right
                        positionX += c.width + 1;
                        letterIndex++;
                    }
                    positionX += spaceSize;
                }
                positionY += lineHeight + LineSpacing;
            }
            _vertexBuffer = Buffer.Create(renderer.Device, BindFlags.VertexBuffer, _vertices);
        }

        private void CreateIndexBuffer(IRenderer renderer)
        {
            UInt32[] indices = new UInt32[_numberOfLetters * 6]; // 6 indices per character

            for (UInt32 i = 0; i < _numberOfLetters; i++)
            {
                indices[i * 6] = i * 4;
                indices[i * 6 + 1] = i * 4 + 1;
                indices[i * 6 + 2] = i * 4 + 2;
                indices[i * 6 + 3] = i * 4;
                indices[i * 6 + 4] = i * 4 + 3;
                indices[i * 6 + 5] = i * 4 + 1;
            }

            _indexBuffer = Buffer.Create(renderer.Device, BindFlags.IndexBuffer, indices);
        }

        private List<TextLine> SplitInLines()
        {
            _numberOfLetters = 0;
            List<TextLine> lines = new List<TextLine>();
            String[] words = Content.Split(new[] {' '});

            TextLine tempLine = new TextLine();
            foreach (string word in words)
            {
                int wordSize;
                _numberOfLetters += CalculateWordSize(word, out wordSize);
                if (tempLine.WordCount != 0 && tempLine.Width + wordSize > Size.X)
                {
                    lines.Add(tempLine);
                    tempLine = new TextLine();
                }
                tempLine.Words.Add(word);
                tempLine.WordCount ++;
                tempLine.Width += wordSize + SpaceSize; // 5 pixel default spacing between words TODO
            }
            tempLine.WordWrapped = false;
            lines.Add(tempLine);
            return lines;
        }

        private int CalculateWordSize(String word, out int width)
        {
            int letterCount = 0;
            width = 0;
            for(int i = 0; i < word.Length; i ++)
            {
                char c = word[i];
                if (c == '[')
                {
                    if (word[i + 1] != '[')
                    {
                        while (word[i] != ']')
                            i++;
                        continue;
                    }
                }
                width += Font.Characters[c].width + 1;
                letterCount ++;
            }
            return letterCount;
        }

        public void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix orthoMatrix)
        {
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, Utilities.SizeOf<FontShader.Vertex>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            _shader.Render(deviceContext, _numberOfLetters * 6, worldMatrix, viewMatrix, orthoMatrix, Font.Texture.TextureResource);
        }
    }
}
