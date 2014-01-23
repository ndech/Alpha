using System.Linq;
using PlaneSimulator.Graphics;
using PlaneSimulator.Toolkit.IO;
using SharpDX;
using SharpDX.Direct3D11;

namespace PlaneSimulator
{
    using System;
    using System.Collections.Generic;
    using Toolkit.Math;
    using System.Globalization;
    public class Airplane : RenderableGameComponent, ICsvLoggable
    {
        public World World { get; private set; }
        public State CurrentState { get; private set; }
        public ObjModel Model { get; private set; }
        public AirplanePhysicalModel PhysicalModel { get; private set; }
        public float Altitude { get { return (float) -CurrentState.Position.Z; } }

        public Airplane(World world)
            : base()
        {
            World = world;
        }
        public override void Update(double delta)
        {
            CurrentState = PhysicalModel.Update(delta, CurrentState);
        }

        public override void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {

        }

        public void Initialize(State state)
        {
            CurrentState = state;
        }

        public override void Dispose() { }

        public bool IsCrashed()
        {
            return Altitude < 0;
        }

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