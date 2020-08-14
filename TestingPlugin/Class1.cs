using System;
using CyberCore;
using log4net;
using MiNET;
using OpenAPI;
using OpenAPI.Plugins;

namespace TestingPlugin
{
    [OpenPluginInfo(Name = "CyberCore", Description = "CyberTech++ Core Plugin", Author = "YungTechBoy1",
        Version = "1.0.0.1-PA", Website = "CyberTechpp.com")]
    public class Class1 : OpenPlugin

    {  public static ILog Log { get; private set; } = LogManager.GetLogger(typeof(Class1));


        public void Configure(MiNetServer s)
        {
            // s.ServerManager = new CyberTechServerManager(s);
            s.PlayerFactory = new CyberPlayerFactory(API);
            Console.WriteLine("================Executed startup successfully. Replaced identity managment=========================");
            Log.Info("================Executed startup successfully. Replaced identity managment=========================");
            
        }

        public OpenApi API;
        
        public override void Enabled(OpenApi api)
        {
            API = api;
        }

        public override void Disabled(OpenApi api)
        {
            
        }
    }
}