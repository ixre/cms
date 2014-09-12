/*
 * Copyright(C) 2010-2012 OPSoft Inc
 * 
 * File Name	: Module
 * Author	: Newmin (new.min@msn.com)
 * Create	: 2012/9/30 9:59:58
 * Description	:
 *
 */


namespace Spc.Models
{
	/// <summary>
	/// 内容模块
	/// </summary>
	public class Module
	{
		/// <summary>
		/// 模块编号
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// 站点编号
		/// </summary>
		public int SiteId { get; set; }

		/// <summary>
		/// 模块名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 是否系统(站点编号为0)
		/// </summary>
		public bool IsSystem { get; set; }

		/// <summary>
		/// 是否删除
		/// </summary>
		public bool IsDelete { get; set; }
		
		public int ExtID1{get;set;}
		public int ExtID2{get;set;}
		public int ExtID3{get;set;}
		public int ExtID4{get;set;}
	}
}
