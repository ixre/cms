using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// 有关程序集的常规信息通过以下
// 特性集控制。更改这些特性值可修改
// 与程序集关联的信息。
using J6.DevFw.PluginKernel;

[assembly: AssemblyTitle("com.plugin.sso")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("com.plugin.sso")]
[assembly: AssemblyCopyright("Copyright ©  2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 将 ComVisible 设置为 false 使此程序集中的类型
// 对 COM 组件不可见。  如果需要从 COM 访问此程序集中的类型，
// 则将该类型上的 ComVisible 特性设置为 true。
[assembly: ComVisible(false)]

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("02561ea4-f187-4e26-a1e3-bd74424552ed")]

// 程序集的版本信息由下面四个值组成: 
//
//      主版本
//      次版本 
//      生成号
//      修订号
//
// 可以指定所有这些值，也可以使用“生成号”和“修订号”的默认值，
// 方法是按如下所示使用“*”: 
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: PluginPack(
    "com.plugin.sso",		        //工作标识，将会在plugins下生成同名的目录，用于存放插件资源
    "icon.png", 	            //插件图标，可为空
    "Platform SSO", 		        //插件名称
    "官方",			            //插件开发者
    "com.plugin.sso.sh.aspx/about", 	//插件访问入口
    ConfigUrl = "",	            //插件设置地址
    Description = "单点登陆插件"
)]

