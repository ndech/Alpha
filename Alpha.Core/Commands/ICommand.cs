namespace Alpha.Core.Commands
{
    public interface ICommand
    {
        void Execute();
        bool IsValid();
    }
}
