using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LVTest
{
    class Program
    {
        static void Main(string[] args)
        {
            LiveViewNet.LiveViewServer srv = new LiveViewNet.LiveViewServer();
            byte[] data = getImageBytes();
            srv.SetMenuItems(new LiveViewNet.MenuItem[] { 
                new LiveViewNet.MenuItem(true, 0, "Menu0", data),
                new LiveViewNet.MenuItem(false, 20, "Menu1", data),
                new LiveViewNet.MenuItem(false, 0, "Menu2", data),
                new LiveViewNet.MenuItem(true, 0, "Menu3", data)
            });
            srv.Start();
            Console.ReadLine();
        }

        public static byte[] getImageBytes()
        {
            return File.ReadAllBytes("test36.png");
        }
    }
}
