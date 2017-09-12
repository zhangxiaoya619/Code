using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ViewModel;

namespace Business.Processer
{
    [Singleton]
    public class UserProcesser
    {
        private const string USER_INFO_FILE_NAME = "userinfo";
        private readonly string userInfoFileName;
        private User user;

        public IPower GetPower()
        {
            return user;
        }

        public User GetUser()
        {
            return user;
        }

        private bool WriteActivationCode(string activeCode)
        {
            string[] parameters = File.ReadAllLines(userInfoFileName);
            parameters[2] = activeCode;
            File.WriteAllLines(userInfoFileName, parameters);
            return true;
        }

        private void CheckPower()
        {
            string activeCode = SingletonManager.Get<ActivationProcessor>().getRNum(user.CPUID);
            string[] parameters = File.ReadAllLines(userInfoFileName);
            user.ChangePower(parameters[2] == activeCode);
        }

        public string GetSavedUserName()
        {
            string[] parameters = File.ReadAllLines(userInfoFileName);
            return parameters[1];
        }

        public string GetSavedUserID()
        {
            string[] parameters = File.ReadAllLines(userInfoFileName);
            return parameters[0];
        }

        private void SaveUserNameID(string userName, string userID)
        {
            string[] parameters = File.ReadAllLines(userInfoFileName);
            parameters[0] = userID;
            parameters[1] = userName;
            File.WriteAllLines(userInfoFileName, parameters);
        }

        public bool UserLogin(string userName, string userID)
        {
            bool ret = true;
            user.ID = userID;
            user.Name = userName;
            SaveUserNameID(userName, userID);
            return ret;
        }

        public bool UserRegister(string activeCode)
        {
            if (activeCode != SingletonManager.Get<ActivationProcessor>().getRNum(user.CPUID))
                return false;

            if (WriteActivationCode(activeCode))
            {
                user.ChangePower(true);
                return true;
            }
            else
                return false;
        }

        private UserProcesser()
        {
            userInfoFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, USER_INFO_FILE_NAME);
            if (!File.Exists(userInfoFileName))
            {
                using (StreamWriter sw = File.CreateText(userInfoFileName))
                {
                    sw.WriteLine();
                    sw.WriteLine();
                    sw.WriteLine();
                }
            }

            user = new User();
            user.CPUID = SingletonManager.Get<LocalSerialNumProcessor>().GetCpuSerialNum();
            CheckPower();
        }
    }
}
