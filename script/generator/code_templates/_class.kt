/** {{.T.Comment}} */
class {{.T.Title}}{
    {{range $i,$c:=.T.Columns}}
    /** {{$c.Comment}} */
    var {{lowerTitle $c.Title}}:{{$c.Type}} = {{end}}
}