//
// global.js
//
// Author:
//  newmin (newmin.net@gmail.com)
//
// Copyright 2011 @ S1N1.COM,All rights reseved!
//
/*-----------------------a---------------------
 * 2011-03-22  修改Ajax部分，不共享共用实例
 * 2011-11-07  修改Dialog部分,添加COOKIE操作
 * 2011-11-08  添加拖动代码
 * 2011-12-21  添加dialog的onclose()事件
 * 2012-09-22  重新组织代码,并添加部分方法(getData,loadDependScript)
 * 2012-10-10  修正了form.getData关于+号和空格的处理
 * 2012-11-03  添加了request请求其他url的参数
 * 2012-12-28  修改验证器
 * 2013-01-05  getElementsByClassName添加elem参数
 * 2013-03-27  getElementsByClassName 删除elem参数,在内部用arguments获取
 *                       添加toJson方法
 * 2013-05-18  添加验证器支持混合验证，regex验证
 * 2013-05-23  添加了tipin设置显示位置
 * 2013-06-04  修改了dynamicTable,支持自定义头部
 * 2013-07-09  js.load实现
 * 2013-10-24  调整contextmenu右键
 ----------------------------------------------*/

function js() {
    this.__VERSION__ = '3.1';       //版本号
    this.__WORKPATH__ = '';         //工作路径
    this.__Extend_PROTOTYPE__ = true;
}

