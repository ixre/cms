//
// global.js
//
// Author:
//  newmin (newmin.net@gmail.com)
//
// Copyright 2011 @ OPS Inc,All rights reseved!
//
/*--------------------------------------------
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
----------------------------------------------*/


/************* JavaScript  Extensions *****************/

//添加document.getElementsByClassName方法
if (!document.getElementsByClassName) {
    document.getElementsByClassName = function (className) {
        var elem=null;
        if (arguments.length > 1) {
            elem = arguments[1];
        }
        var reg = new RegExp('\\s' + className + '\\s');
        var arr = new Array();
        var elems = elem ? elem.getElementsByTagName('*') : document.body.getElementsByTagName('*');
        for (var i = 0, j; j = elems[i]; i++) {
            if (reg.test(' ' + j.className + ' ')) arr.push(j);
        }
        return arr;
    };
}


/************ 核心库 *******************/


function js() { }
js.prototype.$ = function (el, tagName, attrs) {
    var e = el.nodeName ? el : document.getElementById(el);
    if (!tagName) return e;
    e = e.getElementsByTagName(tagName);
    if (!attrs) return e;
    var arr = new Array();
    var attr_name;
    for (var i = 0; i < e.length; i++) {
        var equalAttr = true;
        for (var j in attrs) {
            switch (j) {
                case "className": attr_name = e[i].getAttribute("class") ? "class" : "className"; break;
                default: attr_name = j; break;
            }
            if (e[i].getAttribute(attr_name) != attrs[j]) equalAttr = false;
            if (equalAttr) arr.push(e[i]);
        }
    }
    return arr;
};
js.prototype.path = function () { var d = document.domain, uri = location.href; d = uri.substring(uri.indexOf(d) + d.length); /*if has port*/return d.substring(d.indexOf("/")); };
js.prototype.request = function (id, url) { var u = url ? url : location.search; var ps = u.substring(u.indexOf(id + "=") + (id.toString().length + 1)); if (ps.indexOf("&") == -1) return ps; else { return ps.substring(0, ps.indexOf("&")); } };
js.prototype.val = function (id, val) { if (!val) return document.getElementById(id).value; else document.getElementById(id).value = val; };
js.prototype.toJson = function (str) {
    //IE8,FF等其他浏览器
    if (window.JSON) {
        try {
            //如果json的键没有用引号引起来，则会抛出异常
            return JSON.parse(str);
        } catch (ex) {
        }
    }
    //使用旧的方法，容易产生内存泄露
    return eval('(' + str + ')');
};
//加载脚本依赖库
js.prototype.loadDependScript = function (scriptUrl) {
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
        script.setAttribute('type', 'text/javascript');
        script.setAttribute('src', scriptUrl);
        script.onreadystatechange = null;
        script.onerror = script.onload = null;
        scriptPanel.appendChild(script);
    }

};


/****************************************  对象 ****************************************/

//Event

//添加和移除事件监听
if (!document.addEventListener) {
    document.addEventListener = function (event, func, useCapture) {
        document.attachEvent('on' + event, func);
    };
    document.removeEventListener = function (event, func, useCapture) {
        document.detachEvent('on' + event, func);
    };
}

js.prototype.event = {
    //添加事件
    add: function (elem, event, func, b) {
        document.attachEvent ? elem.attachEvent('on' + event, func) : elem.addEventListener(event, func, b);
    },
    //移除事件
    remove: function (elem, event, func, b) {
        document.detachEvent ? elem.detachEvent('on' + event, func) : elem.removeEventListener(event, func, b);
    }
};

//Cookie操作
js.prototype.cookie = {
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
            offset = document.cookie.indexOf(search);
            if (offset != -1) {
                offset += search.length;
                end = document.cookie.indexOf(";", offset);
                if (end == -1) end = document.cookie.length;
                cookieValue = unescape(document.cookie.substring(offset, end))
            }
        }
        return cookieValue;
    }
};

