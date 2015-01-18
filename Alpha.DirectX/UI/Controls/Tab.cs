using System;
using Alpha.DirectX.UI.Coordinates;
using SharpDX;

namespace Alpha.DirectX.UI.Controls
{
    internal class Tab : Panel
    {
        public String Title { get; set; }

        public Tab(IContext context, string id, string title) : base(context, id, new UniRectangle(), Color.SkyBlue)
        {
            Title = title;
        }

        public Func<bool> IsVisibleFunc { private get; set; } 

        public override bool IsVisible()
        {
            return IsVisibleFunc != null && IsVisibleFunc();
        }
    }
}