js.prototype = {
    __init__: function () {

        //扩展原生JS
        if (this.__Extend_PROTOTYPE__) {
            this.__extendingJsPrototype__();
        }

        //设置当前版本
        var _scripts = document.getElementsByTagName('SCRIPT');
        var s = _scripts[_scripts.length - 1];
        var _sloc = s.src;
        //s.src=_sloc+'#ver='+this.__VERSION__;
        //获取工作目录
        this.__WORKPATH__ = _sloc.replace(/(\/)[^/]+$/, '$1');


        /************* JavaScript  Extensions *****************/
        //添加document.getElementsByClassName方法
        if (!document.getElementsByClassName) {
            document.getElementsByClassName = function (className, ele) {
                if (ele && !ele.nodeName) {
                    ele = document.getElementById(ele);
                }
                var elem = (ele || document).getElementsByTagName('*');
                var reg = new RegExp('\\s' + className + '\\s');
                var arr = [];
                for (var i = 0, j; j = elem[i]; i++) {
                    if (reg.test(' ' + j.className + ' ')) arr.push(j);
                }
                return arr;
            };
        }

        //添加contains
        if (typeof (HTMLElement) != "undefined") {
            HTMLElement.prototype.contains = function (obj) {
                while (obj != null && typeof (obj.tagName) != "undefind") {
                    if (obj == this) return true;
                    obj = obj.parentNode;
                }
                return false;
            };
        }

        //添加window.toJson
        if (!window.toJson) {
            window.toJson = function (s) {
                if (!s) return null;
                //if (s instanceof Object) return s;
                //IE8,FF等其他浏览器
                if (window.JSON) {
                    try {
                        //如果json的键没有用引号引起来，则会抛出异常
                        return JSON.parse(s);
                    } catch (ex) {
                    }
                }
                //使用旧的方法，容易产生内存泄露
                return eval('(' + s + ')');
            };
        }
    },

    __extendingJsPrototype__: function () {
        //仅计算中文为2个字节
        String.prototype.len = function (zh) {
            return this.replace(zh ? /[\u0391-\uFFE5]/g : /[^x00-xff]/g, "00").length;
        };
        Date.prototype.format = function (fmt) {
            var o = {
                "M+": this.getMonth() + 1, //月份
                "d+": this.getDate(), //日
                "h+": this.getHours() % 12 == 0 ? 12 : this.getHours() % 12, //小时
                "H+": this.getHours(), //小时
                "m+": this.getMinutes(), //分
                "s+": this.getSeconds(), //秒
                "q+": Math.floor((this.getMonth() + 3) / 3), //季度
                "S": this.getMilliseconds() //毫秒
            };
            var week = {
                "0": "/u65e5",
                "1": "/u4e00",
                "2": "/u4e8c",
                "3": "/u4e09",
                "4": "/u56db",
                "5": "/u4e94",
                "6": "/u516d"
            };
            if (/(y+)/.test(fmt)) {
                fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
            }
            if (/(E+)/.test(fmt)) {
                fmt = fmt.replace(RegExp.$1, ((RegExp.$1.length > 1) ? (RegExp.$1.length > 2 ? "/u661f/u671f" : "/u5468") : "") + week[this.getDay() + ""]);
            }
            for (var k in o) {
                if (new RegExp("(" + k + ")").test(fmt)) {
                    fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
                }
            }
            return fmt;
        };
    },

    //*************** 扩展 ***************//
    extend: function (prop) {
        if (prop && prop instanceof Object) {
            for (var ext in prop) {
                if (this[ext] == undefined) {
                    this[ext] = prop[ext];
                }
            }
        }
    },
    dom: {
        fitHeight: function (e, offset) {
            var par = e.parentNode;
            var next = e.nextSibling;
            var reg = /;(\s*)height:(.+);/ig;

            var height = (
                    par == document.body
                ? Math.max(document.body.clientHeight, document.documentElement.clientHeight)
                : par.offsetHeight) - e.offsetTop;

            while (next) {
                if (next.nodeName[0] != '#') {
                    height -= next.offsetHeight;
                }
                next = next.nextSibling;
            }

            height -= offset || 0;
            if (reg.test(e.style.cssText)) {
                e.style.cssText = e.style.cssText.replace(reg, '; height:' + height + 'px;');
            } else {
                e.style.cssText += 'height:' + height + 'px;';
            }
        },

        //======= 选择器 =====//
        $: function (el, tagName, attrs) {
            var e = el.nodeName ? el : document.getElementById(el || '');
            if (!e) throw el.nodeName ? 'object refrence null' : 'element ' + el + ' not exits!';
            if (!tagName) return e;
            e = e.getElementsByTagName(tagName);
            if (!attrs) return e;
            var arr =[];
            var attr_name;
            for (var i = 0; i < e.length; i++) {
                var equalAttr = true;
                for (var j in attrs) {
                    switch (j) {
                        case "className":
                            attr_name = e[i].getAttribute("class") ? "class" : "className";
                            break;
                        default:
                            attr_name = j;
                            break;
                    }
                    if (e[i].getAttribute(attr_name) != attrs[j]) equalAttr = false;
                    if (equalAttr) arr.push(e[i]);
                }
            }
            return arr;
        }
    },

    $: function (el, tagName, attrs) {
        return this.dom.$(el, tagName, attrs);
    },
    getElementsByClassName: function (ele, className) {
        ele = ele || document;
        return ele.getElementsByClassName ? ele.getElementsByClassName(className) : document.getElementsByClassName(className, ele);
    },
    each: function (arr, call) {if(arr){ for (var i = 0; i < arr.length; i++) call(i, arr[i]);} },
    style: function (id,attr) {
        var e = this.dom.$(id);
        if(!e)return null;
        //如果包含属性，则设置
        if(attr) {
            if (attr instanceof Object) {
                for (var s in attr) {
                    var cssAttr = s.split("-");
                    for (var i = 1; i < cssAttr.length; i++) {
                        cssAttr[i] = cssAttr[i].replace(cssAttr[i].charAt(0), cssAttr[i].charAt(0).toUpperCase());
                    }
                    var newCssAttr = cssAttr.join('');
                    e.style[newCssAttr] = attr[s];
                }
            } else if (attr instanceof String) {
                e.style.cssText = attr;
            }
        }
        return e.currentStyle || document.defaultView.getComputedStyle(e, null);
    },
    request: function (id, url) {
        var match = new RegExp('(\\?|&)' + id + '=([^&]+)&*').exec(url ? url : location.href);
        return match ? match[2] : '';
    },
    supportHTML5: navigator.geolocation != null,
    template: function (content, data) {
        if (data instanceof Object) {
            var reg = new RegExp();
            for (var n in data) {
                reg.compile('%' + n + '%|\{' + n + '\}', 'g');
                content = content.replace(reg, data[n]);
            }
        }
        return content;
    },
    screen: {
        width: function () {
            return Math.max(document.body.clientWidth, document.documentElement.clientWidth);
        },
        height: function () {
            return Math.max(document.body.clientHeight, document.documentElement.clientHeight);
        },
        offsetWidth: function () {
            return Math.max(document.body.offsetWidth, document.documentElement.offsetWidth);
        },
        offsetHeight: function () {
            return Math.max(document.body.offsetHeight, document.documentElement.offsetHeight);
        }
    },

    //*************** 事件 ***************//
    event: {
        //添加事件
        add: function (elem, event, func, b) {
            if (!elem.attachEvent && !elem.addEventListener) {
                alert('event error! parameter:' + ele + ',event:' + event);
                return;
            }
            document.attachEvent ? elem.attachEvent('on' + event, func) : elem.addEventListener(event, func, b);
        },
        //移除事件
        remove: function (elem, event, func, b) {
            document.detachEvent ? elem.detachEvent('on' + event, func) : elem.removeEventListener(event, func, b);
        },
        //事件元素
        src: function (event) {
            var e = event ? event : window.event;
            return e.target || e.srcElement;
        },
        //停止冒泡
        stopBubble: function (event) {
            var e = event ? event : window.event;
            if (window.event) {
                e.cancelBubble = true;
            } else {
                //e.preventDefault();
                e.stopPropagation();
            }
        },
        //阻止浏览器默认行为
        preventDefault: function (event) {
            if (window.event) {
                window.event.returnvalue = false;
                return false;
            } else {
                event.preventDefault();
                return false;
            }
        }
    },

    //*************** XHR ***************//
    xhr: {
        //最大同时请求数
        max_request: 2,

        //XMLHttpRequest对象
        http_stack: null,

        //处理队列数组
        proc_stack: [],

        //初始化
        init: function () {
            if (this.http_stack) return;
            this.http_stack = [];

            for (var i = 0; i < this.max_request; i++) {
                this.http_stack[i] = {
                    state: 0, //状态,0空闲,1繁忙
                    http: window.XMLHttpRequest ? new XMLHttpRequest() : (new ActiveXObject("MSXML2.XMLHTTP") || new ActiveXObject("MICROSOFT.XMLHTTP"))
                };
            }
        },

        //请求
        request: function (_request, call) {

            //执行初始化
            this.init();

            //请求范例
            //xhr.request({uri:"/",method:"POST",params:"",data:"text"},
            //{start:function(){},over:function(){},error:function(x){//处理错误},success:function(x){}});
            //x为返回的数据*/

            var req = {
                uri: _request.uri || location.href,

                //请求参数
                //method为"POST"时适用
                //格式如:'action=delete&id=123'
                params: _request.params,

                //请求的方法,POST和GET,HEAD
                method: (_request.method || "GET").toUpperCase(),

                //是否异步
                async: _request.async == false ? false : _request.async || true,

                //返回数据格式,Text|XML
                data: (_request.data || 'text').toLowerCase(),

                //是否缓存
                random: _request.random == false ? false : _request.random || true,

                call: call
            };

            //将object转为string
            if (_request.params instanceof Object) {
                var tmpInt = 0;
                req.params = '';
                for (var i in _request.params) {
                    if (tmpInt++ != 0) {
                        req.params += '&';
                    }
                    req.params += i + '=' + encodeURIComponent(_request.params[i]);
                }
            }

            //开始请求时候调用函数
            if (req.call.start != null) req.call.start();

            //发送GET请求时候添加随机数
            if (req.method != "POST" && req.random) {
                if (req.uri.indexOf('#') == -1) {
                    if (req.uri.indexOf("?") == -1) req.uri += "?t=" + Math.random();
                    else req.uri += "&t=" + Math.random();
                }
            }

            var xhr_proc = function (_xhr, _xhrobj, _request) {
                _xhr.state = 1;

                //同步线程:true
                _xhr.http.open(_request.method, _request.uri, _request.async);

                //请求状态发生变化时执行
                _xhr.http.onreadystatechange = function () {
                    if (_xhr.http.readyState == 4) {
                        if (_xhr.http.status == 200) {
                            if (_request.call.success) {
                                _xhr.state = 0;
                                _xhrobj.proc_stack.pop();
                                _request.call.success(_request.data == "text" ?
                                    _xhr.http.responseText :
                                    (_request.data == 'json' ? window.toJson(_xhr.http.responseText) : _xhr.http.responseXML));
                            }
                        } else if (_request.call.error) {
                            _xhr.state = 0;
                            _xhrobj.proc_stack.pop();
                            _request.call.error(_xhr.http.responseText);
                        }
                    }
                };
                //如果为POST请求
                if (_request.method == "POST") _xhr.http.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");

                //发送请求
                _xhr.http.send(_request.params);
            };

            (function (_this, _req) {
                var t = setInterval(function () {
                    if (_this.proc_stack.length < _this.max_request) {
                        _this.proc_stack.push(0);

                        //查找空闲的xhr对象
                        for (var i = 0; i < _this.max_request; i++) {
                            if (_this.http_stack[i].state == 0) {
                                //执行请求
                                xhr_proc(_this.http_stack[i], _this, _req);
                                break;
                            }
                        }

                        clearInterval(t);
                    }

                }, 20);
            }(this, req));
        },

        get: function (param, callback, errorFunc) {
            this.request(
                    param instanceof Object ? param : { uri: param }, {
                    success: function (x) {
                        if (callback) callback(x);
                    },
                    error: function (x) {
                        if (errorFunc) errorFunc(x);
                    }
                });
        },
        post: function (uri, param, okCall, errCall) {
            this.request({ uri: uri, method: 'POST', params: param },
                {
                    success: function (x) {
                        if (okCall) okCall(x);
                    },
                    error: function (x) {
                        if (errCall) errCall(x);
                    }
                });
        },
        jsonPost: function (url, query, success, error) {
            url += ((url || location.href).indexOf('?') == -1 ? '?' : '&') + 'json=1';
            this.request({ uri: url, params: query, method: 'POST', data: 'json' },
                {
                    success: function (json) {
                        if (success) success(json);
                    },
                    error: function (result) {
                        if (error) error(result);
                    }
                });
        }
    },

    //*************** COOKIE 操作 ***************//
    cookie: {
        //写入Cookie
        write: function (name, value, seconds) {
            var expire = "";
            if (seconds) {
                expire = new Date((new Date()).getTime() + seconds);
                expire = "; expires=" + expire.toGMTString();
            }
            document.cookie = name + "=" + escape(value) + expire;
        },
        //移除Cookie
        remove: function (name) {
            var exp = new Date();
            exp.setTime(exp.getTime() - 1);
            document.cookie = name + "=; expires=" + exp.toGMTString();
        },
        //读取Cookie
        read: function (name) {
            var cookieValue = "";
            var search = name + "=";
            if (document.cookie.length > 0) {
                var offset = document.cookie.indexOf(search);
                if (offset != -1) {
                    offset += search.length;
                    var end = document.cookie.indexOf(";", offset);
                    if (end == -1) end = document.cookie.length;
                    cookieValue = unescape(document.cookie.substring(offset, end))
                }
            }
            return cookieValue;
        }
    },

    json: {
        prefix: 'field',
        _objReg: /(.+)\[([^\]]+)\]/,
        //_dtReg2: /^\/(Date\\([^\\)]+\\))\/$/,
        _dtReg: /^(\d{4}((\/|-)\d{2}){2})T(\d{2}(:\d{2}){2})((\.\d+)*)$/i,

        _each: function (list, callback) {
            for (var i = 0; i < list.length; i++) {
                if (callback) callback(i, list[i]);
            }
        },
        _getFields: function (pl) {
            var pre = this.prefix;
            var fields = {};
            var node;
            var proName, subProName, proValue; //属性名称
            if (!pl.nodeName) pl = document.getElementById(pl);

            var objReg = this._objReg;

            this._each(pl.getElementsByTagName('*'), function (i, e) {
                if (e.nodeName != '#text' && e.nodeName != '#comment') {
                    proName = e.getAttribute(pre);
                    if (proName) {
                        if (objReg.test(proName)) {
                            var match = objReg.exec(proName);
                            proName = match[1];
                            subProName = match[2];

                            if (fields[proName] == null) {
                                fields[proName] = {};
                            }
                            fields[proName][subProName] = e;
                        } else {
                            fields[proName] = e;
                        }
                        if (!e.name) e.setAttribute('name', pre + '_' + proName);
                    }
                }
            });

            return fields;
        },
        _bindField: function (node, proValue) {
            if (this._dtReg.test(proValue)) {
                var match = this._dtReg.exec(proValue);
                if (match[4] == '00:00:00') {
                    //如果为00:00:00则只显示日期
                    proValue = proValue.replace(this._dtReg, '$1');
                } else {
                    proValue = proValue.replace(this._dtReg, '$1 $4');
                }

                /*
                 eval("proValue=new " + this._dtReg.exec(proValue)[1]);

                 var val = '';
                 var v = proValue.getMonth() + 1;
                 if (v <= 9) v = '0' + v;

                 val = proValue.getFullYear() + '-' + v + '-';
                 v = proValue.getDate();
                 if (v <= 9) v = '0' + v;

                 val += v + ' ' + proValue.toLocaleTimeString();
                 proValue = val;
                 */

            }


            switch (node.nodeName) {
                case 'TEXTAREA':
                case 'INPUT':
                    switch (node.type) {
                        default:
                            node.value = proValue;
                            break;
                        case "radio":
                            var radios = document.getElementsByName(node.name);
                            for (var i = 0; i < radios.length; i++) {
                                if (radios[i].value == proValue) {
                                    radios[i].setAttribute('checked', 'checked');
                                    break;
                                }
                            }
                            break;
                        case 'checkbox':
                            var isChecked = false;
                            if ((proValue == true && proValue.toString() != '1') || proValue == node.value) {
                                isChecked = true;
                            } else if (proValue.length) {
                                for (var i in proValue) {
                                    if (proValue[i] == node.value) {
                                        isChecked = true;
                                        break;
                                    }
                                }
                            }
                            if (isChecked) {
                                node.setAttribute('checked', 'checked');
                            } else {
                                node.removeAttribute('checked');
                            }
                            break;
                    }
                    break;
                case 'IMG':
                    node.src = proValue;
                    break;
                case 'SELECT':
                    node.value = proValue;
                    break;
                default:
                    node.innerHTML = proValue;
                    break;
            }
        },
        _getFieldVal: function (node) {
            var proValue = '';
            switch (node.nodeName) {
                case 'TEXTAREA':
                case 'INPUT':
                    switch (node.type) {
                        default:
                            proValue = node.value;//.replace('\'', '\\\'');
                            break;
                        case 'radio':
                            var radios = document.getElementsByName(node.name);
                            for (var i = 0; i < radios.length; i++) {
                                if (radios[i].checked) {
                                    proValue = radios[i].value;
                                    break;
                                }
                            }
                            break;
                        case 'checkbox':
                            proValue = node.checked ? node.value : '';
                            break;
                    }
                    break;
                case 'IMG':
                    proValue = node.src;
                    break;
                case 'SELECT':
                    proValue = node.selectedIndex == -1 ? '' : node.options[node.selectedIndex].value;
                    break;
                default:
                    proValue = node.innerHTML;//.replace('\'', '\\\'');
                    break;
            }
            return proValue;
        },
        bind: function (pl, json, formatter) {

            var fields;
            var node;
            var proValue;

            fields = this._getFields(pl);

            for (var proName in fields) {
                node = fields[proName];
                //获取格式化后的值
                if (formatter && formatter instanceof Function) {
                    proValue = formatter(proName, json[proName]);
                } else {
                    proValue = json[proName];
                }

                if (proValue != null) {
                    //如果为对象
                    if (proValue instanceof Object) {
                        //针对列表
                        if (proValue.length) {
                            for (var i in node) {
                                this._bindField(node[i], proValue);
                            }
                        } else {
                            for (var i in proValue) {
                                if (node[i]) {
                                    this._bindField(node[i], proValue[i]);
                                }
                            }
                        }

                        continue;
                    }
                    this._bindField(node, proValue);
                }
            }
        },
        __convert: function (pl, format, formatter) {
            var fields;
            var node;
            var proValue;
            var obj = {};
            var queryString = '';

            fields = this._getFields(pl);

            for (var proName in fields) {
                node = fields[proName];
                if (node.nodeName) {
                    proValue = this._getFieldVal(node);

                    //获取格式化后的值
                    if (formatter && formatter instanceof Function) {
                        proValue = formatter(proName, proValue);
                    }

                    obj[proName] = proValue;
                    queryString += proName + '=' + proValue + '&';

                } else {
                    //如果为对象
                    obj[proName] = {};
                    var j = 0;
                    var isArray = false;

                    for (var i in node) {
                        if (j++ == 0 && /^\d+$/.test(i)) {
                            obj[proName] = [];
                            isArray = true;
                        }
                        if (node[i]) {
                            proValue = this._getFieldVal(node[i]);

                            //获取格式化后的值
                            if (formatter && formatter instanceof Function) {
                                proValue = formatter(proName, proValue);
                            }

                            if (proValue && proValue != '') {
                                if (isArray) {
                                    obj[proName].push(proValue);
                                } else {
                                    obj[proName][i] = proValue;
                                }
                            }

                            queryString += proName + '[' + i + ']=' + proValue + '&';
                        }
                    }
                }
            }
            return format == "object" ? obj : queryString.replace(/&$/g, '');
        },
        toObject: function (pl) {
            return this.__convert(pl, 'object');
        },
        toQueryString: function (pl) {
            return this.__convert(pl, 'string');
        },
        //转为 " id:1;name:刘铭 " 这样的格式guyy
        toString: function (pl) {
            return this.__convert(pl, 'string').replace(/&/g, ';').replace(/=/g, ':');
        },
        //将对象转为json字符串
        string: function (o) {
            var _this = this;
            var arr = [];
            var fmt = function (s) {
                if (typeof s == 'object' && s != null) _this.string(s);
                return /^(string|number)$/.test(typeof s) ? "'" + s + "'" : s;
            };
            for (var i in o) {
                val = fmt(o[i]);
                if (val.pop) {
                    arr.push("'" + i + "':[\'" + escape(val.join('\',\'')) + '\']');
                } else {
                    arr.push("'" + i + "':" + escape(val));
                }
            }
            return '{' + escape(arr.join(',')) + '}';
        }
    },

    //返回元素距离屏幕的坐标(left,right,bottom,top)
    getPosition: function (e) {
        return (e.nodeName ? e : this.$(e)).getBoundingClientRect();
    },

    //*************** Load Plugin *******************//
    loadHTML: function (panel, html) {
        //附加HTML
        var bodyReg = /<body[^>]*>([\s\S]+)<\/body>/im;

        //检测脚本reg
        //获取脚本内容reg
        // reg = /<script(.|\n)*?(src=[^>]+)*>([\s\S]*?)<\/script>/igm;
        var scriptReg = /<script((.|\n)*?)>([\s\S]*?)<\/script>/gim;
        // reg = new RegExp('<script((.|\\n)*?)>([\\s\\S]*?)</script>', 'gim');

        var body = html.match(bodyReg);
        if (body == null) {
            body = ['', html];
        }

        if (!panel.nodeName) panel = this.dom.$(panel);

        //清除脚本并写入panel
        if (panel) {
            try {
                panel.innerHTML = body[1].replace(scriptReg, '').replace(/<style([^>]+)>/ig,
                    '<span style="display:none" class=\"forie\">_</span><style$1>');

                //IE要在style前加上元素
                this.each(panel.getElementsByClassName ?
                        panel.getElementsByClassName('forie') :
                        document.getElementsByClassName('forie', panel)
                    , function (i, e) {
                        panel.removeChild(e);
                    });

                //Chrome要添加到头部
                if (window.navigator.userAgent.indexOf('Chrome') != -1) {
                    this.each(panel.getElementsByTagName('STYLE'), function (i, e) {
                        panel.removeChild(e);
                        document.getElementsByTagName('HEAD')[0].appendChild(e);
                    });
                }
            } catch (ex) {

                //http://stackoverflow.com/questions/4729644/cant-innerhtml-on-tbody-in-ie
                //http://msdn.microsoft.com/en-us/library/ms533897(VS.85).aspx

                if (window.console) {
                    console.log(ex.message);
                }
            }
        }

        //执行脚本
        var spaceReg = /^[\n\s]+$/g;
        var regType = /type=["']*text\/javascript["']*/i;
        //scriptReg = /<script((.|\n)*?)>([\s\S]*?)<\/script>/im;

        var jsSection;
        scriptReg.lastIndex = 0;
        while ((jsSection = scriptReg.exec(html)) != null) {
            if (jsSection[1].indexOf(' type=') == -1
                || regType.test(jsSection[1])) {
                if (!spaceReg.test(jsSection[3])) { //不全为空
                    this.eval(jsSection[3]);
                }
            }
        }
    },

    load: function (panel, url, success, error) {
        (function (_this) {
            _this.xhr.get(url, function (result) {
                _this.loadHTML(panel, result);
                //成功回执
                if (success) {
                    success(result);
                }
            }, error);
        }(this));
    },

    //加载插件/库文件
    ld: function (libName, path) {
        (function (j, _path) { j.xhr.get({ uri: _path + libName + '.js', async: false, random: false }, function (script) { j.eval(script); }); }(this, path || this.__WORKPATH__));
    },

    //转换为Json对象
    toJson: function (str) {
        return window.toJson(str);
    },

    //动态执行脚本
    eval: function (code) {
        if (!code) return code;

        //IE系列
        if (window.execScript) {
            window.execScript(code);
        } else {
            var script = document.createElement('SCRIPT');
            script.setAttribute('type', 'text/javascript');
            script.text = code;
            document.head.appendChild(script);
            document.head.removeChild(script);
        }
        return code;
    }
};


