//设置工作路径
var ASSETS_PATH = "/public/assets";
$js.WORKPATH = "/public/assets/js/";
window.jr = $js;
window.j6 = $js;
window.cms = $js;

//加载插件/库文件
function ld(libName, path) {
  (function (j, _path) {
    j.xhr.get(
      { url: _path + libName + ".js", async: false, random: false },
      function (script) {
        j.eval(script);
      }
    );
  })($js, path || $js.WORKPATH);
}
/****************  页面处理事件 **************/
var _scripts = document.getElementsByTagName("SCRIPT");
var _sloc = _scripts[_scripts.length - 1].src; //Script Location
var _hp = {
  //Script Handle Params
  loadUI: $js.request("ui", _sloc) == "1", //load ui lib
  hoverCList: $js.request("hover", _sloc).indexOf("clist") != -1, //Hover Category List
  hoverAList: $js.request("hover", _sloc).indexOf("alist") != -1, //Hover Archive List
  plugins: $js.request("ld", _sloc),
};

var plugins = null;
if (_hp.loadUI) {
  plugins = ["ui", "scrollbar", "scroller", "form"];
} else if (_hp.plugins) {
  plugins = _hp.plugins.split(",");
}
if (plugins) {
  for (var i = 0; i < plugins.length; i++) {
    ld(plugins[i]);
  }
}

/** 加载图标字体 */
function loadIconFont() {
  var c = document.createElement("link");
  c.rel = "stylesheet";
  c.href = ASSETS_PATH + "/icon-font.css";
  document.head.appendChild(c);
}
/** 延迟加载图片 */
var observer = new IntersectionObserver(function (changes) {
  changes.forEach(function (it) {
    if (it.isIntersecting) {
      var container = it.target;
      container.setAttribute("src", container.getAttribute("data-src"));
      observer.unobserve(container);
    }
  });
});

/**                      
 <img class="lazy" src="${page.fpath}/images/lazy_holder.gif" 
 data-src="${page.tpath}/images/map-address.png" alt="">
*/
function lazyObserve() {
  var arr = Array.from(document.querySelectorAll(".lazy"));
  arr.forEach(function (item) {
    observer.observe(item);
  });
}

// 绑定Toggle触发器
function bindToggleTrigger(ele) {
  if (ele.eleList.length == 0) {
    console.warn("toggle元素不存在");
    return;
  }
  var trigger = ele.attr ? ele.attr("trigger") : "";
  if (!trigger) {
    console.error(
      '元素未设置trigger属性,如：<div class=".toggle" trigger=".toggle-trigger"></div>'
    );
    return;
  }
  $js.$(trigger).click(function () {
    var expand = this.hasClass("expanded");
    if (expand) {
      this.removeClass("expanded");
      ele.slideUp("fast", function () {
        ele.css({ display: "none" });
      });
    } else {
      this.addClass("expanded");
      ele.css({ display: "inherit" });
      ele.slideDown("fast");
    }
    this.find(".icon").attr(
      "class",
      expand ? "icon fa fa-bars" : "icon fa fa-close"
    );
  });
}

