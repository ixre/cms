namespace JR.Cms.Domain.Interface._old
{
    public interface ITemplateBind
    {
        Spc.Models.TemplateBind GetArchiveTemplateBind(string archiveID, int categoryID);
        //Spc.Models.TemplateBind GetBind(TemplateBindType type, string bindID);
        Spc.Models.TemplateBind GetCategoryTemplateBind(int categoryID);
        int RemoveErrorCategoryBind();
        //bool SetBind(Spc.Models.TemplateBind bind);
    }
}
