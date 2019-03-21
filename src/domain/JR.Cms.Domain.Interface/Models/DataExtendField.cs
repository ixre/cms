/*
 * 由SharpDevelop创建。
 * 用户： newmin
 * 日期: 2013/11/27
 * 时间: 7:21
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using J6.Data.Orm.Mapping;

namespace Spc.Models
{
	/// <summary>
	/// Description of ExtendField.
	/// </summary>
	[DataTable("$PREFIX_dataExtendField")]
	public class DataExtendField
	{
		
		/// <summary>
		/// 扩展字段编号
		/// </summary>
		public Int32 Id{get;set;}
		
		/// <summary>
		/// 与扩展相关联的行的ID
		/// </summary>
		public Int32 Rid{get;set;}
		
		/// <summary>
		/// 扩展ID
		/// </summary>
		public Int32 ExtId{get;set;}
		
		/// <summary>
		/// 扩展属性编号
		/// </summary>
		public Int32 AttrId{get;set;}
		
		/// <summary>
		/// 扩展属性值
		/// </summary>
		public string AttrVal{get;set;}

        /// <summary>
        /// 扩展名称
        /// </summary>
        public string _ExtendName { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string _AttrName { get; set; }
	}
}
