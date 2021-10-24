namespace JR.Stand.Core.Framework.TaskBox.Toolkit
{
    public class TaskLogProvider : ITaskLogProvider
    {
        public TaskLogProvider(ITaskBoxStorage storage)
        {
            this.Storage = storage;
        }

        public void LogTaskState(ITaskExecuteClient client, ITask task, TaskMessage message)
        {
            //在这里可以记录日志

            if (this.Storage != null)
                this.Storage.SaveTaskChangedState(task, message);
        }

        public ITaskBoxStorage Storage { get; private set; }
    }
}