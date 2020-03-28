namespace JR.Cms.Domain.Interface
{
    /// <summary>
    /// Data Transfer Object接口，定义转为及互转的接口规范
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITransferObject<T> where T : new()
    {
        T Convert();
        void LoadData(T t);
    }
}