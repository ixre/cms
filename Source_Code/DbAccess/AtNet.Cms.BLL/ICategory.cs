using System;
using Spc.Models;
using Ops.Cms.Infrastructure;
using Ops.Cms.Domain.Interface.Site.Category;

namespace Spc.Logic
{
    public interface ICategoryModel
    {
        Spc.Models.Category Get(Func<Spc.Models.Category, bool> func);
        System.Collections.Generic.IList<Spc.Models.Category> GetCategories();
        System.Collections.Generic.IEnumerable<Spc.Models.Category> GetCategories(Func<Spc.Models.Category, bool> func);
        System.Collections.Generic.IEnumerable<Spc.Models.Category> GetCategories(int lft, int rgt, CategoryContainerOption option);
        Spc.Models.Category GetNext(int left, int right);
        Spc.Models.Category GetParent(int left, int right);
        Spc.Models.Category GetPrevious(int left, int right);
        void HandleCategoryTree(System.Collections.Generic.IEnumerable<Spc.Models.Category> list, CategoryTreeHandler func);
        void HandleCategoryTree(string categoryName, CategoryTreeHandler func);
        void Insert(int siteID, string parentName, int moduleID, string categoryName, string tag, string pagetitle, string keywords, string description, int orderIndex);
        void RebuildCache();
        Spc.Models.Category Root { get; }
        void Update(Spc.Models.Category entity, int parentLft);
    }
}
