using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneSimulator.Toolkit.Math
{
    interface IIntegrable <T>
    {
        T Sum(T t1, T t2);
        T Times(double multiplicator);
        T Add(T other);
    }
}
