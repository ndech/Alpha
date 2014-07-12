using System;
using Alpha.UI.Controls;
using Alpha.UI.Controls.Custom;
using Alpha.UI.Coordinates;

namespace Alpha.UI.Scrollable
{
    class ScrollableConsoleItem : Label, IScrollableItem<ConsoleLine>
    {
        public ScrollableConsoleItem(IGame game, UniVector position) : base(game, "console_line", new UniRectangle(position, new UniVector(1.0f, Height)), "")
        {
        }

        public void Set(ConsoleLine item)
        {
            if (item == null)
            {
                Text = "";
                return;
            }
            String prefix;
            switch (item.Type)
            {
                case ConsoleLine.ConsoleLineType.Command:
                    prefix = "[Yellow]>> ";
                    break;
                case ConsoleLine.ConsoleLineType.Error:
                    prefix = "[Red]";
                    break;
                case ConsoleLine.ConsoleLineType.Info:
                    prefix = "[Blue]";
                    break;
                default:
                    prefix = "";
                    break;
            }
            Text = prefix + Graphics.Text.Escape(item.Content); ;
        }

        public static int Height { get { return 20; } }

    }
}
