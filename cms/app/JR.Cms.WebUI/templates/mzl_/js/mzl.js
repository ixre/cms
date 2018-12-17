var mzl = {
  playVideo:function(url){
  	var l = this.preparePlayer();
    var v = l.find("video");
    v.html("<source src='"+url+"'>");
    l.animate({"opacity":"1"});
    var w = document.documentElement.clientWidth;
    var h = document.documentElement.clientHeight;
    if(w > h){ // 电脑端
    	var vh = parseInt(h*0.9);
        var vw = 16/9*vh;
        var top = (h-vh)/2;
    	v.css({width:vw+"px","height":vh+"px","margin":top+"px auto 0 auto","background":"transparent"});
        l.find(".vpl-close-img").css({"height":"40px"});
    }else{ // 手机端
        var vh = w/(16/9);
        var top = (h-vh)/2;
    	v.css({width:w+"px","height":vh+"px","margin":top+"px auto 0 auto","background":"transparent"});
        l.find(".vpl-close-img").css({"height":"20px"});
    	if(v.elem().requestFullScreen){
    		v.elem().requestFullScreen();
    	}
    }
    v.elem().play();
  },
  preparePlayer:function(){
  	var isMobile = document.documentElement.clientWidth < 800;
    var e = document.createElement("DIV");
    e.className = "mod-fixed-vplayer video-player-box";
    e.style.cssText = "position:fixed;top:0;right:0;bottom:0;left:0;text-align:center;opacity:0;filter:alpha(opacity=0)";
    e.innerHTML = '<div class="btn-vpl-close"><img src="/public/assets/images/close.png" class="vpl-close-img"/></div>'
    	+'<video id="my-video" class="video-js" controls preload="auto" data-setup="{}"></video>';
    document.body.appendChild(e);
    var l = jr.$fn(e);
    l.find(".btn-vpl-close").click(function(){
    	var p = this.parent();
    	p.find("video").elem().pause();
        p.fadeOut("fast",function(){l.remove();});
    });
    return l;
  },
  init:function(){
  	jr.$fn(".btn-play-video").click(function(){
    	var url = this.attr("video-url");
        if(url)mzl.playVideo(url);
    });
  }
};

mzl.init();