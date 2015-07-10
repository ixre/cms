using System.Globalization;
using System;
using System.Collections.Generic;
using J6.Cms.DAL;
using J6.Cms.Domain.Interface.Common;

namespace J6.Cms.ServiceRepository
{
    public class LinkRepository : ILinkRepository
    {
        private LinkDAL _linkDal = new LinkDAL();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkManager"></param>
        /// <param name="typeIndent"></param>
        /// <param name="relatedId"></param>
        private void ReadLinks(ILinkManager linkManager, string typeIndent, int relatedId)
        {
            this._linkDal.ReadLinksOfContent(typeIndent, relatedId, rd =>
            {
                while (rd.Read())
                {
                    linkManager.Add(
                        int.Parse(rd["id"].ToString()),
                        rd["name"].ToString(),
                        rd["title"].ToString(),
                        rd["uri"].ToString(),
                        rd["enabled"].ToString() == "1" || rd["enabled"].ToString()=="True"
                        );
                }
            });
        }
        private void SaveLinks(string typeIndent, int relatedId, IList<ILink> list)
        {
            this._linkDal.SaveLinksOfContent(typeIndent, relatedId, list);
        }


        public void ReadLinksOfContent(ILinkManager linkManager, int contentModelIndent, int contentId)
        {
            this.ReadLinks(linkManager, contentModelIndent.ToString(), contentId);

        }

        public void SaveLinksOfContent(int contentModelIndent, int contentId, IList<ILink> list)
        {
            SaveLinks(contentModelIndent.ToString(), contentId, list);
        }

        public void RemoveRelatedLinks(string typeIndent, int relatedId, IList<int> list)
        {
            String ids = "";

            for (int i = 0; i < list.Count; i++)
            {
                if (i != 0)
                {
                    ids += ",";
                }
                ids += list[i].ToString(CultureInfo.InvariantCulture);
            }

            this._linkDal.RemoveRelatedLinks(typeIndent, relatedId, ids);
        }
    }
}
