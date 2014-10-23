using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpha.DirectX.UI.Controls
{
    class DynamicTooltip : Tooltip
    {
        public Func<String> Expression { private get; set; } 
        public DynamicTooltip(IContext context, string id, Control associatedControl, double delay) 
            : base(context, id, associatedControl, delay)
        { }

        public override string Text
        {
            set { Expression = () => value; }
        }

        protected override void Update(double delta)
        {
            base.Update(delta);
            if(Visible)
                _text.Content = Expression();
        }
    }
}
