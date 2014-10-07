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
        private Settlement _settlement;
        private DynamicLabel _population;

        public SettlementScrollableItem(IContext context) : base(context, "settlement_item_panel")
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            var basePostion = new PositionLayout(this, 60,60, HorizontalAlignment.Left, VerticalAlignment.Middle, new Padding(5))
                .Create(new Icon(Context, "settlement_item_icon"));
            basePostion.Right(28, VerticalAlignment.Top, new Padding(1))
                       .Create(_name = new Label(Context, "settlement_item_name", new UniRectangle(), ""));
            basePostion.Right(28, VerticalAlignment.Bottom, new Padding(5,0,0,8), 78)
                       .Create(_population = new DynamicLabel(Context, "settlement_item_population", new UniRectangle(), () => ""));
            _name.Overlay = true;
        }

        public void Set(Settlement item)
        {
            _settlement = item;
            if (item == null)
            {
                _name.Text = "";
                _population.Expression = () => "";
            }
            else
            {
                _name.Text = item.Name;
                _population.Expression = () => item.Population.ToString();
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
