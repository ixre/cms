
//
// jr.core.js
//
// author:
//  jarrysix@gmail.com
//
// Copyright 2011 - 2018 @ TO2.NET,All rights reserved!
// packer: http://dean.edwards.name/packer/ 
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
 * 2013-07-09  jr.load实现
 * 2013-10-24  调整contextmenu右键
 * 2016-12-10  重构代码,添加FN
 * 2016-12-17  去掉toJson
 * 2017-06-30  $fn.text()方法
 * 2019-03-25  $.animate方法

 ----------------------------------------------*/

function JR() {
    //版本号
    this.VERSION = '4.0';
    //工作路径
    this.WORKPATH = '';
    this._init = function(_Extend_PROTOTYPE) {
         //设置当前版本
         var _scripts = document.getElementsByTagName('SCRIPT');
         var s = _scripts[_scripts.length - 1];
         var _sloc = s.src;
         //s.src=_sloc+'#ver='+this.VERSION;
         //获取工作目录
         this.WORKPATH = _sloc.replace(/(\/)[^/]+$/, '$1');
         //扩展原生JS
         _Extend_PROTOTYPE && this.__extendingJsPrototype__();
         return this;
    };
    this.selector = {
        // 根据表达式查找对象,parent为父对象
        //finds: function(expr, parent) {
        finds: function(expr, parent) {
            expr = expr.replace(/^\s|\s$/g, '').replace(/\s+/, ' ');
            var c = parent && parent.nodeName ? [parent] : (parent || [document]);
            var d = expr.split(' ');
            return this.walk(c, d, 0)
        },
       //findBy: function(arr, finder) {
        findBy: function(a, b) {
            var c = [];
            for (var i = 0; i < a.length; i++) {
                var d = b(a[i]) || [];
                for (var j = 0; j < d.length; j++) {
                    c.push(d[j])
                }
            }
            return c
        },
        walk: function(a, b, c) {
            var d = [];
            var f = b[c];
            switch (f[0]) {
            case "#":
                d = this.findBy(a,
                function() {
                    var e = document.getElementById(f.substring(1));
                    return e ? [e] : []
                });
                break;
            case ".":
                d = this.findBy(a, (function(t) {
                    return function(p) {
                        return p.getElementsByClassName(f.substring(1))
                    }
                })(this));
                break;
            default:
                d = this.findBy(a,
                function(p) {
                    return p.getElementsByTagName(f)
                });
                break
            }
            if (c < b.length - 1) {
                return this.walk(d, b, c + 1)
            }
            return d
        }
    };
    this.fn = {
        _eventArray: ["abort", "blur", "change", "click", "dblclick", "error", "focus", "keydown", "keypress", "keyup", "load", "mousedown", "mousemove", "mouseout", "mouseover", "mouseenter", "mouseup", "touchstart", "touchmove", "touchend", "touchcancel", "reset", "resize", "select", "submit", "unload"],
        g: {},
        eleList: [],
        fnList: null,
        fnProps: {
            'check': ['checked', true],
            'disabled': ['disabled', true],
            'uncheck': ['checked', false],
        },
        aniFn: ["fadeIn", "fadeOut", "fadeTo", "fadeToggle", "slideUp", "slideDown", "slideToggle"],
        _fn: function(a) {
            return this.create(a, this.g)
        },
        extend: function(a) {
            return this.g.extend.apply(this, a)
        },
        create: function(a, g) {
            return Object.create(this).init(a, g)
        },
        init: function(e, g) {
            this.g = g;
            this.fnList = null;
            if (typeof(e) == "string") {
                this.eleList = this.g.selector.finds(e)
            } else if (e instanceof Array || e instanceof HTMLCollection) {
                this.eleList = e
            } else if (e.nodeName) {
                this.eleList = [e]
            }
            var f = this;
            this.g.each(this._eventArray,
            function(i, e) {
                f[e] = (function(e) {
                    return function(a, b) {
                        return f.event(e, a, b != false)
                    }
                })(e)
            });
            for (var k in this.fnProps) {
                this[k] = (function(k, a) {
                    return function() {
                        return a._rawCaller(function(e) {
                            var p = a.fnProps[k];
                            e[p[0]] = p[1]
                        })
                    }
                })(k, this)
            }
            this.g.each(this.aniFn,
            function(i, d) {
                f[d] = (function(c, j) {
                    return function(a, b) {
                        return j._caller(function(e) {
                            j.g.animation[c](e, a, b)
                        })
                    }
                })(d, f)
            });
            return this
        },
        _single: function() {
            if (this.len() > 0) {
                return this.eleList[0]
            }
            return null
        },
        _chkFnList: function() {
            with(this) {
                if (fnList) return;
                fnList = [];
                for (var i = 0,
                j = len(); i < j; i++) {
                    fnList.push(_fn([eleList[i]]))
                }
            }
        },
        _factFn: function(i, b) {
            return (function(p) {
                return function(a) {
                    b.apply(p, [a])
                }
            })(this.get(i))
        },
        each: function(a) {
            this._chkFnList();
            this.g.each(this.fnList, a);
            return this
        },
        get: function(i) {
            this._chkFnList();
            if (i >= 0 && i <= this.fnList.length - 1) {
                return this.fnList[i]
            }
            return this._fn(null)
        },
        first: function() {
            return this.get(0)
        },
        last: function() {
            var l = this.len();
            return l == 0 ? null: this.get(l - 1)
        },
        _caller: function(c) {
            for (var i = 0,
            j = this.len(); i < j; i++) {
                c.call(this, this._fn(this.eleList[i]), i)
            }
            return this
        },
        _rawCaller: function(c) {
            for (var i = 0,
            j = this.len(); i < j; i++) {
                c.call(this, this.eleList[i], i)
            }
            return this
        },
        event: function(b, c, d) {
            var f = this;
            return this._rawCaller(function(e, i) {
                if (c) {
                    var a = f._factFn(i, c);
                    if (d) {
                        this.g.event.add(e, b, a)
                    } else {
                        e["on" + b] = a
                    }
                } else {
                    e["on" + b]()
                }
            })
        },
        raw: function() {
            if (this.len() == 1) {
                return this.eleList[0]
            }
            return this.eleList
        },
        elem: function() {
            return this.raw()
        },
        len: function() {
            return this.eleList.length
        },
        find: function(a) {
            var b = this.g.selector.finds(a, this.eleList);
            return this._fn(b)
        },
        parent: function() {
            var e = this._single();
            if (e) {
                e = e.parentNode;
                return this._fn(e)
            }
            return null
        },
        prev: function() {
            var e = this._single().previousSibling;
            if (e) return this._fn(e);
            return null
        },
        next: function() {
            var e = this._single().nextSibling;
            if (e) return this._fn(e);
            return null
        },
        attr: function(a, v) {
            if (v == null) {
                if (a instanceof Object) {
                    for (var p in a) {
                        this._setAttr(p, a[p])
                    }
                    return this
                }
                var e = this._single();
                return e[a] || e.getAttribute(a)
            }
            this._setAttr(a, v);
            return this
        },
        _setAttr: function(a, v) {
            var b = typeof(v) == "boolean" || a === "value" || a.indexOf("inner") == 0 || a.indexOf("scroll") == 0 || a.indexOf("offset") == 0;
            return this._rawCaller(function(e) {
                b ? e[a] = v: e.setAttribute(a, v)
            })
        },
        removeAttr: function(a) {
            return this._rawCaller(function(e) {
                e.removeAttribute(a)
            })
        },
        css: function(a) {
            if (! (a instanceof Object)) {
                var e = this._single();
                return e.currentStyle || document.defaultView.getComputedStyle(e, null)
            }
            for (var s in a) {
                var b = s.split("-");
                for (var i = 1; i < b.length; i++) {
                    b[i] = b[i].replace(b[i].charAt(0), b[i].charAt(0).toUpperCase())
                }
                var c = b.join('');
                this._rawCaller(function(e) {
                    e.style[c] = a[s]
                })
            }
            return this
        },
        hasClass: function(c) {
            var a = new RegExp('(\\s|^)' + c + '(\\s|$)');
            return this._single().className.match(a) ? true: false
        },
        addClass: function(c) {
            return this.each(function(i, e) {
                if (!e.hasClass(c)) e.raw().className += ' ' + c
            })
        },
        removeClass: function(c) {
            var a = new RegExp('(\\s|^)' + c + '((\\s)|$)');
            this._rawCaller(function(e) {
                e.className = e.className.replace(a, "$3")
            });
            return this
        },
        position: function() {
            return this._single().getBoundingClientRect()
        },
        html: function(v) {
            return this.attr("innerHTML", v)
        },
        text: function(v) {
            return this.attr("innerText", v)
        },
        before: function(a) {
            return this._rawCaller(function(e) {
                e.parentNode && e.parentNode.insertBefore(a, e)
            })
        },
        after: function(a) {
            return this._rawCaller(function(e) {
                e.parentNode && e.parentNode.insertBefore(a, e.nextSibling)
            })
        },
        append: function(e) {
            return this._rawCaller(function(p) {
                if (e.raw) p.appendChild(e.raw());
                else if (e.nodeName) p.appendChild(e);
                else if (typeof(e) == "string") p.innerHTML += e
            })
        },
        appendTo: function(e) {
            e.append(this)
        },
        val: function(v) {
            if (v == null) return this.attr("value");
            this.attr("value", v);
            return this
        },
        remove: function() {
            var a = this;
            this.fnList = null;
            return this._rawCaller(function(e, i) {
                a.eleList.remove(i);
                try {
                    e.parentNode.removeChild(e);
                    e.outerHTML = ""
                } catch(err) {
                    console.log(err)
                }
            })
        },
        animate: function(a, b, c) {
            this._caller.apply(this, [function(e) {
                this.g.animation.animate(e, a, b, c)
            }])
        }
    };
    this.loadHTML = function(a, b) {
        var c = /<body[^>]*>([\s\S]+)<\/body>/im;
        var d = /<script((.|\n)*?)>([\s\S]*?)<\/script>/gim;
        var f = b.match(c);
        if (f == null) {
            f = ['', b]
        }
        if (!a.nodeName) a = this.$(a).elem();
        if (a) {
            try {
                a.innerHTML = f[1].replace(d, '').replace(/<style([^>]+)>/ig, '<span style="display:none" class=\"for-ie\">_</span><style$1>');
                this.$fn(a).find(".for-ie").each(function(i, e) {
                    e.remove()
                });
                if (window.navigator.userAgent.indexOf('Chrome') != -1) {
                    this.each(a.getElementsByTagName('STYLE'),
                    function(i, e) {
                        a.removeChild(e);
                        document.getElementsByTagName('HEAD')[0].appendChild(e)
                    })
                }
            } catch(ex) {
                if (window.console) {
                    console.log(ex.message)
                }
            }
        }
        var g = /^[\n\s]+$/g;
        var h = /type=["']*text\/javascript["']*/i;
        var j;
        d.lastIndex = 0;
        while ((j = d.exec(b)) != null) {
            if (j[1].indexOf(' type=') == -1 || h.test(j[1])) {
                if (!g.test(j[3])) {
                    this.eval(j[3])
                }
            }
        }
    }
}
function __extendingJsPrototype__() {
    String.prototype.len = function(a) {
        return this.replace(a ? /[\u0391-\uFFE5]/g: /[^x00-xff]/g, "00").length
    };
    Array.prototype.remove = function(a, b) {
        var c = this.slice((b || a) + 1 || this.length);
        this.length = a < 0 ? this.length + a: a;
        return this.push.apply(this, c)
    };
    if (!Object.create) {
        Object.create = function(o) {
            function $() {}
            $.prototype = o;
            return new $()
        }
    }
    if (typeof(HTMLElement) != "undefined") {
        const prototype = {
            getElementsByClassName: function(a) {
                var b = this.getElementsByTagName('*');
                var c = new RegExp('\\s' + a + '\\s');
                var d = [];
                for (var i = 0,
                j; j = b[i]; i++) {
                    if (c.test(' ' + j.className + ' ')) d.push(j)
                }
                return d
            },
            contains: function(a) {
                while (a != null && typeof(a.tagName) != "undefined") {
                    if (a == this) return true;
                    a = a.parentNode
                }
                return false
            },
            computedStyle: function() {
                return this.currentStyle || document.defaultView.getComputedStyle(this, null)
            },
            setStyle: function(a) {
                var b = {};
                for (var i in a) {
                    b[i] = this.style[i];
                    this.style[i] = a[i]
                }
                return b
            },
            restoreStyle: function(a) {
                for (var i in a) this.style[i] = a[i]
            },
            realEval: function(a) {
                var b = this.setStyle({
                    "display": "inherit",
                    "visibility": "hidden",
                    "position": "absolute",
                    "height": "auto",
                    width: "auto"
                });
                var c = a(this);
                this.restoreStyle(b);
                return c
            }
        };
        for (var p in prototype) {
            HTMLElement.prototype[p] == undefined && (HTMLElement.prototype[p] = prototype[p])
        }
    }
    Date.prototype.format = function(a) {
        var o = {
            "M+": this.getMonth() + 1,
            "d+": this.getDate(),
            "Ｈ+": this.getHours(),
            "h+": this.getHours() % 12,
            "m+": this.getMinutes(),
            "s+": this.getSeconds(),
            "q+": Math.floor((this.getMonth() + 3) / 3),
            "S": this.getMilliseconds()
        };
        if (/(y+)/.test(a)) {
            a = a.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length))
        }
        for (var k in o) {
            if (new RegExp("(" + k + ")").test(a)) {
                a = a.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)))
            }
        }
        return a
    }
}
JR.prototype = {
    request: function(a, b) {
        return this.param(a, b)
    },
    path: function() {
        var a = location.domain || "";
        var b = location.href;
        a = b.substring(b.indexOf(a) + a.length);
        return a.substring(a.indexOf("/"))
    },
    param: function(a, b) {
        var c = new RegExp('(\\?|&)' + a + '=([^&]+)&*').exec(b || location.href);
        return c ? c[2] : ''
    },
    eval: function(a) {
        if (window.execScript) {
            window.execScript(a)
        } else {
            var b = document.createElement('SCRIPT');
            b.setAttribute('type', 'text/javascript');
            b.text = a;
            document.head.appendChild(b);
            document.head.removeChild(b)
        }
        return a
    },
    each: function(a, b) {
        if (!a) return;
        for (var i = 0; i < a.length; i++) b(i, a[i])
    },
    template: function(a, b) {
        if (b instanceof Object) {
            for (var n in b) {
                var c = new RegExp('%' + n + '%|\{' + n + '\}');
                a = a.replace(c, b[n])
            }
        }
        return a
    },
    extend: function(a) {
        if (a && a instanceof Object) {
            for (var b in a) {
                this[b] == undefined && (this[b] = a[b])
            }
        }
        return this
    },
    $fn: function(a) {
        return this.$(a)
    },
    $: function(a) {
        return this.fn.create(a, this)
    },
};
JR.prototype.animation = {
    speedSet: {
        "slow": 10,
        "normal": 6,
        "fast": 3
    },
    _titAttr: function(a) {
        var b = a.split("-");
        for (var i = 1; i < b.length; i++) {
            b[i] = a[i].replace(b[i].charAt(0), b[i].charAt(0).toUpperCase())
        }
        return b.join('')
    },
    _ani: function(d, e, f, g, h) {
        if (typeof(e) != "integer") {
            e = this.speedSet[e || "normal"]
        }
        var i = 0;
        var j = setInterval(function() {
            var a = f();
            var b = (d - a) / e;
            b = b > 0 ? Math.floor(b) : Math.ceil(b);
            var c = i > 0 && i < Math.abs(b);
            if (Math.abs(b) == 0 || c) {
                clearInterval(j);
                g(d);
                if (h instanceof Function) h()
            } else {
                g(a + b);
                i = Math.abs(b)
            }
        },
        20);
        return j
    },
    animate: function(d, f, g, h) {
        var e = d.raw ? d.raw() : d;
        var i = e.computedStyle();
        var j = {};
        var k = function(e, a, b) {
            return function() {
                if (b === "opacity") {
                    return e["filters"] ? e["filters"]["opacity"] : parseFloat(a["opacity"]) * 100
                }
                return parseFloat(a[b])
            }
        };
        var l = function(e, a) {
            return function(v) {
                if (a === "opacity") {
                    e["filters"] ? e["filters"]["opacity"] = v: e.style[a] = v / 100
                } else {
                    e.style[a] = v + "px"
                }
            }
        };
        var m = (function(t, b, c) {
            return function() {
                for (var a in j) {
                    if (k(e, i, a)() != j[a]) return
                }
                if (c && c instanceof Function) c.apply(b, [])
            }
        })(this, d, h);
        for (var n in f) {
            var o = this._titAttr(n);
            var p = k(e, f, o)();
            j[o] = p;
            this._ani(p, g, k(e, i, o), l(e, o), m)
        }
    },
    toggle: function(e, a, b) {
        e = e.raw ? e.raw() : e;
        e.style["overflow"] = "hidden";
        var c = e.realEval(function(e) {
            return [e.clientWidth, e.clientHeight]
        });
        var d = c[0];
        var f = c[1];
        var g = 1;
        if (f == e.offsetHeight) {
            d = 0;
            f = 0;
            g = 0
        }
        this.animate(e, {
            "width": d + "px",
            "height": f + "px",
            "opacity": g,
        },
        a, b)
    },
    fadeTo: function(e, o, a, b) {
        this.animate(e, {
            "opacity": o
        },
        a, b)
    },
    fadeIn: function(e, a, b) {
        this.fadeTo(e, 1, a, b)
    },
    fadeOut: function(e, a, b) {
        this.fadeTo(e, 0, a, b)
    },
    fadeToggle: function(e, a, b) {
        var d = e.raw ? e.raw() : e;
        var c = d.computedStyle();
        var f = parseFloat(d.filters ? d.filters["opacity"] : c["opacity"]);
        this.fadeTo(d, f < 1 ? 1 : 0, a, b)
    },
    slideDown: function(e, a, b) {
        var c = e.raw ? e.raw() : e;
        var d = c.realEval(function(e) {
            return e.clientHeight
        });
        this.animate(c, {
            "height": d + "px"
        },
        a, b)
    },
    slideUp: function(e, a, b) {
        this.animate(e, {
            "height": "0"
        },
        a, b)
    },
    slideToggle: function(e, a, b) {
        var c = e.raw ? e.raw() : e;
        var d = c.realEval(function(e) {
            return e.clientHeight
        });
        var f = c.offsetHeight;
        this.animate(c, {
            "height": (f != d ? d: 0) + "px"
        },
        a, b)
    }
};
JR.prototype.cookie = {
    write: function(a, b, c, d, e, f) {
        var g = c ? new Date((new Date()).getTime() + c) : null;
        this.set(a, b, g, d, e, f)
    },
    set: function(a, b, c, d, e, f) {
        document.cookie = a + "=" + escape(b) + ((c) ? "; expires=" + c.toGMTString() : "") + ((d) ? "; path=" + d: "") + ((e) ? "; domain=" + e: "") + ((f) ? "; secure": "")
    },
    remove: function(a) {
        this.write(a, "", -9)
    },
    read: function(a) {
        var b = "";
        var c = a + "=";
        if (document.cookie.length > 0) {
            var d = document.cookie.indexOf(c);
            if (d !== -1) {
                d += c.length;
                var e = document.cookie.indexOf(";", d);
                if (e === -1) e = document.cookie.length;
                b = unescape(document.cookie.substring(d, e))
            }
        }
        return b
    }
};
JR.prototype.event = {
    add: function(a, b, c, d) {
        if (!a.attachEvent && !a.addEventListener) {
            alert('event error! parameter:' + ele + ',event:' + b);
            return
        }
        document.attachEvent ? a.attachEvent('on' + b, c) : a.addEventListener(b, c, d || true)
    },
    remove: function(a, b, c, d) {
        document.detachEvent ? a.detachEvent('on' + b, c) : a.removeEventListener(b, c, d || true)
    },
    src: function(a) {
        var e = a ? a: window.event;
        return e.target || e.srcElement
    },
    getTarget: function(a) {
        var e = a ? a: window.event;
        return e.target || e.srcElement
    },
    getRelatedTarget: function(a) {
        if (a.relatedTarget) return a.relatedTarget;
        if (a.toElement) return a.toElement;
        if (a.fromElement) return a.fromElement;
        return null
    },
    stopBubble: function(a) {
        a = a || window.event;
        a.cancelBubble = true;
        if (a.stopPropagation) {
            a.stopPropagation()
        }
    },
    preventDefault: function(a) {
        a = a || window.event;
        if (a.preventDefault) {
            a.preventDefault()
        } else {
            a.returnvalue = false
        }
    }
};
var paramsToString = function(a) {
    if (a instanceof Object) {
        var b = '';
        var i = 0;
        for (var c in a) {
            if (i++!==0) {
                b += '&'
            }
            b += c + '=' + encodeURIComponent(a[c])
        }
        return b
    }
    return a
};
JR.prototype.xhr = {
    maxRequest: 4,
    filter: function(a, b, c) {
        if (a == 2) {
            var d = /<title>(.+)<\/title>/.exec(c);
            if (d && d.length > 0) {
                alert(d[1]);
                return false
            }
        }
        return true
    },
    httpStack: null,
    procStack: [],
    init: function() {
        if (this.httpStack) return;
        this.httpStack = [];
        for (var i = 0; i < this.maxRequest; i++) {
            this.httpStack[i] = {
                state: 0,
                http: new XMLHttpRequest(),
            }
        }
    },
    getUrl: function(a, b, c) {
        if (a == null || a == '') a = location.href;
        if (b == 'GET' && c != false && a.indexOf('#') == -1) {
            a = this.urlJoin(a, 'rd=' + Math.random())
        }
        return a
    },
    _doRequest: function(f, g) {
        this.init();
        var h = (f.method || "GET").toUpperCase();
        var i = {
            body: "",
            url: this.getUrl(f.url, h, f.random),
            form: f.form,
            data: f.data || '',
            method: h,
            async: f.async === false ? false: f.async || true,
            call: g
        };
        if (this.filter && !this.filter(0, i)) {
            return false
        }
        var j = function(c, d, e) {
            c.state = 1;
            c.http.open(e.method, e.url, e.async);
            c.http.withCredentials = true;
            c.http.onreadystatechange = function() {
                if (c.http.readyState === 4) {
                    if (c.http.status === 200) {
                        c.state = 0;
                        d.procStack.pop();
                        if (d.filter && !d.filter(1, e, c.http.responseText)) {
                            return false
                        }
                        if (e.call.success) {
                            var a = c.http.responseText;
                            if (a.startsWith("<?xml")) {
                                e.call.success(c.http.responseXML);
                                return
                            }
                            var b = /^({(.*)}|\[(.*)\])(\s|\n)*$/g.test(a);
                            e.call.success(b ? JSON.parse(a) : a)
                        }
                    } else if (e.call.error) {
                        c.state = 0;
                        d.procStack.pop();
                        if (d.filter && !d.filter(2, e, c.http.responseText)) {
                            return false
                        }
                        if (e.call.error) e.call.error(c.http.responseText)
                    }
                }
                return true
            };
            if (["GET", "HEAD"].indexOf(e.method) == -1) {
                if (e.form) {
                    e.body = paramsToString(e.form);
                    c.http.setRequestHeader("Content-Type", "application/x-www-form-urlencoded")
                } else {
                    e.body = JSON.stringify(e.data);
                    c.http.setRequestHeader("Content-Type", "application/json")
                }
            }
            c.http.send(e.body)
        };
        this._processReq(i, j)
    },
    _processReq: function(a, b) {
        var c = setInterval((function(t) {
            return function() {
                if (t.procStack.length < t.maxRequest) {
                    t.procStack.push(0);
                    for (var i = 0; i < t.maxRequest; i++) {
                        if (t.httpStack[i].state === 0) {
                            try {
                                b(t.httpStack[i], t, a)
                            } catch(exc) {
                                if (a.call.error) a.call.error('request may be blocked!')
                            }
                            break
                        }
                    }
                    clearInterval(c)
                }
            }
        } (this)), 20)
    },
    _callback: function(a, b) {
        return {
            success: function(r) {
                if (a && a instanceof Function) a(r)
            },
            error: function(r) {
                if (b && a instanceof Function) b(r)
            }
        }
    },
    get: function(a, b, c) {
        this._doRequest(a instanceof Object ? this.getUrl(a) : {
            url: a
        },
        this._callback(b, c))
    },
    post: function(a, b, c, d) {
        this._doRequest({
            url: a,
            method: 'POST',
            form: typeof(b) === 'string' ? b: null,
            data: b,
        },
        this._callback(c, d))
    },
    request: function(a, b, c) {
        this._doRequest({
            url: a,
            method: b.method,
            form: b.form,
            params: b.params,
            data: b.data,
        },
        this._callback(c.success, c.error))
    },
    urlJoin: function(a, b) {
        return a + (a.indexOf('?') == -1 ? '?': '&') + b
    },
    jsonp: (function(j) {
        return function(c, d, e) {
            d = d || {};
            var s = document.createElement('SCRIPT');
            var h = "$callback_" + (10000 + parseInt(Math.random() * 90000));
            d['callback'] = h;
            j[h] = (function(g, t, s, f, b) {
                return function(a) {
                    t._jsonpGC(g, f, s);
                    b(a)
                }
            } (j, this, s, h, e));
            s.setAttribute('src', this.urlJoin(c, paramsToString(d)));
            var i = (function(g, t, s, f, a) {
                return function() {
                    t._jsonpGC(g, f, s);
                    a('jsonp : Invalid JSON data returned!', 1)
                }
            } (j, this, s, h, e));
            if (document.attachEvent) {
                s.attachEvent('onerror', i)
            } else {
                s.addEventListener('error', i, true)
            }
            document.getElementsByTagName('head')[0].appendChild(s)
        }
    } (window)),
    _jsonpGC: function(a, b, e) {
        try {
            delete a[b]
        } catch(ex) {
            a[b] = null
        }
        e.parentNode.removeChild(e)
    }
};
JR.prototype.loadScript = function(a, b, c) {
    var d = null;
    var e = document.documentElement.getElementsByTagName("HEAD");
    if (e.length != 0) d = e[0];
    else d = document.body;
    var f = d.getElementsByTagName('SCRIPT');
    var g = 0;
    for (var i = 0; i < f.length; i++) {
        if (f[i].getAttribute('src') && f[i].getAttribute('src').toLowerCase() == a.toLowerCase()) {
            g = 1
        }
    }
    if (!g) {
        var h = document.createElement('SCRIPT');
        if (b) h.onreadystatechange = h.onload = b;
        if (c) h.onerror = c;
        h.setAttribute('type', 'text/javascript');
        h.setAttribute('src', a);
        d.appendChild(h)
    }
};
JR.prototype.load = function(c, d, e, f) { (function(b) {
        b.xhr.get(d,
        function(a) {
            b.loadHTML(c, a);
            e && e(a)
        },
        f)
    } (this))
};
var $js = new JR()._init(true);
if (window.module) module.exports = $js;
JR.prototype.ie6 = function() {
    return /MSIE\s*6\.0/.test(window.navigator.userAgent)
};
JR.prototype.lazyRun = function(a, b) {
    if (a) {
        setTimeout(a, b || 120)
    }
};
$js.extend({
    screen: {
        width: function() {
            return Math.max(document.body.clientWidth, document.documentElement.clientWidth)
        },
        height: function() {
            return Math.max(document.body.clientHeight, document.documentElement.clientHeight)
        },
        offsetWidth: function() {
            return Math.max(document.body.offsetWidth, document.documentElement.offsetWidth)
        },
        offsetHeight: function() {
            return Math.max(document.body.offsetHeight, document.documentElement.offsetHeight)
        }
    },
    getPosition: function(e) {
        return (e.nodeName ? e: this.$(e)).getBoundingClientRect()
    }
});
$js.extend({
    dom: {
        fitHeight: function(e, a) {
            var b = e.parentNode;
            var c = e.nextSibling;
            var d = /;(\s*)height:(.+);/ig;
            var f = (b == document.body ? Math.max(document.body.clientHeight, document.documentElement.clientHeight) : b.offsetHeight) - e.offsetTop;
            while (c) {
                if (c.nodeName[0] != '#') {
                    f -= c.offsetHeight
                }
                c = c.nextSibling
            }
            f -= a || 0;
            if (d.test(e.style.cssText)) {
                e.style.cssText = e.style.cssText.replace(d, '; height:' + f + 'px;')
            } else {
                e.style.cssText += 'height:' + f + 'px;'
            }
        },
    }
}); (function(r) {
    var o = $js;
    if (r) {
        r(function() {
            return o
        })
    } else {
        window.$b = o;
        window.$jr = o
    }
})(window.define);