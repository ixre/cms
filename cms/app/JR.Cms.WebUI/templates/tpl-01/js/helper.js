String.prototype.trim = function() {
    return this.replace(/^\s+|\s+$/g,"");
}

String.prototype.count=function(s1) { 
	return (this.length - this.replace(new RegExp(s1,"g"), '').length) / s1.length;
}

String.prototype.repeat = function(l){
	return new Array(l+1).join(this);
};

function _form_collect_data(thisform) {
    var s_data = "";
    for (var i = 0; i < thisform.elements.length; i++) {
    	if (typeof(FCKeditorAPI) != "undefined" && 
    			typeof(FCKeditorAPI.GetInstance(thisform.elements[i].name)) != "undefined") {
    		s_data += "&" + thisform.elements[i].name + "=" + encodeURIComponent(FCKeditorAPI.GetInstance(thisform.elements[i].name).GetXHTML());
    	} else {
	    	if (thisform.elements[i].tagName.toLowerCase() == "input" && 
	    			(thisform.elements[i].type.toLowerCase() == "checkbox" || thisform.elements[i].type.toLowerCase() == "radio") && 
	    			!thisform.elements[i].checked) {
	    		continue;
	    	} else if (thisform.elements[i].tagName.toLowerCase() == "select") {
	    		for (var j = 0; j < thisform.elements[i].options.length; j++) {
	    			if (thisform.elements[i].options[j].selected) {
	    				s_data += "&" + thisform.elements[i].name + "=" + encodeURIComponent(thisform.elements[i].options[j].value);
	    			}
	    		}
	    	} else {
	    		s_data += "&" + thisform.elements[i].name + "=" + encodeURIComponent(thisform.elements[i].value);
	    	}
    	}
    }
    return s_data.substring(1);
}

function _ajax_submit(thisform, fn_success, fn_failure) {
    // This function require Prototype library
    //var thisform_id = thisform.id;
    $.ajax({
        type: "POST",
        url: thisform.action,
        timeout: 10000, // 设定超时时间 21/07/2010
        data: _form_collect_data(thisform),
        success: fn_success,
        error: fn_failure
    });
}

function _ajax_request(module, action, params, fn_success, fn_failure) {
    // This function require Prototype library
    var url = "index.php@_m=" + module + "&_a=" + action + "&_r=_ajax";
    // Reform query string
    for (key in params) {
       url += "&" + key + "=" + params[key];
    }
    
    $.ajax({
        type: "GET",
        url: url,
        success: fn_success,
        error: fn_failure
    });
}

function _eval_json(s_json) {
    if (s_json.trim().length == 0) return false;
    return eval("(" + s_json + ")");
}

function random_str(len) {
    var chars = "abcdefghijklmnopqrstuvwxyz" 
        + "ABCDEFGHIJKLMNOPQRSTUVWXYZ" 
        + "1234567890";
    var idx = 0;
    var random_s = "";
    
    len = parseInt(len);
    if (!len || len == "NaN" || len < 6) {
        len = 6;
    }
    for (var i = 0; i < len; i++) {
        idx = Math.floor(Math.random() * chars.length);
        random_s += chars.substr(idx, 1);
    }
    
    return random_s;
}

function show_inline_win(load_url, title_txt, w_width, w_height) {
    if (!title_txt) title_txt = "";
    if (!w_width) w_width = 640;
    if (!w_height) w_height = 480;
    load_url += "&width=" + w_width;
    load_url += "&height=" + w_height;
    
    tb_show(title_txt, load_url, false);
}

function show_iframe_win(load_url, title_txt, w_width, w_height) {
    if (!title_txt) title_txt = "";
    if (!w_width) w_width = 640;
    if (!w_height) w_height = 480;
    load_url += "&width=" + w_width;
    load_url += "&height=" + w_height;
    //load_url += "&modal=true";
    load_url += "&TB_iframe=true";
    
    tb_show(title_txt, load_url, false);
}

