using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using TankMitigationOverlay.Windows;

namespace TankMitigationOverlay;

public sealed class Plugin : IDalamudPlugin
{
    public string Name => "Tank Mitigation Overlay by Ninee Ix";

    private IDalamudPluginInterface PluginInterface { get; init; }
    private ICommandManager CommandManager { get; init; }
    private IClientState ClientState { get; init; }
    private IObjectTable ObjectTable { get; init; }

    public Configuration Configuration { get; init; }
    public WindowSystem WindowSystem = new("TankMitigationOverlay");

    private MitigationTracker MitigationTracker { get; init; }
    private MainWindow MainWindow { get; init; }
    private ConfigWindow ConfigWindow { get; init; }

    public Plugin(
        IDalamudPluginInterface pluginInterface,
        ICommandManager commandManager,
        IClientState clientState,
        IObjectTable objectTable)
    {
        this.PluginInterface = pluginInterface;
        this.CommandManager = commandManager;
        this.ClientState = clientState;
        this.ObjectTable = objectTable;

        // Configuration
        this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        this.Configuration.Initialize(this.PluginInterface);

        // Tracker & Fenêtres
        this.MitigationTracker = new MitigationTracker();
        this.MainWindow = new MainWindow(this.Configuration, this.ObjectTable, this.MitigationTracker);
        this.ConfigWindow = new ConfigWindow(this.Configuration);

        this.WindowSystem.AddWindow(this.MainWindow);
        this.WindowSystem.AddWindow(this.ConfigWindow);

        // Commande /tmo
        this.CommandManager.AddHandler("/tmo", new Dalamud.Game.Command.CommandInfo(OnCommand)
        {
            HelpMessage = "Ouvre ou ferme l'overlay de mitigation Tank."
        });

        // Callbacks UI requis par Dalamud Validation
        this.PluginInterface.UiBuilder.Draw += DrawUI;
        this.PluginInterface.UiBuilder.OpenMainUi += ToggleMainUi;
        this.PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;
    }

    public void Dispose()
    {
        this.PluginInterface.UiBuilder.Draw -= DrawUI;
        this.PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUi;
        this.PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUi;

        this.WindowSystem.RemoveAllWindows();
        this.MainWindow.Dispose();
        this.ConfigWindow.Dispose();
        this.CommandManager.RemoveHandler("/tmo");
    }

    private void OnCommand(string command, string args)
    {
        ToggleMainUi();
    }

    private void ToggleMainUi()
    {
        this.MainWindow.IsOpen = !this.MainWindow.IsOpen;
    }

    private void ToggleConfigUi()
    {
        this.ConfigWindow.IsOpen = !this.ConfigWindow.IsOpen;
    }

    private void DrawUI()
    {
        this.WindowSystem.Draw();
    }
}