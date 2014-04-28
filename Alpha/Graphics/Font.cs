using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Alpha.Graphics.Shaders;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Alpha.Graphics
{
    class Font : IDisposable
    {
        public struct Character
        {
            public float uLeft;
            public float uRight;
            public float vTop;
            public float vBottom;
            public int width;
            public int height;
        }

        private Device _device;
        private IRenderer _renderer;
        public Texture Texture { get; set; }

        public SortedList<int, Character> Characters { get; set; }

        public Font(IRenderer renderer, String font, int height)
        {
            _device = renderer.Device;
            _renderer = renderer;
            Texture = new Texture(_device, font + "-" + height + "px.png", @"Data/Fonts/");
            ParseFileData(@"Data/Fonts/"+font+"-"+height+"px.fnt",Texture.Width, Texture.Height);
        }

        private void ParseFileData(string fileName, int textureWidth, int textureHeight)
        {
            using (XmlReader reader = new XmlTextReader(fileName))
            {
                while (reader.Read())
                    if (reader.Name == "chars")
                    {
                        Characters = new SortedList<int, Character>(Int32.Parse(reader.GetAttribute("count") ?? "0"));
                        break;
                    }
                while (reader.Read())
                    if (reader.Name == "char")
                        Characters.Add(Int32.Parse(reader.GetAttribute("id")), 
                            new Character
                            { // x = 0 => left, y = 0 => top
                                height = int.Parse(reader.GetAttribute("height")),
                                width = int.Parse(reader.GetAttribute("width")),
                                uLeft = (float.Parse(reader.GetAttribute("x"))/textureWidth),
                                uRight = ((float.Parse(reader.GetAttribute("x")) + float.Parse(reader.GetAttribute("width"))) / textureWidth),
                                vTop = (float.Parse(reader.GetAttribute("y")) / textureHeight),
                                vBottom = ((float.Parse(reader.GetAttribute("y")) + float.Parse(reader.GetAttribute("height"))) / textureHeight)
                            });
            }
        }

        public Vector2I UpdateVertexArray(string text, ref FontShader.Vertex[] vertices, ref Buffer vertexBuffer, Color defaultColor, List<TexturedRectangle> icons, int positionX = 0, int positionY = 0)
        {
            icons.Clear();
            Vector4 color = defaultColor.ToVector4();
            int width = 0;
            int maxWidth = 0;
            int height = Characters.First().Value.height;
            int maxHeight = height;
            for (int i = 0; i < text.Length; i++)
            {
                char letter = text[i];
                if (letter == '\n')
                {
                    maxWidth = Math.Max(maxWidth, width);
                    width = 0;
                    positionX = 0;
                    positionY += height;
                    maxHeight += height;
                    continue;
                }
                if (letter == '[')
                {
                    if(text[i+1] == '[')
                        continue;
                    String token = text.Substring(i + 1, text.IndexOf(']', i + 1) - (i + 1));
                    if (token == "red")
                        color = Color.Red.ToVector4();
                    else if (token == "yellow")
                        color = Color.Yellow.ToVector4();
                    else if (token == "-")
                        color = defaultColor.ToVector4();
                    else if (token == "gold")
                    {
                        icons.Add(new TexturedRectangle(_renderer, new Vector2I(height, height), _renderer.TextureManager.Create("gold.png", "Data/UI/Icons/")));
                        positionX += height + 1;
                        width += height + 1;
                    }
                    else
                        throw new InvalidOperationException("Unexpected token : " + token);
                    i = text.IndexOf(']', i + 1);
                    continue;
                }
                Character c = Characters[letter];
                vertices[i * 4] = new FontShader.Vertex { position = new Vector3(positionX, positionY, 0.0f), texture = new Vector2(c.uLeft, c.vTop), color = color }; //Top left
                vertices[i * 4 + 1] = new FontShader.Vertex { position = new Vector3(positionX + c.width, positionY + c.height, 0.0f), texture = new Vector2(c.uRight, c.vBottom), color = color }; //Right bottom
                vertices[i * 4 + 2] = new FontShader.Vertex { position = new Vector3(positionX, positionY + c.height, 0.0f), texture = new Vector2(c.uLeft, c.vBottom), color = color }; //Left bottom
                vertices[i * 4 + 3] = new FontShader.Vertex { position = new Vector3(positionX + c.width, positionY, 0.0f), texture = new Vector2(c.uRight, c.vTop), color = color }; //Top right
                
                positionX += c.width + 1;
                width += c.width + 1;
            }
            DataStream mappedResource;
            _device.ImmediateContext.MapSubresource(vertexBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None,
                out mappedResource);
            mappedResource.WriteRange(vertices);
            _device.ImmediateContext.UnmapSubresource(vertexBuffer, 0);
            return  new Vector2I(Math.Max(maxWidth, width), maxHeight);
        }
        
        public void Dispose()
        {
            Texture.Dispose();
        }
    }
}
