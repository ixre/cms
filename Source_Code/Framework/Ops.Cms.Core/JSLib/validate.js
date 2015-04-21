//===================== 验证器(2012-09-30) =====================//
$JS.extend({
    validator: {
        //设置提示
        setTip: function (e, success, summaryKey, msg) {

            //根据键值获取提示信息
            if (summaryKey) {
                var summary = e.getAttribute('summary');
                if (summary) {
                    summary = $JS.toJson(summary);
                    if (summary[summaryKey]) {
                        msg = summary[summaryKey];
                    }
                }
            }

            //如果设置了提示信息容器
            if (e.getAttribute('tipin')) {
                var tipin = document.getElementById(e.getAttribute('tipin'));
                if (tipin) {
                    if (tipin.className.indexOf('validator') == -1) {
                        tipin.className += ' validator';
                    }
                    tipin.innerHTML = '<span class="' + (success ? 'valid-right' : 'valid-error') + '"><span class="msg">' + msg + '</span></span>';
                    return false;
                }
            }


            //未指定提示信息容器,则生成显示的容器
            var tipID = e.getAttribute('validate_id');
            var tipEle = document.getElementById(tipID);
            if (!tipEle) {
                tipEle = document.createElement('DIV');
                tipEle.id = tipID;
                tipEle.className = 'validator';

                var pos = $JS.getPosition(e);

                tipEle.style.cssText = 'position:absolute;left:' + (pos.right + document.documentElement.scrollLeft) + 'px;top:'
                        + (pos.top + document.documentElement.scrollTop) + 'px';
                document.body.appendChild(tipEle);
            }

            //设置值
            tipEle.innerHTML = '<span class="' + (success ? 'valid-right' : 'valid-error') + '"><span class="msg">' + msg + '</span></span>';

        },
        //移除提示
        removeTip: function (e) {

            //如果指定了提示信息容器
            if (e.getAttribute('tipin')) {
                var tipin = document.getElementById(e.getAttribute('tipin'));
                if (tipin) {
                    tipin.innerHTML = '';
                    return false;
                }
            }

            //如果未指定提示信息容器
            var tipEle = document.getElementById(e.getAttribute('validate_id'));
            if (tipEle) {
                document.body.removeChild(tipEle);
            }

        },
        //验证结果
        result: function (id) {
            //指定了父元素
            if (id) {
                var isSuccess = true;
                var ele = document.getElementById(id);
                $JS.each($JS.getElementsByClassName(ele, 'ui-validate'), function (i, e) {
                    if (isSuccess) {
                        //获取显示在指定位置的信息
                        if (e.getAttribute('tipin')) {
                            if ($JS.$(e.getAttribute('tipin')).innerHTML.indexOf('valid-error') != -1) {
                                isSuccess = false;
                            }
                        } else {

                            //获取浮动的信息
                            e = document.getElementById(e.getAttribute('validate_id'));

                            if ($JS.getElementsByClassName(e, 'valid-error').length != 0) {
                                isSuccess = false;
                            }
                        }
                    }
                });
                return isSuccess;

            } else {
                return $JS.getElementsByClassName(document, 'valid-error').length == 0;
            }
        },

        //初始化事件
        init: function () {
            var $J = window.J;
            if (!$J) {
                alert('未引用核心库!');
                return false;
            }

            var eles = document.getElementsByClassName('ui-validate');
            var tipID;

            for (var i = 0; i < eles.length; i++) {

                tipID = eles[i].getAttribute('validate_id');
                while (tipID == null) {
                    tipID = eles[i].id;
                    if (tipID && tipID != '') {
                        tipID = 'validate_item_' + tipID;
                    } else {
                        tipID = 'validate_item_' + parseInt(Math.random() * 1000).toString();
                    }

                    if (document.getElementById(tipID) != null) {
                        tipID = null;
                    } else {
                        eles[i].setAttribute('validate_id', tipID);
                    }
                }

                //验证方法组
                var validFuncs = new Array();

                //添加本身的事件
                if (eles[i].onblur) {
                    validFuncs[validFuncs.length] = eles[i].onblur;
                }

                //只能输入数字
                if (eles[i].getAttribute('isnumber') == 'true') {

                    eles[i].style.cssText += 'ime-mode:disabled';
                    var func = (function (validater, e) {
                        return function () {
                            if (/\D/.test(e.value)) {
                                e.value = e.value.replace(/\D/g, '');
                            }
                            e.value = e.value.replace(/^0([0-9])/, '$1');
                        };
                    })(this, eles[i]);

                    $J.event.add(eles[i], 'keyup', func);
                    $J.event.add(eles[i], 'change', func);
                }

                //============= 使用正则表达式 ==============/
                if (eles[i].getAttribute('regex')) {
                    var func = (function (validator, e) {
                        return function () {
                            var reg = new RegExp();
                            reg.compile(e.getAttribute('regex'));
                            if (!reg.test(e.value)) {
                                validator.setTip(e, false, 'regex', '输入不正确');
                            } else {
                                validator.removeTip(e);
                            }
                        };
                    })(this, eles[i]);

                    //绑定正则验证事件
                    validFuncs[validFuncs.length] = func;

                } else {
                    //================ 常规校验 =================/

                    //不能为空
                    if (eles[i].getAttribute('isrequired') == 'true' || eles[i].getAttribute('required') == 'true') {
                        var func = (function (validator, e) {
                            return function () {
                                if (e.value.replace(/\s/g, '') == '') {
                                    validator.setTip(e, false, 'required', '该项不能为空');
                                } else {
                                    validator.removeTip(e);
                                }
                            };
                        })(this, eles[i]);

                        //绑定空值验证事件
                        validFuncs[validFuncs.length] = func;
                    }


                    //长度限制
                    if (eles[i].getAttribute('length')) {
                        var func = (function (validator, e) {
                            return function () {
                                var pro_val = e.getAttribute('length');
                                var reg = /\[(\d*),(\d*)\]/ig;
                                var l_s = parseInt(pro_val.replace(reg, '$1')),
                                    l_e = parseInt(pro_val.replace(reg, '$2'));

                                if (e.value.length < l_s) {
                                    validator.setTip(e, false, 'length', l_e ? '长度必须为' + l_s + '-' + l_e + '位' : '长度必须大于' + (l_s - 1) + '位');
                                } else if (e.value.length > l_e) {
                                    validator.setTip(e, false, 'length', l_s ? '长度必须为' + l_s + '-' + l_e + '位' : '长度必须小于' + (l_e + 1) + '位');
                                } else if (e.getAttribute('required') == null || e.value.length > 0) {
                                    validator.removeTip(e);
                                }
                            };
                        })(this, eles[i]);

                        //绑定长度验证事件
                        validFuncs[validFuncs.length] = func;
                    }

                }


                var callFuncs = (function (funcs) {
                    return function () {
                        for (var i = 0; i < funcs.length; i++) {
                            if (funcs[i]) {
                                //解决变量引用
                                funcs[i].apply(this, arguments);
                                // _funcs[i]();
                            }
                        }
                    };
                })(validFuncs);

                eles[i].onblur = callFuncs;

                //添加keyup
                // $JS.event.add(eles[i], 'keyup', callFuncs);
            }
        },

        validate: function (id) {
            var eles;
            if (id) {
                //指定了父元素
                eles = $JS.getElementsByClassName(document.getElementById(id), 'ui-validate');

            } else {
                //所有元素，未指定父元素
                eles = $JS.getElementsByClassName(document, 'ui-validate');
            }

            var chkV = function (e) {
                return e.getAttribute('required') == "true" ||
                    e.getAttribute('isrequired') == "true" ||
                    e.getAttribute('length') ||
                    e.getAttribute('regex');
            };

            for (var i = 0; i < eles.length; i++) {
                if (chkV(eles[i])) {
                    if (eles[i].onblur) { 
                        eles[i].onblur();
                    }
                }
            }
            return this.result(id);
        }
    }
});

$JS.event.add(window, 'load', function () {
    $JS.validator.init();
});