﻿//
//文件：滚动效果插件
//版本: 1.0
//时间：2011-10-01
//

function scroller(b, c, d) { this.$P = null; this.$C = null; this.$L = null; this.pagePanel = null; this.index = 0; this.offset = 0; this.scroll = 0; this.scrollUnit = c.unit || 5; this.lock = false; this.direction = c.direction || 'left'; this.interval = d; this.timer = null; this.taskTimer = null; this.$P = document.getElementById(b); this.$P.style.cssText += 'overflow:hidden;position:relative;'; var f = this.$P.getElementsByTagName('UL'); if (f.length == 0) { alert('无法找到元素ID为:' + this.$P.getAttribute('id') + '下的UL元素!'); return false } else { this.$C = f[0]; this.$L = this.$C.getElementsByTagName('LI') } if (this.direction == 'left') { var g = 0; var h = 0; for (var i = 0; i < this.$L.length; i++) { h = this.$L[i].offsetWidth; if (h == 0) { h = this.$P.offsetWidth } g += h; this.$L[i].style.cssText += 'float:left;width:' + h + 'px'; this.$L[i].setAttribute('scroll-index', i) } this.$C.style.width = g + 'px'; this.pagePanel = document.getElementById(c.pagerid); if (this.pagePanel) { for (var i = 0; i < this.$L.length; i++) { var e = document.createElement("A"); if (i == 0) { e.className = 'current' } e.innerHTML = (i + 1).toString(); e.href = 'javascript:;'; e.onclick = (function (t, a) { return function () { t.setIndex(a) } })(this, i); this.pagePanel.appendChild(e) } } } else if (this.direction == 'up') { this.scrollUnit = c.unit || null; this.$C.style.cssText += 'float:left' } else { alert('仅支持方向：left和up'); return false } this._restart() } scroller.prototype.logger = function (a) { var b = document.getElementById('scroll-logger'); if (b) { b.innerHTML = a } }; scroller.prototype._async = function () { this.index = parseInt(this.$L[0].getAttribute('scroll-index')); if (this.index == this.$L.length) { this.index = 0 } this.logger((this.index + 1) + '/' + this.$L.length); if (this.pagePanel) { var a = this.pagePanel.getElementsByTagName('A'); for (var i = 0; i < a.length; i++) { a[i].className = i == this.index ? 'current' : '' } } }; scroller.prototype.setIndex = function (a) { this.lock = true; var b = 0; var c = -1; for (var i = 0; i < this.$L.length; i++) { if (parseInt(this.$L[i].getAttribute('scroll-index')) == a) { var d = this.$L[i]; this.$C.removeChild(d); this.$C.insertBefore(d, this.$L[0]); break } } this.$C.style.marginLeft = '0px'; this.index = a; this.lock = false; this._async(); this._restart() }; scroller.prototype.prev = function () { if (this.lock || this.scroll > this.scrollUnit) { return false } else { this.lock = true; var t = this; this._start(true, true, function () { t.lock = false }) } }; scroller.prototype.next = function () { if (this.lock || this.scroll > this.scrollUnit) { return false } else { this.lock = true; var t = this; this._start(!true, true, function () { t.lock = false }) } }; scroller.prototype._start = function (a, b, c) { if (this.lock && !b) { return false } if (this.direction == "left") { if (!a) { this._sc_left(c) } else { this._sc_right(c) } this._async() } else if (this.direction == "up") { this._sc_up(c) } }; scroller.prototype._restart = function () { var t = this; if (this.taskTimer) { clearTimeout(this.taskTimer) } this.taskTimer = setTimeout(function () { t._start() }, this.interval) }; scroller.prototype._sc_left = function (b) { var c = this; var d = c.$L[0].offsetWidth; this.timer = setInterval(function () { if (c.scroll == d) { clearInterval(c.timer); c.scroll = 0; var a = c.$L[0]; c.$C.removeChild(a); c.$C.appendChild(a); if (b) { b() } c._restart(); c.$C.style.marginLeft = '0px'; return false } c.scroll += c.scrollUnit / 10; if (c.scroll > d) { c.scroll = d } c.$C.style.marginLeft = (-c.scroll) + 'px' }, 10) }; scroller.prototype._sc_right = function (a) { var b = this; var c = b.$L[b.$L.length - 1].offsetWidth; var d = b.$C.lastChild; b.$C.removeChild(d); b.$C.insertBefore(d, b.$L[0]); b.scroll = -c; this.timer = setInterval(function () { if (b.scroll == 0) { clearInterval(b.timer); if (a) { a() } b._restart(); b.$C.style.marginLeft = '0px'; return false } b.scroll += b.scrollUnit / 10; if (b.scroll > 0) { b.scroll = 0 } b.$C.style.marginLeft = (b.scroll) + 'px' }, 10) }; scroller.prototype._sc_up = function (c) { var d = this; var e = d.$C.clientHeight; var f = d.$L[0].offsetHeight; var g = 0; this.timer = setInterval(function () { if (d.scroll >= e) { clearInterval(d.timer); var a = e / (1000 / 10 / 2); var b = setInterval(function () { d.scroll -= a; if (d.scroll - a < 0) { d.scroll = 0 } if (d.scroll == 0) { clearInterval(b); d._restart() } d.$P.scrollTop = d.scroll }, 10); return false } if (g == f) { clearInterval(d.timer); d.scroll += g; d._restart() } else { g += d.scrollUnit / 10; if (g + d.scrollUnit / 10 > f) { g = f } d.$P.scrollTop = d.scroll + g } }, 10) }; j6.extend({ scroller: function (a, b, c) { return new scroller(a, b, c) } });