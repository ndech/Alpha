using System.Linq;
using Alpha.Core.Provinces;
using Alpha.DirectX.UI.Coordinates;
using Alpha.Toolkit.Math;
using SharpDX;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class ResourceStatisticsWindow : Window
    {
        private ScrollableContainer<ResourceItem, ResourceType> _resourceScrollableContainer;

        class ResourceItem : Panel, IScrollableItem<ResourceType>
        {
            private Label _label;
            public ResourceItem(IContext context) : base(context, "resources_stats_resource_panel", new UniRectangle(), Color.Azure)
            {
            }

            public override void Initialize()
            {
                base.Initialize();
                _label = Register(_label = new Label(Context, Id, new UniRectangle(0, 0, 1.0f, 1.0f), ""));
            }

            public void Set(ResourceType item)
            {
                if (item == null)
                    _label.Text = "";
                else
                    _label.Text = item.Name;
            }
            public override Vector2I Size
            {
                get { return new Vector2I(100, 50); }
            }
        }
        public ResourceStatisticsWindow(IContext context, UniRectangle coordinates) : base(context, "resources_stats_window", coordinates, "Resources statistics")
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            Register(new Label(Context, "resources_stats_label", new UniRectangle(5, 5, 190, 20), "Resources :"));
            _resourceScrollableContainer = new ScrollableContainer<ResourceItem, ResourceType>(Context,
                "ressource_stats_container", 3, (c) => new ResourceItem(c));
            _resourceScrollableContainer.Coordinates = new UniRectangle(5, 30, 200,new UniScalar(1.0f, -35));
            Register(_resourceScrollableContainer);
            _resourceScrollableContainer.Refresh(Context.World.ProvinceManager.ResourceTypes.ToList());
        }

        public override UniVector MinimumSize
        {
            get { return new UniVector(300, 300);}
        }
    }
}
