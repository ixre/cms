//
//文件：Cms Javascript WebApi
//版本: 0.0.1
//时间：2014-06-21
//
if (!window.$JS) alert('未加载core.js！');
$JS.extend({
    api: {
        path:'',
        request: function (apiName, params, call, errCall) {
            var uri = this.path+'/webapi?key=11857832134&name=' + apiName;
            for (var key in params) {
                uri += '&' + key + '=' + params[key];
            }
            $JS.xhr.request({ uri: uri, params: {}, method: 'GET', data: 'json' }, {
                success: call,
                error:errCall});
        },
        getRLink: function (contentId,callback,error) {
            this.request('rlink',
                { contentId: contentId },
                callback,
                error
                );
        }
    }
});