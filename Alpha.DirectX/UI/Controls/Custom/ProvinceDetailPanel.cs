using System.Linq;
using System.Windows.Input;
using Alpha.Core.Commands;
using Alpha.Core.Provinces;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.Layouts;
using Alpha.DirectX.UI.Styles;
using SharpDX;
using HorizontalAlignment = Alpha.DirectX.UI.Styles.HorizontalAlignment;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class ProvinceDetailPanel : Panel
    {
        private LandProvince _province;
        private Label _name;
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
                .Create(_name = new Label(Context, "province_name", new UniRectangle(),""));
            IconButton closeButton;
            new PositionLayout(this, 20,20, HorizontalAlignment.Right, VerticalAlignment.Top, new Padding(3))
                .Create(closeButton = new IconButton(Context, "close_button"));
            closeButton.Clicked += () => Visible = false;
            _settlementScrollableContainer =
                new ScrollableContainer<SettlementScrollableItem, Settlement>(Context, "settlements", 4,
                    c => new SettlementScrollableItem(c, _settlementDetailPanel));
            new PositionLayout(this, _settlementScrollableContainer.Size.X, _settlementScrollableContainer.Size.Y, 
                               HorizontalAlignment.Center, VerticalAlignment.Middle)
                              .Create(_settlementScrollableContainer);
            Button newSettlementButton = new Button(Context, "new_settlement", 
                new UniRectangle(0.2f,_settlementScrollableContainer.RelativePosition.Y + _settlementScrollableContainer.Size.Y + 40, 0.6f, 40), 
                "Create new settlement" );
            Register(newSettlementButton);
            newSettlementButton.Clicked += b => Context.RegisterCommand(new FoundSettlementCommand(_province, _province.AvailableSettlementTypes.First()));

            Context.NotificationResolver.NewSettlement +=
                s =>
                {
                    if (Visible && s.Province == _province) ShowProvince(_province);
                    _settlementScrollableContainer.ShowLast();
                };
        }

        public void ShowProvince(LandProvince province)
        {
            Visible = true;
            _province = province;
            _name.Text = province.Name;
            _settlementScrollableContainer.Refresh(province.Settlements.OrderByDescending(s=>s.Population).ToList());
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
