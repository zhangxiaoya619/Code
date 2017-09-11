using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class OfficeOperator
    {
        #region 方法

        #region public static
        /// <summary>
        /// 检测MS-Office是否正确安装
        /// 通过注册表检测
        /// </summary>
        public static bool IsInstall()
        {
            return GetOffice();
        }

        public static bool GetOffice()
        {
            Microsoft.Win32.RegistryKey regKey = null;
            Microsoft.Win32.RegistryKey regSubKey = null;

            try
            {
                regKey = Microsoft.Win32.Registry.LocalMachine;

                if (regSubKey == null)
                {//office97
                    regSubKey = regKey.OpenSubKey(@"SOFTWARE\Microsoft\Office\8.0\Common\InstallRoot", false);
                }

                if (regSubKey == null)
                {//Office2000
                    regSubKey = regKey.OpenSubKey(@"SOFTWARE\Microsoft\Office\9.0\Common\InstallRoot", false);
                }

                if (regSubKey == null)
                {//officeXp
                    regSubKey = regKey.OpenSubKey(@"SOFTWARE\Microsoft\Office\10.0\Common\InstallRoot", false);
                }

                if (regSubKey == null)
                {//Office2003
                    regSubKey = regKey.OpenSubKey(@"SOFTWARE\Microsoft\Office\11.0\Common\InstallRoot", false);
                }

                if (regSubKey == null)
                {//office2007 
                    regSubKey = regKey.OpenSubKey(@"SOFTWARE\Microsoft\Office\12.0\Common\InstallRoot", false);
                }

                if (regSubKey == null)
                {//office2010
                    regSubKey = regKey.OpenSubKey(@"SOFTWARE\Microsoft\Office\14.0\Common\InstallRoot", false);
                }

                if (regSubKey == null)
                {//office2013
                    regSubKey = regKey.OpenSubKey(@"SOFTWARE\Microsoft\Office\15.0\Common\InstallRoot", false);
                }

                if (regSubKey == null)
                {//office2016
                    regSubKey = regKey.OpenSubKey(@"SOFTWARE\Microsoft\Office\16.0\Common\InstallRoot", false);
                }

                if (regSubKey == null)
                {//office2017
                    regSubKey = regKey.OpenSubKey(@"SOFTWARE\Microsoft\Office\17.0\Common\InstallRoot", false);
                }

                if (regSubKey == null)
                {//office2018
                    regSubKey = regKey.OpenSubKey(@"SOFTWARE\Microsoft\Office\18.0\Common\InstallRoot", false);
                }

                if (regSubKey == null)
                {//office2019
                    regSubKey = regKey.OpenSubKey(@"SOFTWARE\Microsoft\Office\19.0\Common\InstallRoot", false);
                }

                if (regSubKey != null)
                    return true;
                else
                    return false;
            }
            finally
            {

                if (regKey != null)
                {
                    regKey.Close();
                    regKey = null;
                }

                if (regSubKey != null)
                {
                    regSubKey.Close();
                    regSubKey = null;
                }
            }
        }
        #endregion public static

        #endregion 方法
    }
}
