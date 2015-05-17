using System.Collections.Generic;
using System.Linq;
using Alpha.DirectX.UI.Controls;
using Alpha.DirectX.UI.Coordinates;

namespace Alpha.DirectX.UI.Layouts
{
    class HorizontalLayout
    {
        private class LayoutItem
        {
            public Control Control { get; }
            public int Width { get; }

            public LayoutItem(Control control, int width)
            {
                Control = control;
                Width = width;
            }
        }
        private readonly UniScalar _verticalPosition;
        private readonly UniScalar _height;
        private readonly int _leftOffset;
        private readonly int _rightOffset;
        private readonly List<LayoutItem> _controls = new List<LayoutItem>(); 
        public Control Parent { get; }

        public HorizontalLayout(Control parent, UniScalar verticalPosition, UniScalar height, int leftOffset = 0, int rightOffset = 0)
        {
            _verticalPosition = verticalPosition;
            _height = height;
            _leftOffset = leftOffset;
            _rightOffset = rightOffset;
            Parent = parent;
        }

        public HorizontalLayout AddControl(Control item, int width)
        {
            _controls.Add(new LayoutItem(item, width));
            Parent.Register(item);
            return this;
        }

        public void Create()
        {
            int count = _controls.Count;
            int controlsWidth = _controls.Sum(c => c.Width);
            int totalFixedWidth = controlsWidth + _leftOffset + _rightOffset;
            UniScalar blank = new UniScalar(1.0f/(count-1), -totalFixedWidth/(count-1));
            UniScalar currentPosition = new UniScalar(_leftOffset);
            foreach (LayoutItem layoutItem in _controls)
            {
                layoutItem.Control.Coordinates = new UniRectangle(currentPosition, _verticalPosition, layoutItem.Width, _height);
                currentPosition += layoutItem.Width + blank;
                Parent.Register(layoutItem.Control);
            }
        }
    }
}
