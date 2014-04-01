using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Alpha.Graphics
{
    interface ICamera : IService
    {
        Matrix UiMatrix { get; }
        Matrix ViewMatrix { get; }
    }
}
