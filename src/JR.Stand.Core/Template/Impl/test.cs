namespace JR.Stand.Core.Template.Impl
{
    internal class test
    {
        private void Main()
        {
            IDataContainer dc = new BasicDataContainer(null);
            TemplatePage tp = new TemplatePage(dc);
            tp.SetTemplateContent( "$m=item  KEY:${item.key} Note:${item.note}<br />${m.key}");
            tp.AddVariable("item", new {Key = "Key", Note = "Note"});
            //var x = System.Web.HttpContext.Current.Items;
            string y = tp.ToString();
            // return y;
        }
    }
}