js.plugin = js.extend;


js.prototype.ie6 = function () { return /MSIE\s*6\.0/.test(window.navigator.userAgent); };
js.prototype.path = function () { var d = document.domain, uri = location.href; d = uri.substring(uri.indexOf(d) + d.length); /*if has port*/return d.substring(d.indexOf("/")); };
js.prototype.val = function (id, val) { if (!val) return document.getElementById(id).value; else document.getElementById(id).value = val; };

//延迟执行方法，常用于ld异步执行
js.prototype.lazyRun = function (func, timer) { if (func) { setTimeout(func, timer || 120); } };

//Hover组件，用于下拉菜单或者元素支持:hover
js.prototype.hover = function (e, hoverFunc, leaveFunc) {
    if (!e.nodeName) e = this.$(e);
    var isIE6 = this.ie6();
    this.event.add(e, 'mouseover', (function (t) {
        return function () {
            if (isIE6) t.className += ' hover';
            if (hoverFunc) hoverFunc(t);
        };
    })(e));
    this.event.add(e, 'mouseout', (function (t) {
        return function () {
            if (isIE6) t.className = t.className.replace(' hover', '');
            if (leaveFunc) leaveFunc(t);
        };
    })(e));
};


//加载脚本依赖库
js.prototype.ldScript = function (scriptUrl, loadfunc, errorfunc) {
    var scriptPanel = null;
    var heads = document.documentElement.getElementsByTagName("HEAD");

    if (heads.length != 0) scriptPanel = heads[0];
    else scriptPanel = document.body;

    var scripts = scriptPanel.getElementsByTagName('SCRIPT');
    var hasLoaded = false;

    for (var i = 0; i < scripts.length; i++) {
        if (scripts[i].getAttribute('src') && scripts[i].getAttribute('src').toLowerCase() == scriptUrl.toLowerCase()) {
            hasLoaded = true;
        }
    }

    if (!hasLoaded) {
        var script = document.createElement('SCRIPT');

        if (loadfunc) script.onreadystatechange = script.onload = loadfunc;       //IE ReadStateChange
        if (errorfunc) script.onerror = errorfunc;

        script.setAttribute('type', 'text/javascript');
        script.setAttribute('src', scriptUrl);

        scriptPanel.appendChild(script);
    }
};


