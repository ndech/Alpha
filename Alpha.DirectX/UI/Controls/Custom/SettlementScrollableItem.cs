using Alpha.Core.Provinces;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.Layouts;
using Alpha.DirectX.UI.Styles;
using Alpha.Toolkit.Math;
using SharpDX;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class SettlementScrollableItem : Panel, IScrollableItem<Settlement>
    {
        private Label _name;
        public SettlementScrollableItem(IContext context) : base(context, "settlement_item", new UniRectangle(), Color.SlateGray)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            new PositionLayout(this, 100, 20, HorizontalAlignment.Center, VerticalAlignment.Middle)
                .Create(_name = new Label(Context, "settlement_item_name", new UniRectangle(), ""));
        }

        public void Set(Settlement item)
        {
            _name.Text = item == null ? "" : item.Name;
        }

        public Vector2I ItemSize()
        {
            return new Vector2I(350, 50);
        }
    }
}
