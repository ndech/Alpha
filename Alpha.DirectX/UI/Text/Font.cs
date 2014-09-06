using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Alpha.DirectX.Shaders;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using Color = SharpDX.Color;

namespace Alpha.DirectX.UI.Text
{
    class Font
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

        public Texture Texture { get; set; }

        public SortedList<int, Character> Characters { get; set; }
        private readonly IContext _context;

        public Font(IContext context, String font, int height)
        {
            Texture = new Texture(context.DirectX.Device, font + "-" + height + "px.png", @"Data/Fonts/");
            ParseFileData(@"Data/Fonts/"+font+"-"+height+"px.fnt",Texture.Width, Texture.Height);
            _context = context;
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

        public Vector2I UpdateVertexArray(string text, ref VertexDefinition.PositionTextureColor[] vertices, ref Buffer vertexBuffer, Color defaultColor, List<TexturedRectangle> icons, int positionX = 0, int positionY = 0)
        {
            icons.Clear();
            Color color = defaultColor;
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
                    if (text[i + 1] == '[')
                        continue;
                    String token = text.Substring(i + 1, text.IndexOf(']', i + 1) - (i + 1));
                    if (!ColorParser.TryParse(token, out color))
                    {
                        if (token == "-")
                            color = defaultColor;
                        else if (token == "gold")
                        {
                            icons.Add(new TexturedRectangle(_context,
                                _context.TextureManager.Create("gold.png", "Data/UI/Icons/"), new Vector2I(height, height)));
                            positionX += height + 1;
                            width += height + 1;
                        }
                        else
                            throw new InvalidOperationException("Unexpected token : " + token);
                    }
                    i = text.IndexOf(']', i + 1);
                    continue;
                }
                Character c = Characters[letter];
                Vector4 colorAsVector = color.ToVector4();
                vertices[i * 4] = new VertexDefinition.PositionTextureColor { position = new Vector3(positionX, positionY, 0.0f), texture = new Vector2(c.uLeft, c.vTop), color = colorAsVector }; //Top left
                vertices[i * 4 + 1] = new VertexDefinition.PositionTextureColor { position = new Vector3(positionX + c.width, positionY + c.height, 0.0f), texture = new Vector2(c.uRight, c.vBottom), color = colorAsVector }; //Right bottom
                vertices[i * 4 + 2] = new VertexDefinition.PositionTextureColor { position = new Vector3(positionX, positionY + c.height, 0.0f), texture = new Vector2(c.uLeft, c.vBottom), color = colorAsVector }; //Left bottom
                vertices[i * 4 + 3] = new VertexDefinition.PositionTextureColor { position = new Vector3(positionX + c.width, positionY, 0.0f), texture = new Vector2(c.uRight, c.vTop), color = colorAsVector }; //Top right
                
                positionX += c.width + 1;
                width += c.width + 1;
            }
            DataStream mappedResource;
            _context.DirectX.Device.ImmediateContext.MapSubresource(vertexBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None,
                out mappedResource);
            mappedResource.WriteRange(vertices);
            _context.DirectX.Device.ImmediateContext.UnmapSubresource(vertexBuffer, 0);
            return  new Vector2I(Math.Max(maxWidth, width), maxHeight);
        }
    }
}
