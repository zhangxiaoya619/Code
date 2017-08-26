using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModel;

namespace Business.Processer
{
    [Singleton]
    public class UserProcesser
    {
        private User user;

        public IPower GetPower()
        {
            return user;
        }

        public User GetUser()
        {
            return user;
        }

        public bool UserLogin(string userName, string ID)
        {
            bool ret = true;
            user.ID = ID;
            user.Name = userName;
            return ret;
        }
        private UserProcesser()
        {
            user = new User();
            user.CPUID = SingletonManager.Get<LocalSerialNumProcessor>().GetCpuSerialNum();
            //user.ID = "51123";
            //user.Name = "张三";
            //user.CPUID = "CSZS-123";
        }
    }
}
