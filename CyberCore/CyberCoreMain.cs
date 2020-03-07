using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CyberCore.Manager.Factions;
using CyberCore.Utils;
using log4net;
using MiNET.Net;
using MiNET.Utils;
using OpenAPI;
using OpenAPI.Plugins;

namespace CyberCore
{
    [OpenPluginInfo(Name = "CyberCore", Description = "CyberTech++ Core Plugin", Author = "YungTechBoy1", Version = "1.0.0.0-PA", Website = "CyberTechpp.com")]
    public class CyberCoreMain : OpenPlugin
    {
    private static readonly ILog Log = LogManager.GetLogger(typeof(CyberCoreMain));
    private static CyberCoreMain instance { get; set; }
    public ConfigSection MasterConfig { get; }
    public FactionsMain FM { get; private set; }
    public SqlManager SQL { get; private set; }


    public static CyberCoreMain GetInstance()
    {
        return instance;
    }
    
    public CyberCoreMain()
    {
        instance = this;
        MasterConfig = new ConfigSection(){ConfigFileName = "MasterConfig.conf"};
        
    }

    public override void Enabled(OpenApi api)
    {
        SQL = new SqlManager(this);
        
        FM = new FactionsMain(this);
        
        api.CommandManager.RegisterPermissionChecker(new FactionPermissionChecker(FactionManager));

        api.CommandManager.LoadCommands(CommandsClass);
        api.CommandManager.LoadCommands(new FactionCommands(FactionManager));
    }

    public override void Disabled(OpenApi api)
    {
        api.CommandManager.UnloadCommands(CommandsClass);
    }

    public void HelloWorld(string message, [CallerMemberName] string memberName = "")
    {
        StackTrace stackTrace = new StackTrace();
        var method = stackTrace.GetFrame(1).GetMethod();
        Log.Info($"[TestPlugin] {(method.DeclaringType.FullName)}.{method.Name}: " + message);
    }
    }
}