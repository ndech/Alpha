using System;

namespace Alpha.DirectX.UI.Controls
{
    class DynamicTooltip : Tooltip
    {
        public Func<string> Expression { private get; set; } 
        public DynamicTooltip(IContext context, string id, Control associatedControl, double delay) 
            : base(context, id, associatedControl, delay)
        { }

        protected override void Update(double delta)
        {
            base.Update(delta);
            if(Visible && Expression!=null)
                Text = Expression();
        }
    }
}
