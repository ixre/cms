
/* 滚动条 */
var JS_scrollbar = {
    $: window.$JS,
    pnode: null,
    ele: null,
    startP: { x: 0, y: 0 },
    moveP: { x: 0, y: 0, z: -1 },
    slideBar: null,
    init: function (e) {
        this.ele = e.nodeName ? e : document.getElementById(e); //元素
        this.pnode = document.createElement('DIV');
        this.slideBar = null;                                   //滚动条
        this.timer = null;
        var parentNode = this.ele.parentNode;

        //if (parentNode.className == 'scrollbar') {
        //    this.pnode = parentNode;
        // } else {
        //更改元素
        parentNode.insertBefore(this.pnode, this.ele);
        this.pnode.appendChild(this.ele);
        //parentNode.removeChild(this.ele);

        //设置样式
        this.pnode.className = 'scrollbar';
        this.pnode.style.cssText += 'height:' + this.ele.clientHeight + 'px;overflow:hidden;position:relative';
        this.ele.style.height = 'auto';
        this.ele.style.position = 'absolute';
        // }
        this.showbar();

        return this;

        /*

        var slider_bar2 = this.sliderBar;

        //设置滚动条的宽度
        var resetSize = (function (t) {
        return function () {
        var x = parseInt(t.ele.getAttribute('last-scroll-x')),
        y = parseInt(t.ele.getAttribute('last-scroll-y'));

        var setPoint = function () {
        //重新记录宽度
        t.ele.setAttribute('last-scroll-x', t.ele.scrollWidth);
        t.ele.setAttribute('last-scroll-y', t.ele.scrollHeight);
        };

        if (!x || !y) {
        setPoint();
        return false;
        }


        if (x != t.ele.scrollWidth || y != t.ele.scrollHeight) {
        setPoint();
        var y1 = t.ele.clientHeight,
        y2 = t.ele.clientHeight;

        var bt1 = t.bit;
        t.bit = t.ele.clientHeight / t.ele.scrollHeight;
        //j.$('navigator').innerHTML = t.bit + '/' + bt1 + "<br />" + t.ele.clientHeight + '/' + t.ele.scrollHeight;

        //判断是否可以滚动
        var slidebarIns = document.all ? document.getElementsByClassName('slidebar', t.ele) : t.ele.getElementsByClassName('slidebar');

        if (t.ele.scrollHeight - y2 <= 0) {
        if (slidebarIns.length != 0) {
        for (var i = 0; i < slidebarIns.length; i++) {
        t.ele.removeChild(slidebarIns[i]);
        }
        return false;
        }
        } else if (slidebarIns.length == 0) {
        t.showbar();
        } else {
        t.bit = y1 / t.ele.scrollHeight;
        var slider = slider_bar2.parentNode;
        slider.style.height = y1 + 'px';
        // slider.style.top ='0px';
        slider_bar2.style.cssText = 'height:' + Math.round(t.bit * slider_bar2.parentNode.clientHeight) + 'px;';
        }
        //重设宽度
        //slider_bar2.parentNode.style.top = '0px';
        //slider_bar2.parentNode.style.height = '0px';
        //slider_bar2.style.cssText = 'height:' + Math.round(t.bit * slider_bar2.parentNode.clientHeight) + 'px;';


        // slider_bar.style.offsetTop = t.ele.scrollTop * t.bit;
        //j.$('navigator').style.cssText += 'width:' + t.ele.offsetWidth + 'px;height:' + t.ele.scrollHeight + 'px';
        }
        };
        })(this);
        resetSize();
        this.timer = setInterval(resetSize, 100);


        return this;
        */
    },
    showbar: function () {
        //获取比例
        var bit = this.pnode.clientHeight / this.ele.scrollHeight;

        //添加滚动条
        var slider = document.createElement('DIV');
        var slider_width = 0;
        slider.className = 'bar';
        slider.style.cssText = 'position:absolute;right:0;top:0;height:' + this.pnode.clientHeight + 'px';
        this.pnode.appendChild(slider);

        var slider_bar = document.createElement('DIV');
        slider_bar.className = 'btn';
        slider_bar.innerHTML = '<div class="top"></div><div class="center"></div><div class="bottom"></div>';
        slider_bar.style.cssText = 'height:' + Math.round(bit * slider.clientHeight) + 'px;';
        slider.appendChild(slider_bar);


        //自动添加宽度和背景颜色
        if (this.$.style(slider_bar)['backgroundImage'] == 'none' && this.$.style(slider_bar)['backgroundColor'] == 'transparent') {
            slider_bar.style.backgroundColor = '#a0a0a0';
        }
        if (!/^(?!0)\d+px$/.test(this.$.style(slider)['width'])) {
            slider.style.width = '12px';
            slider.style.backgroundColor = '#f5f5f5';
        }
        //设置父容器的宽度
        this.pnode.style.width = (this.ele.offsetWidth + slider.offsetWidth) + 'px';

        this.sliderBar = slider_bar;


        //设置拖动事件
        var t = this;

        var moveCall;
        var removeCall = function (event) {
            //t.$.event.remove(slider_bar, 'mousemove', moveCall);
            t.$.event.remove(document.body, 'mousemove', moveCall);
        };

        this.$.event.add(slider_bar, 'mousedown', function () {
            var e = window.event || arguments[0];
            var y1 = e.clientY - slider_bar.offsetTop;

            moveCall = function () {
                var offset = (window.event || arguments[0]).clientY - y1;
                t.scroll(offset);
                window.getSelection ? window.getSelection().removeAllRanges() : document.selection.empty();
            };

            //t.$.event.add(slider_bar, 'mousemove', moveCall);
            t.$.event.add(document.body, 'mousemove', moveCall);
        });
        document.body.onmouseup = removeCall;
    },
    scroll: function (offset) {
        var passHeight = this.sliderBar.parentNode.offsetHeight - this.sliderBar.offsetHeight;  //滚动条出去按钮的高度
        if (offset < 0) offset = 0;
        if (offset > passHeight) offset = passHeight;

        var bit = offset / passHeight;      //获取比例
        this.sliderBar.style.marginTop = offset + 'px';
        this.ele.style.top = -Math.round((this.ele.offsetHeight - this.pnode.offsetHeight) * bit + 1) + 'px';
        //alert(t.startP.x + '/' + t.moveP.x + '\n' + t.startP.y + '/' + t.moveP.y + '\n' + this.style.marginTop);
        //j.$('navigator').innerHTML = this.sliderBar.offsetTop + '/' + offset;
    }
};



$JS.extend({
    scrollbar: function (e) {
        return JS_scrollbar.init(e);
    }
});

//页面初始化时
$JS.event.add(window, 'load', function () {
    $JS.each(document.getElementsByClassName('ui-scrollbar'), function (i, e) {
        JS_scrollbar.init(e);
    });
});


