/* 此文件由系统自动生成! */
//
//文件：Cms Javascript WebApi
//版本: 0.0.1
//时间：2014-06-21
//
if (!window.j6) alert('未加载core.js！');
j6.extend({
    api: {
        path: '',
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
        getRLink: function (contentId, callback, error, data) {
            this.request(data === 'html' ? 'rel_link' : 'rel_link_json',
                { content_id: contentId },
                callback,
                error
                );
        }
    }
});