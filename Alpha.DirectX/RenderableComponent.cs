using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX
{
    abstract class RenderableComponent
    {
        public int UpdateOrder { get; protected set; }
        public int DrawOrder { get; protected set; }
        public bool DisplayWireframe { get; private set; }
        public bool BlendingEnabled { get; private set; }
        public bool ZBufferEnabled { get; private set; }
        protected IContext Context { get; private set; }

        protected RenderableComponent(IContext context, 
                                        int updateOrder, 
                                        int drawOrder, 
                                        bool zBufferEnabled = true, 
                                        bool blendingEnabled = false, 
                                        bool displayWireframe = false)
        {
            Context = context;
            UpdateOrder = updateOrder;
            DrawOrder = drawOrder;
            ZBufferEnabled = zBufferEnabled;
            BlendingEnabled = blendingEnabled;
            ZBufferEnabled = zBufferEnabled;
        }

        public abstract void Update(double delta);
        public abstract void Dispose();
        public abstract void Initialize();
        public abstract void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix);
    }
}