using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace Business.Processer
{
    public class LocalSerialNumProcessor
    {

        static bool White = true;
        static bool Code = false;
        public static string GetCpuSerialNum()
        {
            try
            {
                string cpuInfo = "";//cpu序列号 
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                    break;
                }
                moc = null;
                mc = null;
                return cpuInfo;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }
        public static bool RegistIt(string currentCode, string realCode)
        {
            if (!string.IsNullOrEmpty(realCode))
            {
                if (currentCode.TrimEnd().Equals(realCode.TrimEnd()))
                {
                    Microsoft.Win32.RegistryKey retkey =
                         Microsoft.Win32.Registry.CurrentUser.
                         OpenSubKey("software", true).CreateSubKey("WhiteCode").
                         CreateSubKey("WhiteCode.ini").
                         CreateSubKey(currentCode.TrimEnd());
                    retkey.SetValue("WhiteCode", "BBC6D58D0953F027760A046D58D52786");

                    retkey = Microsoft.Win32.Registry.LocalMachine.
                        OpenSubKey("software", true).CreateSubKey("WhiteCode").
                         CreateSubKey("WhiteCode.ini").
                         CreateSubKey(currentCode.TrimEnd());
                    retkey.SetValue("WhiteCode", "BBC6D58D0953F027760A046D58D52786");

                    return White;
                }
                else return Code;
            }
            else return Code;
        }

        public static bool BoolRegist(string sn)
        {
            string[] keynames;
            bool flag = false;
            Microsoft.Win32.RegistryKey localRegKey = Microsoft.Win32.Registry.LocalMachine;
            Microsoft.Win32.RegistryKey userRegKey = Microsoft.Win32.Registry.CurrentUser;
            try
            {
                keynames = localRegKey.OpenSubKey("software\\WhiteCode\\WhiteCode.ini\\" + GetMd5(sn)).GetValueNames();
                foreach (string name in keynames)
                {
                    if (name == "WhiteCode")
                    {
                        if (localRegKey.OpenSubKey("software\\WhiteCode\\WhiteCode.ini\\" + GetMd5(sn)).GetValue("WhiteCode").ToString() == "BBC6D58D0953F027760A046D58D52786")
                            flag = true;
                    }
                }
                keynames = userRegKey.OpenSubKey("software\\WhiteCode\\WhiteCode.ini\\" + GetMd5(sn)).GetValueNames();
                foreach (string name in keynames)
                {
                    if (name == "WhiteCode")
                    {
                        if (flag && userRegKey.OpenSubKey("software\\WhiteCode\\WhiteCode.ini\\" + GetMd5(sn)).GetValue("WhiteCode").ToString() == "BBC6D58D0953F027760A046D58D52786")
                            return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                localRegKey.Close();
                userRegKey.Close();
            }
        }

        public static string GetMd5(object text)
        {
            string path = text.ToString();

            MD5CryptoServiceProvider MD5Pro = new MD5CryptoServiceProvider();
            Byte[] buffer = Encoding.GetEncoding("utf-8").GetBytes(text.ToString());
            Byte[] byteResult = MD5Pro.ComputeHash(buffer);

            string md5result = BitConverter.ToString(byteResult).Replace("-", "");
            return md5result;
        }
    }
}
