using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alpha.UI.Controls;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI
{
    class MenuScreen : Screen
    {
        public MenuScreen()
            :base()
        {
            _components.Add(new Button());
        }

        public override void Update(double delta)
        {

        }

        public override void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {

        }
    }
}
