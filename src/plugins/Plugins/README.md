# 插件开发指南 J6.Cms Plugin Development Guide #

## 插件DEMO ##
见com.plugin.datapicker,插件插件主要实现了一个内容采集的功能。

## 插件的扩展名 ##
	插件是一个.dll(或.so)为扩展名的类库。建议使用.so作为插件的后缀，
	ASP.NET下.so插件放在bin目录下，可以自动加载。

## 如何编译为.so格式 ##
	VS项目属性=》生成事件=》后期生成事件中输入下面命令，编译后可以自动得到.so文件。
	move $(TargetPath)  $(SolutionDir)$(OutDir)$(TargetName).so

## 如何加载插件 ##
	编写插件后，编译项目，将编译生成的.dll文件拷贝到网站的plugins目录下，
	重新启动可加载插件。

## 自动生成并加载插件 ##
	此操作需要将插件的后缀设置为.so.同样可以定义编译事件，将插件拷贝到网站的bin目录下即可。
