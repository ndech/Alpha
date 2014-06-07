using System;
using System.Collections.Generic;
using Alpha.UI.Controls;
using Alpha.UI.Controls.Custom;
using Alpha.UI.Coordinates;
using SharpDX;

namespace Alpha.UI.Scrollable
{
    class ScrollableConsoleContainer : Panel, IVerticalScrollableContainer<ScrollableConsoleItem, ConsoleLine>
    {
        private readonly List<ConsoleLine> _items;
        private readonly IList<ScrollableConsoleItem> _renderingItems;
        private ScrollBar _scrollBar;
        private int _lastVisible;
        public ScrollableConsoleContainer(IGame game, UniRectangle coordinates, List<ConsoleLine> items ) 
            : base(game, "console_scrollable_panel", coordinates, Color.Transparent)
        {
            _items = items;
            _renderingItems = new List<ScrollableConsoleItem>();
        }

        public override void Initialize()
        {
            base.Initialize();
            int size = Size.Y;
            for (int i = size % ScrollableConsoleItem.Height / 2 + ScrollableConsoleItem.Height; i < size; i += ScrollableConsoleItem.Height)
                _renderingItems.Add(Register(new ScrollableConsoleItem(Game, new UniVector(0,size-i))));
            Register(_scrollBar = new ScrollBar(Game, "console_scrollbar"));
            _scrollBar.Moved += delta => OnMouseScrolled(delta);
            _lastVisible = _items.Count;
            Refresh();
        }
        
        public void Refresh()
        {
            for (int i = 0; i < _renderingItems.Count; i++)
            {
                if (i < _lastVisible)
                    _renderingItems[i].Set(_items[_lastVisible - 1 - i]);
                else
                    _renderingItems[i].Set(null);
            }
            _scrollBar.Refresh(_items.Count, _renderingItems.Count, _lastVisible-_renderingItems.Count);
        }

        protected override bool OnMouseScrolled(int delta)
        {
            if (_renderingItems.Count >= _items.Count)
                return false;
            _lastVisible = Math.Max(Math.Min(_lastVisible - delta, _items.Count), _renderingItems.Count);
            Refresh();
            return true;
        }

        public void MoveToLast()
        {
            _lastVisible = _items.Count;
        }
    }
}
