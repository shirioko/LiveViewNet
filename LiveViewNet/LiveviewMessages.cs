using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveViewNet.Messages
{
    public class LiveViewMessageBase
    {
        public int messageId;
    }

    public class DisplayCapabilities : LiveViewMessageBase
    {
        public int width;
        public int height;
        public int statusBarWidth;
        public int statusBarHeight;
        public int viewWidth;
        public int viewHeight;
        public int announceWidth;
        public int announceHeight;
        public int textChunkSize;
        public int idleTimer;
        public string softwareVersion;

        public DisplayCapabilities(int messageId, byte[] data)
        {
            this.messageId = messageId;
            this.width = data[0];
            this.height = data[1];
            this.statusBarWidth = data[2];
            this.statusBarHeight = data[3];
            this.viewWidth = data[4];
            this.viewHeight = data[5];
            this.announceWidth = data[6];
            this.announceHeight = data[7];
            this.textChunkSize = data[8];
            this.idleTimer = data[9];
            byte[] version = new byte[data.Length - 10];
            Array.Copy(data, 10, version, 0, version.Length);
            this.softwareVersion = System.Text.Encoding.ASCII.GetString(version);
        }

        public override string ToString()
        {
            return string.Format("rx <DisplayCapabilities Width:{0} Height:{1} StatusBarWidth:{2} StatusBarHeight:{3} ViewWidth:{4} ViewHeight:{5} AnnounceWidth:{6} AnnounceHeight:{7} TextChunkSize:{8} SoftwareVersion:{9}>",
                this.width,
                this.height,
                this.statusBarWidth,
                this.statusBarHeight,
                this.viewWidth,
                this.viewHeight,
                this.announceWidth,
                this.announceHeight,
                this.textChunkSize,
                this.softwareVersion);
        }
    }

    public class Result : LiveViewMessageBase
    {
        public int code;
        public Result(int messageId, byte[] data)
        {
            this.messageId = messageId;
            this.code = data[0];
        }

        public override string ToString()
        {
            string type;
            switch (this.code)
            {
                case LiveViewMessage.RESULT_OK:
                    type = "OK";
                    break;
                case LiveViewMessage.RESULT_ERROR:
                    type = "ERROR";
                    break;
                case LiveViewMessage.RESULT_OOM:
                    type = "OOM";
                    break;
                case LiveViewMessage.RESULT_EXIT:
                    type = "EXIT";
                    break;
                case LiveViewMessage.RESULT_CANCEL:
                    type = "CANCEL";
                    break;
                default:
                    type = "UNKNOWN";
                    break;
            }
            return string.Format("rx <Result MessageId:{0} Code:{1}>", this.messageId, type);
        }
    }

    public class GetMenuItem : LiveViewMessageBase
    {
        public int menyItemId;
        public GetMenuItem(int messageId, byte[] data)
        {
            this.messageId = messageId;
            this.menyItemId = data[0];
        }
        public override string ToString()
        {
            return string.Format("rx <GetMenuItem MenuItemId:{0}>", this.menyItemId);
        }
    }

    public class GetMenuItems : LiveViewMessageBase
    {
        public int unknown;
        public GetMenuItems(int messageId, byte[] data)
        {
            this.messageId = messageId;
            this.unknown = data[0];
            if (this.unknown != 0)
            {
                Console.WriteLine(string.Format("GetMenuItems with non-zero unknown byte {0}", this.unknown));
            }
        }

        public override string ToString()
        {
            return "rx <GetMenuItems>";
        }
    }

    public class GetTime : LiveViewMessageBase
    {
        public int unknown;
        public GetTime(int messageId, byte[] data)
        {
            this.messageId = messageId;
            this.unknown = data[0];
            if (this.unknown != 0)
            {
                Console.WriteLine(string.Format("GetTime with non-zero unknown byte {0}", this.unknown));
            }
        }

        public override string ToString()
        {
            return "<GetTime>";
        }
    }

    public class DeviceStatus : LiveViewMessageBase
    {
        public int deviceStatus;
        public DeviceStatus(int messageId, byte[] data)
        {
            this.messageId = messageId;
            this.deviceStatus = data[0];
            if (this.deviceStatus > 2)
            {
                Console.WriteLine(string.Format("DeviceStatus with unknown value {0}", this.deviceStatus));
            }
        }

        public override string ToString()
        {
            string type;
            switch (this.deviceStatus)
            {
                case LiveViewMessage.DEVICESTATUS_OFF:
                    type = "OFF";
                    break;
                case LiveViewMessage.DEVICESTATUS_ON:
                    type = "ON";
                    break;
                case LiveViewMessage.DEVICESTATUS_MENU:
                    type = "MENU";
                    break;
                default:
                    type = "UNKNOWN";
                    break;
            }
            return string.Format("rx <DeviceStatus Status:{0}>", type);
        }
    }

    public class GetAlert : LiveViewMessageBase
    {
        
    }

    public class Navigation : LiveViewMessageBase
    {

    }

    public class GetScreenMode : LiveViewMessageBase
    {

    }
}
