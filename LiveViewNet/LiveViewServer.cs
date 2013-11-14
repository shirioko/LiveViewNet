using InTheHand.Net.Bluetooth;
using InTheHand.Net.Bluetooth.Widcomm;
using InTheHand.Net.Sockets;
using LiveViewNet.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiveViewNet
{
    public class LiveViewServer
    {
        protected BluetoothListener listener;
        protected BluetoothClient client;
        public int menuVibrationTime = 5;
        public bool use24HourClock = true;
        protected Thread listenThread;
        protected MenuItem[] menuItems = new MenuItem[0];
        public Messages.DisplayCapabilities Capabilities;

        public LiveViewServer()
        {
            //construct
        }

        public void Start()
        {
            this.listener = new BluetoothListener(BluetoothService.SerialPort);
            this.listener.ServiceClass = ServiceClass.Information;
            this.listener.ServiceName = "LiveView";
            this.listenThread = new Thread(new ThreadStart(acceptClient));
            this.listenThread.IsBackground = true;
            this.listenThread.Start();
        }

        protected void acceptClient()
        {
            this.listener.Start();
            while (true)
            {
                Console.WriteLine("Listening  for client...");
                this.client = this.listener.AcceptBluetoothClient();
                Console.WriteLine(string.Format("Client '{0}' connected", client.RemoteMachineName));
                this.listenThread = new Thread(new ThreadStart(onConnected));
                this.listenThread.IsBackground = true;
                this.listenThread.Start();
            }
        }

        protected void onConnected()
        {
            try
            {
                //send get caps
                this.send(LiveViewMessage.encodeGetCaps());
                while (this.client.Connected)
                {
                    this.listen();
                }
            }
            catch (IOException)
            {
                //connection timeout
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected void send(byte[] data)
        {
            this.client.Client.Send(data);
        }

        public void SetMenuItems(MenuItem[] items)
        {
            this.menuItems = items;
        }

        public string timeTest()
        {
            byte[] data3 = LiveViewMessage.encodeGetTimeResponse((long)1384372096, this.use24HourClock);
            string foo = Convert.ToBase64String(data3);
            Console.WriteLine(foo);
            return foo;
        }

        protected void listen()
        {
            byte[] data = new byte[4096];
            int length = this.client.Client.Receive(data);
            if (length > 0)
            {
                byte[] data2 = new byte[length];
                Array.Copy(data, 0, data2, 0, length);
                LiveViewMessageBase[] items = LiveViewMessage.decode(data2);
                foreach (LiveViewMessageBase item in items)
                {
                    Console.WriteLine(item.ToString());
                    Type cmp = typeof(Messages.Result);
                    if (item.GetType() == cmp)
                    {
                        //result message
                        Messages.Result msg = item as Messages.Result;
                        if (msg.code != LiveViewMessage.RESULT_OK)
                        {
                            Console.WriteLine("Non-OK result received");
                            Console.WriteLine(msg.ToString());
                            continue;
                        }
                    }

                    this.send(LiveViewMessage.encodeAck(item.messageId));

                    cmp = typeof(Messages.GetMenuItems);
                    if (item.GetType() == cmp)
                    {
                        //getmenuitems message
                        int i = 0;
                        foreach (MenuItem menuItem in this.menuItems)
                        {
                            this.send(LiveViewMessage.encodeGetMenuItemResponse(i, menuItem.isAlert, menuItem.unreadCount, menuItem.text, menuItem.bitmap));
                            i++;
                        }
                    }
                    else
                    {
                        cmp = typeof(Messages.GetMenuItem);
                        if (item.GetType() == cmp)
                        {
                            //getmenuitem message
                            throw new NotImplementedException();
                        }
                        else
                        {
                            cmp = typeof(Messages.DisplayCapabilities);
                            if (item.GetType() == cmp)
                            {
                                //displayitems message
                                this.Capabilities = item as Messages.DisplayCapabilities;
                                this.send(LiveViewMessage.encodeSetMenuSize(this.menuItems.Length));
                                this.send(LiveViewMessage.encodeSetMenuSettings(this.menuVibrationTime, 0));
                            }
                            else
                            {
                                cmp = typeof(Messages.GetTime);
                                if (item.GetType() == cmp)
                                {
                                    //gettime message
                                    long time = this.getLocalTime();
                                    byte[] data3 = LiveViewMessage.encodeGetTimeResponse(this.getLocalTime(), this.use24HourClock);
                                    this.send(data3);
                                }
                                else
                                {
                                    cmp = typeof(Messages.DeviceStatus);
                                    if (item.GetType() == cmp)
                                    {
                                        //devicestatus message
                                        this.send(LiveViewMessage.encodeDeviceStatusAck());
                                    }
                                    else
                                    {
                                        cmp = typeof(Messages.GetAlert);
                                        if (item.GetType() == cmp)
                                        {
                                            //getalert message
                                            throw new NotImplementedException();
                                        }
                                        else
                                        {
                                            cmp = typeof(Messages.Navigation);
                                            if (item.GetType() == cmp)
                                            {
                                                //navigation message
                                                this.send(LiveViewMessage.encodeNavigationResponse(LiveViewMessage.RESULT_EXIT));
                                            }
                                            else
                                            {
                                                throw new Exception("Unknown message received");
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }

        private long getLocalTime()
        {
            TimeSpan span = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0));

            //return the total seconds (which is a UNIX timestamp)
            return (long)Math.Round(span.TotalSeconds);
        }
    }
}
