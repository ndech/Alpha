using System.Linq;
using System.Windows.Input;
using Alpha.Core.Provinces;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.Layouts;
using Alpha.DirectX.UI.Styles;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;
using SharpDX;
using HorizontalAlignment = Alpha.DirectX.UI.Styles.HorizontalAlignment;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class ProvinceDetailPanel : Panel
    {
        private LandProvince _province;
        private Label _provinceName;
        private ScrollableContainer<SettlementScrollableItem, Settlement> _settlementScrollableContainer;
        private readonly SettlementDetailPanel _settlementDetailPanel;

        public ProvinceDetailPanel(IContext context, SettlementDetailPanel settlementDetailPanel) 
            : base(context, "province_panel", new UniRectangle(), Color.DarkSlateGray)
        {
            Visible = false;
            _settlementDetailPanel = settlementDetailPanel;
        }

        public override void Initialize()
        {
            base.Initialize();

            new PositionLayout(this, new UniScalar(1.0f,-50), 40, HorizontalAlignment.Center, VerticalAlignment.Top)
                .Create(_provinceName = new Label(Context, "province_name"));
            
            new PositionLayout(this, 20,20, HorizontalAlignment.Right, VerticalAlignment.Top, new Padding(3))
                .Create(new IconButton(Context, "close_button", () => Visible = false));

            TabContainer container = Register(new TabContainer(Context, "province_tabs", new UniRectangle(0,150,1.0f, new UniScalar(1,-150))));
            Tab overiewTab = container.RegisterTab(new Tab(Context, "province_overview_tab", "Overview"));
            Tab economyTab = container.RegisterTab(new Tab(Context, "province_economy_tab", "Economy"));
            Tab politicsTab = container.RegisterTab(new Tab(Context, "province_politics_tab", "Politics"));
            container.SetOffset(250);


            _settlementScrollableContainer =
                new ScrollableContainer<SettlementScrollableItem, Settlement>(Context, "settlements", 4,
                    c => new SettlementScrollableItem(c, _settlementDetailPanel));
            new PositionLayout(this, _settlementScrollableContainer.Size.X, _settlementScrollableContainer.Size.Y, 
                               HorizontalAlignment.Center, VerticalAlignment.Bottom)
                              .Create(_settlementScrollableContainer);
        }

        public void ShowProvince(LandProvince province)
        {
            Visible = true;
            _province = province;
            _provinceName.Text = "Province of " + province.Name;
            _settlementScrollableContainer.Refresh(province.Capital.Yield().ToList());
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
