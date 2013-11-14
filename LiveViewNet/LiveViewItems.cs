using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveViewNet
{
    public class MenuItem : LiveViewItem
    {
        public MenuItem(bool isAlert, int unreadCount, string text, byte[] bitmap)
        {
            this.type = MENU_ITEM;
            this.isAlert = isAlert;
            this.unreadCount = unreadCount;
            this.text = text;
            this.bitmap = bitmap;
        }
    }


    public class LiveViewItem
    {
        public const int MENU_ITEM = 1;
        public const int ALERT_RESPONSE = 2;
        public const int DISPLAY_PANEL = 3;
        public int type;
        public bool isAlert;
        public int unreadCount;
        public string text;
        public byte[] bitmap;
        public int totalCount;
        public int alertIndex;
        public string timestampText;
        public string headerText;
        public string bodyText;
        public string topText;
        public string bottomText;
        public bool alertUser;
    }

    public class AlertResponse : LiveViewItem
    {
        public AlertResponse(int totalCount, int unreadCount, int alertIndex, string timestampText, string headerText, string bodyText, byte[] bitmap)
        {
            this.type = ALERT_RESPONSE;
            this.totalCount = totalCount;
            this.unreadCount = unreadCount;
            this.alertIndex = alertIndex;
            this.timestampText = timestampText;
            this.headerText = headerText;
            this.bodyText = bodyText;
            this.bitmap = bitmap;
        }
    }

    public class DisplayPanel : LiveViewItem
    {
        public DisplayPanel(string topText, string bottomText, byte[] bitmap, bool alertUser)
        {
            this.type = DISPLAY_PANEL;
            this.topText = topText;
            this.bottomText = bottomText;
            this.bitmap = bitmap;
            this.alertUser = alertUser;
        }
    }
}
