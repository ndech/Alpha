using System.Collections.Generic;
using Alpha.Graphics;
using Alpha.Toolkit.Math;
using Alpha.UI.Controls;
using Alpha.UI.Controls.Custom;
using Alpha.UI.Coordinates;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI.Scrollable
{
    class ScrollableConsoleContainer : Panel, IVerticalScrollableContainer<ScrollableConsoleItem, ConsoleLine>
    {
        private readonly List<ConsoleLine> _items;
        private readonly IList<ScrollableConsoleItem> _renderingItems;
        private TexturedExtensibleRectangle _fixedPart;

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
            
            IRenderer renderer = Game.Services.GetService<IRenderer>();

            Texture fixedTexture = renderer.TextureManager.Create("SlidingBarConsole_fix.png", @"Data/UI/");
            int width = fixedTexture.Width;
            _fixedPart = new TexturedExtensibleRectangle(renderer, new Vector2I(width,Size.Y), fixedTexture, width);
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            base.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
            _fixedPart.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
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