$js.event.add(window, "load", function () {
  if (_hp.hoverNavi && _auto_navigator_ele) {
    clearInterval(_auto_navigator_timer);
  }

  var loc = window.location.pathname;

  loadIconFont();
  lazyObserve();

  // 绑定手机页面,导航菜单
  bindToggleTrigger($b.$(".toggle"));

  /****************** 设置分类菜单 *******************/

  //根据className设置Hover状态
  var setHoverByClassName = function (e) {
    var lis = e.childNodes;
    var link;
    var isHovered = false;

    var setHover = function (_loc) {
      for (var i = 0; i < lis.length; i++) {
        if (lis[i].nodeName[0] == "#") continue;
        link = lis[i].getElementsByTagName("A")[0];
        if (link.href.indexOf(_loc) != -1) {
          lis[i].className +=
            lis[i].className.indexOf("current") == -1 ? " current" : "";
          isHovered = true;
          break;
        }
      }
    };

    //全局匹配
    setHover(loc);

    //模糊匹配
    if (!isHovered) {
      var splitIndex = loc.lastIndexOf("/");
      if (splitIndex == loc.length - 1) {
        splitIndex = loc.substring(0, loc.length - 1).lastIndexOf("/");
      }
      setHover(loc.substring(0, splitIndex + 1));
    }
  };

  // 设置CList选中效果 (2012-11-03) **
  var _e_clist = document.getElementsByClassName("clist");
  if (_hp.hoverCList && _e_clist.length != 0) {
    setHoverByClassName(_e_clist[0]);
  }

  // 设置AList选中效果 (2012-11-03) **
  _e_clist = document.getElementsByClassName("alist");
  if (_hp.hoverAList && _e_clist.length != 0) {
    setHoverByClassName(_e_clist[0]);
  }

  // 选项卡

  $js.$fn(".tab").each(function (i, e) {
    var tabFN = function () {
      var t = this;
      var parent = this.parent();
      var active_i = -1;
      var active = function (e, b) {
        if (b) e.addClass("active");
        else e.removeClass("active");
      };
      parent.find(".tab").each(function (i, e) {
        var same = e.raw() == t.raw();
        if (same) active_i = i;
        active(e, same);
      });
      while (parent && parent.find(".frame").len() == 0) {
        parent = parent.parent();
      }
      parent.find(".frame").each(function (i, e) {
        active(e, active_i == i);
      });
    };
    if (e.hasClass("tab-hover") && document.documentElement.offsetWidth > 991) {
      e.mouseover(tabFN);
    } else {
      e.click(tabFN);
    }
  });

  // 将元素绝对定位
  $js.event.add(document, "scroll", function () {
    var scrollTop = document.documentElement.scrollTop;
    // 头部元素自动浮动
    var header = $js.$fn(".header");
    if (!header.hasClass("header-nofix")) {
      if (scrollTop > 30) {
        header.addClass("header-fixed");
      } else {
        header.removeClass("header-fixed");
      }
    }
    // 其他设置了.scroll-link的元素自动浮动
    var fixedArr = $js.$fn(".scroll-link");
    fixedArr.each(function (i, e) {
      var top = e.attr("offsetTop") + e.parent().attr("offsetTop");
      if (scrollTop > top) {
        if (e.css().position !== "fixed") {
          var left = e.attr("offsetLeft");
          var width = e.attr("offsetWidth");
          e.css({
            position: "fixed",
            top: "0",
            left: left + "px",
            width: width + "px",
          });
          e.addClass("scroll-linked");
        }
      } else {
        e.css({
          position: "unset",
          top: "inherit",
          left: "inherit",
          width: "inherit",
        });
        e.removeClass("scroll-linked");
      }
    });
  });

  // 滚动到目标
  var scrollLock = 0;
  $js.$fn(".scroll-to").click(function () {
    if (scrollLock == 1) return;
    scrollLock = 1;
    var target = this.attr("target");
    if (!target) throw '.scoll-top missing attribute "target"';
    //var ele = this;
    var target = $js.$fn(target);
    var offset = parseInt(this.attr("offset") || 0);
    var y = target.attr("offsetTop") + offset;
    var doc = document.documentElement;
    var timer = setInterval(function () {
      var setup = (y - doc.scrollTop) / 5;
      setup = setup > 0 ? Math.floor(setup) : Math.ceil(setup);
      if (Math.abs(setup) == 0) {
        doc.scrollTop = y;
        clearInterval(timer);
        scrollLock = 0;
      } else {
        doc.scrollTop += setup;
      }
    }, 10);
  });

  // Exchange
  $js.$fn(".ui-exchange").each(function (i, e) {
    e = e.raw();
    var v = null;
    var d = null;
    var f = null;
    var g = "exchange";
    switch (e.nodeName) {
      case "IMG":
        f = "src";
        break;
      default:
        f = "innerHTML";
        break;
    }
    if (f == null) return;
    v = e[f];
    d = e.getAttribute(g);
    if (d) {
      $js.event.add(
        e,
        "mouseover",
        (function (a, b, c) {
          return function () {
            a[b] = c;
          };
        })(e, f, d)
      );
      $js.event.add(
        e,
        "mouseout",
        (function (a, b, c) {
          return function () {
            a[b] = c;
          };
        })(e, f, v)
      );
    }
  });
  // 初始化wow.js
  if (window.WOW && !window.wowInit) {
    new WOW().init();
    window.wowInit = true;
  }
});

/***********************  设置自动时间  ***********************/
/*
var ele_dts = document.getElementsByClassName('autotime');
var weeks = new Array('日', '一', '二', '三', '四', '五', '六');
var cmath = function (v) {
    if (v < 10) return '0' + v;
    return v;
};
var setDate = function () {
    var dt = new Date();
    var str = cmath(dt.getFullYear()) + '年' + cmath(dt.getMonth() + 1) + '月' + cmath(dt.getDate()) + '日&nbsp;/&nbsp;周' +
          weeks[dt.getDay()] + '&nbsp;' + cmath(dt.getHours()) + ':' + cmath(dt.getMinutes()) + ':' + cmath(dt.getSeconds());

    for (var i = 0; i < ele_dts.length; i++) {
        ele_dts[i].innerHTML = str;
    }
};
if (ele_dts.length != 0) {
    setDate();
    setInterval(setDate, 1000);
}
*/
