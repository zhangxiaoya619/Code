using Business.Processer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestActivationCode
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(LocalSerialNumProcessor.GetCpuSerialNum());
            Console.ReadKey();
        }
    }
}
