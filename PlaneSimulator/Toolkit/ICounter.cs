namespace PlaneSimulator.Toolkit
{
    using System;
    interface ICounter : IUpdatable
    {
        float Value { get; }
        String Unit { get; }
    }
}