function show_iframe_win_n_modal(load_url, title_txt, w_width, w_height) {
    if (!title_txt) title_txt = "";
    if (!w_width) w_width = 640;
    if (!w_height) w_height = 480;
    load_url += "&width=" + w_width;
    load_url += "&height=" + w_height;
    load_url += "&TB_iframe=true";
    
    tb_show(title_txt, load_url, false);
}

function show_imgpicker(img_id) {
	var picker_url = "index.php@_m=mod_media&_a=image_picker&imgid=" + img_id;
    show_iframe_win_n_modal(picker_url, "选择图片", 380, 60);
}

function show_flvpicker(flv_id) {
	var picker_url = "index.php@_m=mod_media&_a=flash_picker&flvid=" + flv_id;
    show_iframe_win_n_modal(picker_url, "Choose Flash", 380, 60);
}

function show_adpicker( vl, title, pos ) {
	var picker_url = "index.php@_m=mod_advert&_a=ad_picker&adtype=" + vl + '&p=' + pos;
    show_iframe_win_n_modal(picker_url, title, 550, 330);
}

function show_markpicker( vl, title, status, msg ) {
	var picker_url = "index.php@_m=mod_attachment&_a=watermark_preview&mktype=" + vl + "&wt=" + status + "&msg=" + msg;
    show_iframe_win_n_modal(picker_url, title, 520, 425);
}

function save_position(thisform, fn_success, fn_failure) {
    _ajax_submit(thisform, fn_success, fn_failure);
}

