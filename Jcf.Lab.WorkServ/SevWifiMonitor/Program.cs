using System;
using SimpleWifi;

class Program
{
    static void Main()
    {
        var wifi = new Wifi();

        foreach (var accessPoint in wifi.GetAccessPoints())
        {
            Console.WriteLine($"SSID: {accessPoint.Name}");
            Console.WriteLine($"Sinal: {accessPoint.SignalStrength}%");
            Console.WriteLine($"Protegido: {accessPoint.IsSecure}");
            Console.WriteLine("---------------------------");

         
            
        }
    }
}
