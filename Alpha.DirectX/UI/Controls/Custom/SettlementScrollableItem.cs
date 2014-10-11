using System;
using System.Globalization;
using Alpha.Core.Provinces;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.Layouts;
using Alpha.DirectX.UI.Styles;
using Alpha.Toolkit.Math;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class SettlementScrollableItem : IconButton, IScrollableItem<Settlement>
    {
        private Label _name;
        private Icon _icon;
        private Tooltip _iconTooltip;
        private Tooltip _populationTooltip;
        private Settlement _settlement;
        private DynamicLabel _population;
        private DynamicLabel _income;
        private readonly SettlementDetailPanel _settlementDetailPanel;
        public Action<Settlement> Action { get { return settlement => _settlementDetailPanel.ShowSettlement(settlement); } }

        public SettlementScrollableItem(IContext context, SettlementDetailPanel settlementDetailPanel)
            : base(context, "settlement_item_panel")
        {
            _settlementDetailPanel = settlementDetailPanel;
        }

        public override void Initialize()
        {
            base.Initialize();
            var basePostion = new PositionLayout(this, 60,60, HorizontalAlignment.Left, VerticalAlignment.Middle, new Padding(5))
                .Create(_icon = new Icon(Context, "settlement_item_icon"));
            basePostion.Right(28, VerticalAlignment.Top, new Padding(1))
                       .Create(_name = new Label(Context, "settlement_item_name", new UniRectangle(), ""));
            basePostion.Right(28, VerticalAlignment.Bottom, new Padding(4,0,0,8), 78)
                       .Create(_population = new DynamicLabel(Context, "settlement_item_population", new UniRectangle(), () => ""));
            new PositionLayout(_population, 25, 25, HorizontalAlignment.Left, VerticalAlignment.Middle)
                .Create(new Icon(Context, "population"));
            _population.Overlay = true;
            basePostion.Right(28, VerticalAlignment.Bottom, new Padding(96, 0, 0, 8), 78)
                       .Create(_income = new DynamicLabel(Context, "settlement_item_income", new UniRectangle(), () => ""));
            new PositionLayout(_income, 25, 25, HorizontalAlignment.Left, VerticalAlignment.Middle)
                .Create(new Icon(Context, "income"));
            _income.Overlay = true;
            _name.Overlay = true;
            _iconTooltip = Register(new Tooltip(Context, "tooltip", _icon, .5f, ""));
            _populationTooltip = Register(new Tooltip(Context, "tooltip", _population, 0.5f, ""));
        }

        public void Set(Settlement item)
        {
            _settlement = item;
            if (item == null)
            {
                _name.Text = "";
                _population.Expression = () => "";
                _iconTooltip.Text = "";
                _icon.Visible = false;
            }
            else
            {
                _name.Text = item.Name;
                _population.Expression = () => item.Population.ToString(CultureInfo.InvariantCulture);
                _icon.BaseTexture = Context.TextureManager.Create(item.Type + ".dds", "Data/UI/").TextureResource;
                _iconTooltip.Text = item.Type+ " of " +item.Name;
                _populationTooltip.Text = item.Population + " inhabitants" + Environment.NewLine +
                                          "Growing fast as fuck";
                _icon.Visible = true;
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

        public override void OnMouseReleasedInBounds()
        {
            base.OnMouseReleasedInBounds();
            Action(_settlement);
        }
    }
}
