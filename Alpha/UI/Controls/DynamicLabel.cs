using System;
using Alpha.UI.Coordinates;

namespace Alpha.UI.Controls
{
    class DynamicLabel : Label
    {
        public readonly Func<String> Expression; 
        public DynamicLabel(IGame game, string id, UniRectangle coordinates, Func<String> expression ) 
            : base(game, id, coordinates, "")
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
