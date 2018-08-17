using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T2.Cms.DataTransfer
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
