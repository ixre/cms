namespace JR.Stand.Core.Framework.TaskBox
{
    public interface ITaskExecuteClient
    {
        void Execute(ITask task);
        string ClientName { get; }
    }
}