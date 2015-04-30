
//
// DIV卷轴效果
// 创建时间：2011-10-26 14:51
// _setup控制每步卷的像素
//

$JS.extend({
    roller: {
        init: function (arguments) {
            //参数
            this.elem = arguments.elem;
            this.direction = arguments.direction;
            this.pix = arguments.pix;
            this.elem.style.cssText += 'overflow:hidden;';
        },
        start: function (_setup, callback) {
            var _elem = this.elem,
            _pix = this.pix,
            _setup = _setup | 1;

            //i和j为变量
            var i, j;

            //定时器
            var timer;

            var callbackFunc = function () { if (callback != null) callback(); };

            //
            // 向上和向左逻辑一样,向下和向右则反之
            //
            switch (this.direction) {
                case "up":
                    i = _pix; j = 0;
                    timer = setInterval(function () {
                        i -= _setup;
                        if (i < 0) {
                            i = 0;
                            clearInterval(timer); callbackFunc();
                        }
                        _elem.style.height = i.toString() + "px";
                    }, 10);
                    break;

                case "left":
                    i = _pix; j = 0;
                    timer = setInterval(function () {
                        i -= _setup;
                        if (i < 0) {
                            i = 0;
                            clearInterval(timer); callbackFunc();
                        }
                        _elem.style.width = i.toString() + "px";
                    }, 10);
                    break;

                case "down":
                    i = 0; j = _pix;
                    timer = setInterval(function () {
                        i += _setup;
                        if (i > j) {
                            i = j;
                            clearInterval(timer); callbackFunc();
                        }
                        _elem.style.height = i.toString() + "px";
                    }, 10);
                    break;

                case "right":
                    i = 0; j = _pix;
                    timer = setInterval(function () {
                        i += _setup;
                        if (i > j) {
                            i = j;
                            clearInterval(timer); callbackFunc();
                        }
                        _elem.style.width = i.toString() + "px";
                    }, 10);
                    break;

            }

        }
    }
});

/*
function roller(arguments) {
    //参数
    this.elem = arguments.elem;
    this.direction = arguments.direction;
    this.pix = arguments.pix;
    this.elem.style.cssText += 'overflow:hidden;';
}
roller.prototype.start = function (_setup, callback) {

    var _elem = this.elem,
            _pix = this.pix,
            _setup = _setup | 1;

    //i和j为变量
    var i, j;

    //定时器
    var timer;

    var callbackFunc = function () { if (callback != null) callback(); };

    //
    // 向上和向左逻辑一样,向下和向右则反之
    //
    switch (this.direction) {
        case "up":
            i = _pix; j = 0;
            timer = setInterval(function () {
                i -= _setup;
                if (i < 0) {
                    i = 0;
                    clearInterval(timer); callbackFunc();
                }
                _elem.style.height = i.toString() + "px";
            }, 10);
            break;

        case "left":
            i = _pix; j = 0;
            timer = setInterval(function () {
                i -= _setup;
                if (i < 0) {
                    i = 0;
                    clearInterval(timer); callbackFunc();
                }
                _elem.style.width = i.toString() + "px";
            }, 10);
            break;

        case "down":
            i = 0; j = _pix;
            timer = setInterval(function () {
                i += _setup;
                if (i > j) {
                    i = j;
                    clearInterval(timer); callbackFunc();
                }
                _elem.style.height = i.toString() + "px";
            }, 10);
            break;

        case "right":
            i = 0; j = _pix;
            timer = setInterval(function () {
                i += _setup;
                if (i > j) {
                    i = j;
                    clearInterval(timer); callbackFunc();
                }
                _elem.style.width = i.toString() + "px";
            }, 10);
            break;

    }

};
*/


