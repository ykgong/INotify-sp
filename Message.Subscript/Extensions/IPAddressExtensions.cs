using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Message.Subscript.Server.Extensions
{
    /// <summary>
    /// Useful IPAddressExtensions from: 
    /// http://blogs.msdn.com/knom/archive/2008/12/31/ip-address-calculations-with-c-subnetmasks-networks.aspx
    /// 
    /// </summary>
    public static class IPAddressExtensions
    {
        public static IPAddress GetBroadcastAddress(this IPAddress address, IPAddress subnetMask)
        {
            var ipAdressBytes = address.GetAddressBytes();
            var subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            var broadcastAddress = new byte[ipAdressBytes.Length];
            for (var i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | subnetMaskBytes[i] ^ 255);
            }
            return new IPAddress(broadcastAddress);
        }

        public static IPAddress GetNetworkAddress(this IPAddress address, IPAddress subnetMask)
        {
            var ipAdressBytes = address.GetAddressBytes();
            var subnetMaskBytes = subnetMask.GetAddressBytes();

            return new IPAddress(GetNetworkAddressBytes(ipAdressBytes, subnetMaskBytes));
        }

        public static byte[] GetNetworkAddressBytes(byte[] ipAdressBytes, byte[] subnetMaskBytes)
        {
            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            var broadcastAddress = new byte[ipAdressBytes.Length];
            for (var i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] & subnetMaskBytes[i]);
            }
            return broadcastAddress;
        }

        public static bool IsInSameIpv6Subnet(this IPAddress address2, IPAddress address)
        {
            if (address2.AddressFamily != AddressFamily.InterNetworkV6 || address.AddressFamily != AddressFamily.InterNetworkV6)
            {
                throw new ArgumentException("Both IPAddress must be IPV6 addresses");
            }
            var address1Bytes = address.GetAddressBytes();
            var address2Bytes = address2.GetAddressBytes();

            return address1Bytes.IsInSameIpv6Subnet(address2Bytes);
        }

        public static bool IsInSameIpv6Subnet(this byte[] address1Bytes, byte[] address2Bytes)
        {
            if (address1Bytes.Length != address2Bytes.Length)
                throw new ArgumentException("Lengths of IP addresses do not match.");

            for (var i = 0; i < 8; i++)
            {
                if (address1Bytes[i] != address2Bytes[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsInSameIpv4Subnet(this IPAddress address2, IPAddress address, IPAddress subnetMask)
        {
            if (address2.AddressFamily != AddressFamily.InterNetwork || address.AddressFamily != AddressFamily.InterNetwork)
            {
                throw new ArgumentException("Both IPAddress must be IPV4 addresses");
            }
            var network1 = address.GetNetworkAddress(subnetMask);
            var network2 = address2.GetNetworkAddress(subnetMask);

            return network1.Equals(network2);
        }
         

        /// <summary>
        /// Gets the ipv4 addresses from all Network Interfaces that have Subnet masks.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<IPAddress, IPAddress> GetAllLoaclIpv4Addresses()
        {
            var map = new Dictionary<IPAddress, IPAddress>();

            try
            {
                foreach (var ni in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                {
                    foreach (var uipi in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (uipi.Address.AddressFamily != AddressFamily.InterNetwork) continue;

                        if (uipi.IPv4Mask == null) continue; //ignore 127.0.0.1
                        map[uipi.Address] = uipi.IPv4Mask;
                    }
                }
            }
            catch /*(NotImplementedException ex)*/
            {
                //log.Warn("MONO does not support NetworkInterface.GetAllNetworkInterfaces(). Could not detect local ip subnets.", ex);
            }
            return map;
        }

        /// <summary>
        /// Gets the ipv6 addresses from all Network Interfaces.
        /// </summary>
        /// <returns></returns>
        public static List<IPAddress> GetAllLoaclIpv6Addresses()
        {
            var list = new List<IPAddress>();

            try
            {
                foreach (var ni in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                {
                    foreach (var uipi in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (uipi.Address.AddressFamily != AddressFamily.InterNetworkV6) continue;
                        list.Add(uipi.Address);
                    }
                }
            }
            catch /*(NotImplementedException ex)*/
            {
                //log.Warn("MONO does not support NetworkInterface.GetAllNetworkInterfaces(). Could not detect local ip subnets.", ex);
            }

            return list;
        }

        public static string GetInternetIP(this string ipService)
        {
            using (var webClient = new WebClient())
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(ipService))
                    {
                        ipService = "http://whois.pconline.com.cn/ipJson.jsp";
                    }
                    var temp = webClient.DownloadString(ipService);
                    var ip = Regex.Match(temp, @"((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.){3}(1\d\d|2[0-4]\d|25[0-5]|[1-9]\d|\d)").Groups["0"].Value;
                    return !string.IsNullOrEmpty(ip) ? ip : string.Empty;
                }
                catch (Exception ex)
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 获取本机的ip地址,默认ipv4，也可以获取v6
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="ipFamily"></param>
        /// <param name="startIpFilters">已经默认有192.->10.->172.</param>
        /// <returns></returns>
        public static IPAddress GetLocalIpAddress(this string hostName, AddressFamily ipFamily = AddressFamily.InterNetwork, string startIpFilter = null)
        {
            var ipList = Dns.GetHostEntry(string.Empty).AddressList;
            var ips = ipList.Where(f => f.AddressFamily.Equals(ipFamily)).Select(m => new { Ip = m.ToString(), IpAddress = m });
            IPAddress ipAddress = null;

            if (!string.IsNullOrWhiteSpace(startIpFilter))
            {
                if (ipAddress == null && ips.Count(f => f.Ip.StartsWith(startIpFilter)) > 0)
                {
                    ipAddress = ips.Where(f => f.Ip.StartsWith(startIpFilter)).FirstOrDefault()?.IpAddress;
                }
            }
            else
            {
                if (ips.Count(f => f.Ip.StartsWith("192.")) > 0)
                {
                    ipAddress = ips.Where(f => f.Ip.StartsWith("192.")).FirstOrDefault()?.IpAddress;
                }
                else if (ipAddress == null && ips.Count(f => f.Ip.StartsWith("10.")) > 0)
                {
                    ipAddress = ips.Where(f => f.Ip.StartsWith("10.")).FirstOrDefault()?.IpAddress;
                }
                else if (ipAddress == null && ips.Count(f => f.Ip.StartsWith("172.")) > 0)
                {
                    ipAddress = ips.Where(f => f.Ip.StartsWith("172.")).FirstOrDefault()?.IpAddress;
                }
            }
            return ipAddress ?? ips.FirstOrDefault().IpAddress;
        }

    }
}