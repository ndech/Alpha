namespace Alpha.Toolkit
{
    public static class PreprocessorHelper
    {
        public static T IfDebug<T>(T debug, T release)
        {
#if DEBUG
            return debug;
#else
            return release;
#endif
        }
    }
}
