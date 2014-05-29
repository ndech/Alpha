using System;
using Alpha.UI.Coordinates;
using Alpha.UI.Styles;
using SharpDX;

namespace Alpha.UI.Controls.Custom
{
    class AccountLine : Label
    {
        private Label _valueLabel;
        private readonly Func<double> _valueCallback;
        private readonly ValueType _valueType;

        private Color _positiveColor;
        private Color _negativeColor;

        public enum ValueType
        {
            Income,
            Spending,
            Revenue
        }
        public AccountLine(IGame game, string id, UniRectangle coordinates, string name, Func<double> value, ValueType valueType = ValueType.Income) : 
            base(game, id+"_account", coordinates, name + " :")
        {
            _valueCallback = value;
            _valueType = valueType;
        }

        public override void Initialize()
        {
            base.Initialize();
            _positiveColor = UiManager.StyleManager.GetStyleItem<TextColorStyleItem>(ComponentType, "account_value_income").TextColor;
            _negativeColor = UiManager.StyleManager.GetStyleItem<TextColorStyleItem>(ComponentType, "account_value_spending").TextColor;
            _valueLabel = Register(new Label(Game, "account_value_" + _valueType.ToString().ToLower(), new UniRectangle(0,0,1.0f,1.0f), ""));
            _valueLabel.Text = GenerateValue();
        }

        protected override void Update(double delta)
        {
            _valueLabel.Text = GenerateValue();
        }

        private String GenerateValue()
        {
            double value = _valueCallback.Invoke();
            if (_valueType == ValueType.Revenue)
                _valueLabel.TextColor = value < 0 ? _negativeColor : _positiveColor;
            return value.ToString("N2");
        }
    }
}
