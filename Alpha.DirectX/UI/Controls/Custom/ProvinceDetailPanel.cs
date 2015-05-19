using System.Linq;
using System.Windows.Input;
using Alpha.Core.Provinces;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.Layouts;
using Alpha.DirectX.UI.Styles;
using Alpha.Toolkit;
using SharpDX;
using SharpDX.Direct3D11;
using HorizontalAlignment = Alpha.DirectX.UI.Styles.HorizontalAlignment;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class ProvinceDetailPanel : Panel
    {
        private LandProvince _province;
        private Label _provinceName;
        private ScrollableContainer<SettlementScrollableItem, Settlement> _settlementScrollableContainer;
        private readonly SettlementDetailPanel _settlementDetailPanel;
        private ProvinceMap _provinceMap;

        public ProvinceDetailPanel(IContext context, SettlementDetailPanel settlementDetailPanel) 
            : base(context, "province_panel", new UniRectangle(), Color.DarkSlateGray)
        {
            Visible = false;
            _settlementDetailPanel = settlementDetailPanel;
        }

        public override void Initialize()
        {
            base.Initialize();
            
            TabContainer container = Register(new TabContainer(Context, "province_tabs", new UniRectangle(0,150,1.0f, new UniScalar(1,-150))));
            Tab overiewTab = container.RegisterTab(new Tab(Context, "province_overview_tab", "Overview"));
            Tab economyTab = container.RegisterTab(new Tab(Context, "province_economy_tab", "Economy"));
            Tab politicsTab = container.RegisterTab(new Tab(Context, "province_politics_tab", "Politics"));
            container.SetOffset(256-container.TabTitleSpacing);
            
            new PositionLayout(this, 250, 150 + container.TabHeight - 6, HorizontalAlignment.Left, VerticalAlignment.Top, new Padding(3))
                .Create(_provinceMap = new ProvinceMap(Context))
                .Right(40, VerticalAlignment.Top, new Padding(3))
                .Create(_provinceName = new Label(Context, "province_name"));

            _settlementScrollableContainer =
                new ScrollableContainer<SettlementScrollableItem, Settlement>(Context, "settlements", 4,
                    c => new SettlementScrollableItem(c, _settlementDetailPanel));
            new PositionLayout(overiewTab, _settlementScrollableContainer.Size.X, _settlementScrollableContainer.Size.Y, 
                               HorizontalAlignment.Center, VerticalAlignment.Bottom)
                              .Create(_settlementScrollableContainer);

            new PositionLayout(this, 20, 20, HorizontalAlignment.Right, VerticalAlignment.Top, new Padding(3))
                .Create(new IconButton(Context, "close_button", () => Visible = false));
        }

        public void ShowProvince(LandProvince province)
        {
            Visible = true;
            _province = province;
            _provinceName.Text = "Province of " + province.Name;
            _settlementScrollableContainer.Refresh(province.Capital.Yield().ToList());
            _provinceMap.SelectedProvince = province;
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
