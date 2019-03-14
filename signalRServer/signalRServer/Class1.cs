using System;
using System.Drawing;
using ConsoleApplication116_SignalRServer;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Cors;
using Owin;
using System.Threading;

[assembly: OwinStartup(typeof(Program.Startup))]
namespace ConsoleApplication116_SignalRServer
{
   class Program
   {
      static IDisposable SignalR;

      static void Main(string[] args)
      {
            
            Console.WriteLine("Podaj ip: ");
            string ip1 = Console.ReadLine();
            Console.WriteLine("Podaj port: ");
            string port1 = Console.ReadLine();
            string url = String.Concat("http://", ip1, ":", port1);
            SignalR = WebApp.Start(url);

            Console.WriteLine("Polaczono");
         Console.ReadKey();
      }

      public class Startup
      {
         public void Configuration(IAppBuilder app)
         {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
         }
      }

      [HubName("MyHub")]
      public class MyHub : Hub
      {
         public void Send(Guid name, int[] message)
         {
                //bramka - to co dostanie od jednego klienta rozpowie wszytskim
            Console.WriteLine($"message sent from {name.ToString()}: move from {message[0]},{message[1]} to {message[2]},{message[3]}");
           Clients.All.addMessage(name, message);
         }
      }
   }
}