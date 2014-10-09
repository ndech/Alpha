using System;
using System.Windows;
using Alpha.Core.Provinces;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.Layouts;
using Alpha.DirectX.UI.Styles;
using Alpha.Toolkit.Math;
using SharpDX;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class SettlementScrollableItem : IconButton, IScrollableItem<Settlement>
    {
        private Label _name;
        private Icon _icon;
        private Tooltip _iconTooltip;
        private Settlement _settlement;
        private DynamicLabel _population;

        public SettlementScrollableItem(IContext context) : base(context, "settlement_item_panel")
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            var basePostion = new PositionLayout(this, 60,60, HorizontalAlignment.Left, VerticalAlignment.Middle, new Padding(5))
                .Create(_icon = new Icon(Context, "settlement_item_icon"));
            basePostion.Right(28, VerticalAlignment.Top, new Padding(1))
                       .Create(_name = new Label(Context, "settlement_item_name", new UniRectangle(), ""));
            basePostion.Right(28, VerticalAlignment.Bottom, new Padding(5,0,0,8), 78)
                       .Create(_population = new DynamicLabel(Context, "settlement_item_population", new UniRectangle(), () => ""));
            new PositionLayout(_population, 25, 25, HorizontalAlignment.Left, VerticalAlignment.Middle)
                .Create(new Icon(Context, "population"));
            _name.Overlay = true;
            _iconTooltip = Register(new Tooltip(Context, "tooltip", _icon, .5f, ""));
        }

        public void Set(Settlement item)
        {
            _settlement = item;
            if (item == null)
            {
                _name.Text = "";
                _population.Expression = () => "";
                _iconTooltip.Text = "";
            }
            else
            {
                _name.Text = item.Name;
                _population.Expression = () => item.Population.ToString();
                _icon.BaseTexture = Context.TextureManager.Create(item.Type + ".dds", "Data/UI/").TextureResource;
                _iconTooltip.Text = item.Type;
            }
        }

        public override Vector2I Size
        {
            get { return new Vector2I(420, 70); }
        }

        public override void OnMouseEntered()
        {
            if(_settlement!=null)
                base.OnMouseEntered();
        }
    }
}
