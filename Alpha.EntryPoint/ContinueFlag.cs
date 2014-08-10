namespace Alpha.EntryPoint
{
    class ContinueFlag
    {
        private volatile bool _continue = true;
        public static implicit operator bool(ContinueFlag flag)
        {
            return flag._continue;
        }

        public void Stop()
        {
            _continue = false;
        }
    }
}
