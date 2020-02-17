using System;

namespace JR.Cms.ServiceDto
{
   public class UserFormObject:MarshalByRefObject
    {
        public int Id { get; set; }

        public string Password { get; set; }
       public string UserName { get; set; }

       public int Enabled { get; set; }

       public string Phone { get; set; }

       public string Email { get; set; }

       public string Name { get; set; }
    }
}
