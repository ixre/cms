var videoPath = "/resources/mzl/demo/video/";
// 初始化链接点击事件
var prodVLS = $b.$fn(".mod-prod-vl");
prodVLS.click(function(){
	var ptr = this;
	prodVLS.each(function(i,v){
    	if(ptr === v){
        	v.addClass("active");
            playVideo(videoPath + (v.attr("video-file")||""));
        }else{
        	v.removeClass("active");
        }
    });
});



// 设置头部导航
function setHeaderNav(n){
	$b.$fn(".mod-prod-menu li").each(function(i,v){
    	if(v.attr("menu-data")===n){
           v.find("A").addClass("active");
        }else{
           v.find("A").removeClass("active");
        }
    });
}

// 播放视频
function playVideo(path){
	if(path === videoPath){
    	alert("未上传相关视频");return false;
    }
    mzl.playVideo(path+"xx");
    /*
	$b.$fn(".product-video").html("<div style='background:#000;height:100%'>"
    	+"<embed src='"+path+"' style='width:100%;height:100%;'/></div>");*/
}

// 播放视频按钮事件
$b.$fn(".watch-video").click(function(){
	playVideo(videoPath + (prodVLS.get(0).attr("video-file")||""));
});

// 设置视频尺寸
function resizeVideo(){
  var h = document.documentElement.clientHeight;
  var max = h- $b.$fn(".product-wrap-detail").attr("offsetTop")-30;
  $b.$fn(".mod-prod-d").css({"height":max+"px","overflow":"hidden"});
}
$b.$fn(document.body).resize(resizeVideo);
resizeVideo();
