namespace JR.Stand.Core.Template
{
    /// <summary>
    /// 模板解析类
    /// </summary>
    public interface ITemplateResolver
    {
        /// <summary>
        /// 设置数据容器 
        /// </summary>
        /// <param name="dataContainer"></param>
        void SetContainer(IDataContainer dataContainer);

        /// <summary>
        /// 获取数据容器
        /// </summary>
        /// <returns></returns>
        IDataContainer GetContainer();
    }
}