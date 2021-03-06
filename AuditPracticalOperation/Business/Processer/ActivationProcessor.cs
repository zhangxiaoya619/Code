﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace Business.Processer
{
    [Singleton]
    public class ActivationProcessor
    {
        private ActivationProcessor()
        {
            
        }
        
        public string GetMd5(object text)
        {
            string path = text.ToString();

            MD5CryptoServiceProvider MD5Pro = new MD5CryptoServiceProvider();
            Byte[] buffer = Encoding.GetEncoding("utf-8").GetBytes(text.ToString());
            Byte[] byteResult = MD5Pro.ComputeHash(buffer);

            string md5result = BitConverter.ToString(byteResult).Replace("-", "");
            return md5result;
        }

        #region code
        /// <summary>
        /// 存储密钥
        /// </summary>
        int[] intCode = new int[127];
        /// <summary>
        /// 存机器码的Ascii值
        /// </summary>
        int[] intNumber = new int[15];
        /// <summary>
        /// 存储机器码字
        /// </summary>
        char[] Charcode = new char[15];

        /// <summary>
        /// 生成机器码
        /// </summary>
        /// <param name="serialnum"></param>
        /// <returns></returns>
        private string getMNum(string serialnum)
        {

            string strNum = serialnum;//+ GetDiskVolumeSerialNumber();//获得24位Cpu和硬盘序列号

            string strMNum = strNum.Substring(0, 16);//从生成的字符串中取出前16个字符做为机器码

            return strMNum;

        }

        /// <summary>
        /// 给数组赋值小于10的数
        /// </summary>
        private void setIntCode()
        {
            for (int i = 1; i < intCode.Length; i++)
            {
                intCode[i] = i % 9;
            }
        }

        /// <summary>
        /// 获取激活码
        /// </summary>
        /// <param name="serialnum"></param>
        /// <returns></returns>
        public string getRNum(string serialnum)
        {

            setIntCode();//初始化127位数组

            for (int i = 1; i < Charcode.Length; i++)//把机器码存入数组中
            {

                Charcode[i] = Convert.ToChar(getMNum(serialnum).Substring(i - 1, 1));

            }

            for (int j = 1; j < intNumber.Length; j++)//把字符的ASCII值存入一个整数组中。
            {

                intNumber[j] = intCode[Convert.ToInt32(Charcode[j])] + Convert.ToInt32(Charcode[j]);

            }

            string strAsciiName = "";//用于存储注册码

            for (int j = 1; j < intNumber.Length; j++)
            {

                if (intNumber[j] >= 48 && intNumber[j] <= 57)//判断字符ASCII值是否0－9之间
                {

                    strAsciiName += Convert.ToChar(intNumber[j]).ToString();

                }

                else if (intNumber[j] >= 65 && intNumber[j] <= 90)//判断字符ASCII值是否A－Z之间
                {

                    strAsciiName += Convert.ToChar(intNumber[j]).ToString();

                }

                else if (intNumber[j] >= 97 && intNumber[j] <= 122)//判断字符ASCII值是否a－z之间
                {

                    strAsciiName += Convert.ToChar(intNumber[j]).ToString();

                }

                else//判断字符ASCII值不在以上范围内
                {

                    if (intNumber[j] > 122)//判断字符ASCII值是否大于z
                    {

                        strAsciiName += Convert.ToChar(intNumber[j] - 10).ToString();

                    }

                    else
                    {

                        strAsciiName += Convert.ToChar(intNumber[j] - 9).ToString();

                    }

                }

            }

            return strAsciiName;

        }
        #endregion
    }
}
