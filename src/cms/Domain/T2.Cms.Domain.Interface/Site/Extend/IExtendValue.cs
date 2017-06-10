namespace T2.Cms.Domain.Interface.Site.Extend
{
    public interface IExtendValue:IDomain<int>
    {
        IExtendField Field { get; }
        string Value { get; set; }
    }
}
