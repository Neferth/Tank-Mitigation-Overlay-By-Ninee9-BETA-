using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;

namespace TankMitigationOverlay.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly Configuration configuration;

    public ConfigWindow(Configuration configuration)
        : base("Configuration - Tank Mitigation Overlay##Config", ImGuiWindowFlags.AlwaysAutoResize)
    {
        this.configuration = configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
        bool isVisible = this.configuration.IsVisible;
        if (ImGui.Checkbox("Afficher l'Overlay", ref isVisible))
        {
            this.configuration.IsVisible = isVisible;
            this.configuration.Save();
        }

        ImGui.Separator();

        // Slider Taille
        float scale = this.configuration.Scale;
        if (ImGui.SliderFloat("Taille du HUD", ref scale, 0.5f, 2.5f, "%.2f"))
        {
            this.configuration.Scale = scale;
            this.configuration.Save();
        }

        // Slider Opacité
        float opacity = this.configuration.BackgroundOpacity;
        if (ImGui.SliderFloat("Opacité du Fond", ref opacity, 0.0f, 1.0f, "%.2f"))
        {
            this.configuration.BackgroundOpacity = opacity;
            this.configuration.Save();
        }

        ImGui.Separator();

        // Réglage de la mitigation passive de ton stuff
        float baseArmor = this.configuration.BaseArmorMitigation;
        if (ImGui.SliderFloat("Mitigation de Base Perso (%)", ref baseArmor, 0.0f, 50.0f, "%.1f%%"))
        {
            this.configuration.BaseArmorMitigation = baseArmor;
            this.configuration.Save();
        }
        ImGui.TextDisabled("Ajuste ce % selon la défense passive brute de ton Ilvl actuel.");
    }
}