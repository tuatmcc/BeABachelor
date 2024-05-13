using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace BeABachelor.Util
{
    public static class NetworkingUtil
    {
        public static (IPAddress, IPAddress)[] GetDirectedBroadcast()
        {
            List<(IPAddress, IPAddress)> ips = new();
            // ネットワークインターフェースを取得
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in interfaces)
            {
                // インターフェースのタイプをチェック（Ethernet、Wirelessなど）
                if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet || 
                    networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    // ディレクティッドブロードキャストアドレスを取得
                    foreach (UnicastIPAddressInformation ip in networkInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            IPAddress directedBroadcast = CalculateDirectedBroadcast(ip.Address, ip.IPv4Mask);
                            ips.Add((directedBroadcast, ip.Address));
                            //Console.WriteLine($"Interface: {networkInterface.Name}, Directed Broadcast: {directedBroadcast}");
                        }
                    }
                }
            }
            
            return ips.ToArray();
        }
        
        // ディレクティッドブロードキャストアドレスを計算
        static IPAddress CalculateDirectedBroadcast(IPAddress ip, IPAddress subnetMask)
        {
            byte[] ipBytes = ip.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastBytes = new byte[ipBytes.Length];
            for (int i = 0; i < broadcastBytes.Length; i++)
            {
                broadcastBytes[i] = (byte)(ipBytes[i] | (subnetMaskBytes[i] ^ 255));
            }

            return new IPAddress(broadcastBytes);
        }
    }
}