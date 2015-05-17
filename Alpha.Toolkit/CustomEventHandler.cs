using System.Runtime.CompilerServices;

namespace Alpha
{
    public delegate void CustomEventHandler<in T, in T1, in T2>(T arg, T1 arg2, T2 arg3);
    public delegate void CustomEventHandler<in T, in T1>(T arg, T1 arg2);
    public delegate void CustomEventHandler<in T>(T arg);
    public delegate void CustomEventHandler();

    public static class CustomEventHandlerExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Raise<T, T1, T2>(this CustomEventHandler<T, T1, T2> handler, T arg, T1 arg2, T2 arg3)
        {
            handler?.Invoke(arg, arg2, arg3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Raise<T, T1>(this CustomEventHandler<T, T1> handler, T arg, T1 arg2)
        {
            handler?.Invoke(arg, arg2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Raise<T>(this CustomEventHandler<T> handler, T arg)
        {
            handler?.Invoke(arg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Raise(this CustomEventHandler handler)
        {
            handler?.Invoke();
        }
    }
}
