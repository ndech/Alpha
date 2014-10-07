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
        private readonly Padding _padding;

        public PositionLayout(UiComponent parent, UniScalar width, UniScalar height, HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment, Padding padding = null) : base(parent)
        {
            _width = width;
            _height = height;
            _horizontalAlignment = horizontalAlignment;
            _verticalAlignment = verticalAlignment;
            _padding = padding ?? new Padding(0);
        }

        public PositionLayout Create(Control control)
        {
            UniScalar x, y;
            if (_horizontalAlignment == HorizontalAlignment.Left)
                x = _padding.Left;
            else if(_horizontalAlignment == HorizontalAlignment.Right)
                x = new UniScalar(1.0f) - _width - _padding.Right;
            else
                x = new UniScalar(0.5f) - ((_width - _padding.Left - _padding.Right) / 2 + _padding.Left) ;
            if (_verticalAlignment == VerticalAlignment.Top)
                y = _padding.Top;
            else if (_verticalAlignment == VerticalAlignment.Bottom)
                y = new UniScalar(1.0f) - _height - _padding.Bottom;
            else
                y = new UniScalar(0.5f) - ((_height - _padding.Top - _padding.Bottom)/2 + _padding.Top);

            control.Coordinates = new UniRectangle(x, y, _width, _height);
            ParentComponent.Register(control);
            return this;
        }

        public PositionLayout Right(UniScalar heigth, VerticalAlignment alignment, Padding padding)
        {
            return Right(heigth, alignment, padding, new UniScalar(1.0f, -_padding.Left - _padding.Right) - _width);
        }
        
        public PositionLayout Right(UniScalar heigth, VerticalAlignment alignment, Padding padding, UniScalar width)
        {
            return new PositionLayout(ParentComponent, width, heigth, HorizontalAlignment.Left,
                alignment, new Padding(padding.Left + _padding.Left + _padding.Right + _width.ToOffset(0), padding.Right, padding.Top, padding.Bottom));
        }
    }
}
