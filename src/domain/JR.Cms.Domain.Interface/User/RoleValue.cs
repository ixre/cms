
namespace JR.Cms.Domain.Interface.User
{
   public  class RoleValue:IValueObject
    {
       public RoleValue()
       {
           
       }

       public RoleValue(int flag, int appId, string name,bool enabled)
       {
           this.Name = name;
           this.AppId = appId;
           this.Flag = flag;
           this.Enabled = enabled ? 1 : 0;
       }

       public RoleValue(int flag, string name):this(flag,0,name,true)
       {
       }


       /// <summary>
       /// 角色名称
       /// </summary>
       public string Name { get; set; }

       /// <summary>
       /// 特定于系统的角色，为0表示所有系统都拥有的角色
       /// </summary>
       public int AppId { get; set; }

       /// <summary>
       /// 标志位
       /// </summary>
       public int Flag { get; set; }

       /// <summary>
       /// 是否启用
       /// </summary>
       public int Enabled { get; set; }

        public bool Equal(IValueObject that)
        {
            return ((RoleValue) that).Flag == this.Flag;
        }

       /// <summary>
       /// 是否匹配权限
       /// </summary>
       /// <param name="roleFlag"></param>
       /// <returns></returns>
       public bool Match(int roleFlag)
       {
           return (roleFlag & this.Flag) != 0;
       }
    }
}
