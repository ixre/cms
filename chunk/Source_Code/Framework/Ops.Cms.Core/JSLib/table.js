//
//名称 ： Table库
//版本:1.0
//时间：2012-09-22
//

$JS.extend({
    table: {
        //================ 动态表格插件 ====================//
        dynamic: function (table, multSelected, clickFunc) {
            if (table && table.nodeName === "TABLE") {

                //设置table的样式
                table.className += ' ui-table';

                //设置th的分割
                var ths = table.getElementsByTagName('TH');
                window.J.each(ths, function (i, e) {
                    if (i != ths.length - 1) {
                        if ((e.getElementsByClassName ? e.getElementsByClassName('th-split') : document.getElementsByClassName('th-split', e)).length == 0) {
                            var split = document.createElement("SPAN");
                            split.className = 'th-split';
                            e.appendChild(split);
                        }
                    }
                });


                var rows = table.getElementsByTagName("tr");
                for (var i = 0; i < rows.length; i++) {
                    if (i % 2 == 1) if (!rows[i].className) rows[i].className = 'even';
                    rows[i].onmouseover = function () {
                        if (this.className.indexOf('selected') == -1) {
                            this.className = this.className.indexOf('even') != -1 ? "hover even" : "hover";
                        }
                    };
                    rows[i].onmouseout = function () {
                        if (this.className.indexOf('selected') == -1) {
                            this.className = this.className.indexOf("even") == -1 ? "" : "even";
                        }
                    };

                    rows[i].onclick = (function (_rows, _this, _multSel) {
                        return function () {
                            var trs = new Array();

                            //取消其他选中
                            window.J.each(_rows, function (i, e) {
                                if (!_multSel) {
                                    if (e != _this) {
                                        e.className = e.className.indexOf("even") == -1 ? "" : "even";
                                    }
                                }

                                //将选中的加入到数组
                                if (e.className.indexOf('selected') != -1) {
                                    trs.push(e);
                                }
                            });

                            //未选中情况下，设置选中
                            if (_this.className.indexOf('selected') == -1) {

                                _this.className = _this.className.indexOf("even") == -1 ? "selected" : "selected even";

                                //将当前行加入到数组
                                trs.push(_this);
                            }

                            //触发点击事件
                            if (clickFunc) { clickFunc(trs); }
                        };
                    })(rows, rows[i], multSelected);
                }
            }
        }
    }
});