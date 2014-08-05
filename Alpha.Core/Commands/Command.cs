namespace Alpha.Core.Commands
{
    public abstract class Command
    {
        internal abstract void Execute();

        internal bool IsValid()
        {
            return true;
        }
    }
}
