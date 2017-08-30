using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckAdobSetupOrNot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(IsSetupAdob() ? "已安装" : "未安装");
            Console.ReadKey();
        }
        static bool IsSetupAdob()
        {
            RegistryKey adobe = Registry.LocalMachine.OpenSubKey("Software").OpenSubKey("Adobe");

            if (adobe != null)
            {

                RegistryKey acroRead = adobe.OpenSubKey("Acrobat Reader");

                if (acroRead != null)
                {

                    string[] acroReadVersions = acroRead.GetSubKeyNames();

                    return true;

                }

            }
            adobe = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Adobe");
            if (adobe != null)
            {

                RegistryKey acroRead = adobe.OpenSubKey("Acrobat Reader");

                if (acroRead != null)
                {

                    string[] acroReadVersions = acroRead.GetSubKeyNames();

                    return true;

                }

            }
            return false;
        }
    }
}
