namespace PlaneSimulator
{
    using System;
    using System.Globalization;
    using Graphics;
    using Toolkit.IO;
    using SharpDX;
    using SharpDX.Direct3D11;
    public class Airplane : RenderableGameComponent, ICsvLoggable
    {
        public World World { get; private set; }
        public State CurrentState { get; private set; }
        public ObjModel Model { get; private set; }
        public AirplanePhysicalModel PhysicalModel { get; private set; }
        public float Altitude { get { return (float) -CurrentState.Position.Z; } }
        public bool IsPlayer { get; set; }

        public Airplane(World world, State state, Game game, Renderer renderer, bool isPlayer)
            : base(game, renderer, 0)
        {
            IsPlayer = isPlayer;
            World = world;
            CurrentState = state;
            PhysicalModel = new AirplanePhysicalModel(this);
            Model = new ObjModel(renderer.DirectX.Device, "Airplane.obj", "Metal.png");
        }

        public override void Update(double delta)
        {
            CurrentState = PhysicalModel.Update(delta, CurrentState);
        }

        public override void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            Model.Render(deviceContext);
            Renderer.LightShader.Render(deviceContext, Model.IndexCount,
                Matrix.RotationY(MathUtil.Pi) * Matrix.Translation(0, Altitude, (float)CurrentState.Position.X), 
                viewMatrix, projectionMatrix, Model.Texture, Renderer.Light, Renderer.Camera);
        }

        public bool IsCrashed()
        {
            return World.Altitude(CurrentState.Position) > Altitude;
        }
        public override void Dispose() { }
        public override String ToString()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "Position : ({0}, {1})\nAltitude : {2}\nSpeed : {3}",
                CurrentState.Position.X, CurrentState.Position.Y, -CurrentState.Position.Z, CurrentState.Speed.Magnitude);
        }
        public string ToCsv()
        {
            return string.Format(CultureInfo.CurrentCulture,"{0};{1};{2};{3}",
                CurrentState.Position.X, CurrentState.Position.Y, -CurrentState.Position.Z, CurrentState.Speed.Magnitude);
        }
    }
}