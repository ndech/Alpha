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
        public bool Visible { get; set; }
        private LandProvince _province;
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
            _settlementScrollableContainer =
                new ScrollableContainer<SettlementScrollableItem, Settlement>(Context, "settlements", 3,
                    c => new SettlementScrollableItem(c));
            new PositionLayout(this, _settlementScrollableContainer.Size.X, _settlementScrollableContainer.Size.Y, HorizontalAlignment.Center, VerticalAlignment.Middle)
                .Create(_settlementScrollableContainer);
            Button newSettlementButton = new Button(Context, "new_settlement", new UniRectangle(0.2f,_settlementScrollableContainer.Position.Y + 40, 0.6f, 40), "Found settlement" );
            Register(newSettlementButton);
            newSettlementButton.Clicked += b => Context.RegisterCommand(new FoundSettlementCommand(_province));

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
