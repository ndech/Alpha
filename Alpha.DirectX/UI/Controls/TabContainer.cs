using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Alpha.DirectX.UI.Coordinates;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.Controls
{
    class TabContainer : Panel
    {
        private const int TabHeight = 30;
        private const int TabTitleSpacing = 4;
        private List<Tab> _tabs;
        private List<TogglableButton> _titleButtons;
        private TogglableButtonGroup _buttonGroup;


        public TabContainer(IContext context, string id) : this(context, id, new UniRectangle(0, 0, 1.0f, 1.0f))
        { }
        public TabContainer(IContext context, string id, UniRectangle coordinates) : base(context, id, coordinates, Color.Silver)
        {
            _tabs = new List<Tab>();
            _titleButtons = new List<TogglableButton>();
        }

        public Tab RegisterTab(Tab tab)
        {
            tab.Coordinates = new UniRectangle(0, TabHeight, 1.0f, new UniScalar(1.0f, -TabHeight));
            Register(tab);
            _tabs.Add(tab);
            TogglableButton button = Register(new TogglableButton(Context, "tab_text", new UniRectangle(), "Data/UI/default.png", "Data/UI/tooltip.png"));
            button.Register(new Label(Context, "tabLabel", new UniRectangle(0, 0, 1.0f, 1.0f), tab.Title) {Overlay = true});
            if(_buttonGroup ==null)
                _buttonGroup = new TogglableButtonGroup(button);
            button.Group = _buttonGroup;
            _titleButtons.Add(button);
            tab.IsVisibleFunc = () => button.CurrentState == TogglableButton.State.Toggled;
            Resize();
            return tab;
        }

        private void CalculateTitlePosition()
        {
            int count = _titleButtons.Count;
            float buttonWidth = (float)(Size.X - TabTitleSpacing*(count + 1))/count;
            float position = TabTitleSpacing;
            for (int i = 0; i < count; i++)
            {
                _titleButtons[i].Coordinates = new UniRectangle(Convert.ToInt32(position), TabTitleSpacing, Convert.ToInt32(buttonWidth), TabHeight - TabTitleSpacing);
                position += buttonWidth + TabTitleSpacing;
            }
        }
        
        public override void OnResize()
        {
            CalculateTitlePosition();
            base.OnResize();
        }
    }
}
