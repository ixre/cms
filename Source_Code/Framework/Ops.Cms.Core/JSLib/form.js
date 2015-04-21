/*
* 名称 ： 表单(使用框架)
* 修改说明：
*      2012-09-22  newmin [+] : GetData方法用于获取表单内部的数据
*/

$JS.extend({ form: {
    //获取表单数据
    getData: function (formIdOrIndex) {
        var data = '';
        var form = document.forms[formIdOrIndex || 0];

        return $JS.json.toQueryString(form);

        /*

        //迭代操作函数
        var foreach = function (arr, func) {
            for (var i = 0; i < arr.length; i++) {
                func(arr[i]);
            }
        };

        //用于装载input type="radio"
        var __eles = new Array();

        //查找是否存在名字
        var findName = function (name) {
            for (var i = 0; i < __eles.length; i++) {
                if (__eles[i] == name) {
                    return true;
                }
            }
            return false;
        };

        foreach(form, function (ele) {
            var attr_name = ele.getAttribute('name');
            if (ele.nodeName == 'IMG') {
                // alert(attr_name);
            }
            if (attr_name) {
                //针对Radio单独做处理
                if (ele.nodeName == 'INPUT' && ele.type && ele.type == 'radio') {
                    if (!findName(attr_name)) {
                        foreach(document.getElementsByName(attr_name), function (e) {
                            if (e.checked) {
                                data += '&' + attr_name + '=' + encodeURIComponent(e.value);
                                __eles.push(attr_name);
                            }
                        });
                    }
                }
                //<input type="checkbox"/>无论如何都为on，所以需单独处理
                else if (ele.nodeName == 'INPUT' && ele.type && ele.type == 'checkbox') {
                    data += '&' + attr_name + '=' + encodeURIComponent(ele.checked ? ele.value : '');
                }
                else {
                    data += '&' + attr_name + '=' + encodeURIComponent(ele.value);
                }
            }
            else {
                //自定义标签取消
                //var field = ele.getAttribute('field');
                //if (field) {
                //data += '&' + field + '=' + escape(ele.getAttribute('value'));
                //}
                
            }
           
        }); 

        return data.replace('&', '');*/
    },
    //异步提交
    asyncSubmit: function (formIdOrIndex, showTarget) {
        var form = document.forms[formIdOrIndex || 0];
        var $async_ifr = document.getElementById('$async_ifr');

        //添加DOM元素
        if (!$async_ifr) {
            try {
                //IE核心
                $async_ifr = document.createElement('<iframe name="$async_ifr">');
            } catch (ex) {
                //非IE浏览器
                $async_ifr = document.createElement('iframe');
                $async_ifr.setAttribute('name', '$async_ifr');
            }
            $async_ifr.setAttribute('id', '$async_ifr');

            if (!showTarget) {
                $async_ifr.style.cssText = 'display:none';
            } else {
                $async_ifr.style.cssText = 'width:600px;height:400px';
            }
            document.body.insertBefore($async_ifr, document.body.firstChild);
        }

        //设置表单目标
        if (form.getAttribute('target') != $async_ifr.name) {
            form.setAttribute('target', $async_ifr.getAttribute('name'));
        }

        //提交表单
        form.submit();
    }
}
});