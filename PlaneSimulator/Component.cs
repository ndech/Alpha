using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneSimulator
{
    public abstract class Component
    {
        public abstract double Mass { get; }

        public Component()
        {
        }
    }
}