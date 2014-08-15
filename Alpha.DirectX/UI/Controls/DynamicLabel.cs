using System;
using Alpha.DirectX.UI.Coordinates;

namespace Alpha.DirectX.UI.Controls
{
    class DynamicLabel : Label
    {
        public readonly Func<String> Expression; 
        public DynamicLabel(IContext context, string id, UniRectangle coordinates, Func<String> expression ) 
            : base(context, id, coordinates, "")
        {
            Expression = expression;
        }

        public override void Initialize()
        {
            base.Initialize();
            Text = Expression();
        }

        protected override void Update(double delta)
        {
            Text = Expression();
        }
    }
}
