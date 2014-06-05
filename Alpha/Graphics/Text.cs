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

    public struct Padding
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;

        public Padding(int padding)
        {
            Left = padding;
            Right = padding;
            Top = padding;
            Bottom = padding;
        }
    }
    class Text : IDisposable
    {
        public VerticalAlignment VerticalAlignment { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        private String _content;
        private Boolean _isEmpty;
        public String Content
        {
            get { return _content; }
            set
            {
                _content = value;
                _isEmpty = (_content.Trim().Length == 0);
                Update();
            } 
        }
        public Color BaseColor { get; set; }
        public Font Font;
        public Vector2I Size;
        private Padding Padding { get; set; }
        private int _numberOfLetters;
        private int SpaceSize {get { return Font.Characters[' '].width; }}
        private const int LineSpacing = 3;

        private Buffer _indexBuffer;
        private Buffer _vertexBuffer;
        private FontShader.Vertex[] _vertices;
        private readonly FontShader _shader;
        private readonly IRenderer _renderer;

        public List<TextLine> Lines { get; set; } 

        public Text(IRenderer renderer, string content, Font font, Vector2I size, Color color, HorizontalAlignment horizontalAligment, VerticalAlignment verticalAlignment, Padding padding)
        {
            Font = font;
            Size = size;
            Padding = padding;
            VerticalAlignment = verticalAlignment;
            HorizontalAlignment = horizontalAligment;
            BaseColor = color;
            _shader = renderer.FontShader;
            _renderer = renderer;
            Content = content;
        }

        private void Update()
        {
            if(_isEmpty) return;
            Lines = SplitInLines();
            CreateIndexBuffer(_renderer);
            CreateVertexBuffer(_renderer);
        }

        private void CreateVertexBuffer(IRenderer renderer)
        {
            _vertices = new FontShader.Vertex[_numberOfLetters * 4];
            int letterIndex = 0;
            Vector4 color = BaseColor.ToVector4();
            int lineHeight = Font.Characters.First().Value.height;
            float spaceSize = SpaceSize;

            float positionY = Padding.Top;
            if (VerticalAlignment == VerticalAlignment.Bottom)
                positionY = Size.Y - (lineHeight * Lines.Count + LineSpacing * (Lines.Count -1)) - Padding.Bottom;
            if (VerticalAlignment == VerticalAlignment.Middle)
                positionY = Padding.Top + (float)(Size.Y - Padding.Bottom - Padding.Top - (lineHeight * Lines.Count + LineSpacing * (Lines.Count - 1))) / 2;

            for(int i = 0; i<Lines.Count; i++)
            {
                TextLine line = Lines[i];
                line.Width -= SpaceSize + 1; //Remove last space and pixel padding after last character
                float positionX = Padding.Left;
                if (HorizontalAlignment == HorizontalAlignment.Right)
                    positionX = Size.X - line.Width - Padding.Right;
                if(HorizontalAlignment == HorizontalAlignment.Center)
                    positionX = Padding.Left + (float)(Size.X - Padding.Left - Padding.Right - line.Width) / 2;
                if (HorizontalAlignment == HorizontalAlignment.Justify && line.WordWrapped)
                    spaceSize = SpaceSize + (float)(Size.X - Padding.Left - Padding.Right - line.Width)/(line.WordCount - 1);
                for (int j = 0; j < line.Words.Count; j++)
                {
                    String word = line.Words[j];
                    for (int k = 0; k < word.Length; k ++)
                    {
                        char letter = word[k];
                        if (letter == '[')
                        {
                            if (word[k + 1] != '[')
                            {
                                String token = word.Substring(k + 1, word.IndexOf(']', k + 1) - (k + 1));
                                if (token == "red")
                                    color = Color.Red.ToVector4();
                                else if (token == "yellow")
                                    color = Color.Yellow.ToVector4();
                                else if (token == "green")
                                    color = Color.Green.ToVector4();
                                else if (token == "blue")
                                    color = Color.Blue.ToVector4();
                                else if (token == "-")
                                    color = BaseColor.ToVector4();
                                else
                                    throw new InvalidOperationException("Unexpected token : " + token);
                                k = word.IndexOf(']', k + 1);
                                continue;
                            }
                            else
                            {
                                k++;
                            }
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
            if(_vertexBuffer != null)
                _vertexBuffer.Dispose();
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
            if(_indexBuffer != null)
                _indexBuffer.Dispose();
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
                if (tempLine.WordCount != 0 && tempLine.Width + wordSize > (Size.X-Padding.Left-Padding.Right))
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
                    else
                        i++;
                }
                width += Font.Characters[c].width + 1;
                letterCount ++;
            }
            return letterCount;
        }

        public void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix orthoMatrix)
        {
            if (_isEmpty) return;
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, Utilities.SizeOf<FontShader.Vertex>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            _shader.Render(deviceContext, _numberOfLetters * 6, worldMatrix, viewMatrix, orthoMatrix, Font.Texture.TextureResource);
        }

        public static String Escape(String text)
        {
            return text.Replace("[", "[[");
        }

        public void Dispose()
        {
            if(_indexBuffer != null) _indexBuffer.Dispose();
            if(_vertexBuffer != null) _vertexBuffer.Dispose();
        }
    }
}
