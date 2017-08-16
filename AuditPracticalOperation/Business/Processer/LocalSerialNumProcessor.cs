﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace Business.Processer
{
    public class LocalSerialNumProcessor
    {
        private LocalSerialNumProcessor()
        {

        }

        public string GetCpuID()
        {
            try
            {
                string cpuInfo = "";//cpu序列号 
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
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
    }
}