/************ 宽高 ***************/
js.prototype._width = function(e,client){
    e = this.$(e);
    var s = this.style(e);
    if(s["display"]!='none'){
        return client? e.clientWidth:e.offsetWidth;
    }
    var cls ={};
    for(var i in s){
        cls[i] = s[i];
    }
    this.style(e,{position:'absolute',visibility:'hidden',display:'block'});

    var w = client? e.clientWidth:e.offsetWidth;

    //restore
    this.style(e,cls);
    this.style(e,{display:'none'});
    return w;
};

js.prototype._height = function(e,client){
    e = this.$(e);
    var s = this.style(e);
    if(s["display"]!='none'){
        return client? e.clientHeight:e.offsetHeight;
    }
    var cls ={};
    for(var i in s){
        cls[i] = s[i];
    }
    this.style(e,{position:'absolute',visibility:'hidden',display:'block'});
    var h = client?e.clientHeight: e.offsetHeight;
     cls.display='none';
    //restore
    this.style(e,cls);
    this.style(e,{display:'none'});
    return h;
};

js.prototype.width =function(e){
    return this._width(e);
};

js.prototype.height = function(e){
    return this._height(e);
};

//获取宽度，不包含边框和滚动条
js.prototype.clientWidth =function(e){
    return this._width(e,true);
};

js.prototype.clientHeight = function(e){
    return this._height(e,true);
};

window.$JS = new js();
window.$JS.__init__();
window.J = $JS;