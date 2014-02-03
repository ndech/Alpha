using System.Runtime.Remoting.Channels;

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
        public String Name { get; set; }
        public String ModelName { get; set; }

        public Airplane(World world, State state, Game game, Renderer renderer, bool isPlayer, Airplane playerPlane, String name, String modelName)
            : base(game, renderer, isPlayer ? -1000 : 0)
        {
            IsPlayer = isPlayer;
            World = world;
            CurrentState = state;
            Name = name;
            ModelName = modelName;
            PhysicalModel = new AirplanePhysicalModel(this, isPlayer ? game.Input : null);
            Model = new ObjModel(renderer.DirectX.Device, "Airplane.obj", Renderer.TextureManager.Create("Metal.png"));
            if(!isPlayer && ConfigurationManager.Config.DisplayOverlay)
                game.Register(new AirplaneOverlay(game, renderer, this, playerPlane));
        }

        public override void Update(double delta)
        {
            CurrentState = PhysicalModel.Update(delta, CurrentState);
            if (!IsPlayer && IsCrashed())
            {
                
            }
        }

        public override void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            Model.Render(deviceContext);
            Renderer.LightShader.Render(deviceContext, Model.IndexCount,
                Matrix.RotationY(MathUtil.Pi) 
                * Matrix.RotationYawPitchRoll((float)CurrentState.AngularPosition.X, (float)CurrentState.AngularPosition.Y, (float)CurrentState.AngularPosition.Z) 
                * Matrix.Translation((float)CurrentState.Position.Y, Altitude, (float)CurrentState.Position.X), 
                viewMatrix, projectionMatrix, Model.Texture, Renderer.Light, Renderer.Camera);
        }

        public bool IsCrashed()
        {
            return World.Altitude(CurrentState.Position) > Altitude;
        }

        public Vector3 PositionInDxCoordinates()
        {
            return new Vector3((float) CurrentState.Position.Y, Altitude, (float) CurrentState.Position.X);
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