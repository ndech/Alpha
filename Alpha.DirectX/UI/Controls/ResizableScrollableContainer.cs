using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.DirectX.UI.Coordinates;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.Controls
{
    class ResizableScrollableContainer<T, T1> : Panel where T : Control, IScrollableItem<T1>
    {
        private readonly Func<IContext, T> _generator;
        private readonly Func<Vector2I> _itemSize;

        private float _numberOfVisibleItems;
        private ScrollBar _scrollBar;

        private float _currentPosition = 0;

        private float CurrentPosition
        {
            get { return _currentPosition; }
            set
            {
                _currentPosition = Math.Max(0.0f, Math.Min(value, _data.Count - _numberOfVisibleItems));
                CalculateItemPositions();
                Refresh();
            }
        }

        private List<T> _items;
        private List<T1> _data;

        public ResizableScrollableContainer(IContext context, string id, UniRectangle coordinates, Func<IContext, T> generator, Func<Vector2I> itemSize) 
            : base(context, id, coordinates, Color.Tomato)
        {
            _generator = generator;
            _itemSize = itemSize;
        }

        public override void Initialize()
        {
            base.Initialize();
            Update();
        }

        public void Update()
        {
            _numberOfVisibleItems = (float)Size.Y / _itemSize().Y;

            _items = new List<T>();

            for (int i = 0; i < _numberOfVisibleItems+2; i++)
                _items.Add(_generator(Context));
            CalculateItemPositions();

            foreach (T item in _items)
                Register(item);
            Register(_scrollBar = new ScrollBar(Context));
            _scrollBar.Moved += pos =>
            {
                CurrentPosition += pos*0.1f;
            };
        }

        public void Refresh(List<T1> data)
        {
            _data = data;
            Refresh();
        }

        public void CalculateItemPositions()
        {
            for (int i = 0; i < _numberOfVisibleItems + 2; i++)
                _items[i].Coordinates = new UniRectangle(0, (int)((-(CurrentPosition % 1) + i) * _itemSize().Y), _itemSize().X, _itemSize().Y);
        }

        public void Refresh()
        {
            if(_data == null)
                return;
            _scrollBar.Refresh(_data.Count(), _numberOfVisibleItems, CurrentPosition);
            for (int i = 0; i < _numberOfVisibleItems+2; i++)
                _items[i].Set(i + (int)CurrentPosition < _data.Count() ? _data[i + (int)CurrentPosition] : default(T1));
            _scrollBar.Visible = !(_data.Count <= _numberOfVisibleItems);
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        { }

        internal override void RenderTree(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            deviceContext.Rasterizer.SetScissorRectangle(Position.X, Position.Y, Position.X + Size.X, Position.Y + Size.Y);
            base.RenderTree(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
            deviceContext.Rasterizer.SetScissorRectangle(0, 0, Context.ScreenSize.X, Context.ScreenSize.Y);
        }

        public override bool OnMouseScrolled(int delta)
        {
            CurrentPosition -= delta * 0.1f;
            return true;
        }
    }
}
