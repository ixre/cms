/*
 * 由SharpDevelop创建。
 * 用户： newmin
 * 日期: 2013/11/27
 * 时间: 7:15
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using Ops.Data.Orm.Mapping;

namespace Spc.Models
{
	/// <summary>
	/// Description of Extend.
	/// </summary>
	[DataTable("$PREFIX_dataExtend")]
	public class DataExtend
	{
		/// <summary>
		/// 编号
		/// </summary>
		[Column("ID",IsPrimaryKey=true,AutoGeneried=true)]
		public Int32 ID{get;set;}
		
		/// <summary>
		/// 扩展名称
		/// </summary>
		[Column("Name")]
		public String Name{get;set;}
		
		/// <summary>
		/// 状态(0不可用;1可用)
		/// </summary>
		public Int32 State{get;set;}
	}
}
