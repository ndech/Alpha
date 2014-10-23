using System;
using Alpha.Core.Provinces;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.Layouts;
using Alpha.DirectX.UI.Styles;
using Alpha.Toolkit.Math;
using SharpDX;

namespace Alpha.DirectX.UI.Controls.Custom
{
    internal class ResourceScrollableItem : Panel, IScrollableItem<Resource>
    {
        private Icon _backgroundIcon;
        private Icon _icon;
        private Tooltip _iconTooltip;

        public ResourceScrollableItem(IContext context) : base(context,"resource_scrollable_item", new UniRectangle(), Color.Red )
        { }

        public void Set(Resource item)
        {
            if (item == null)
            {
                _icon.BaseTexture = Context.TextureManager.Create("default.png", "Data/UI/").TextureResource;
                _backgroundIcon.BaseTexture = Context.TextureManager.Create("default.png", "Data/UI/").TextureResource;
            }
            else
            {
                _icon.BaseTexture = Context.TextureManager.Create(item.Type.Id + ".dds", "Data/UI/Resources/").TextureResource;
                _backgroundIcon.BaseTexture = Context.TextureManager.Create("level_" + item.Level.Id + ".dds", "Data/UI/Resources/").TextureResource;
                _iconTooltip.Text = item.Level.Name + " resource of " + item.Type.Name.ToLower();
            }
        }
        public override void Initialize()
        {
            base.Initialize();
            new PositionLayout(this, 80, 70, HorizontalAlignment.Left, VerticalAlignment.Middle)
                .Create(_backgroundIcon = new Icon(Context, "resource_item_icon_background"));
            new PositionLayout(this, 54, 54, HorizontalAlignment.Left, VerticalAlignment.Middle, new Padding(8))
                .Create(_icon = new Icon(Context, "resource_item_icon"));
            _iconTooltip = Register(new Tooltip(Context, "tooltip", _backgroundIcon, .5f, ""));
            //new PositionLayout(this, 200, 50, HorizontalAlignment.Center, VerticalAlignment.Middle)
            //    .Create(new )
        }
        public override Vector2I Size
        {
            get { return new Vector2I(400, 70); }
        }
    }
}