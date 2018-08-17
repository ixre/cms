// this file is auto generated!

using JR.DevFw.Framework;
using JR.DevFw.Data.Orm.Mapping;

namespace zunxinApp.App_Start
{

    /// <summary>
    /// {{.T.Comment}}
    /// </summary>
    [DataTable("{{.T.Name}}")]
    public class {{.T.Title}}Entity
    {
        
        {{range $i,$c:=.T.Columns}}
        /// <summary>
        /// {{$c.Comment}}
        /// </summary>
        [Alias("{{$c.Name}}")]
        public {{$c.Type}} {{$c.Title}} { get; set; }{{end}}
    }
}