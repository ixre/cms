using System.Collections.Generic;
using T2.Cms.Dal;
using T2.Cms.Domain.Interface.Site.Category;
using T2.Cms.Domain.Interface.Site.Template;
using T2.Cms.Infrastructure;

namespace T2.Cms.ServiceRepository
{
    public class TemplateRepoImpl:ITemplateRepo
    {
        private TemplateBindDal tpldal = new TemplateBindDal();

        public TemplateBind GetTemplateBind(int bindRelationId, TemplateBindType templateBindType)
        {
            return this.GetBind(templateBindType, bindRelationId);
        }

        /// <summary>
        ///模板绑定列表
        /// </summary>
        private IList<TemplateBind> TemplateBinds
        {
            get
            {
                if (RepositoryDataCache._tplbind == null)
                {
                    IList<TemplateBind> tpls = new List<TemplateBind>();
                    TemplateBind tpl;
                    tpldal.GetBindList(rd =>
                    {
                        while (rd.Read())
                        {
                            tpl = this.CreateTemplateBind(
                                int.Parse(rd["id"].ToString()),
                                (TemplateBindType)int.Parse(rd["bind_type"].ToString()),
                                rd["tpl_path"].ToString()
                                );
                            tpl.BindRefrenceId = int.Parse(rd["bind_id"].ToString());
                            tpls.Add(tpl);
                        }
                    });

                    RepositoryDataCache._tplbind = tpls;
                }

                return RepositoryDataCache._tplbind;
            }
        }


        public IEnumerable<TemplateBind> GetTemplateBindsForCategory(ICategory category)
        {
           TemplateBind bind=  this.GetBind(TemplateBindType.CategoryTemplate, category.GetDomainId());
           if (bind != null) yield return bind;
           bind= this.GetBind(TemplateBindType.CategoryArchiveTemplate, category.GetDomainId());
           if (bind != null) yield return bind;
        }

        private TemplateBind GetBind(TemplateBindType templateBindType, int bindRefrenceId)
        {
            foreach (TemplateBind bind in this.TemplateBinds)
            {
                if (bind.BindType == templateBindType && bind.BindRefrenceId == bindRefrenceId)
                    return bind;
            }
            return null;
        }


        public TemplateBind CreateTemplateBind(int id, TemplateBindType type, string templatePath)
        {
            return new TemplateBind(id, type, templatePath);
        }

        public int SaveTemplateBind(int refrenceId, TemplateBind templateBind)
        {
            tpldal.SetBind(templateBind.BindType, refrenceId, templateBind.TplPath);
            RepositoryDataCache.ClearTemplateBinds();
            return templateBind.ID;
        }


        public Error RemoveBind(int refrenceId, TemplateBindType templateBindType)
        {
            bool result = tpldal.RemoveBind(templateBindType, refrenceId);
            RepositoryDataCache.ClearTemplateBinds();
            return null;
        }


        public Error RemoveBinds(int refrenceId,TemplateBind[] list)
        {
            if (list == null || list.Length == 0) return null;
            foreach(TemplateBind b in list)
            {
                this.tpldal.RemoveBind(b.BindType, refrenceId);
                RepositoryDataCache.ClearTemplateBinds();
            }
            return null;
        }

        public Error SaveTemplateBinds(int refrenceId, TemplateBind[] list)
        {
            if (list == null || list.Length == 0) return null;
            foreach (TemplateBind b in list)
            {
                tpldal.SetBind(b.BindType, refrenceId, b.TplPath);

            }
            RepositoryDataCache.ClearTemplateBinds();
            return null;
        }
    }
}
