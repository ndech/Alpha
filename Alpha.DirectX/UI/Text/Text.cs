using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.DirectX.Shaders;
using Alpha.DirectX.UI.Styles;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace Alpha.DirectX.UI.Text
{

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
        private VertexDefinition.PositionTextureColor[] _vertices;
        private readonly FontShader _shader;
        private readonly Device _device;

        public List<TextLine> Lines { get; set; } 

        public Text(IContext context, string content, Font font, Vector2I size, Color color, HorizontalAlignment horizontalAligment, VerticalAlignment verticalAlignment, Padding padding)
        {
            Font = font;
            Size = size;
            Padding = padding;
            VerticalAlignment = verticalAlignment;
            HorizontalAlignment = horizontalAligment;
            BaseColor = color;
            _shader = context.Shaders.FontShader;
            _device = context.DirectX.Device;
            Content = content;
        }

        private void Update()
        {
            if(_isEmpty) return;
            Lines = SplitInLines();
            CreateIndexBuffer(_device);
            CreateVertexBuffer(_device);
        }

        private void CreateVertexBuffer(Device device)
        {
            _vertices = new VertexDefinition.PositionTextureColor[_numberOfLetters * 4];
            int letterIndex = 0;
            Color color = BaseColor;
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
                                if (token == "-")
                                    color = BaseColor;
                                else if(!ColorParser.TryParse(token, out color))
                                    throw new InvalidOperationException("Unexpected token : " + token);
                                k = word.IndexOf(']', k + 1);
                                continue;
                            }
                            k++;
                        }
                        Vector4 colorAsVector = color.ToVector4();
                        Font.Character c = Font.Characters[letter];
                        _vertices[letterIndex * 4] = new VertexDefinition.PositionTextureColor { position = new Vector3(positionX, positionY, 0.0f), texture = new Vector2(c.uLeft, c.vTop), color = colorAsVector }; //Top left
                        _vertices[letterIndex * 4 + 1] = new VertexDefinition.PositionTextureColor { position = new Vector3(positionX + c.width, positionY + c.height, 0.0f), texture = new Vector2(c.uRight, c.vBottom), color = colorAsVector }; //Right bottom
                        _vertices[letterIndex * 4 + 2] = new VertexDefinition.PositionTextureColor { position = new Vector3(positionX, positionY + c.height, 0.0f), texture = new Vector2(c.uLeft, c.vBottom), color = colorAsVector }; //Left bottom
                        _vertices[letterIndex * 4 + 3] = new VertexDefinition.PositionTextureColor { position = new Vector3(positionX + c.width, positionY, 0.0f), texture = new Vector2(c.uRight, c.vTop), color = colorAsVector }; //Top right
                        positionX += c.width + 1;
                        letterIndex++;
                    }
                    positionX += spaceSize;
                }
                positionY += lineHeight + LineSpacing;
            }
            if(_vertexBuffer != null)
                _vertexBuffer.Dispose();
            _vertexBuffer = Buffer.Create(device, BindFlags.VertexBuffer, _vertices);
            UsedSize = new Vector2I(Lines.Max(l=>l.Width) + Padding.Left + Padding.Right,
                (lineHeight * Lines.Count + LineSpacing * (Lines.Count - 1)) + Padding.Bottom + Padding.Top);
        }

        private void CreateIndexBuffer(Device device)
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
            _indexBuffer = Buffer.Create(device, BindFlags.IndexBuffer, indices);
        }

        private List<TextLine> SplitInLines()
        {
            _numberOfLetters = 0;
            List<TextLine> lines = new List<TextLine>();
            foreach (var stringLine in Content.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries))
            {
                String[] words = stringLine.Split(new[] { ' ' });
                TextLine tempLine = new TextLine();
                foreach (string word in words)
                {
                    int wordSize;
                    _numberOfLetters += CalculateWordSize(word, out wordSize);
                    if (tempLine.WordCount != 0 && tempLine.Width + wordSize > (Size.X - Padding.Left - Padding.Right))
                    {
                        lines.Add(tempLine);
                        tempLine = new TextLine();
                    }
                    tempLine.Words.Add(word);
                    tempLine.WordCount++;
                    tempLine.Width += wordSize + SpaceSize; // 5 pixel default spacing between words TODO
                }
                tempLine.WordWrapped = false;
                lines.Add(tempLine);
            }
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
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, Utilities.SizeOf<VertexDefinition.PositionTextureColor>(), 0));
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

        public Vector2I UsedSize { get; private set; }
    }
}
