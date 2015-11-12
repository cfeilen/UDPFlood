namespace UdpFloodApp
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine("UDP Flood application.");
            Console.WriteLine("Settings:");
            Console.WriteLine("\tTargetPort:         {0}", App.Default.PortNumber);
            Console.WriteLine("\tPackets per second: {0}", App.Default.NumberOfPacketsPerSecond);
            Console.WriteLine("\tPacket Size:        {0}bytes", App.Default.PacketSizeInBytes);
            Console.WriteLine("");
            Console.WriteLine("Press [CTRL] + [C] to exit application");
            if (!App.Default.AutoStart)
            {
                Console.WriteLine("Press [ENTER] to start");
                Console.ReadLine();
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            Stopwatch watch = new Stopwatch();
            watch.Start();

            UdpClient client = new UdpClient();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, App.Default.PortNumber);
            client.EnableBroadcast = true;

            string header = $"{Environment.UserDomainName}\\{Environment.UserName} UDP Flood App.";

            var ticksPerCycle = (int)(Stopwatch.Frequency / App.Default.NumberOfPacketsPerSecond);

            long count = 0;
            long lastSecond = 0;
            while (true)
            {
                long nextPause = watch.ElapsedTicks + ticksPerCycle;
                ++count;
                byte[] bytes = Encoding.ASCII.GetBytes($"{header}..{count}");
                var newArray = new byte[App.Default.PacketSizeInBytes];

                Buffer.BlockCopy(bytes, 0, newArray, 0, bytes.Length);
                client.Send(newArray, newArray.Length, endPoint);

                // This spins the CPU for a bit, which allows for tighter loops than Thread.Sleep();
                while (watch.ElapsedTicks < nextPause)
                {
                }

                if (lastSecond < (watch.ElapsedMilliseconds / 1000))
                {
                    lastSecond = watch.ElapsedMilliseconds / 1000;
                    // This is a second boundry
                    Console.Write("\rPackets per second: {0}               ", count / lastSecond);
                }
            }
        }
    }
}