function _reform_url(url, q_extra) {
    var q_hash = Array();
    var url_parts = url.split(/#/);
    var anchor = url_parts[1];
    var q_new = "";
    
    // Parse query string
    url_parts = url_parts[0].split(/\?/);
	if (url_parts[1] != "undefined") {
	    var queries = url_parts[1].split(/&/);
	    for (var i in queries) {
	        q_parts = queries[i].split(/=/);
	        q_hash[q_parts[0]] = q_parts[1];
	    }
	}
	for (q_key in q_extra) {
	    q_hash[q_key] = q_extra[q_key];
	}
	
	// Reform query string
	for (q_key in q_hash) {
	   q_new += "&" + q_key + "=" + q_hash[q_key];
	}
	
	return url_parts[0] + "?" + q_new.substr(1) + "#" + anchor;
}

function goto(url, q_extra) {
	window.location.href = _reform_url(url, q_extra);
}

function reloadPage() {
    window.location.reload();
}

function reloadParent() {
    window.parent.location.reload();
}

function goto_d(url) {
    window.location.href = url;
}

function parent_goto_d(url) {
    window.parent.location.href = url;
}

function confirm_r(module, action, params, fn_success, fn_failure, message) {
	if (confirm(message)) {
		_ajax_request(module, action, params, fn_success, fn_failure);
	}
}

function remove_block(mb_id,mb_lang) {
	confirm_r('mod_tool', 'rm_mblock', {mb_id:mb_id}, 
			rmblock_success, rmblock_failure, mb_lang=='zh_CN'?"您确认删除该模块吗?":"Delete the module block");
}

function rmblock_success(response) {
    var o_result = _eval_json(response);
    if (!o_result) {
        return rmblock_failure(response);
    }
    
    if (o_result.result == "ERROR") {
        alert(o_result.errmsg);
        return false;
    } else if (o_result.result == "OK") {
	    miss_block(o_result.dom_id);
    } else {
        return rmblock_failure(response);
    }
}

function rmblock_failure(response) {
    alert("Request failed!");
    return false;
}

function miss_block(dom_id) {
	if (typeof(document.getElementById(dom_id)) != 'undefined') {
		$("#" + dom_id).hide("normal");
	}
}

function resizeImg(el, maxwidth) {
    if (!maxwidth) {
        maxwidth = 280;
    }
    
	if (el.width > maxwidth) {
		el.height = el.height / el.width * maxwidth;
		el.width = maxwidth;
	}
}

function totop() {
    window.scroll(0,0);
}

function changePic(isTrue,id,url)
{
	$.ajax({
		type:"POST",
		url:url,
		beforeSend:function(data){
			$("#"+id).attr('src','template/images/loader.gif');
		},
		success:function(data){
			if(data == 1)
			{
				$("#"+id).attr('src','template/images/yes.gif').attr('alt','Yes');
			}
			else
			{
				$("#"+id).attr('src','template/images/no.gif').attr('alt','No');
			}
		},
		error:function(data){
			$("#"+id).attr('src','template/images/warning.gif');
		}
	});
}

function menuCollapse()
{
	if(findCookie("collapseStatus=") == 'extend')
	{
		$("#menu_collapse").hide(1000);
		document.cookie = 'collapseStatus=collapse';
	}
	else
	{
		$("#menu_collapse").show(1000);
		document.cookie = 'collapseStatus=extend';
	}
}

function menuToolCollapse()
{
	if(findCookie("collapseMenuToolStatus=") == 'menuToolExtend')
	{
		$("#menu_tool").hide("slow");
		document.cookie = 'collapseMenuToolStatus=menuToolCollapse';
	}
	else
	{
		$("#menu_tool").show("slow");
		document.cookie = 'collapseMenuToolStatus=menuToolExtend';
	}
}

function menuSystemCollapse()
{
	if(findCookie("collapseMenuSystemStatus=") == 'menuSystemExtend')
	{
		$("#menu_system").hide("slow");
		document.cookie = 'collapseMenuSystemStatus=menuSystemCollapse';
	}
	else
	{
		$("#menu_system").show("slow");
		document.cookie = 'collapseMenuSystemStatus=menuSystemExtend';
	}
}

function findCookie(search)
{
	var cookieValue;
	if (document.cookie.length > 0)
	{
		offset = document.cookie.indexOf(search)
	    if (offset != -1)
	    {
	         offset += search.length ;
	         end = document.cookie.indexOf(";", offset)
	         if (end == -1)
	            end = document.cookie.length;  
	         cookieValue = unescape(document.cookie.substring(offset, end))
	    }
		return cookieValue;
	 }
	else
	{
		return false;
	}
}

function select_for_menu_item(text, id) {
//    window.top.show_selected(text);
//    window.top.set_tmp_id(id);
//    window.top.tb_remove();
	parent.$('#showContents').contents().find('#menu_link_content_title').html(text);
	parent.$('#showContents').contents().find('#tmp_id').val(id);
	parent.$('#showContents').contents().find('#mi_selected_content_').attr('value',parent.$('#showContents').contents().find('#menu_link_content_title').html());
	parent.$('#showContents').show();
	parent.$('#showContents1').remove();
//	parent.$('iframe:last').parent().dialog('close');
}

function clickCheckbox()
{
	var uid;
	uid = $('#uid').val();
	
	$.ajax({
        type: "GET",
        url: "index.php@_m=mod_wizard&_a=admin_wizard&_r=_ajax&action=wizard&uid="+uid,
        success:function(msg){
		//alert(msg);
		},
		error:function(msg){
			//alert(msg);
		}
    });
}

function updatecartstate(response) {
    var o_result = _eval_json(response);
    if (!o_result) 
    {
        return addprodfailed(response);
    }
    
    if (o_result.result == "ERROR") 
    {
        alert(o_result.errmsg);
        return false;
    } 
    else if (o_result.result == "OK") 
    {
        $("#disp_n_prds").html(o_result.n_prds);
//        alert("<?php _e('The product has been added to cart!'); ?>");
        window.location.href = "index.php@_m=mod_cart&_a=viewcart";
        return true;
    } 
    else 
    {
        return on_failure(response);
    }
}

function addprodfailed(response) {
    alert("请求失败！\nRequest failed!");
    return false;
}

function add2cart(p_id) {
    var p_num = document.getElementById("prod_num_" + p_id).value;
    if(/^[1-9]\d{0,}$/.test(p_num))
    {
    	_ajax_request("mod_cart", "addtocart", 
    	        { p_id: p_id, p_num: p_num }, updatecartstate, addprodfailed);
    }
    else
    {
    	alert("请输入自然数！\nPlease input number!");
    }
}

//可拖拉，变尺寸弹出框
function popup_window(load_url, title_txt, popup_width, popup_height, fresh_window, relative_position_x, relative_position_y,no_sizeable,no_showMax,no_tag)
{
	if (!title_txt) title_txt = "";
    if (!popup_width) popup_width = 640;
//    if (!popup_height) popup_height = 480;
    var loading = 'admin/template/view/frontpage/loading.php';
	if (no_tag) loading = 'template/view/frontpage/loading.php';
    
	var isMax = false;
	var posX = ($(window).width()-popup_width)/2;//X Y轴坐标,使弹出框在页面中央
	var posY = 10;
	
	if(relative_position_x) posX += relative_position_x;
	if(relative_position_y) posY -= relative_position_y;
	
	var domId = 'dialog_' + makeRandomNum(100);
	var tmp1 = '';
	var tmp2 = '';
	
	$('body').append("<div id='"+domId+"'></div>");
	
	$("#"+domId).dialog({
		resizable: true,
		show: 'slide',
		title: title_txt,
		modal: true,
		autoOpen: true,
		position: [posX,posY],
		width: popup_width,
		height: popup_height,
		close: function(event,ui){
			$('.ui-dialog').remove();
			$("#"+domId).remove();//为了解决页面刷新弹出框大小变小的问题
			if(!fresh_window) {
				window.location.reload();//关闭窗口时刷新页面
			}
			
		},
		resizeStop: function(event,ui){
			$('#showContents,#showContents1').attr({width:$("#"+domId).dialog( "option", "width")-40,height:$("#"+domId).dialog("option", "height")-68});
			$("#"+domId).dialog( "option", "resizable", true );
		},
		open: function(event, ui) {
			if(no_sizeable) {
				$("#"+domId).dialog( "option", "resizable", false );
			}
			
			$("#"+domId).children().remove();
			if (!popup_height) {
				tmp1 = $("#"+domId).css('height');
				tmp2 = $(".ui-dialog").css('height');
				$("#"+domId).append("<iframe onload='iFrameWidthHeight(\""+domId+"\",false,1);' name='showContents' allowtransparency='yes' frameborder='no' style='border:1px solid #99bbe8' scrolling='auto' id='showContents2' height="+(popup_height-68)+" width="+(popup_width-40)+"  src="+loading+"></iframe>");
				$("#showContents2").attr('height','70px');
				$("#"+domId).css('height','90px');
				$(".ui-dialog").css('height','90px');
				$("#"+domId).append("<iframe onload='iFrameWidthHeight(\""+domId+"\");' name='showContents' allowtransparency='yes' frameborder='no' style='border:1px solid #99bbe8' scrolling='auto' id='showContents' height="+(popup_height-68)+" width="+(popup_width-40)+"  src='"+load_url+"'></iframe>");
			} else {
				$("#"+domId).append("<iframe name='showContents' allowtransparency='yes' frameborder='no' style='border:1px solid #99bbe8' scrolling='auto' id='showContents' height="+(popup_height-68)+" width="+(popup_width-40)+"  src='"+load_url+"'></iframe>");
			}
			
			
		}
	});
	
	//弹出框添加最大化按钮
	if(!no_showMax) {
		$('<a href="#" style="margin-right:18px;" class="ui-dialog-titlebar-close ui-corner-all" role="button" unselectable="on" style="-moz-user-select: none;"><span class="ui-icon ui-icon-closethick1" unselectable="on" style="-moz-user-select: none;">max</span></a>').appendTo('.ui-widget-header')
		.hover(function(){
			$(this).addClass('ui-state-hover');//添加凹陷效果
		},function(){
			$(this).removeClass('ui-state-hover');//注销凹陷效果
		}).click(function(){
			//添加最大化单击事件
			if(!isMax){
				$("#"+domId).dialog( "option", "width", $(document).width()-5);
				$("#"+domId).dialog( "option", "height", $(document).height()-5);
				$("#"+domId).dialog( "option", "position", [0,0]);
				$('#showContents,#showContents1').attr({width:$("#"+domId).dialog( "option", "width")-40,height:$("#"+domId).dialog("option", "height")-68});
				$("#"+domId).dialog( "option", "resizable", false );//最大化后就不能再用拖动来修改尺寸了
				$('.ui-icon-closethick1').css('background-image','url("images/msize1.gif")');//把最大化按钮修改为最小化按钮
				isMax = true;
			} else {
				if((popup_width >= $(document).width()-10) && (popup_height >= $(document).height()-10)){
					popup_width = 640;//将要还原的尺寸
					popup_height = 480;
				}
				if (!popup_height) {
					iFrameWidthHeight(domId);
				} else {
					iFrameWidthHeight(domId,popup_height);
				}
				if($("#"+domId).dialog( "option", "height" ) + 70 <= parseInt($(window).height()) / 2 + 50) {
					$('.ui-dialog').css({"position":"absolute","left":posX,"_left":posX,"top":posY + 130,"_top":posY + 130,"width":popup_width,"_width":popup_width});//定位弹出框位置
				} else {
					$('.ui-dialog').css({"position":"absolute","left":posX,"_left":posX,"top":10,"_top":10,"width":popup_width,"_width":popup_width});//定位弹出框位置
				}
				
				$('#showContents,#showContents1').attr({width:(popup_width-40)});
				$("#"+domId).dialog( "option", "resizable", true );
				
				$('.ui-icon-closethick1').css('background-image','url("images/msize.gif")');
				isMax = false;
			}
		});
	}
	//ajax获取目标界面			
//	$.ajax({
//		type: "GET",
//		url: load_url,
//		dataType: "html",
//		timeout:10000,
//		success:function(msg){
//			$("#"+domId).children().remove();//删除上一次动态添加的ajax内容
//			$("#"+domId).append(msg);
//		},
//		error:function(msg){
//			$("#"+domId).children().remove();
//			$("#"+domId).append("<p>error</p>");
//		}
//	});
}

//产生随机数
function makeRandomNum(n)
{
	var rand_num = 0;
	rand_num = Math.floor(Math.random()*n+1);
	return rand_num;
}

//iframe自适应长宽
function iFrameWidthHeight(domId,popup_height,flag)
{
	if(flag != 1)
	{
		$('#showContents2').remove();
	}
	var ifm = '';
	var subWeb = '';
	if($('#showContents').css('display') != 'none') {
		ifm = document.getElementById("showContents");
		subWeb = document.frames ? document.frames["showContents"].document : ifm.contentDocument;
	} else if($('#showContents1').css('display') != 'none') {
		ifm = document.getElementById("showContents1");
		subWeb = document.frames ? document.frames["showContents1"].document : ifm.contentDocument;
	} else if($('#showContents2').css('display') != 'none') {
		ifm = document.getElementById("showContents2");
		subWeb = document.frames ? document.frames["showContents2"].document : ifm.contentDocument;
	}
	if(ifm != null && subWeb != null) {
//		if((parseInt(subWeb.body.scrollHeight)+70+20) < parseInt($(window).height())){
			if(!popup_height) {
				ifm.height = subWeb.body.scrollHeight+30;
				$("#"+domId).dialog( "option", "height", parseInt(subWeb.body.scrollHeight)+85);//弹出框自适应
			} else {
				$("#"+domId).dialog( "option", "height", parseInt(popup_height));//弹出框自适应
			}
//		} else {
//			ifm.height = subWeb.body.scrollHeight;
//			$("#"+domId).dialog( "option", "height", parseInt(subWeb.body.scrollHeight)+70);//弹出框自适应
//		}
		if(flag != 1)
		{	
			if((parseInt(subWeb.body.scrollHeight)+70) <= parseInt($(window).height()) / 2 + 50) {
				if(parseInt($('.ui-dialog').css("top")) + 130 <= 140) {
					$('.ui-dialog').css('top',parseInt($('.ui-dialog').css("top")) + 130);
				}
			} else {
				$('.ui-dialog').css('top',parseInt($('.ui-dialog').css("top")));
			}
		}
	}
}

function set_default_lang(lang_id,local) {
    $.ajax({
        type: "GET",
        url: "admin/index.php@_m=mod_lang&_a=admin_make_default&_r=_ajax&l_id="+lang_id+"&lg="+local,
    	success: function(data) {
    		var o_result = _eval_json(data);
    		if (o_result.local != 'zh_TW') window.location.reload();
    	}
    });
}