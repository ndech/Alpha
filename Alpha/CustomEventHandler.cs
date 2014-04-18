using System.Runtime.CompilerServices;

namespace Alpha
{
    public delegate void CustomEventHandler<in T, in T1>(T arg, T1 arg2);
    public delegate void CustomEventHandler<in T>(T arg);
    public delegate void CustomEventHandler();

    public static class CustomEventHandlerExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Raise<T, T1>(this CustomEventHandler<T, T1> handler, T arg, T1 arg2)
        {
            if (handler != null) handler(arg, arg2);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Raise<T>(this CustomEventHandler<T> handler, T arg)
        {
            if (handler != null) handler(arg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Raise(this CustomEventHandler handler)
        {
            if (handler != null) handler();
        }
    }
}
