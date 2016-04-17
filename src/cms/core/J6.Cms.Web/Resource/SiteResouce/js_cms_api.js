//
//文件：Cms Javascript WebApi
//版本: 0.0.1
//时间：2014-06-21
// Note : 请调用API前，先调用j6.api.setPath('${page.domain}');这样才能请求到指定站点的接口。
// Modify :  2015-09-18 19:37  [jsix] [!] : 重构
//
if (!window.j6) alert('未加载core.js！');
j6.extend({
    api: {
        path: '',
        setPath:function(p) {
            this.path = p;
        },
        request: function (apiName, params, call, errCall) {
            var uri = this.path + '/webapi?key=11857832134&name=' + apiName;
            for (var key in params) {
                uri += '&' + key + '=' + params[key];
            }
            j6.xhr.request({ uri: uri, params: {}, method: 'GET', data: 'json' }, {
                success: call,
                error: errCall
            });
        },
        getRLink: function (contentId, data,callback, error) {
            this.request(data === 'html' ? 'rel_link' : 'rel_link_json',
                { content_id: contentId },
                callback,
                error
                );
        }
    }
});