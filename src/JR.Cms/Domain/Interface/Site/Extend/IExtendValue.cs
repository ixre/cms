namespace JR.Cms.Domain.Interface.Site.Extend
{
    public interface IExtendValue : IDomain<int>
    {
        IExtendField Field { get; }
        string Value { get; set; }
    }
}