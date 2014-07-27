using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Graphics;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha
{
    class FleetMoveOrder : MoveOrder, IDailyUpdatable, IUpdatable
    {
        public Province Destination { get { return Steps.Last().Destination; } }
        public List<Step> Steps { get; set; } 
        public Fleet Fleet { get; set; }
        public Int32 CurrentStepProgress { get; set; }
        public Int32 ElapsedProgress { get; set; }
        private readonly FleetMoveOrderRenderer _fleetMoveOrderRenderer;
        
        public FleetMoveOrder(IRenderer renderer, Fleet fleet, List<Step> steps)
        {
            Fleet = fleet;
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
