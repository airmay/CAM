namespace CAM.Commands
{
    public abstract class CommandBase
    {
        public abstract void Execute();

        public virtual bool CanExecute() => true;
    }
}