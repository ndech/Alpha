using System;
using System.Collections.Generic;
using System.IO;
using Alpha.UI.Controls;
using Alpha.UI.Coordinates;
using SharpDX;

namespace Alpha.UI.Screens
{
    class SaveScreen : Screen
    {
        public SaveScreen(IGame game) : base(game, "save_screen", true)
        {
            List<String> saves = SaveGame.ExistingSaves();
            Panel overlay = Register(new Panel(game, "menu_overlay", new UniRectangle(0, 0, 1.0f, 1.0f), new Color(0, 0, 0, 0.6f)));
            Panel panel = overlay.Register(new Panel(game, "menu_panel", new UniRectangle(new UniScalar(.5f, -150), new UniScalar(.5f, -172), 300, 345), Color.Black));
            
            TextInput input = panel.Register(new TextInput(game, "save_input", new UniRectangle(0.05f, 225, 0.9f, 40)));
            for (int i = 0; i < saves.Count; i++)
            {
                Button button = panel.Register(new Button(game, "save_item", new UniRectangle(0.05f, 15 + (55 * i), 0.9f, 40), Path.GetFileNameWithoutExtension(saves[i])));
                button.Clicked += (b) => { input.Text = b.Text; };
            }
                

            Button cancelButton = panel.Register(new Button(game, "menu_cancel", new UniRectangle(0.25f, 290, 0.5f, 40), "Cancel"));
            cancelButton.Clicked += (b) => UiManager.DeleteScreen(this);
        }
    }
}
