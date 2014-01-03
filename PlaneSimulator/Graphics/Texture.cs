using System;
using System.Net.Mime;
using PlaneSimulator.Toolkit;
using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;

namespace PlaneSimulator.Graphics
{
    class Texture : IDisposable
    {
        public ShaderResourceView TextureResource { get; private set; }
        public Texture(Device device, String fileName)
        {
		    TextureResource = ShaderResourceView.FromFile(device, "Data/Textures/"+fileName);
        }

        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(TextureResource);
        }
    }
}
