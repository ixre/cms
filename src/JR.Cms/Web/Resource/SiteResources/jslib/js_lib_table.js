//
//���� �� Table��
//�汾:1.0
//ʱ�䣺2012-09-22
//
jr.extend({ table: { dynamic: function (f, g, h) { if (f && f.nodeName === "TABLE") { f.className += ' ui-table'; var j = f.getElementsByTagName('TH'); window.jr.each(j, function (i, e) { if (i != j.length - 1) { if ((e.getElementsByClassName ? e.getElementsByClassName('th-split') : document.getElementsByClassName('th-split', e)).length == 0) { var a = document.createElement("SPAN"); a.className = 'th-split'; e.appendChild(a) } } }); var k = f.getElementsByTagName("tr"); for (var i = 0; i < k.length; i++) { if (i % 2 == 1) if (!k[i].className) k[i].className = 'even'; k[i].onmouseover = function () { if (this.className.indexOf('selected') == -1) { this.className = this.className.indexOf('even') != -1 ? "hover even" : "hover" } }; k[i].onmouseout = function () { if (this.className.indexOf('selected') == -1) { this.className = this.className.indexOf("even") == -1 ? "" : "even" } }; k[i].onclick = (function (b, c, d) { return function () { var a = []; window.jr.each(b, function (i, e) { if (!d) { if (e != c) { e.className = e.className.indexOf("even") == -1 ? "" : "even" } } if (e.className.indexOf('selected') != -1) { a.push(e) } }); if (c.className.indexOf('selected') == -1) { c.className = c.className.indexOf("even") == -1 ? "selected" : "selected even"; a.push(c) } if (h) { h(a) } } })(k, k[i], g) } } } } });
