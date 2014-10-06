using System.Linq;
using System.Windows.Input;
using Alpha.Core.Provinces;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.Layouts;
using Alpha.DirectX.UI.Styles;
using SharpDX;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class ProvinceDetailPanel : Panel
    {
        public bool Visible { get; set; }
        private Label _name;
        private ScrollableContainer<SettlementScrollableItem, Settlement> _settlementScrollableContainer;
        public override bool IsVisible()
        {
            return Visible;
        }

        public ProvinceDetailPanel(IContext context) : base(context, "province_panel", new UniRectangle(), Color.OrangeRed)
        {
            Visible = false;
        }

        public override void Initialize()
        {
            base.Initialize();
            new PositionLayout(this, new UniScalar(1.0f,-50), 40, HorizontalAlignment.Center, VerticalAlignment.Top)
                .Create(_name = new Label(Context, "province_name", new UniRectangle(),""));
            IconButton closeButton;
            new PositionLayout(this, 20,20, HorizontalAlignment.Right, VerticalAlignment.Top)
                .Create(closeButton = new IconButton(Context, "close_button"));
            closeButton.Clicked += () => Visible = false;
            new PositionLayout(this, 0.8f, 0.5f, HorizontalAlignment.Center, VerticalAlignment.Middle)
                .Create(_settlementScrollableContainer = 
                    new ScrollableContainer<SettlementScrollableItem, Settlement>(Context, "settlements", 3, c=>new SettlementScrollableItem(c)));
        }

        public void ShowProvince(LandProvince province)
        {
            Visible = true;
            _name.Text = province.Id;
            _settlementScrollableContainer.Refresh(province.Settlements.ToList());
        }

        protected override bool OnKeyPressed(Key key, char? character, bool repeat)
        {
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
