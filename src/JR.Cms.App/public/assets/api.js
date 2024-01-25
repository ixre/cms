/* 警告:此文件由系统自动生成,请勿修改,因为可能导致您的更改丢失! */
//
//文件：Cms Javascript WebApi
//版本: 0.0.1
//时间：2014-06-21
// Note : 请调用API前，先调用jr.spi.setPath('${page.domain}');这样才能请求到指定站点的接口。
// Modify :  2015-09-18 19:37  [jsix] [!] : 重构
//
if (!window.$js) alert('未加载core.js！');
$js.extend({
    /** 兼容版本 */
    webapi: {
        path: '/1/',
        setPath:function(p) {
            this.path = p;
        },
        request: function (apiName, params, call, errCall) {
            var uri = this.path + '/web_api?key=11857832134&domain='+ location.host+'&name=' + apiName;
            for (var key in params) {
                uri += '&' + key + '=' + params[key];
            }
            $js.xhr.request(uri,{params: {}, method: 'GET', data: 'json' }, {
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
        },
        // 获取文档信息
        loadArchiveInfo: function(path,callback){
            this.request("archive/info",{path:path},callback);
        }
    },
    // .net6版本
    api:{
        path: '/cms/webapi',
        setPath:function(p) {
            this.path = p;
        },
        request: function (apiName, params, call, errCall) {
            var url = this.path +apiName+ (apiName.indexOf("?") === -1?"?":"&")+'key=11857832134';
            $js.xhr.get(url,call, errCall);
        },
        getRLink: function (contentId, callback, error) {
            this.request("relate/"+contentId, callback, error);
        }
    },
});