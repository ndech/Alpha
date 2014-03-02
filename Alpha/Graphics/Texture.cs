using System;
using System.Net.Mime;
using PlaneSimulator.Toolkit;
using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;

namespace PlaneSimulator.Graphics
{
    public class Texture : IDisposable
    {
        public ShaderResourceView TextureResource { get; private set; }
        public Texture(Device device, String fileName, String path = "Data/Textures/")
        {
            TextureResource = ShaderResourceView.FromFile(device, path + fileName);
        }

        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(TextureResource);
        }

        public int Width
        {
            get
            {
                using (var resource = TextureResource.Resource)
                {
                    using (var texture2D = resource.QueryInterface<Texture2D>())
                    {
                        return texture2D.Description.Width;
                    }
                }   
            }
        }

        public int Height
        {
            get
            {
                using (var resource = TextureResource.Resource)
                {
                    using (var texture2D = resource.QueryInterface<Texture2D>())
                    {
                        return texture2D.Description.Height;
                    }
                }
            }
        }
    }
}
