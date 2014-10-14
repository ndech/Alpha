namespace Alpha.Core.Commands
{
    public abstract class Command
    {
        public RealmToken Source { get; set; }

        internal abstract void Execute();

        internal virtual bool IsValid()
        {
            return true;
        }
    }
}
