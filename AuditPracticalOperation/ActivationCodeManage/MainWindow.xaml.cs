using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ActivationCodeManage
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.header.OnClose += header_OnClose;
            this.Loaded += MainWindow_Loaded;
        }

        void header_OnClose()
        {
            this.Close();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Textbox_SerialNum.Focus();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Textbox_SerialNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnCreate_Click(this, null);
            }
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            Textbox_ActivationCode.Text = getRNum(Textbox_SerialNum.Text);
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
        private string getRNum(string serialnum)
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
