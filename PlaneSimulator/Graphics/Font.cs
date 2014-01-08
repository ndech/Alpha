using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using PlaneSimulator.Graphics.Shaders;
using SharpDX;
using SharpDX.Direct3D11;

namespace PlaneSimulator.Graphics
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
        }

        public Texture Texture { get; set; }

        public SortedList<int, Character> Characters { get; set; }

        public int Height { get; private set; }

        public Font(Device device, String font, int height)
        {
            Texture = new Texture(device, font + "-" + height + "px.png", @"Data/Fonts/");
            ParseFileData(@"Data/Fonts/"+font+"-"+height+"px.fnt",Texture.Width, Texture.Height, 22);
            Height = 22; //Todo to change
        }

        private void ParseFileData(string fileName, int textureWidth, int textureHeight, int textHeight)
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
                                width = int.Parse(reader.GetAttribute("width")),
                                uLeft = (float.Parse(reader.GetAttribute("x"))/textureWidth),
                                uRight = ((float.Parse(reader.GetAttribute("x")) + float.Parse(reader.GetAttribute("width"))) / textureWidth),
                                vTop = (float.Parse(reader.GetAttribute("y")) / textureHeight),
                                vBottom = ((float.Parse(reader.GetAttribute("y")) + textHeight) / textureHeight)
                            });
            }
        }

        public void BuildVertexArray(String text, int positionX, int positionY, ref FontShader.Vertex[] vertices, out int width, out int height)
        {
            width = 0;
            height = Height;
            for (int i = 0; i < text.Length; i++)
            {
                Character c = Characters[text[i]];
                vertices[i * 4] = new FontShader.Vertex { position = new Vector3(positionX, positionY, 0.0f), texture = new Vector2(c.uLeft, c.vTop) }; //Top left
                vertices[i * 4 + 1] = new FontShader.Vertex { position = new Vector3(positionX + c.width, positionY - Height, 0.0f), texture = new Vector2(c.uRight, c.vBottom) }; //Right bottom
                vertices[i * 4 + 2] = new FontShader.Vertex { position = new Vector3(positionX, positionY - Height, 0.0f), texture = new Vector2(c.uLeft, c.vBottom) }; //Left bottom
                vertices[i * 4 + 3] = new FontShader.Vertex { position = new Vector3(positionX + c.width, positionY, 0.0f), texture = new Vector2(c.uRight, c.vTop) }; //Top right
                
                positionX += c.width + 1;
                width += c.width + 1;
            }
        }

        public void Dispose()
        {
            Texture.Dispose();
        }
    }
}
