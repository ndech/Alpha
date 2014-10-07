namespace Alpha.DirectX.UI.Layouts
{
    abstract class Layout
    {
        protected readonly UiComponent ParentComponent;

        public Layout(UiComponent parent)
        {
            ParentComponent = parent;
        }
    }
}
