using LiveViewNet.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LiveViewNet
{
    public class LiveViewMessage
    {
        public const string CLIENT_SOFTWARE_VERSION = "0.0.3";

        public const int MSG_GETCAPS = 1;
        public const int MSG_GETCAPS_RESP = 2;

        public const int MSG_DISPLAYTEXT = 3;
        public const int MSG_DISPLAYTEXT_ACK = 4;

        public const int MSG_DISPLAYPANEL = 5;
        public const int MSG_DISPLAYPANEL_ACK = 6;

        public const int MSG_DEVICESTATUS = 7;
        public const int MSG_DEVICESTATUS_ACK = 8;

        public const int MSG_DISPLAYBITMAP = 19;
        public const int MSG_DISPLAYBITMAP_ACK = 20;

        public const int MSG_CLEARDISPLAY = 21;
        public const int MSG_CLEARDISPLAY_ACK = 22;

        public const int MSG_SETMENUSIZE = 23;
        public const int MSG_SETMENUSIZE_ACK = 24;

        public const int MSG_GETMENUITEM = 25;
        public const int MSG_GETMENUITEM_RESP = 26;

        public const int MSG_GETALERT = 27;
        public const int MSG_GETALERT_RESP = 28;

        public const int MSG_NAVIGATION = 29;
        public const int MSG_NAVIGATION_RESP = 30;

        public const int MSG_SETSTATUSBAR = 33;
        public const int MSG_SETSTATUSBAR_ACK = 34;

        public const int MSG_GETMENUITEMS = 35;

        public const int MSG_SETMENUSETTINGS = 36;
        public const int MSG_SETMENUSETTINGS_ACK = 37;

        public const int MSG_GETTIME = 38;
        public const int MSG_GETTIME_RESP = 39;

        public const int MSG_SETLED = 40;
        public const int MSG_SETLED_ACK = 41;

        public const int MSG_SETVIBRATE = 42;
        public const int MSG_SETVIBRATE_ACK = 43;

        public const int MSG_ACK = 44;

        public const int MSG_SETSCREENMODE = 64;
        public const int MSG_SETSCREENMODE_ACK = 65;

        public const int MSG_GETSCREENMODE = 66;
        public const int MSG_GETSCREENMODE_RESP = 67;

        public const int DEVICESTATUS_OFF = 0;
        public const int DEVICESTATUS_ON = 1;
        public const int DEVICESTATUS_MENU = 2;

        public const int RESULT_OK = 0;
        public const int RESULT_ERROR = 1;
        public const int RESULT_OOM = 2;
        public const int RESULT_EXIT = 3;
        public const int RESULT_CANCEL = 4;

        public const int NAVACTION_PRESS = 0;
        public const int NAVACTION_LONGPRESS = 1;
        public const int NAVACTION_DOUBLEPRESS = 2;

        public const int NAVTYPE_UP = 0;
        public const int NAVTYPE_DOWN = 1;
        public const int NAVTYPE_LEFT = 2;
        public const int NAVTYPE_RIGHT = 3;
        public const int NAVTYPE_SELECT = 4;
        public const int NAVTYPE_MENUSELECT = 5;

        public const int ALERTACTION_CURRENT = 0;
        public const int ALERTACTION_FIRST = 1;
        public const int ALERTACTION_LAST = 2;
        public const int ALERTACTION_NEXT = 3;
        public const int ALERTACTION_PREV = 4;

        public const int BRIGHTNESS_OFF = 48;
        public const int BRIGHTNESS_DIM = 49;
        public const int BRIGHTNESS_MAX = 50;

        public static LiveViewMessageBase[] decode(byte[] data)
        {
            List<LiveViewMessageBase> result = new List<LiveViewMessageBase>();
            int consumed = 0;
            while (consumed <= data.Length)
            {
                int messageId;
                byte[] payload;
                Int32 msglength;
                decodeLVMessage(data, consumed, out messageId, out payload, out msglength);
                consumed += msglength;

                switch (messageId)
                {
                    case MSG_GETCAPS_RESP:
                        result.Add(new DisplayCapabilities(messageId, payload));
                        break;
                    case MSG_SETLED_ACK:
                    case MSG_SETVIBRATE_ACK:
                    case MSG_DEVICESTATUS_ACK:
                    case MSG_SETSCREENMODE_ACK:
                    case MSG_CLEARDISPLAY_ACK:
                    case MSG_SETSTATUSBAR_ACK:
                    case MSG_DISPLAYTEXT_ACK:
                    case MSG_DISPLAYBITMAP_ACK:
                    case MSG_DISPLAYPANEL_ACK:
                        result.Add(new Result(messageId, payload));
                        break;
                    case MSG_GETMENUITEMS:
                        result.Add(new GetMenuItems(messageId, payload));
                        break;
                    case MSG_GETMENUITEM:
                        result.Add(new GetMenuItem(messageId, payload));
                        break;
                    case MSG_GETTIME:
                        result.Add(new GetTime(messageId, payload));
                        break;
                    case MSG_GETALERT:
                        result.Add(new GetAlert());
                        break;
                    case MSG_DEVICESTATUS:
                        result.Add(new DeviceStatus(messageId, payload));
                        break;
                    case MSG_NAVIGATION:
                        result.Add(new Navigation());
                        break;
                    case MSG_GETSCREENMODE_RESP:
                        result.Add(new GetScreenMode());
                        break;
                    default:
                        throw new Exception("Unknown message");
                }
            }
            return result.ToArray();
        }

        protected static void decodeLVMessage(byte[] data, int index, out int messageId, out byte[] payload, out Int32 msgLength)
        {
            messageId = data[0];
            int headerLen = data[1];
            byte[] foo = new byte[4];
            Array.Copy(data, 2, foo, 0, 4);
            Array.Reverse(foo);
            Int32 payloadLen = BitConverter.ToInt32(foo, 0);
            msgLength = 2 + headerLen + payloadLen;
            payload = new byte[payloadLen];
            Array.Copy(data, (2 + headerLen), payload, 0, payloadLen);

            if (headerLen != 4)
                throw new Exception(string.Format("Unexpected header length {0}", headerLen));
        }

        protected static byte[] longToBytes(long data)
        {
            byte[] data2 = BitConverter.GetBytes(UInt32.Parse(data.ToString()));
            byte[] foo = new byte[4];
            Array.Copy(data2, 0, foo, 0, 4);
            Array.Reverse(foo);
            return foo;
        }

        public static byte[] encodeGetTimeResponse(long time, bool use24h)
        {
            List<byte> data = new List<byte>();
            data.AddRange(longToBytes(time));
            data.Add((byte)(use24h ? 0x00 : 0x01));
            Console.WriteLine(string.Format("<GetTimeResponse Time:{0} 24H:{1}>", BitConverter.ToUInt32(longToBytes(time), 0), use24h));
            return encodeLVMessage(MSG_GETTIME_RESP, data.ToArray());
        }

        protected static byte[] encodeLVMessage(int messageId, byte[] data)
        {
            List<byte> output = new List<byte>();
            output.Add((byte)messageId);
            output.Add((byte)4);
            output.AddRange(longToBytes((long)data.Length));
            output.AddRange(data);
            return output.ToArray();
        }

        public static byte[] encodeGetCaps()
        {
            List<byte> data = new List<byte>();
            data.Add((byte)CLIENT_SOFTWARE_VERSION.Length);
            data.AddRange(System.Text.Encoding.ASCII.GetBytes(CLIENT_SOFTWARE_VERSION));
            Console.WriteLine("<GetCaps>");
            return encodeLVMessage(MSG_GETCAPS, data.ToArray());
        }

        public static byte[] encodeAck(int messageId)
        {
            Console.WriteLine(string.Format("<Ack ID:{0}>", messageId));
            return encodeLVMessage(MSG_ACK, new byte[] { (byte)messageId });
        }

        public static byte[] encodeSetMenuSize(int menuSize)
        {
            Console.WriteLine(string.Format("<SetMenuSize Size:{0}>", menuSize));
            return encodeLVMessage(MSG_SETMENUSIZE, new byte[] { (byte)menuSize });
        }

        public static byte[] encodeSetMenuSettings(int vibrationTime, int initialMenuItemId)
        {
            List<byte> data = new List<byte>();
            data.Add((byte)vibrationTime);
            data.Add((byte)12);
            data.Add((byte)initialMenuItemId);
            Console.WriteLine(string.Format("<SetMenuSettings VibrationTime:{0} InitialMenuItemId:{1}>", vibrationTime, initialMenuItemId));
            return encodeLVMessage(MSG_SETMENUSETTINGS, data.ToArray());
        }

        public static byte[] encodeGetMenuItemResponse(int idx, bool isAlert, int unreadCount, string text, byte[] bitmap)
        {
            List<byte> payload = new List<byte>();
            payload.Add((byte)(isAlert ? 0 : 1));
            payload.AddRange(BitConverter.GetBytes((ushort)0));
            byte[] foo = BitConverter.GetBytes((ushort)unreadCount);
            Array.Reverse(foo);
            payload.AddRange(foo);
            payload.AddRange(BitConverter.GetBytes((ushort)0));
            payload.Add((byte)(idx + 3));
            payload.Add((byte)0);

            payload.AddRange(new byte[] { 0, 0, 0, 0 });
            foo = BitConverter.GetBytes((ushort)(text.Length));
            Array.Reverse(foo);
            payload.AddRange(foo);
            payload.AddRange(System.Text.Encoding.ASCII.GetBytes(text));
            payload.AddRange(bitmap);
            Console.WriteLine(string.Format("<GetMenuItemResponse IDX:{0} IsAlert:{1} UnreadCount:{2} Text:{3}>", idx, isAlert, unreadCount, text));
            return encodeLVMessage(MSG_GETMENUITEM_RESP, payload.ToArray());
        }

        public static byte[] encodeDeviceStatusAck()
        {
            Console.WriteLine("<DeviceStatusAck>");
            return encodeLVMessage(MSG_DEVICESTATUS_ACK, new byte[] { RESULT_OK });
        }

        public static byte[] encodeNavigationResponse(int code)
        {
            Console.WriteLine("<NavigationResponse>");
            return encodeLVMessage(MSG_NAVIGATION_RESP, new byte[] { (byte)code });
        }
    }
}
