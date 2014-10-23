using System.Linq;
using System.Windows.Input;
using Alpha.Core.Provinces;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.Layouts;
using Alpha.DirectX.UI.Styles;
using SharpDX;
using HorizontalAlignment = Alpha.DirectX.UI.Styles.HorizontalAlignment;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class SettlementDetailPanel : Panel
    {
        private Settlement _settlement;
        private Label _name;
        private ScrollableContainer<ResourceScrollableItem, Resource> _resourceScrollableContainer; 
        public SettlementDetailPanel(IContext context)
            : base(context, "settlement_panel", new UniRectangle(), Color.LightSlateGray)
        {
            Visible = false;
        }

        public override void Initialize()
        {
            base.Initialize();
            new PositionLayout(this, new UniScalar(1.0f, -50), 40, HorizontalAlignment.Center, VerticalAlignment.Top)
                .Create(_name = new Label(Context, "province_name", new UniRectangle(), ""));
            IconButton closeButton;
            new PositionLayout(this, 20, 20, HorizontalAlignment.Right, VerticalAlignment.Top, new Padding(3))
                .Create(closeButton = new IconButton(Context, "close_button"));

            _resourceScrollableContainer =
                new ScrollableContainer<ResourceScrollableItem, Resource>(Context, "settlements", 4,
                    c => new ResourceScrollableItem(c));
            new PositionLayout(this, _resourceScrollableContainer.Size.X, _resourceScrollableContainer.Size.Y,
                               HorizontalAlignment.Center, VerticalAlignment.Middle)
                              .Create(_resourceScrollableContainer);
            closeButton.Clicked += () => Visible = false;
        }

        public void ShowSettlement(Settlement settlement)
        {
            if(settlement == null) return;
            Visible = true;
            _settlement = settlement;
            _name.Text = settlement.Name;
            _resourceScrollableContainer.Refresh(settlement.Resources.OrderByDescending(r=>r.Level.Value).ToList());
        }

        protected override bool OnKeyPressed(Key key, char? character, bool repeat)
        {
            if (!Visible) return false;
            if (key == Key.Delete)
            {
                Visible = false;
                return false;
            }
            if (key == Key.Escape)
            {
                Visible = false;
                return true;
            }
            return false;
        }
    }
}
