using System;
using System.Collections.Generic;
using Alpha.Graphics;
using Alpha.WorldGeneration;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha
{
    class FleetMoveOrder : IDailyUpdatable, IUpdatable
    {
        public class Step
        {
            public VoronoiSite Destination { get; set; }
            public Int32 Duration { get; set; }

            public Step(VoronoiSite destination, Int32 duration)
            {
                Destination = destination;
                Duration = duration;
            }
        }
        public VoronoiSite Destination { get; set; }
        public List<Step> Steps { get; set; } 
        public Fleet Fleet { get; set; }
        public Int32 CurrentStepProgress { get; set; }
        public Int32 ElapsedProgress { get; set; }
        private readonly FleetMoveOrderRenderer _fleetMoveOrderRenderer;
        
        public FleetMoveOrder(IRenderer renderer, Fleet fleet, VoronoiSite destination, List<Step> steps)
        {
            CurrentStepProgress = 0;
            Fleet = fleet;
            Destination = destination;
            Steps = steps;
            _fleetMoveOrderRenderer = new FleetMoveOrderRenderer(renderer, this);
        }

        public void DayUpdate()
        {
            CurrentStepProgress++;
            if (CurrentStepProgress >= Steps[0].Duration)
            {
                Fleet.Location = Steps[0].Destination;
                ElapsedProgress += CurrentStepProgress;
                Steps.RemoveAt(0);
                CurrentStepProgress = 0;
            }
        }

        public void Update(double delta)
        {
            _fleetMoveOrderRenderer.Update(delta);
        }

        public void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _fleetMoveOrderRenderer.Render(deviceContext, viewMatrix, projectionMatrix);
        }
    }
}