//Ajax
var ajaxprocessing = 1;
js.prototype.ajax = {
    __process: function (func) {
        var t = setInterval(function () {
            if (ajaxprocessing == 1) {
                ajaxprocessing = 0;

                func();

                clearInterval(t);
            }
        }, 10);
    },
    get: function (uri, callback, errorFunc) {
        this.__process(function () {
            new ajaxObj().request({ uri: uri }, {
                success: function (x) {
                    ajaxprocessing = 1;
                    if (callback) callback(x);
                }, error: function (x) {
                    ajaxprocessing = 1;
                    if (errorFunc) errorFunc(x);
                }
            });
        });
    },
    post: function (uri, param, okCall, errCall) {
        this.__process(function () {
            new ajaxObj().request({ uri: uri, method: 'POST', params: param },
                        { success: function (x) {
                            ajaxprocessing = 1;
                            if (okCall) okCall(x);
                        },
                            error: function (x) {
                                ajaxprocessing = 1;
                                if (errCall) errCall(x);
                            }
                        });
        });
    }
};

/*--------AJAX---------------*/
function ajaxObj() {
    //创建XMLHttpRequest对象
    this.http = window.XMLHttpRequest ?
        new XMLHttpRequest() :
        (new ActiveXObject("MSXML2.XMLHTTP") || new ActiveXObject("MICROSOFT.XMLHTTP"));
}
//请求范例
//ajax.request({uri:"/",method:"POST",params:"",data:"text"},
//{start:function(){},over:function(){},error:function(x){//处理错误},success:function(x){}});
//x为返回的数据*/
ajaxObj.prototype.request = function (req, call) {
    this.uri = req.uri;
    //请求参数
    //method为"POST"时适用
    //格式如:'action=delete&id=123'
    this.params = req.params;

    //请求的方法,POST和GET
    this.method = req.method == null || req.method.toLowerCase() != "post" ? "GET" : "POST";

    //返回数据格式,Text|XML
    this.data = req.data == null || req.data.toLowerCase() != "xml" ? "text" : "xml";

    //开始请求时候调用函数
    if (call.start != null) call.start();

    //发送GET请求时候添加随机数
    if (this.method != "POST") {
        if (this.uri.indexOf("?") == -1) this.uri += "?t=" + Math.random();
        else this.uri += "&t=" + Math.random();
    }

    //引用变量
    var _aj = this;

    //同步线程:true
    this.http.open(this.method, this.uri, true);

    //请求状态发生变化时执行
    this.http.onreadystatechange = function () {
        if (_aj.http.readyState == 4) {
            if (_aj.http.status == 200) {
                if (call.success) call.success(_aj.data == "xml" ? _aj.http.responseXML : _aj.http.responseText);
            } else if (call.error) call.error(_aj.http.responseText);
        }
    };
    //如果为POST请求
    if (this.method == "POST") this.http.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");

    //发送请求
    this.http.send(this.params);
};

/*
* 名称 ： 表单(使用框架)
* 修改说明：
*      2012-09-22  newmin [+] : GetData方法用于获取表单内部的数据
*/
js.prototype.form = {
    //获取表单数据
    getData: function (formIdOrIndex) {
        var data = '';
        var form = document.forms[formIdOrIndex || 0];

        //迭代操作函数
        var foreach = function (arr, func) {
            for (var i = 0; i < arr.length; i++) {
                func(arr[i]);
            }
        };

        //用于装载input type="radio"
        var __eles = new Array();

        //查找是否存在名字
        var findName = function (name) {
            for (var i = 0; i < __eles.length; i++) {
                if (__eles[i] == name) {
                    return true;
                }
            }
            return false;
        };

        foreach(form, function (ele) {
            var attr_name = ele.getAttribute('name');
            if (ele.nodeName == 'IMG') {
                // alert(attr_name);
            }
            if (attr_name) {
                //针对Radio单独做处理
                if (ele.nodeName == 'INPUT' && ele.type && ele.type == 'radio') {
                    if (!findName(attr_name)) {
                        foreach(document.getElementsByName(attr_name), function (e) {
                            if (e.checked) {
                                data += '&' + attr_name + '=' + encodeURIComponent(e.value);
                                __eles.push(attr_name);
                            }
                        });
                    }
                }
                //<input type="checkbox"/>无论如何都为on，所以需单独处理
                else if (ele.nodeName == 'INPUT' && ele.type && ele.type == 'checkbox') {
                    data += '&' + attr_name + '=' + encodeURIComponent(ele.checked ? ele.value : '');
                }
                else {
                    data += '&' + attr_name + '=' + encodeURIComponent(ele.value);
                }
            }
            else {
                /*自定义标签取消
                var field = ele.getAttribute('field');
                if (field) {
                data += '&' + field + '=' + escape(ele.getAttribute('value'));
                }
                */
            }
        });

        return data.replace('&', '');
    },
    //异步提交
    asyncSubmit: function (formIdOrIndex, showTarget) {
        var form = document.forms[formIdOrIndex || 0];
        var $async_ifr = document.getElementById('$async_ifr');

        //添加DOM元素
        if (!$async_ifr) {
            try {
                //IE核心
                $async_ifr = document.createElement('<iframe name="$async_ifr">');
            } catch (ex) {
                //非IE浏览器
                $async_ifr = document.createElement('iframe');
                $async_ifr.setAttribute('name', '$async_ifr');
            }
            $async_ifr.setAttribute('id', '$async_ifr');

            if (!showTarget) {
                $async_ifr.style.cssText = 'display:none';
            } else {
                $async_ifr.style.cssText = 'width:600px;height:400px';
            }
            document.body.insertBefore($async_ifr, document.body.firstChild);
        }

        //设置表单目标
        if (form.getAttribute('target') != $async_ifr.name) {
            form.setAttribute('target', $async_ifr.getAttribute('name'));
        }

        //提交表单
        form.submit();
    }
};

