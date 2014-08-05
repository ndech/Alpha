namespace Alpha.Core.Commands
{
    public abstract class Command : ICommand
    {
        internal abstract void Execute();

        public virtual bool IsValid()
        {
            return true;
        }
    }
}
