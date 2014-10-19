using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Alpha.Core.Fleets;
using Alpha.Core.Provinces;
using Alpha.DirectX.Shaders;
using Alpha.DirectX.UI.Styles;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.World
{
    class FleetRenderer
    {
        class FleetRenderingInfo
        {
            public readonly Matrix WorldMatrix;
            public readonly Vector3 WorldPosition;
            public Int32 ShipCount;
            public readonly Text.Text Text;
            public readonly List<Fleet> Fleets;
            public Status CurrentStatus;

            public Int32 FleetCount { get { return Fleets.Count; } }

            public enum Status
            {
                Ally,
                Neutral,
                Mine,
                Enemy
            }

            public FleetRenderingInfo(IContext context, Fleet fleet, Vector2I size)
            {
                WorldPosition = (Vector3)fleet.Location.Center;
                WorldMatrix = Matrix.RotationY(-(float) (Math.PI/2))*Matrix.Translation(WorldPosition);
                ShipCount = fleet.ShipCount;
                Fleets = new List<Fleet> { fleet };
                if(fleet.Owner.Equals(context.Realm))
                    CurrentStatus = Status.Mine;
                else
                    CurrentStatus = new List<Status> {Status.Ally, Status.Enemy, Status.Neutral}.RandomItem();
                Text = context.TextManager.Create("Courrier", 14, RandomGenerator.Get(1,2000).ToString(CultureInfo.InvariantCulture), 
                    new Vector2I(size.X-8, size.Y), Color.Wheat, HorizontalAlignment.Center, VerticalAlignment.Middle, new Padding(2));
            }

            public Matrix ProjectedPosition(IContext context, Vector3 offset3D, Vector3 offset2D = new Vector3())
            {
                return Matrix.Translation(Vector3.Project(WorldPosition + offset3D, 0, 0,
                    context.ScreenSize.X, context.ScreenSize.Y, 0.0f, 1.0f,
                    -context.Camera.ViewMatrix * context.DirectX.ProjectionMatrix)
                    -new Vector3(context.ScreenSize.X, context.ScreenSize.Y, 0)/2
                    +offset2D);
            }
        }

        private readonly IContext _context;
        private readonly ObjModel _model;
        private readonly LightShader _shader;
        private readonly Dictionary<Zone, FleetRenderingInfo> _fleetRenderingInfos = new Dictionary<Zone, FleetRenderingInfo>();
        private readonly TexturedRectangle _baseOverlay;
        private readonly TexturedRectangle _enemySubOverlay;
        private readonly TexturedRectangle _mineSubOverlay;
        private readonly TexturedRectangle _allySubOverlay;
        private readonly TexturedRectangle _neutralSubOverlay;
        public FleetRenderer(IContext context)
        {
            _context = context;
            _shader = context.Shaders.LightShader;
            _model = new ObjModel(context.DirectX.Device, "BasicBoat.obj", context.TextureManager.Create("Metal.png"));
            _baseOverlay = new TexturedRectangle(context, context.TextureManager.Create("fleet_overlay.dds", "Data/UI/"));
            _enemySubOverlay = new TexturedRectangle(context, context.TextureManager.Create("fleet_overlay_relation_enemy.dds", "Data/UI/"));
            _allySubOverlay = new TexturedRectangle(context, context.TextureManager.Create("fleet_overlay_relation_ally.dds", "Data/UI/"));
            _neutralSubOverlay = new TexturedRectangle(context, context.TextureManager.Create("fleet_overlay_relation_neutral.dds", "Data/UI/"));
            _mineSubOverlay = new TexturedRectangle(context, context.TextureManager.Create("fleet_overlay_relation_mine.dds", "Data/UI/"));
            foreach (Fleet fleet in context.World.FleetManager.Fleets)
                OnNewFleet(context, fleet);
            context.NotificationResolver.FleetMoved += f => OnFleetUpdate(context, f);
        }

        private void OnNewFleet(IContext context, Fleet fleet)
        {
            if(_fleetRenderingInfos.ContainsKey(fleet.Location))
                _fleetRenderingInfos[fleet.Location].Fleets.Add(fleet);
            else
                _fleetRenderingInfos[fleet.Location] = new FleetRenderingInfo(context, fleet, _baseOverlay.Size);
        }

        private void OnFleetUpdate(IContext context, Fleet fleet)
        {
            OnFleetDelete(fleet);
            OnNewFleet(context, fleet);
        }

        private void OnFleetDelete(Fleet fleet)
        {
            var info = _fleetRenderingInfos.Single(kvp => kvp.Value.Fleets.Contains(fleet));
            if (info.Value.FleetCount == 1)
                _fleetRenderingInfos.Remove(info.Key);
            else info.Value.Fleets.Remove(fleet);
        }
        
        public void Render3D(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix, Light light, ICamera camera)
        {
            foreach (FleetRenderingInfo info in _fleetRenderingInfos.Values)
            {
                _model.Render(deviceContext);
                _shader.Render(deviceContext, 
                    _model.IndexCount, 
                    info.WorldMatrix, 
                    viewMatrix, 
                    projectionMatrix, 
                    _model.Texture, 
                    light, 
                    camera);
            }
        }

        public void RenderOverlay(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            foreach (FleetRenderingInfo info in _fleetRenderingInfos.Values)
            {
                //if(Vector3.Distance(info.WorldPosition, _context.Camera.Position)>1500) 
                //    return;
                _baseOverlay.Render(deviceContext, 
                    info.ProjectedPosition(_context, new Vector3(0,20,0), new Vector3(-(float)(_baseOverlay.Size.X)/2,-10,0)),
                    viewMatrix, projectionMatrix);
                GetSubOverlay(info.CurrentStatus).Render(deviceContext, 
                    info.ProjectedPosition(_context, new Vector3(0, 20, 0), new Vector3(-(float)(_baseOverlay.Size.X) / 2 +3, -3, 0))
                    , viewMatrix, projectionMatrix);
                info.Text.Render(deviceContext,
                    info.ProjectedPosition(_context, new Vector3(0, 20, 0), new Vector3(-(float) (_baseOverlay.Size.X)/2, -10, 0)), 
                    viewMatrix, projectionMatrix);
            }
        }

        private TexturedRectangle GetSubOverlay(FleetRenderingInfo.Status status)
        {
            if (status == FleetRenderingInfo.Status.Mine)
                return _mineSubOverlay;
            if (status == FleetRenderingInfo.Status.Neutral)
                return _neutralSubOverlay;
            if (status == FleetRenderingInfo.Status.Enemy)
                return _enemySubOverlay;
            return _allySubOverlay;
        }
    }
}
