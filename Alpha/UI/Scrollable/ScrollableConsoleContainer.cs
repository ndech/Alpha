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
            Refresh();
        }

        public void Refresh()
        {
            for (int i = 0; i < _renderingItems.Count; i++)
            {
                if(i<_items.Count)
                    _renderingItems[i].Set(_items[_items.Count-1-i]);
                else
                    _renderingItems[i].Set(null);
            }
        }
    }
}
