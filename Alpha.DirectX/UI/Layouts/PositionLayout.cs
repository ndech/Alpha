using Alpha.DirectX.UI.Controls;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.Styles;

namespace Alpha.DirectX.UI.Layouts
{
    class PositionLayout : Layout
    {
        private readonly UniScalar _width;
        private readonly UniScalar _height;
        private readonly HorizontalAlignment _horizontalAlignment;
        private readonly VerticalAlignment _verticalAlignment;

        public PositionLayout(UiComponent parent, UniScalar width, UniScalar height, HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment) : base(parent)
        {
            _width = width;
            _height = height;
            _horizontalAlignment = horizontalAlignment;
            _verticalAlignment = verticalAlignment;
        }

        public void Create(Control control)
        {
            UniScalar x, y;
            if (_horizontalAlignment == HorizontalAlignment.Left)
                x = 0;
            else if(_horizontalAlignment == HorizontalAlignment.Right)
                x = new UniScalar(1.0f) - _width;
            else
                x = new UniScalar(0.5f) - _width / 2;
            if (_verticalAlignment == VerticalAlignment.Top)
                y = 0;
            else if(_verticalAlignment == VerticalAlignment.Bottom)
                y = new UniScalar(1.0f) -_height;
            else
                y = new UniScalar(0.5f) - _height/2;

            control.Coordinates = new UniRectangle(x, y, _width, _height);
            ParentComponent.Register(control);
        }
    }
}
