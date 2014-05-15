using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using Alpha.UI.Controls;
using Alpha.UI.Coordinates;
using SharpDX;

namespace Alpha.UI.Screens
{
    class LoadScreen : Screen
    {
        public LoadScreen(IGame game) : base(game, "load_screen", true)
        {

            List<String> saves = SaveGame.ExistingSaves();
            Panel overlay = Register(new Panel(game, "menu_overlay", new UniRectangle(0, 0, 1.0f, 1.0f), new Color(0, 0, 0, 0.6f)));
            Panel panel = overlay.Register(new Panel(game, "menu_panel", new UniRectangle(new UniScalar(.5f, -150), new UniScalar(.5f, -172), 300, 345), Color.Black));

            for (int i = 0; i < saves.Count; i++)
            {
                Button button = panel.Register(new Button(game, "save_item", new UniRectangle(0.05f, 15 + (55 * i), 0.9f, 40), Path.GetFileNameWithoutExtension(saves[i])));
                button.Clicked += (b) => Load(b.Text);
            }

            Button cancelButton = panel.Register(new Button(game, "menu_cancel", new UniRectangle(0.3f, 290, 0.4f, 40), "Cancel"));
            cancelButton.Clicked += (b) => UiManager.DeleteScreen(this);
            cancelButton.Shortcut = Key.Escape;
        }

        public void Load(String fileName)
        {
            LoadingScreen screen = new LoadingScreen(Game);
            UiManager.AddScreen(screen);
            Game.Load(fileName, (text) =>
            {
                screen.LoadedContent = text;
                Game.Draw();
            });
        }
    }
}
