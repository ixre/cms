# 模板挂件
1. 视频播放器
```
<link rel="stylesheet" href="${page.fpath}/widget/video-js/video.min.css" />
<script type="text/javascript" src="${page.fpath}/widget/video-js/video.min.js"></script>
<!-- 播放器 -->
<div class="mod-player">
    <div class="mod-player-mask"></div>
    <div class="mod-player-mark">LMS MACHINERY</div>
    <video class="video-js" controls preload="auto" width="640" poster="${archive.thumbnail}" data-setup="{}">
        <source src="${archive.__dict__[视频地址]}" type="video/mp4">
        <p class='vjs-no-js'>
            To view this video please enable JavaScript, and consider upgrading to a web browser that supports HTML5 video.
        </p>
    </video>
</div>
```
