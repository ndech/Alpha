using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alpha.DirectX.UI.Controls;
using Alpha.DirectX.UI.Coordinates;

namespace Alpha.DirectX.UI.Screens
{
    class WorldParametersScreen : Screen
    {
        public WorldParametersScreen(IContext context) : base(context, "world_parameters_screen")
        {
            Register(new Button(context, "ok_button",
                new UniRectangle(new UniScalar(1.0f, -300), new UniScalar(1.0f, -120), 250, 70),"Generate world"));
        }
    }
}
