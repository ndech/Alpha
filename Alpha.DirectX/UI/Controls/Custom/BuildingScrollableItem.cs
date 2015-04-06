using System;
using Alpha.Core.Buildings;
using Alpha.Toolkit.Math;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class BuildingScrollableItem : IconButton, IScrollableItem<Building>
    {
        public BuildingScrollableItem(IContext context, string id) : base(context, id)
        {
        }

        public void Set(Building item)
        {

        }

        public Action<Building> Action { get; private set; }

        public override Vector2I Size
        {
            get { return new Vector2I(300, 50); }
        }
    }
}