/*
* 名称 ： UI库
* 创建时间：2012-09-22
*/
js.prototype.ui = {
    dynamicTable: function (table) {
        if (table && table.nodeName === "TABLE") {
            var rows = table.getElementsByTagName("tr");
            for (var i = 0; i < rows.length; i++) {
                if (i % 2 == 1) if (!rows[i].className) rows[i].className = 'even';
                rows[i].onmouseover = function () {
                    this.className = this.className.indexOf('even') != -1 ? "hover even" : "hover";
                };
                rows[i].onmouseout = function () {
                    this.className = this.className.indexOf("even") == -1 ? "" : "even";
                };
            }
        }
    },
    //验证器(2012-09-30)
    validator: {
        //设置提示
        setTip: function (e, success, summaryKey, msg) {
            var p = e.parentNode;
            var tipEle = e.nextSibling;

            //找到下一个类为"validator"的元素
            while (tipEle && tipEle.nodeType != 1) {
                tipEle = tipEle.nextSibling;
            }


            if (tipEle == null || tipEle.getAttribute('class') != 'validator') {
                //如果未指定显示元素，则设置父元素为relative定位
                if (p.style.cssText.indexOf('position') == -1) {
                    if (p.style.position) {
                        p.style.position = 'relative';
                    } else {
                        p.style.cssText += 'position:relative;';
                    }
                }
                tipEle = document.createElement('span');
                tipEle.setAttribute('class', 'validator');
                tipEle.style.cssText = 'position:absolute;top:' + e.offsetTop + 'px;left:' + (e.offsetLeft + e.offsetWidth + 5) + 'px';

                if (e.nextSibing) {
                    p.insertBefore(tipEle, e.nextSibling);
                } else {
                    p.appendChild(tipEle);
                }
            }

            //根据键值获取提示信息
            if (summaryKey) {
                var summary = e.getAttribute('summary');
                this.msg;
                if (summary) {
                    eval('summary=' + summary + ';');
                    eval('if(summary.' + summaryKey + '){this.msg=summary.' + summaryKey + ';}');
                    if (this.msg) {
                        msg = this.msg;
                    }
                }
            }

            //设置值
            tipEle.innerHTML = '<span class="' + (success ? 'valid-right' : 'valid-error') + '">' + msg + '</span>';

        },
        //移除提示
        removeTip: function (e) {
            var p = e.parentNode;
            var eles = p.getElementsByTagName('SPAN');
            for (var i = 0; i < eles.length; i++) {
                if (eles[i].className.indexOf('valid-') != -1) {
                    p.removeChild(eles[i].parentNode);
                }
            }
        },
        //验证结果
        result: function (id) {
            var eles;
            //指定了父元素
            if (id) {
                var ele = document.getElementById(id);
                eles = ele.getElementsByClassName ? ele.getElementsByClassName('valid-error') : document.getElementsByClassName('valid-error', ele);
            } else {
                eles = document.getElementsByClassName('valid-error');
            }

            return eles.length == 0;
        },

        //初始化事件
        init: function () {
            var eles = document.getElementsByClassName('ui-validate');
            for (var i = 0; i < eles.length; i++) {

                //不能为空
                if (eles[i].getAttribute('required') == 'true') {
                    eles[i].onblur = (function (validator, e) {
                        return function () {
                            if (e.value.replace(/\s/g, '') == '') {
                                validator.setTip(e, false, 'required', '该项不能为空');
                            } else {
                                validator.removeTip(e);
                            }
                        };
                    })(this, eles[i]);
                }

                //长度限制
                if (eles[i].getAttribute('length')) {
                    eles[i].onblur = (function (validator, e) {
                        return function () {
                            var pro_val = e.getAttribute('length');
                            var reg = /\[(\d*),(\d*)\]/ig;
                            var l_s = parseInt(pro_val.replace(reg, '$1')),
                                l_e = parseInt(pro_val.replace(reg, '$2'));

                            if (e.value.length < l_s) {
                                validator.setTip(e, false, 'length', l_e ? '长度必须为' + l_s + '-' + l_e + '位' : '长度必须大于' + (l_s - 1) + '位');
                            } else if (e.value.length > l_e) {
                                validator.setTip(e, false, 'length', l_s ? '长度必须为' + l_s + '-' + l_e + '位' : '长度必须小于' + (l_e + 1) + '位');
                            }
                            else {
                                validator.removeTip(e);
                            }
                        };
                    })(this, eles[i]);

                }
                //只能输入数字
                if (eles[i].getAttribute('isnumber') == 'true') {

                    eles[i].style.cssText += 'ime-mode:disabled';
                    eles[i].onchange = eles[i].onkeyup = (function (validater, e) {
                        return function () {
                            if (/\D/.test(e.value)) {
                                e.value = e.value.replace(/\D/g, '');
                            }
                            e.value = e.value.replace(/^0([0-9])/, '$1');
                        };
                    })(this, eles[i]);
                }
            }
        },

        validate: function (id) {
            var eles;
            if (id) {
                //指定了父元素
                var ele = document.getElementById(id);
                eles = ele.getElementsByClassName ?
                         ele.getElementsByClassName('ui-validate')
                         : document.getElementsByClassName('ui-validate', ele);
            } else {
                //所有元素，未指定父元素
                eles = document.getElementsByClassName('ui-validate');
            }

            for (var i = 0; i < eles.length; i++) {
                if (eles[i].onblur) {
                    eles[i].onblur();
                }
            }
            return this.result(id);
        }
    }
};


