using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneSimulator
{
    public abstract class Component
    {
        virtual public double Mass { get; }

        public Component()
        {

        }
    }
}
