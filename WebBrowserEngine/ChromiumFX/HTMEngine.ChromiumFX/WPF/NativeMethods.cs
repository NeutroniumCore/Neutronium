using System;
using System.Runtime.InteropServices;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF
{
    internal static class NativeMethods
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32")]
        internal static extern bool ReleaseCapture();

        [DllImport("user32")]
        internal static extern IntPtr SetCapture(IntPtr hWnd);

        internal static int LoWord(int dwValue)
        {
            return dwValue & 0xffff;
        }

        internal static int HiWord(int dwValue)
        {
            return (dwValue >> 16) & 0xffff;
        }

        internal static int LoWord(IntPtr dwValue)
        {
            return (int)dwValue & 0xffff;
        }

        internal struct windowsParam
        {
            internal const int MK_LBUTTON = 0x0001;
        }

        internal struct HitTest
        {
            internal const int HTNOWHERE = 0;
            internal const int HTCLIENT = 1;
            internal const int HTCAPTION = 2;
            internal const int HTGROWBOX = 4;
            internal const int HTSIZE = HTGROWBOX;
            internal const int HTMINBUTTON = 8;
            internal const int HTMAXBUTTON = 9;
            internal const int HTLEFT = 10;
            internal const int HTRIGHT = 11;
            internal const int HTTOP = 12;
            internal const int HTTOPLEFT = 13;
            internal const int HTTOPRIGHT = 14;
            internal const int HTBOTTOM = 15;
            internal const int HTBOTTOMLEFT = 16;
            internal const int HTBOTTOMRIGHT = 17;
            internal const int HTREDUCE = HTMINBUTTON;
            internal const int HTZOOM = HTMAXBUTTON;
            internal const int HTSIZEFIRST = HTLEFT;
            internal const int HTSIZELAST = HTBOTTOMRIGHT;
        }

        internal struct WindowsMessage
        {
            internal const int WM_ACTIVATE = 0x0006;
            internal const int WM_ACTIVATEAPP = 0x001C;
            internal const int WM_AFXFIRST = 0x0360;
            internal const int WM_AFXLAST = 0x037F;
            internal const int WM_APP = 0x8000;
            internal const int WM_ASKCBFORMATNAME = 0x030C;
            internal const int WM_CANCELJOURNAL = 0x004B;
            internal const int WM_CANCELMODE = 0x001F;
            internal const int WM_CAPTURECHANGED = 0x0215;
            internal const int WM_CHANGECBCHAIN = 0x030D;
            internal const int WM_CHANGEUISTATE = 0x0127;
            internal const int WM_CHAR = 0x0102;
            internal const int WM_CHARTOITEM = 0x002F;
            internal const int WM_CHILDACTIVATE = 0x0022;
            internal const int WM_CLEAR = 0x0303;
            internal const int WM_CLOSE = 0x0010;
            internal const int WM_COMMAND = 0x0111;
            internal const int WM_COMPACTING = 0x0041;
            internal const int WM_COMPAREITEM = 0x0039;
            internal const int WM_CONTEXTMENU = 0x007B;
            internal const int WM_COPY = 0x0301;
            internal const int WM_COPYDATA = 0x004A;
            internal const int WM_CREATE = 0x0001;
            internal const int WM_CTLCOLORBTN = 0x0135;
            internal const int WM_CTLCOLORDLG = 0x0136;
            internal const int WM_CTLCOLOREDIT = 0x0133;
            internal const int WM_CTLCOLORLISTBOX = 0x0134;
            internal const int WM_CTLCOLORMSGBOX = 0x0132;
            internal const int WM_CTLCOLORSCROLLBAR = 0x0137;
            internal const int WM_CTLCOLORSTATIC = 0x0138;
            internal const int WM_CUT = 0x0300;
            internal const int WM_DEADCHAR = 0x0103;
            internal const int WM_DELETEITEM = 0x002D;
            internal const int WM_DESTROY = 0x0002;
            internal const int WM_DESTROYCLIPBOARD = 0x0307;
            internal const int WM_DEVICECHANGE = 0x0219;
            internal const int WM_DEVMODECHANGE = 0x001B;
            internal const int WM_DISPLAYCHANGE = 0x007E;
            internal const int WM_DRAWCLIPBOARD = 0x0308;
            internal const int WM_DRAWITEM = 0x002B;
            internal const int WM_DROPFILES = 0x0233;
            internal const int WM_ENABLE = 0x000A;
            internal const int WM_ENDSESSION = 0x0016;
            internal const int WM_ENTERIDLE = 0x0121;
            internal const int WM_ENTERMENULOOP = 0x0211;
            internal const int WM_ENTERSIZEMOVE = 0x0231;
            internal const int WM_ERASEBKGND = 0x0014;
            internal const int WM_EXITMENULOOP = 0x0212;
            internal const int WM_EXITSIZEMOVE = 0x0232;
            internal const int WM_FONTCHANGE = 0x001D;
            internal const int WM_GETDLGCODE = 0x0087;
            internal const int WM_GETFONT = 0x0031;
            internal const int WM_GETHOTKEY = 0x0033;
            internal const int WM_GETICON = 0x007F;
            internal const int WM_GETMINMAXINFO = 0x0024;
            internal const int WM_GETOBJECT = 0x003D;
            internal const int WM_GETTEXT = 0x000D;
            internal const int WM_GETTEXTLENGTH = 0x000E;
            internal const int WM_HANDHELDFIRST = 0x0358;
            internal const int WM_HANDHELDLAST = 0x035F;
            internal const int WM_HELP = 0x0053;
            internal const int WM_HOTKEY = 0x0312;
            internal const int WM_HSCROLL = 0x0114;
            internal const int WM_HSCROLLCLIPBOARD = 0x030E;
            internal const int WM_ICONERASEBKGND = 0x0027;
            internal const int WM_IME_CHAR = 0x0286;
            internal const int WM_IME_COMPOSITION = 0x010F;
            internal const int WM_IME_COMPOSITIONFULL = 0x0284;
            internal const int WM_IME_CONTROL = 0x0283;
            internal const int WM_IME_ENDCOMPOSITION = 0x010E;
            internal const int WM_IME_KEYDOWN = 0x0290;
            internal const int WM_IME_KEYLAST = 0x010F;
            internal const int WM_IME_KEYUP = 0x0291;
            internal const int WM_IME_NOTIFY = 0x0282;
            internal const int WM_IME_REQUEST = 0x0288;
            internal const int WM_IME_SELECT = 0x0285;
            internal const int WM_IME_SETCONTEXT = 0x0281;
            internal const int WM_IME_STARTCOMPOSITION = 0x010D;
            internal const int WM_INITDIALOG = 0x0110;
            internal const int WM_INITMENU = 0x0116;
            internal const int WM_INITMENUPOPUP = 0x0117;
            internal const int WM_INPUTLANGCHANGE = 0x0051;
            internal const int WM_INPUTLANGCHANGEREQUEST = 0x0050;
            internal const int WM_KEYDOWN = 0x0100;
            internal const int WM_KEYFIRST = 0x0100;
            internal const int WM_KEYLAST = 0x0108;
            internal const int WM_KEYUP = 0x0101;
            internal const int WM_KILLFOCUS = 0x0008;
            internal const int WM_LBUTTONDBLCLK = 0x0203;
            internal const int WM_LBUTTONDOWN = 0x0201;
            internal const int WM_LBUTTONUP = 0x0202;
            internal const int WM_MBUTTONDBLCLK = 0x0209;
            internal const int WM_MBUTTONDOWN = 0x0207;
            internal const int WM_MBUTTONUP = 0x0208;
            internal const int WM_MDIACTIVATE = 0x0222;
            internal const int WM_MDICASCADE = 0x0227;
            internal const int WM_MDICREATE = 0x0220;
            internal const int WM_MDIDESTROY = 0x0221;
            internal const int WM_MDIGETACTIVE = 0x0229;
            internal const int WM_MDIICONARRANGE = 0x0228;
            internal const int WM_MDIMAXIMIZE = 0x0225;
            internal const int WM_MDINEXT = 0x0224;
            internal const int WM_MDIREFRESHMENU = 0x0234;
            internal const int WM_MDIRESTORE = 0x0223;
            internal const int WM_MDISETMENU = 0x0230;
            internal const int WM_MDITILE = 0x0226;
            internal const int WM_MEASUREITEM = 0x002C;
            internal const int WM_MENUCHAR = 0x0120;
            internal const int WM_MENUCOMMAND = 0x0126;
            internal const int WM_MENUDRAG = 0x0123;
            internal const int WM_MENUGETOBJECT = 0x0124;
            internal const int WM_MENURBUTTONUP = 0x0122;
            internal const int WM_MENUSELECT = 0x011F;
            internal const int WM_MOUSEACTIVATE = 0x0021;
            internal const int WM_MOUSEFIRST = 0x0200;
            internal const int WM_MOUSEHOVER = 0x02A1;
            internal const int WM_MOUSELAST = 0x020D;
            internal const int WM_MOUSELEAVE = 0x02A3;
            internal const int WM_MOUSEMOVE = 0x0200;
            internal const int WM_MOUSEWHEEL = 0x020A;
            internal const int WM_MOUSEHWHEEL = 0x020E;
            internal const int WM_MOVE = 0x0003;
            internal const int WM_MOVING = 0x0216;
            internal const int WM_NCACTIVATE = 0x0086;
            internal const int WM_NCCALCSIZE = 0x0083;
            internal const int WM_NCCREATE = 0x0081;
            internal const int WM_NCDESTROY = 0x0082;
            internal const int WM_NCHITTEST = 0x0084;
            internal const int WM_NCLBUTTONDBLCLK = 0x00A3;
            internal const int WM_NCLBUTTONDOWN = 0x00A1;
            internal const int WM_NCLBUTTONUP = 0x00A2;
            internal const int WM_NCMBUTTONDBLCLK = 0x00A9;
            internal const int WM_NCMBUTTONDOWN = 0x00A7;
            internal const int WM_NCMBUTTONUP = 0x00A8;
            internal const int WM_NCMOUSEHOVER = 0x02A0;
            internal const int WM_NCMOUSELEAVE = 0x02A2;
            internal const int WM_NCMOUSEMOVE = 0x00A0;
            internal const int WM_NCPAINT = 0x0085;
            internal const int WM_NCRBUTTONDBLCLK = 0x00A6;
            internal const int WM_NCRBUTTONDOWN = 0x00A4;
            internal const int WM_NCRBUTTONUP = 0x00A5;
            internal const int WM_NCXBUTTONDBLCLK = 0x00AD;
            internal const int WM_NCXBUTTONDOWN = 0x00AB;
            internal const int WM_NCXBUTTONUP = 0x00AC;
            internal const int WM_NCUAHDRAWCAPTION = 0x00AE;
            internal const int WM_NCUAHDRAWFRAME = 0x00AF;
            internal const int WM_NEXTDLGCTL = 0x0028;
            internal const int WM_NEXTMENU = 0x0213;
            internal const int WM_NOTIFY = 0x004E;
            internal const int WM_NOTIFYFORMAT = 0x0055;
            internal const int WM_NULL = 0x0000;
            internal const int WM_PAINT = 0x000F;
            internal const int WM_PAINTCLIPBOARD = 0x0309;
            internal const int WM_PAINTICON = 0x0026;
            internal const int WM_PALETTECHANGED = 0x0311;
            internal const int WM_PALETTEISCHANGING = 0x0310;
            internal const int WM_PARENTNOTIFY = 0x0210;
            internal const int WM_PASTE = 0x0302;
            internal const int WM_PENWINFIRST = 0x0380;
            internal const int WM_PENWINLAST = 0x038F;
            internal const int WM_POWER = 0x0048;
            internal const int WM_POWERBROADCAST = 0x0218;
            internal const int WM_PRINT = 0x0317;
            internal const int WM_PRINTCLIENT = 0x0318;
            internal const int WM_QUERYDRAGICON = 0x0037;
            internal const int WM_QUERYENDSESSION = 0x0011;
            internal const int WM_QUERYNEWPALETTE = 0x030F;
            internal const int WM_QUERYOPEN = 0x0013;
            internal const int WM_QUEUESYNC = 0x0023;
            internal const int WM_QUIT = 0x0012;
            internal const int WM_RBUTTONDBLCLK = 0x0206;
            internal const int WM_RBUTTONDOWN = 0x0204;
            internal const int WM_RBUTTONUP = 0x0205;
            internal const int WM_RENDERALLFORMATS = 0x0306;
            internal const int WM_RENDERFORMAT = 0x0305;
            internal const int WM_SETCURSOR = 0x0020;
            internal const int WM_SETFOCUS = 0x0007;
            internal const int WM_SETFONT = 0x0030;
            internal const int WM_SETHOTKEY = 0x0032;
            internal const int WM_SETICON = 0x0080;
            internal const int WM_SETREDRAW = 0x000B;
            internal const int WM_SETTEXT = 0x000C;
            internal const int WM_SETTINGCHANGE = 0x001A;
            internal const int WM_SHOWWINDOW = 0x0018;
            internal const int WM_SIZE = 0x0005;
            internal const int WM_SIZECLIPBOARD = 0x030B;
            internal const int WM_SIZING = 0x0214;
            internal const int WM_SPOOLERSTATUS = 0x002A;
            internal const int WM_STYLECHANGED = 0x007D;
            internal const int WM_STYLECHANGING = 0x007C;
            internal const int WM_SYNCPAINT = 0x0088;
            internal const int WM_SYSCHAR = 0x0106;
            internal const int WM_SYSCOLORCHANGE = 0x0015;
            internal const int WM_SYSCOMMAND = 0x0112;
            internal const int WM_SYSDEADCHAR = 0x0107;
            internal const int WM_SYSKEYDOWN = 0x0104;
            internal const int WM_SYSKEYUP = 0x0105;
            internal const int WM_TCARD = 0x0052;
            internal const int WM_TIMECHANGE = 0x001E;
            internal const int WM_TIMER = 0x0113;
            internal const int WM_UNDO = 0x0304;
            internal const int WM_UNINITMENUPOPUP = 0x0125;
            internal const int WM_USER = 0x0400;
            internal const int WM_USERCHANGED = 0x0054;
            internal const int WM_VKEYTOITEM = 0x002E;
            internal const int WM_VSCROLL = 0x0115;
            internal const int WM_VSCROLLCLIPBOARD = 0x030A;
            internal const int WM_WINDOWPOSCHANGED = 0x0047;
            internal const int WM_WINDOWPOSCHANGING = 0x0046;
            internal const int WM_WININICHANGE = 0x001A;
            internal const int WM_XBUTTONDBLCLK = 0x020D;
            internal const int WM_XBUTTONDOWN = 0x020B;
            internal const int WM_XBUTTONUP = 0x020C;
        }
    }
}