/* 
* name     : js.load()
* author   : newmin
* date     : 2010/11/07 10:09
* description:使用ajax加载一个页面到容器
* --parameters explan------------
*   e:页面容器
*   r:ajax的request对象
*   r.time:ajax请求成功后填充内容到容器等待的时间
*   callback.start(): 请求开始执行的函数(可用于提示加载中)
*   callback.over(): ajax成功请求完成后执行的函数
*   callback.error(): 请求发生错误执行函数
*/
/*
js.prototype.load = function (e, r, callback) {
if (!r.uri) alert('page uri parameters not be found!');
var prevTag = "ops-load-";
var width = e.style.width || j.x() + "px", height = e.style.height || j.y() + "px";
var aj = new ajax();
aj.request(r, {
start: function () {
e.innerHTML = '<div id="' + prevTag + '-div" style="background:#e0e0e0;font-size:14px;' +
'width:250px;line-height:40px;color:green;text-align:center;margin:' +
(parseInt(height.replace("px", "")) - 40) / 2 + 'px ' +
(parseInt(width.replace("px", "")) - 160) / 2 + 'px;">加载中</div>';
// 将div传递给callback.start方法 
if (callback && callback.start) callback.start(j.$(prevTag + "-div"));
},
error: function (x) {
var _div = j.$(prevTag + "-div");
if (callback && callback.error) callback.error(_div);
else e.innerHTML = x;
},
success: function (x) {
var func = function () {
e.innerHTML = "<iframe name='" + prevTag + "-iframe' frameborder='0' scrolling='no' style='width:" +
width + ";height:" + height + ";overflow:hidden;margin:0;padding:0;'></iframe>";
var ifr = window.frames[prevTag + "-iframe"];
ifr.src = 'about:blank'; ifr.document.write(x);
//执行成功后触发函数
if (callback && callback.over) callback.over();
//为其连接添加目标,功能完善中
//                var as=ifr.document.getElementsByTagName("a");
//                for(var i in as){
//                as[i].onclick=function(){alert('f');
//                if(this.href.indexOf("javascript:")==-1&&this.target!="_blank")this.target="_parent";alert(this.target);
//                }
//                }
};
if (r.time) { var t = new timer(r.time, func); t.start(); }
else func();
}
});
};

*/



window.j = new js();