禁止手机缩放
```
<meta name="viewport" content="width=device-width, initial-scale=1.0,maximum-scale=1.0,minimum-scale=1.0,user-scalable=0" />
```
多语言切换
```  
<div class="mod-lang-drop">
   <i class="fa fa-flag"></i>
    <div class="list clearfix">
        <i class="arrow arrow-t"></i>
        <ul>
          <li>chinese</li>
          <li>english</li>
        </ul>
    </div>
</div>
```
视频播放按钮
```
<div class="mod-play-box">
    <div class="mask"></div>
    <div class="play"><i></i></div>
    <img class="thumbnail" src="{thumbnail}" alt="{raw_title}" />
</div>
```
图片按比例缩放
```
<div class="gra-suited-img gra-suited-img-34">
    <div class="mask"></div>
    <img class="thumbnail" src="{thumbnail}" alt="{raw_title}" />
</div>
```
视频播放器
```
<link rel="stylesheet" type="text/css" href="${page.fpath}/widget/video-js/video.min.css" />
<script type="text/javascript" src="${page.fpath}/widget/video-js/video.min.js?${page.built}"></script>
<div class="mod-player">
   <div class="mod-player-mask"></div>
   <div class="mod-player-mark">VIDEO PLAYER</div>
   <video class="video-js" controls muted="muted" preload="auto" poster="${archive.thumbnail}" data-setup="{}">
       <source src="${archive.__dict__[视频地址]}" type="video/mp4">
       <p class='vjs-no-js'>
           To view this video please enable JavaScript, and consider upgrading to a web browser
           that supports HTML5 video.
       </p>
   </video>
</div>
```
