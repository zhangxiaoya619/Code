using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;

namespace Common
{
    public class KeyboardHook
    {
        int hHook;

        Win32Api.HookProc KeyboardHookDelegate;

        public event KeyUpHandler OnKeyUp;

        private const byte VK_LSHIFT = 0xA0;
        private const byte VK_RSHIFT = 0xA1;

        /// <summary>
        /// 安装键盘钩子
        /// </summary>
        public void SetHook()
        {

            KeyboardHookDelegate = new Win32Api.HookProc(KeyboardHookProc);

            ProcessModule cModule = Process.GetCurrentProcess().MainModule;

            var mh = Win32Api.GetModuleHandle(cModule.ModuleName);

            hHook = Win32Api.SetWindowsHookEx(Win32Api.WH_KEYBOARD_LL, KeyboardHookDelegate, mh, 0);

        }

        /// <summary>
        /// 卸载键盘钩子
        /// </summary>
        public void UnHook()
        {

            Win32Api.UnhookWindowsHookEx(hHook);

        }

        /// <summary>
        /// 获取键盘消息
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            // 如果该消息被丢弃（nCode<0
            if (nCode >= 0)
            {

                Win32Api.KeyboardHookStruct KeyDataFromHook = (Win32Api.KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(Win32Api.KeyboardHookStruct));

                int keyData = KeyDataFromHook.vkCode;

                bool shift = (Win32Api.GetKeyState(VK_LSHIFT) & 0x80) != 0 || (Win32Api.GetKeyState(VK_RSHIFT) & 0x80) != 0;

                if (OnKeyUp != null && (wParam == Win32Api.WM_KEYUP || wParam == Win32Api.WM_SYSKEYUP))
                    OnKeyUp((Key)keyData, shift);
            }

            return Win32Api.CallNextHookEx(hHook, nCode, wParam, lParam);
        }
    }

    public delegate void KeyUpHandler(Key key, bool shift);
}
