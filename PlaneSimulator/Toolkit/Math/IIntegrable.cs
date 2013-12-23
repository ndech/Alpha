using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneSimulator.Toolkit.Math
{
    internal interface IIntegrable<T>
    {
        T Times(double multiplicator);
        T Add(T other);
    }
}