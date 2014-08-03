namespace Alpha.Core.Commands
{
    abstract class Command : ICommand
    {
        public abstract void Execute();

        public virtual bool IsValid()
        {
            return true;
        }
    }
}
