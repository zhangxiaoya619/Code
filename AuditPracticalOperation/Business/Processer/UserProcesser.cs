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
        public bool UserRegister(string ActiveCode)
        {
            bool issuccess = SingletonManager.Get<ActivationProcessor>().RegistIt(ActiveCode, SingletonManager.Get<ActivationProcessor>().getRNum(user.CPUID));
            if (issuccess)
                user.ChangePower(issuccess);
            return issuccess;
        }
        private UserProcesser()
        {
            user = new User();
            user.CPUID = SingletonManager.Get<LocalSerialNumProcessor>().GetCpuSerialNum();
            user.ChangePower(SingletonManager.Get<ActivationProcessor>().BoolRegist(SingletonManager.Get<ActivationProcessor>().getRNum(user.CPUID)));
        }
    }
}
