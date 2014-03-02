namespace Alpha.Toolkit
{
    using System;
    interface ICounter : IUpdatable
    {
        float Value { get; }
        String Unit { get; }
    }
}
