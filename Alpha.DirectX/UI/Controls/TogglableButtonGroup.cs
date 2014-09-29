namespace Alpha.DirectX.UI.Controls
{
    class TogglableButtonGroup
    {
        public TogglableButton Default { get; private set; }
        public TogglableButton CurrentButton { get; private set; }

        public TogglableButtonGroup(TogglableButton defaultButton)
        {
            Default = defaultButton;
            CurrentButton = defaultButton;
            defaultButton.Toggle();
        }
        public void SetActiveButton(TogglableButton togglableButton)
        {
            CurrentButton.Untoggle();
            CurrentButton = togglableButton;
        }
    }
}
