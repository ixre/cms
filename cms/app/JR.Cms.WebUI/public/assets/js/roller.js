/* 此文件由系统自动生成! */
//
//文件：卷轴插件
//版本: 1.0
//时间：2011-10-01
//

jr.extend({ roller: { init: function (a) { this.elem = a.elem; this.direction = a.direction; this.pix = a.pix; this.elem.style.cssText += 'overflow:hidden;' }, start: function (a, b) { var c = this.elem, _pix = this.pix, a = a | 1; var i, j; var d; var e = function () { if (b != null) b() }; switch (this.direction) { case "up": i = _pix; j = 0; d = setInterval(function () { i -= a; if (i < 0) { i = 0; clearInterval(d); e() } c.style.height = i.toString() + "px" }, 10); break; case "left": i = _pix; j = 0; d = setInterval(function () { i -= a; if (i < 0) { i = 0; clearInterval(d); e() } c.style.width = i.toString() + "px" }, 10); break; case "down": i = 0; j = _pix; d = setInterval(function () { i += a; if (i > j) { i = j; clearInterval(d); e() } c.style.height = i.toString() + "px" }, 10); break; case "right": i = 0; j = _pix; d = setInterval(function () { i += a; if (i > j) { i = j; clearInterval(d); e() } c.style.width = i.toString() + "px" }, 10); break } } } });
