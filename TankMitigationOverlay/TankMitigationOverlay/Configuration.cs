using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace TankMitigationOverlay;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool IsVisible { get; set; } = true;
    public float Scale { get; set; } = 1.0f;
    public float BackgroundOpacity { get; set; } = 0.6f;

    // Ta mitigation de base d'armure personnalisée (par défaut à 39% si tu veux !)
    public float BaseArmorMitigation { get; set; } = 39.0f;

    [NonSerialized]
    private IDalamudPluginInterface? pluginInterface;

    public void Initialize(IDalamudPluginInterface pluginInterface)
    {
        this.pluginInterface = pluginInterface;
    }

    public void Save()
    {
        this.pluginInterface?.SavePluginConfig(this);
    }
}