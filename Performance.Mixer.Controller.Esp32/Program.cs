using System;
using System.Diagnostics;
using System.Threading;
using System.Device.Wifi;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using Performance.Mixer.Controller.Esp32.Osc;
using nanoFramework.Hosting;
using Performance.Mixer.Controller.Esp32.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Device.Gpio;

namespace Performance.Mixer.Controller.Esp32
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("performance controller initialization");
            SetupAndConnectToWifi();
            Debug.WriteLine("connected to WiFi");

            GpioWatcher.SetupGpioWatcher();

            var performanceMixerIp = "192.168.0.236";
            var port = 12345;

            var socket = new UdpClient();
            socket.Connect(performanceMixerIp, port);
            Debug.WriteLine("Connected to UDP socket");

            var count = 0;
            while (true)
            {
                Debug.WriteLine("Sending message");
                var message = new OscMessageBuilder()
                    .WithAddress("/test")
                    .WithParameter(count)
                    .WithParameter(3.566f)
                    .Build();
                count++;
                socket.Send(message);
                Thread.Sleep(10000);
            }
        }

        public static void SetupAndConnectToWifi()
        {
            var wifiAdapter = WifiAdapter.FindAllAdapters()[0];
            wifiAdapter.ScanAsync();
            var wifiConfig = Wireless80211Configuration.GetAllWireless80211Configurations()[0];
            var ipAddress = NetworkInterface.GetAllNetworkInterfaces()[0].IPv4Address;
            var needToConnect = string.IsNullOrEmpty(ipAddress) || (ipAddress == "0.0.0.0");
            var ssid = wifiConfig.Ssid;
            var password = wifiConfig.Password;
            while (needToConnect)
            {
                foreach (var network in wifiAdapter.NetworkReport.AvailableNetworks)
                {
                    if (network.Ssid == ssid)
                    {
                        var result = wifiAdapter.Connect(network, WifiReconnectionKind.Automatic, password);

                        if (result.ConnectionStatus == WifiConnectionStatus.Success)
                        {
                            Debug.WriteLine($"Connected to Wifi network {network.Ssid}.");
                            needToConnect = false;
                        }
                        else
                        {
                            Debug.WriteLine($"Error {result.ConnectionStatus} connecting to Wifi network {network.Ssid}.");
                        }
                    }
                }

                Thread.Sleep(10000);
            }
            ipAddress = NetworkInterface.GetAllNetworkInterfaces()[0].IPv4Address;
            Debug.WriteLine($"Connected to Wifi network with IP address {ipAddress}");
        }
    }
}
