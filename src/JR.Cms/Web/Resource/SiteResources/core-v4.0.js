/* 此文件由系统自动生成! */

//
// jr.core.js
//
// author:
//  jarrysix@gmail.com
//
// Copyright 2011 - 2018 @ fze.NET,All rights reserved!
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
 * 2023-09-23  重构

 ----------------------------------------------*/

function JR() {
  this.VERSION = "4.0"; //版本号
  this.WORKPATH = ""; //工作路径

  /*** 初始化 ***/
  this._init = function (extendJs) {
    //设置当前版本
    var _scripts = document.getElementsByTagName("SCRIPT");
    var s = _scripts[_scripts.length - 1];
    var _sloc = s.src;
    //s.src=_sloc+'#ver='+this.VERSION;
    //获取工作目录
    this.WORKPATH = _sloc.replace(/(\/)[^/]+$/, "$1");
    //扩展原生JS
    extendJs && __extendingJsPrototype__();
    return this;
  };
  // 选择器
  this.selector = {
    // 根据表达式查找对象,parent为父对象
    finds: function (expr, parent) {
      expr = expr.replace(/^\s|\s$/g, "").replace(/\s+/, " ");
      var arr = parent && parent.nodeName ? [parent] : parent || [document];
      var expArr = expr.split(" ");
      return this.walk(arr, expArr, 0);
    },
    findBy: function (arr, finder) {
      var list = [];
      for (var i = 0; i < arr.length; i++) {
        var fr = finder(arr[i]) || [];
        for (var j = 0; j < fr.length; j++) {
          list.push(fr[j]);
        }
      }
      return list;
    },
    walk: function (arr, expArr, level) {
      var root = [];
      var eleStr = expArr[level];
      switch (eleStr[0]) {
        case "#":
          root = this.findBy(arr, function () {
            var e = document.getElementById(eleStr.substring(1));
            return e ? [e] : [];
          });
          break;
        case ".":
          root = this.findBy(
            arr,
            (function (t) {
              return function (p) {
                return p.getElementsByClassName(eleStr.substring(1));
              };
            })(this)
          );
          break;
        default:
          root = this.findBy(arr, function (p) {
            return p.getElementsByTagName(eleStr);
          });
          break;
      }
      if (level < expArr.length - 1) {
        return this.walk(root, expArr, level + 1);
      }
      return root;
    },
  };
  // 功能
  this.fn = {
    _eventArray: [
      "abort",
      "blur",
      "change",
      "click",
      "dblclick",
      "error",
      "focus",
      "keydown",
      "keypress",
      "keyup",
      "load",
      "mousedown",
      "mousemove",
      "mouseout",
      "mouseover",
      "mouseenter",
      "mouseup",
      "touchstart",
      "touchmove",
      "touchend",
      "touchcancel",
      "reset",
      "resize",
      "select",
      "submit",
      "unload",
    ],
    g: {},
    eleList: [],
    fnList: null,
    fnProps: {
      check: ["checked", true],
      disabled: ["disabled", true],
      uncheck: ["checked", false],
    },
    aniFn: [
      "fadeIn",
      "fadeOut",
      "fadeTo",
      "fadeToggle",
      "slideUp",
      "slideDown",
      "slideToggle",
    ],
    // 将元素包装为FN
    _fn: function (expression) {
      return this.create(expression, this.g);
    },
    // 扩展
    extend: function (prop) {
      return this.g.extend.apply(this, prop);
    },
    create: function (expression, g) {
      return Object.create(this).init(expression, g);
    },
    init: function (e, g) {
      this.g = g;
      //清理拷贝的数据
      this.fnList = null;
      //获取元素
      if (typeof e == "string") {
        this.eleList = this.g.selector.finds(e);
      } else if (e instanceof Array || e instanceof HTMLCollection) {
        this.eleList = e;
      } else if (e.nodeName) {
        this.eleList = [e];
      }
      //注册所有事件,如:this.mouseover(function(){},false);
      var ptr = this;
      this.g.each(this._eventArray, function (i, e) {
        ptr[e] = (function (e) {
          return function (fn, attach) {
            return ptr.event(e, fn, attach != false);
          };
        })(e);
      });
      //注册函数
      for (var k in this.fnProps) {
        this[k] = (function (k, ptr) {
          return function () {
            return ptr._rawCaller(function (e) {
              var p = ptr.fnProps[k];
              e[p[0]] = p[1];
            });
          };
        })(k, this);
      }
      //注册动画
      this.g.each(this.aniFn, function (i, fn) {
        ptr[fn] = (function (fn, j) {
          return function (speed, callback) {
            return j._caller(function (e) {
              j.g.animation[fn](e, speed, callback);
            });
          };
        })(fn, ptr);
      });
      return this;
    },
    // 获取第一个元素
    _single: function () {
      if (this.len() > 0) {
        return this.eleList[0];
      }
      return null;
    },
    // 检查FN列表是否初始化，如为否则初始化
    _chkFnList: function () {
      with (this) {
        if (fnList) return;
        fnList = [];
        for (var i = 0, j = len(); i < j; i++) {
          fnList.push(_fn([eleList[i]]));
        }
      }
    },
    // 生成新的函数，将元素FN绑定到this指针
    _factFn: function (i, fn) {
      return (function (p) {
        return function (event) {
          fn.apply(p, [event]);
        };
      })(this.get(i));
    },
    // 遍历
    each: function (fn) {
      this._chkFnList();
      this.g.each(this.fnList, fn);
      return this;
    },
    // 获取第i项
    get: function (i) {
      this._chkFnList();
      if (i >= 0 && i <= this.fnList.length - 1) {
        return this.fnList[i];
      }
      return this._fn(null);
    },
    first: function () {
      return this.get(0);
    },
    last: function () {
      var l = this.len();
      return l == 0 ? null : this.get(l - 1);
    },
    // 包装后调用
    _caller: function (c) {
      for (var i = 0, j = this.len(); i < j; i++) {
        c.call(this, this._fn(this.eleList[i]), i);
      }
      return this;
    },
    // 使用原始元素遍历调用
    _rawCaller: function (c) {
      for (var i = 0, j = this.len(); i < j; i++) {
        c.call(this, this.eleList[i], i);
      }
      return this;
    },
    // 注册事件
    event: function (event, fn, attach) {
      var ptr = this;
      return this._rawCaller(function (e, i) {
        if (fn) {
          var newFn = ptr._factFn(i, fn);
          if (attach) {
            this.g.event.add(e, event, newFn);
          } else {
            e["on" + event] = newFn;
          }
        } else {
          e["on" + event]();
        }
      });
    },
    // 获取源元素
    raw: function () {
      if (this.len() == 1) {
        return this.eleList[0];
      }
      return this.eleList;
    },
    // 获取源元素
    elem: function () {
      return this.raw();
    },
    // 获取长度
    len: function () {
      return this.eleList.length;
    },
    // 根据表达式查找对象
    find: function (expression) {
      var arr = this.g.selector.finds(expression, this.eleList);
      return this._fn(arr);
    },
    // 获取父元素
    parent: function () {
      var e = this._single();
      if (e) {
        e = e.parentNode;
        return this._fn(e);
      }
      return null;
    },
    // 前一个元素
    prev: function () {
      var e = this._single().previousSibling;
      if (e) return this._fn(e);
      return null;
    },
    // 下一个元素
    next: function () {
      var e = this._single().nextSibling;
      if (e) return this._fn(e);
      return null;
    },
    // 获取或设置属性
    attr: function (attr, v) {
      if (v == null) {
        //如果为object，则设置多个属性
        if (attr instanceof Object) {
          for (var p in attr) {
            this._setAttr(p, attr[p]);
          }
          return this;
        }
        var e = this._single();
        return e[attr] || e.getAttribute(attr);
      }
      this._setAttr(attr, v);
      return this;
    },
    _setAttr: function (attr, v) {
      // value firefox又需要setAttribute设置
      var b =
        typeof v == "boolean" ||
        attr === "value" ||
        attr.indexOf("inner") == 0 ||
        attr.indexOf("scroll") == 0 ||
        attr.indexOf("offset") == 0;
      return this._rawCaller(function (e) {
        b ? (e[attr] = v) : e.setAttribute(attr, v);
      });
    },
    // 移除属性
    removeAttr: function (attr) {
      return this._rawCaller(function (e) {
        e.removeAttribute(attr);
      });
    },

    // 设置样式
    css: function (prop) {
      if (!(prop instanceof Object)) {
        var e = this._single();
        return e.currentStyle || document.defaultView.getComputedStyle(e, null);
      }
      //如果包含属性，则设置
      for (var s in prop) {
        var attr = s.split("-");
        for (var i = 1; i < attr.length; i++) {
          attr[i] = attr[i].replace(
            attr[i].charAt(0),
            attr[i].charAt(0).toUpperCase()
          );
        }
        var newAttr = attr.join("");
        this._rawCaller(function (e) {
          e.style[newAttr] = prop[s];
        });
      }
      return this;
    },
    // 是否包含样式
    hasClass: function (c) {
      var reg = new RegExp("(\\s|^)" + c + "(\\s|$)");
      return this._single().className.match(reg) ? true : false;
    },
    // 添加样式
    addClass: function (c) {
      return this.each(function (i, e) {
        if (!e.hasClass(c)) e.raw().className += " " + c;
      });
    },
    // 移除样式
    removeClass: function (c) {
      var reg = new RegExp("(\\s|^)" + c + "((\\s)|$)");
      this._rawCaller(function (e) {
        e.className = e.className.replace(reg, "$3");
      });
      return this;
    },
    //返回元素距离屏幕的坐标(left,right,bottom,top)
    position: function () {
      return this._single().getBoundingClientRect();
    },
    // 获取HTML
    html: function (v) {
      return this.attr("innerHTML", v);
    },
    text: function (v) {
      return this.attr("innerText", v);
    },
    // 在前面添加新元素
    before: function (elem) {
      return this._rawCaller(function (e) {
        e.parentNode && e.parentNode.insertBefore(elem, e);
      });
    },
    // 在后面添加新元素
    after: function (elem) {
      return this._rawCaller(function (e) {
        e.parentNode && e.parentNode.insertBefore(elem, e.nextSibling);
      });
    },
    // 在后面填充HTML
    append: function (e) {
      return this._rawCaller(function (p) {
        if (e.raw) p.appendChild(e.raw());
        else if (e.nodeName) p.appendChild(e);
        else if (typeof e == "string") p.innerHTML += e;
      });
    },
    // 添加到目标元素
    appendTo: function (e) {
      e.append(this);
    },
    // 获取值
    val: function (v) {
      if (v == null) return this.attr("value");
      this.attr("value", v);
      return this;
    },
    // 移除元素
    remove: function () {
      var ptr = this;
      this.fnList = null;
      return this._rawCaller(function (e, i) {
        ptr.eleList.remove(i);
        try {
          e.parentNode.removeChild(e);
          e.outerHTML = "";
        } catch (err) {
          console.log(err);
        }
      });
    },
    //动画效果
    animate: function (style, speed, callback) {
      this._caller.apply(this, [
        function (e) {
          this.g.animation.animate(e, style, speed, callback);
        },
      ]);
    },
  };

  //*************** Load Plugin *******************//
  this.loadHTML = function (panel, html) {
    //附加HTML
    var bodyReg = /<body[^>]*>([\s\S]+)<\/body>/im;

    //检测脚本reg
    //获取脚本内容reg
    // reg = /<script(.|\n)*?(src=[^>]+)*>([\s\S]*?)<\/script>/igm;
    var scriptReg = /<script((.|\n)*?)>([\s\S]*?)<\/script>/gim;
    // reg = new RegExp('<script((.|\\n)*?)>([\\s\\S]*?)</script>', 'gim');

    var body = html.match(bodyReg);
    if (body == null) {
      body = ["", html];
    }

    if (!panel.nodeName) panel = this.$(panel).elem();

    //清除脚本并写入panel
    if (panel) {
      try {
        panel.innerHTML = body[1]
          .replace(scriptReg, "")
          .replace(
            /<style([^>]+)>/gi,
            '<span style="display:none" class="for-ie">_</span><style$1>'
          );

        //IE要在style前加上元素，后删除
        this.$fn(panel)
          .find(".for-ie")
          .each(function (i, e) {
            e.remove();
          });

        //Chrome要添加到头部
        if (window.navigator.userAgent.indexOf("Chrome") != -1) {
          this.each(panel.getElementsByTagName("STYLE"), function (i, e) {
            panel.removeChild(e);
            document.getElementsByTagName("HEAD")[0].appendChild(e);
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
      if (jsSection[1].indexOf(" type=") == -1 || regType.test(jsSection[1])) {
        if (!spaceReg.test(jsSection[3])) {
          //不全为空
          this.eval(jsSection[3]);
        }
      }
    }
  };
}

/*** JavaScript  Extensions ***/
function __extendingJsPrototype__() {
  //仅计算中文为2个字节
  if (!String.prototype.len) {
    String.prototype.len = function (zh) {
      return this.replace(zh ? /[\u0391-\uFFE5]/g : /[^x00-xff]/g, "00").length;
    };
  }
  //数组删除元素
  if (!Array.prototype.remove) {
    Array.prototype.remove = function (from, to) {
      var rest = this.slice((to || from) + 1 || this.length);
      this.length = from < 0 ? this.length + from : from;
      return this.push.apply(this, rest);
    };
  }

  //IE老式浏览器创建对象
  if (!Object.create) {
    Object.create = function (o) {
      function $() {}
      $.prototype = o;
      return new $();
    };
  }

  //添加contains
  if (typeof HTMLElement != "undefined") {
    const prototype = {
      getElementsByClassName: function (className) {
        var elem = this.getElementsByTagName("*");
        var reg = new RegExp("\\s" + className + "\\s");
        var arr = [];
        for (var i = 0, j; (j = elem[i]); i++) {
          if (reg.test(" " + j.className + " ")) arr.push(j);
        }
        return arr;
      },
      contains: function (obj) {
        while (obj != null && typeof obj.tagName != "undefined") {
          if (obj == this) return true;
          obj = obj.parentNode;
        }
        return false;
      },
      computedStyle: function () {
        return (
          this.currentStyle || document.defaultView.getComputedStyle(this, null)
        );
      },
      setStyle: function (props) {
        var old = {};
        for (var i in props) {
          old[i] = this.style[i];
          this.style[i] = props[i];
        }
        return old;
      },
      restoreStyle: function (props) {
        for (var i in props) this.style[i] = props[i];
      },
      realEval: function (expr) {
        var old = this.setStyle({
          display: "inherit",
          visibility: "hidden",
          position: "absolute",
          height: "auto",
          width: "auto",
        });
        var value = expr(this);
        this.restoreStyle(old);
        return value;
      },
    };
    for (var p in prototype) {
      HTMLElement.prototype[p] == undefined &&
        (HTMLElement.prototype[p] = prototype[p]);
    }
  }
  if (!Date.prototype.format) {
    Date.prototype.format = function (fmt) {
      var o = {
        "M+": this.getMonth() + 1, //月份
        "d+": this.getDate(), //日
        "Ｈ+": this.getHours(), //小时
        "h+": this.getHours() % 12, //小时
        "m+": this.getMinutes(), //分
        "s+": this.getSeconds(), //秒
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度
        S: this.getMilliseconds(), //毫秒
      };
      if (/(y+)/.test(fmt)) {
        fmt = fmt.replace(
          RegExp.$1,
          (this.getFullYear() + "").substr(4 - RegExp.$1.length)
        );
      }
      for (var k in o) {
        if (new RegExp("(" + k + ")").test(fmt)) {
          fmt = fmt.replace(
            RegExp.$1,
            RegExp.$1.length == 1
              ? o[k]
              : ("00" + o[k]).substr(("" + o[k]).length)
          );
        }
      }
      return fmt;
    };
  }
  // //IE7及以下未内置JSON,可引用json2.js
  // if (!window.JSON) {
  //     window["JSON"] = {
  //         //容易产生内存泄露
  //         parse: function (s) {
  //             return eval('(' + s + ')');
  //         }
  //     };
  // }
}

JR.prototype = {
  // @deprecated
  request: function (key, url) {
    return this.param(key, url);
  },
  // 获取路径
  path: function () {
    var domain = location.domain || "";
    var url = location.href;
    domain = url.substring(url.indexOf(domain) + domain.length);
    /*if has port*/
    return domain.substring(domain.indexOf("/"));
  },
  // 获取请求参数，如: ?id=1 获取id: $js.param("id")
  param: function (key, url) {
    var match = new RegExp("(\\?|&)" + key + "=([^&]+)&*").exec(
      url || location.href
    );
    return match ? match[2] : "";
  },
  // 动态执行脚本
  eval: function (code) {
    //IE系列
    if (window.execScript) {
      window.execScript(code);
    } else {
      var script = document.createElement("SCRIPT");
      script.setAttribute("type", "text/javascript");
      script.text = code;
      document.head.appendChild(script);
      document.head.removeChild(script);
    }
    return code;
  },

  // 遍历: $js.forEach([1,3],function(i){});
  each: function (arr, call) {
    if (!arr) return;
    for (var i = 0; i < arr.length; i++) call(i, arr[i]);
  },
  // 模板: $js.template("{id}",{id:1});
  template: function (content, data) {
    if (data instanceof Object) {
      for (var n in data) {
        var reg = new RegExp("%" + n + "%|{" + n + "}");
        content = content.replace(reg, data[n]);
      }
    }
    return content;
  },
  // 扩展
  extend: function (prop) {
    if (prop && prop instanceof Object) {
      for (var ext in prop) {
        this[ext] == undefined && (this[ext] = prop[ext]);
      }
    }
    return this;
  },
  // @deprecated
  $fn: function (expression) {
    return this.$(expression);
  },
  // 功能
  $: function (expression) {
    return this.fn.create(expression, this);
  },
};

//动画
JR.prototype.animation = {
  speedSet: { slow: 10, normal: 6, fast: 3 },
  //_cmpStyle: function(e) {
  //    return (e.raw ? e.raw() : e).computedStyle();
  //var e = (e.raw ? e.raw() : e);
  //return e.currentStyle || document.defaultView.getComputedStyle(e, null);
  //},
  _titAttr: function (attr) {
    var arr = attr.split("-");
    for (var i = 1; i < arr.length; i++) {
      arr[i] = attr[i].replace(
        arr[i].charAt(0),
        arr[i].charAt(0).toUpperCase()
      );
    }
    return arr.join("");
  },
  _ani: function (target, speed, getFn, setFn, callback) {
    if (typeof speed != "integer") {
      speed = this.speedSet[speed || "normal"];
    }
    // 记录上次的步进
    var latest_setup = 0;
    // 定时器动画
    var timer = setInterval(function () {
      // 当前长度
      var current = getFn();
      // 计算步长(速度),当除以10(speed)得到小数，舍入后为0时，停止运动
      var setup = (target - current) / speed;
      // 如果步长大于0，则向上舍入，反之向下舍入
      setup = setup > 0 ? Math.floor(setup) : Math.ceil(setup);
      // 检查动画是否冲突,如果当前步进大于上次步进，则表示动画冲突
      var conflict = latest_setup > 0 && latest_setup < Math.abs(setup);
      // 检测动画是否完成(步长为0)
      if (Math.abs(setup) == 0 || conflict) {
        clearInterval(timer);
        setFn(target);
        if (callback instanceof Function) callback();
      } else {
        setFn(current + setup);
        latest_setup = Math.abs(setup);
      }
    }, 20);
    return timer;
  },
  animate: function (elem, style, speed, callback) {
    var e = elem.raw ? elem.raw() : elem;
    var srcStyle = e.computedStyle();
    // 记录目标数值字典
    var attrMap = {};
    // 获取当前属性值
    var getFn = function (e, style, attr) {
      return function () {
        if (attr === "opacity") {
          return e["filters"]
            ? e["filters"]["opacity"]
            : parseFloat(style["opacity"]) * 100;
        }
        return parseFloat(style[attr]);
      };
    };
    // 设置属性值
    var setFn = function (e, attr) {
      return function (v) {
        if (attr === "opacity") {
          e["filters"]
            ? (e["filters"]["opacity"] = v)
            : (e.style[attr] = v / 100);
        } else {
          e.style[attr] = v + "px";
        }
      };
    };
    // 包装回调，等待所有动画执行完成后，触发原始回调函数
    var wrapCallback = (function (t, elem, src) {
      return function () {
        for (var attr in attrMap) {
          if (getFn(e, srcStyle, attr)() != attrMap[attr]) return;
        }
        if (src && src instanceof Function) src.apply(elem, []);
      };
    })(this, elem, callback);
    // 遍历执行动画
    for (var prop in style) {
      var attr = this._titAttr(prop);
      var target = getFn(e, style, attr)();
      attrMap[attr] = target; // 将目标值存到Map
      this._ani(
        target,
        speed,
        getFn(e, srcStyle, attr),
        setFn(e, attr),
        wrapCallback
      );
    }
  },
  toggle: function (e, speed, callback) {
    e = e.raw ? e.raw() : e;
    e.style["overflow"] = "hidden";
    var arr = e.realEval(function (e) {
      return [e.clientWidth, e.clientHeight];
    });
    var width = arr[0];
    var height = arr[1];
    //var width = e.scrollWidth;
    //var height = e.scrollHeight;
    var opacity = 1;
    if (height == e.offsetHeight) {
      width = 0;
      height = 0;
      opacity = 0;
    }
    this.animate(
      e,
      {
        width: width + "px",
        height: height + "px",
        opacity: opacity,
      },
      speed,
      callback
    );
  },
  fadeTo: function (e, o, speed, callback) {
    this.animate(e, { opacity: o }, speed, callback);
  },
  fadeIn: function (e, speed, callback) {
    this.fadeTo(e, 1, speed, callback);
  },
  fadeOut: function (e, speed, callback) {
    this.fadeTo(e, 0, speed, callback);
  },
  fadeToggle: function (e, speed, callback) {
    var src = e.raw ? e.raw() : e;
    var c = src.computedStyle();
    var opacity = parseFloat(
      src.filters ? src.filters["opacity"] : c["opacity"]
    );
    this.fadeTo(src, opacity < 1 ? 1 : 0, speed, callback);
  },
  slideDown: function (e, speed, callback) {
    var src = e.raw ? e.raw() : e;
    var height = src.realEval(function (e) {
      return e.clientHeight;
    });
    this.animate(src, { height: height + "px" }, speed, callback);
    //this.animate(e, { "height": (e.raw ? e.raw() : e).scrollHeight + "px" }, speed, callback);
  },
  slideUp: function (e, speed, callback) {
    this.animate(e, { height: "0" }, speed, callback);
  },
  slideToggle: function (e, speed, callback) {
    var src = e.raw ? e.raw() : e;
    var height = src.realEval(function (e) {
      return e.clientHeight;
    });
    //var height = (e.raw ? e.raw() : e).clientHeight;
    var offset = src.offsetHeight;
    this.animate(
      src,
      {
        height: (offset != height ? height : 0) + "px",
      },
      speed,
      callback
    );
  },
};

//*************** COOKIE 操作 ***************//
JR.prototype.cookie = {
  //写入Cookie
  write: function (name, value, seconds, path, domain, secure) {
    var expires = seconds ? new Date(new Date().getTime() + seconds) : null;
    this.set(name, value, expires, path, domain, secure);
  },
  set: function (name, value, expires, path, domain, secure) {
    document.cookie =
      name +
      "=" +
      escape(value) +
      (expires ? "; expires=" + expires.toGMTString() : "") +
      (path ? "; path=" + path : "") +
      (domain ? "; domain=" + domain : "") +
      (secure ? "; secure" : "");
  },
  //移除Cookie
  remove: function (name) {
    this.write(name, "", -9);
  },
  //读取Cookie
  read: function (name) {
    var cookieValue = "";
    var search = name + "=";
    if (document.cookie.length > 0) {
      var offset = document.cookie.indexOf(search);
      if (offset !== -1) {
        offset += search.length;
        var end = document.cookie.indexOf(";", offset);
        if (end === -1) end = document.cookie.length;
        cookieValue = unescape(document.cookie.substring(offset, end));
      }
    }
    return cookieValue;
  },
};

//*************** 事件 ***************//
//document.onclick=function(){...};
//jr.event.add(document.body,'click',function(){...}
JR.prototype.event = {
  //添加事件
  add: function (elem, event, func, capture) {
    if (!elem.attachEvent && !elem.addEventListener) {
      alert("event error! parameter:" + ele + ",event:" + event);
      return;
    }
    document.attachEvent
      ? elem.attachEvent("on" + event, func)
      : elem.addEventListener(event, func, capture || true);
  },
  //移除事件
  remove: function (elem, event, func, capture) {
    document.detachEvent
      ? elem.detachEvent("on" + event, func)
      : elem.removeEventListener(event, func, capture || true);
  },
  // 事件元素
  src: function (event) {
    var e = event ? event : window.event;
    return e.target || e.srcElement;
  },
  // 事件元素
  getTarget: function (event) {
    var e = event ? event : window.event;
    return e.target || e.srcElement;
  },
  //relatedTarget的兼容写法，获取所到的DIV参数
  getRelatedTarget: function (event) {
    if (event.relatedTarget) return event.relatedTarget;
    if (event.toElement) return event.toElement;
    if (event.fromElement) return event.fromElement;
    return null;
  },
  // 停止冒泡
  stopBubble: function (event) {
    event = event || window.event;
    event.cancelBubble = true;
    if (event.stopPropagation) {
      event.stopPropagation();
    }
    // var e = event ? event : window.event;
    // if (window.event) {
    //     e.cancelBubble = true;
    // } else {
    //     //e.preventDefault();
    //     e.stopPropagation();
    // }
  },
  //阻止浏览器默认行为
  preventDefault: function (event) {
    event = event || window.event;
    if (event.preventDefault) {
      event.preventDefault();
    } else {
      event.returnvalue = false;
    }
  },
};

//将object转为string
var paramsToString = function (params) {
  if (params instanceof Object) {
    var str = "";
    var i = 0;
    for (var attr in params) {
      if (i++ !== 0) {
        str += "&";
      }
      str += attr + "=" + encodeURIComponent(params[attr]);
    }
    return str;
  }
  return params;
};

//*************** XHR ***************//
JR.prototype.xhr = {
  //最大同时请求数
  maxRequest: 4,
  // 处理事件,0:开始请求,1:成功,2:失败. 返回false則停止执行
  // jr.xhr.filter = function(status,method,data){}
  filter: function (status, method, content) {
    if (status == 2) {
      var matches = /<title>(.+)<\/title>/.exec(content);
      if (matches && matches.length > 0) {
        alert(matches[1]);
        return false;
      }
    }
    return true;
  },
  //HttpRequest队列
  httpStack: null,
  //处理队列数组
  procStack: [],
  //初始化
  init: function () {
    if (this.httpStack) return;
    this.httpStack = [];
    for (var i = 0; i < this.maxRequest; i++) {
      this.httpStack[i] = {
        state: 0, //状态,0空闲,1繁忙
        http: new XMLHttpRequest(),
      };
    }
  },
  getUrl: function (url, reqMethod, getRandom) {
    if (url == null || url == "") url = location.href;
    if (reqMethod == "GET" && getRandom != false && url.indexOf("#") == -1) {
      url = this.urlJoin(url, "rd=" + Math.random());
    }
    return url;
  },
  //请求
  _doRequest: function (_request, call) {
    //执行初始化
    this.init();
    var reqMethod = (_request.method || "GET").toUpperCase();

    //请求范例
    //xhr.request({url:"/",method:"POST",params:"",data:"text"},
    //{start:function(){},over:function(){},error:function(x){//处理错误},success:function(x){}});
    //x为返回的数据*/
    var reqArgs = {
      body: "",
      //是否缓存
      url: this.getUrl(_request.url, reqMethod, _request.random),
      //请求参数
      //method为"POST"时适用
      //格式如:'action=delete&id=123'
      form: _request.form,
      data: _request.data || "",
      //请求的方法,POST和GET,HEAD
      method: reqMethod,
      //是否异步
      async: _request.async === false ? false : _request.async || true,
      call: call,
    };
    if (this.filter && !this.filter(0, reqArgs)) {
      return false;
    }

    var xhrProcess = function (xhr, ptr, req) {
      //请求中
      xhr.state = 1;
      //同步线程:true
      xhr.http.open(req.method, req.url, req.async);
      xhr.http.withCredentials = true; // 跨域发送cookie信息
      //请求状态发生变化时执行
      xhr.http.onreadystatechange = function () {
        if (xhr.http.readyState === 4) {
          if (xhr.http.status === 200) {
            xhr.state = 0;
            ptr.procStack.pop();
            // 拦截成功请求
            if (ptr.filter && !ptr.filter(1, req, xhr.http.responseText)) {
              return false;
            }
            if (req.call.success) {
              // 自动识别返回的内容格式
              var ret = xhr.http.responseText;
              if (ret.startsWith("<?xml")) {
                req.call.success(xhr.http.responseXML);
                return;
              }
              var isJson = /^({(.*)}|\[(.*)\])(\s|\n)*$/g.test(ret);
              req.call.success(isJson ? JSON.parse(ret) : ret);
            }
          } else if (req.call.error) {
            xhr.state = 0;
            ptr.procStack.pop();
            // 拦截失败请求
            if (ptr.filter && !ptr.filter(2, req, xhr.http.responseText)) {
              return false;
            }
            if (req.call.error) req.call.error(xhr.http.responseText);
          }
        }
        return true;
      };
      //如果为POST请求
      if (["GET", "HEAD"].indexOf(req.method) == -1) {
        if (req.form) {
          req.body = paramsToString(req.form);
          xhr.http.setRequestHeader(
            "Content-Type",
            "application/x-www-form-urlencoded"
          );
        } else {
          req.body = JSON.stringify(req.data);
          xhr.http.setRequestHeader("Content-Type", "application/json");
        }
      }
      //发送请求
      xhr.http.send(req.body);
    };
    this._processReq(reqArgs, xhrProcess);
  },
  //处理请求
  _processReq: function (req, xhrCallback) {
    var timer = setInterval(
      (function (t) {
        return function () {
          if (t.procStack.length < t.maxRequest) {
            t.procStack.push(0);
            //查找空闲的xhr对象
            for (var i = 0; i < t.maxRequest; i++) {
              if (t.httpStack[i].state === 0) {
                //执行请求
                try {
                  xhrCallback(t.httpStack[i], t, req);
                } catch (exc) {
                  if (req.call.error) req.call.error("request may be blocked!");
                }
                break;
              }
            }
            clearInterval(timer);
          }
        };
      })(this),
      20
    );
  },
  _callback: function (success, err) {
    return {
      success: function (r) {
        if (success && success instanceof Function) success(r);
      },
      error: function (r) {
        if (err && success instanceof Function) err(r);
      },
    };
  },
  get: function (url, success, err) {
    this._doRequest(
      url instanceof Object ? this.getUrl(url) : { url: url },
      this._callback(success, err)
    );
  },
  post: function (url, param, success, err) {
    this._doRequest(
      {
        url: url,
        method: "POST",
        form: typeof param === "string" ? param : null,
        data: param,
      },
      this._callback(success, err)
    );
  },
  request: function (url, options, callback) {
    this._doRequest(
      {
        url: url,
        method: options.method,
        form: options.form,
        params: options.params,
        data: options.data,
      },
      this._callback(callback.success, callback.error)
    );
  },
  urlJoin: function (path, query) {
    return path + (path.indexOf("?") == -1 ? "?" : "&") + query;
  },
  jsonp: (function (global) {
    return function (url, params, callback) {
      params = params || {};
      var s = document.createElement("SCRIPT");
      var callName = "$callback_" + (10000 + parseInt(Math.random() * 90000));
      params["callback"] = callName;
      global[callName] = (function (g, t, s, f, callback) {
        return function (data) {
          t._jsonpGC(g, f, s);
          callback(data);
        };
      })(global, this, s, callName, callback);
      s.setAttribute("src", this.urlJoin(url, paramsToString(params)));
      var errCall = (function (g, t, s, f, callback) {
        return function () {
          t._jsonpGC(g, f, s);
          callback("jsonp : Invalid JSON data returned!", 1);
        };
      })(global, this, s, callName, callback);
      if (document.attachEvent) {
        s.attachEvent("onerror", errCall);
      } else {
        s.addEventListener("error", errCall, true);
      }
      document.getElementsByTagName("head")[0].appendChild(s);
    };
  })(window),
  _jsonpGC: function (global, funcName, e) {
    try {
      delete global[funcName];
    } catch (ex) {
      global[funcName] = null;
    }
    e.parentNode.removeChild(e);
  },
};

//加载脚本依赖库
JR.prototype.loadScript = function (scriptUrl, loadfunc, errorfunc) {
  var scriptPanel = null;
  var heads = document.documentElement.getElementsByTagName("HEAD");

  if (heads.length != 0) scriptPanel = heads[0];
  else scriptPanel = document.body;

  var scripts = scriptPanel.getElementsByTagName("SCRIPT");
  var loadFlag = 0;

  for (var i = 0; i < scripts.length; i++) {
    if (
      scripts[i].getAttribute("src") &&
      scripts[i].getAttribute("src").toLowerCase() == scriptUrl.toLowerCase()
    ) {
      loadFlag = 1;
    }
  }
  if (!loadFlag) {
    var script = document.createElement("SCRIPT");
    if (loadfunc) script.onreadystatechange = script.onload = loadfunc; //IE ReadStateChange
    if (errorfunc) script.onerror = errorfunc;
    script.setAttribute("type", "text/javascript");
    script.setAttribute("src", scriptUrl);
    scriptPanel.appendChild(script);
  }
};

// 加载url到元素中
JR.prototype.load = function (ele, url, success, error) {
  (function (_this) {
    _this.xhr.get(
      url,
      function (result) {
        _this.loadHTML(ele, result);
        success && success(result); //成功回执
      },
      error
    );
  })(this);
};

var $js = new JR()._init(true);
if (window.module) module.exports = $js;

//初始化
//$js.extend({
// style: function (id, attr) {
//     var e = id.nodeName ? id : this.dom.id(id);
//     if (attr instanceof String) {
//         e.style.cssText = attr;
//         return true;
//     }
//     return this.fn.init([e], this).css(attr);
// },

// //加载插件/库文件
// ld: function (libName, path) {
//     (function (j, _path) {
//         j.xhr.get({ url: _path + libName + '.js', async: false, random: false }, function (script) {
//             j.eval(script);
//         });
//     }(this, path || this.WORKPATH));
// }
//});

JR.prototype.ie6 = function () {
  return /MSIE\s*6\.0/.test(window.navigator.userAgent);
};

//延迟执行方法，常用于ld异步执行
JR.prototype.lazyRun = function (func, timer) {
  if (func) {
    setTimeout(func, timer || 120);
  }
};

$js.extend({
  screen: {
    width: function () {
      return Math.max(
        document.body.clientWidth,
        document.documentElement.clientWidth
      );
    },
    height: function () {
      return Math.max(
        document.body.clientHeight,
        document.documentElement.clientHeight
      );
    },
    offsetWidth: function () {
      return Math.max(
        document.body.offsetWidth,
        document.documentElement.offsetWidth
      );
    },
    offsetHeight: function () {
      return Math.max(
        document.body.offsetHeight,
        document.documentElement.offsetHeight
      );
    },
  },
  //返回元素距离屏幕的坐标(left,right,bottom,top)
  getPosition: function (e) {
    return (e.nodeName ? e : this.$(e)).getBoundingClientRect();
  },
});

$js.extend({
  dom: {
    // _b: {},
    // set: function (ptr) {
    //     this._b = ptr;
    // },
    fitHeight: function (e, offset) {
      var par = e.parentNode;
      var next = e.nextSibling;
      var reg = /;(\s*)height:(.+);/gi;

      var height =
        (par == document.body
          ? Math.max(
              document.body.clientHeight,
              document.documentElement.clientHeight
            )
          : par.offsetHeight) - e.offsetTop;

      while (next) {
        if (next.nodeName[0] != "#") {
          height -= next.offsetHeight;
        }
        next = next.nextSibling;
      }

      height -= offset || 0;
      if (reg.test(e.style.cssText)) {
        e.style.cssText = e.style.cssText.replace(
          reg,
          "; height:" + height + "px;"
        );
      } else {
        e.style.cssText += "height:" + height + "px;";
      }
    },

    // //======= 选择器 =====//
    // $: function (el, tagName, attrs) {
    //     var exp = "#" + el;
    //     if (tagName && tagName != "" && tagName != "*") {
    //         exp += " " + tagName;
    //     }
    //     if (attrs instanceof Object) {
    //         var s = attrs["className"];
    //         if (s == null) {
    //             s = attrs["class"];
    //         }
    //         if (s) {
    //             exp += " ." + s;
    //         }
    //     }
    //     var arr = this._b.selector.finds(exp);
    //     var len = arr.length;
    //     if (len > 1) {
    //         return arr;
    //     } else if (len == 1) {
    //         return arr[0];
    //     }
    //     return null;
    // },
  },
});

//Require.JS
(function (r) {
  var o = $js;
  if (r) {
    r(function () {
      return o;
    });
  } else {
    window.$b = o;
    window.$jr = o;
  }
})(window.define);
