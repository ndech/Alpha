﻿using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.DirectX.UI.Coordinates;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.Controls
{
    class ScrollableContainer<T, T1> : Panel where T : Control, IScrollableItem<T1>
    {
        private readonly int _numberOfVisibleItems;
        private ScrollBar _scrollBar;
        private int _firstVisibleItem;

        private int FirstVisibleItem
        {
            get { return _firstVisibleItem; }
            set
            {
                _firstVisibleItem = Math.Max(0, Math.Min(value, _content.Count - _numberOfVisibleItems));
                Refresh();
            }
        }

        private readonly T[] _items;
        private List<T1> _content; 
        public ScrollableContainer(IContext context, string id, int numberOfVisibleItems, Func<IContext, T> generator) 
            : base(context, id, new UniRectangle(), Color.Black)
        {
            _numberOfVisibleItems = numberOfVisibleItems;
            _items = new T[_numberOfVisibleItems];
            for (int i = 0; i < _numberOfVisibleItems; i ++)
            {
                _items[i] = generator(Context);
                _items[i].Coordinates = new UniRectangle(0, i * _items[i].ItemSize().Y, _items[i].ItemSize().X, _items[i].ItemSize().Y);
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            foreach (T item in _items)
                Register(item);
            Coordinates = new UniRectangle(Coordinates.Position.X, Coordinates.Position.Y,
                _items[0].ItemSize().X + ScrollBar.Width, _numberOfVisibleItems * _items[0].ItemSize().Y);
            Register(_scrollBar = new ScrollBar(Context));
            _scrollBar.Moved += (pos) =>
            {
                FirstVisibleItem += pos;
            };
        }

        public void Refresh(List<T1> content)
        {
            _content = content;
            FirstVisibleItem = 0;
        }

        public void Refresh()
        {
            _scrollBar.Refresh(_content.Count(),_numberOfVisibleItems, FirstVisibleItem);
            for (int i = 0; i < _numberOfVisibleItems; i++)
                _items[i].Set(i+FirstVisibleItem < _content.Count() ? _content[i+FirstVisibleItem] : default(T1));
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        { }

        public override bool OnMouseScrolled(int delta)
        {
            FirstVisibleItem -= delta;
            return true;
        }
    }
}