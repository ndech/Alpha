using System.Linq;
using Alpha.Core.Provinces;
using Alpha.DirectX.UI.Coordinates;
using Alpha.Toolkit.Math;
using SharpDX;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class ResourceStatisticsWindow : Window
    {
        private ResizableScrollableContainer<ResourceItem, ResourceType> _resourceScrollableContainer;

        class ResourceItem : Panel, IScrollableItem<ResourceType>
        {
            private Label _label;
            private Icon _icon;
            public ResourceItem(IContext context) : base(context, "resources_stats_resource_panel", new UniRectangle(), Color.Azure)
            { }

            public override void Initialize()
            {
                base.Initialize();
                _label = Register(new Label(Context, Id, new UniRectangle(50, 0, 150, 1.0f), ""));
                _icon = new Icon(Context, "resource_item_icon");
                _icon.Coordinates = new UniRectangle(0, 0, 50, 50);
                Register(_icon);
            }

            public void Set(ResourceType item)
            {
                if (item == null)
                {
                    _label.Text = "";
                    _icon.BaseTexture = Context.TextureManager.Create("default.png", "Data/UI/").TextureResource;
                }
                else
                {
                    _label.Text = item.Name;
                    _icon.BaseTexture = Context.TextureManager.Create(item.Id + ".dds", "Data/UI/Resources/").TextureResource;
                }
            }
            public override Vector2I Size
            {
                get { return StaticSize; }
            }

            public static Vector2I StaticSize
            {
                get { return new Vector2I(200, 50); }
            }
        }
        public ResourceStatisticsWindow(IContext context, UniRectangle coordinates) : base(context, "resources_stats_window", coordinates, "Resources statistics")
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            Register(new Label(Context, "resources_stats_label", new UniRectangle(5, 5, 190, 20), "Resources :"));
            _resourceScrollableContainer = new ResizableScrollableContainer<ResourceItem, ResourceType>(Context,
                "ressource_stats_container", new UniRectangle(5, 30, 200,new UniScalar(1.0f, -35)), c => new ResourceItem(c), ()=> ResourceItem.StaticSize);
            Register(_resourceScrollableContainer);
            _resourceScrollableContainer.Refresh(Context.World.ProvinceManager.ResourceTypes.OrderBy(t=>t.Name).ToList());
        }

        public override UniVector MinimumSize
        {
            get { return new UniVector(300, 300);}
        }
    }
}
