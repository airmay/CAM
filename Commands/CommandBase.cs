namespace CAM.Commands
{
    public abstract class CommandBase
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        public abstract void Execute();

        public virtual bool CanExecute() => true;
    }
}