# JRCms.NET 模板指南 #

## 一. 内置JS库
CMS基础JS库:`base.min.js`,包含基本的JS操作,不依赖外部框架. 加载文件的格式如下:
```
<script type="text/javascript" src="${page.fpath}/base.min.js?hover=nav"></script>
```
其典型应用有如下:
- 1.懒加载图片


###１.懒加载图片
在html中添加html,如:
```
<img class="lazy" src="${page.fpath}/images/lazy_holder.gif" data-src="/images/raw.png" alt="图片">
```
当图片可见时才会加载, 必须JS调用


