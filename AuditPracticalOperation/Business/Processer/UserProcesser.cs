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

        private UserProcesser()
        {
            user = new User();
            user.ID = "51123";
            user.Name = "张三";
        }
    }
}
