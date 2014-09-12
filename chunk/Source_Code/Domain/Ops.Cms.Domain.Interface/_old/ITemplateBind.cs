using System;
using Spc.Models;
using Ops.Cms.Domain.Interface.Site.Template;

namespace Spc.Logic
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
