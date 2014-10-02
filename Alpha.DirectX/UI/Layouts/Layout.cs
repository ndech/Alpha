using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpha.DirectX.UI.Layouts
{
    abstract class Layout
    {
        protected readonly UiComponent ParentComponent;

        public Layout(UiComponent parent)
        {
            ParentComponent = parent;
        }
    }
}
