using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using J6.DevFw.PluginKernel;

// 有关程序集的常规信息通过以下
// 特性集控制。更改这些特性值可修改
// 与程序集关联的信息。
[assembly: AssemblyTitle("J6.Cms XML-RPC")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("J6.Cms XML-RPC Plugin")]
[assembly: AssemblyCopyright("Copyright © ops 2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 将 ComVisible 设置为 false 使此程序集中的类型
// 对 COM 组件不可见。如果需要从 COM 访问此程序集中的类型，
// 则将该类型上的 ComVisible 特性设置为 true。
[assembly: ComVisible(false)]

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("b7be3f8d-90ff-44d7-a32a-00cd3581c102")]

// 程序集的版本信息由下面四个值组成:
//
//      主版本
//      次版本 
//      内部版本号
//      修订号
//
// 可以指定所有这些值，也可以使用“内部版本号”和“修订号”的默认值，
// 方法是按如下所示使用“*”:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.*")]

[assembly:PluginPack(
	"sp.xmlrpc",		        //工作标识，将会在plugins下生成同名的目录，用于存放插件资源
	"icon.png", 	            //插件图标，可为空
	"WeblogRPC", 		        //插件名称
	"官方",			            //插件开发者
    "sp.xmlrpc.sh.aspx?wsd", 	//插件访问入口
	ConfigUrl="",	            //插件设置地址
	Description="XML-RPC离线内容发布支持插件"
)]
                            
