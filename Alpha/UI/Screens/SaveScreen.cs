using System;
using System.Collections.Generic;
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

            for(int i = 0; i< saves.Count; i++)
                panel.Register(new Button(game, "save_item", new UniRectangle(0.05f, 15+(55*i), 0.9f, 40), saves[i]));

            Button cancelButton = panel.Register(new Button(game, "menu_cancel", new UniRectangle(0.25f, 290, 0.5f, 40), "Cancel"));
            cancelButton.Clicked += (b) => UiManager.DeleteScreen(this);
        }
    }
}
