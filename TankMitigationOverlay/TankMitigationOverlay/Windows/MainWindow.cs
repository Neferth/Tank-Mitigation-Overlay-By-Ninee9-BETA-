using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;

namespace TankMitigationOverlay.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly Configuration configuration;
    private readonly IObjectTable objectTable;
    private readonly MitigationTracker mitigationTracker;

    public MainWindow(
        Configuration configuration,
        IObjectTable objectTable,
        MitigationTracker mitigationTracker)
        : base("Tank Mitigation Overlay##Main",
            ImGuiWindowFlags.NoTitleBar |
            ImGuiWindowFlags.NoScrollbar |
            ImGuiWindowFlags.AlwaysAutoResize |
            ImGuiWindowFlags.NoBackground)
    {
        this.configuration = configuration;
        this.objectTable = objectTable;
        this.mitigationTracker = mitigationTracker;
    }

    public void Dispose() { }

    public override void Draw()
    {
        if (!this.configuration.IsVisible) return;

        var localPlayer = this.objectTable.Length > 0 ? this.objectTable[0] as IBattleChara : null;
        if (localPlayer == null) return;

        IGameObject? currentTarget = localPlayer.TargetObjectId != 0 ? this.objectTable.SearchById(localPlayer.TargetObjectId) : null;

        // 1. Buffs actifs
        var activeMitigations = this.mitigationTracker.GetActiveMitigations(localPlayer, currentTarget);
        var (buffPhys, buffMagic, buffGlobal) = this.mitigationTracker.CalculateMitigations(activeMitigations);

        // 2. Vraie mitigation passive personnalisée (ex: 39%)
        float baseArmorPercent = this.configuration.BaseArmorMitigation;

        // 3. Calcul multiplicatif réel
        float totalPhysMitig = (1.0f - ((1.0f - (baseArmorPercent / 100f)) * (1.0f - (buffPhys / 100f)))) * 100f;
        float totalMagicMitig = (1.0f - ((1.0f - (baseArmorPercent / 100f)) * (1.0f - (buffMagic / 100f)))) * 100f;
        float totalGlobalMitig = (1.0f - ((1.0f - (baseArmorPercent / 100f)) * (1.0f - (buffGlobal / 100f)))) * 100f;

        DrawDoubleRingHUD(totalPhysMitig, totalMagicMitig, totalGlobalMitig, baseArmorPercent);
    }

    private void DrawDoubleRingHUD(float physPercent, float magicPercent, float globalPercent, float baseArmorPercent)
    {
        float scale = Math.Clamp(this.configuration.Scale, 0.5f, 2.5f);

        float outerRadius = 80f * scale;
        float innerRadius = 66f * scale;
        float thickness = 6f * scale;

        Vector2 size = new Vector2(outerRadius * 2 + 20, outerRadius * 2 + 20);
        Vector2 cursorPos = ImGui.GetCursorScreenPos();
        Vector2 center = cursorPos + new Vector2(outerRadius + 10, outerRadius + 10);

        var drawList = ImGui.GetWindowDrawList();

        if (this.configuration.BackgroundOpacity > 0.01f)
        {
            uint darkBgColor = ImGui.GetColorU32(new Vector4(0.02f, 0.02f, 0.02f, this.configuration.BackgroundOpacity));
            drawList.AddCircleFilled(center, outerRadius + 10f, darkBgColor, 64);
        }

        uint redRingColor = ImGui.GetColorU32(new Vector4(1.0f, 0.15f, 0.15f, 1.0f));
        uint redBgTrackColor = ImGui.GetColorU32(new Vector4(0.4f, 0.05f, 0.05f, 0.5f));

        uint blueRingColor = ImGui.GetColorU32(new Vector4(0.0f, 0.75f, 1.0f, 1.0f));
        uint blueBgTrackColor = ImGui.GetColorU32(new Vector4(0.0f, 0.2f, 0.4f, 0.5f));

        // Extérieur (Physique)
        drawList.AddCircle(center, outerRadius, redBgTrackColor, 64, thickness);
        if (physPercent > 0)
        {
            float physAngleEnd = -MathF.PI / 2f + ((physPercent / 100f) * MathF.PI * 2f);
            drawList.PathClear();
            drawList.PathArcTo(center, outerRadius, -MathF.PI / 2f, physAngleEnd, 64);
            drawList.PathStroke(redRingColor, 0, thickness);
        }

        // Intérieur (Magique)
        drawList.AddCircle(center, innerRadius, blueBgTrackColor, 64, thickness);
        if (magicPercent > 0)
        {
            float magicAngleEnd = -MathF.PI / 2f + ((magicPercent / 100f) * MathF.PI * 2f);
            drawList.PathClear();
            drawList.PathArcTo(center, innerRadius, -MathF.PI / 2f, magicAngleEnd, 64);
            drawList.PathStroke(blueRingColor, 0, thickness);
        }

        // Textes
        string physText = $"{physPercent:F0}%";
        Vector2 physTextSize = ImGui.CalcTextSize(physText);
        drawList.AddText(new Vector2(center.X - (40f * scale) - (physTextSize.X / 2f), center.Y - (physTextSize.Y / 2f)), redRingColor, physText);

        string magicText = $"{magicPercent:F0}%";
        Vector2 magicTextSize = ImGui.CalcTextSize(magicText);
        drawList.AddText(new Vector2(center.X + (22f * scale) - (magicTextSize.X / 2f), center.Y - (magicTextSize.Y / 2f)), blueRingColor, magicText);

        string globalText = $"TOTAL: {globalPercent:F1}%";
        Vector2 globalTextSize = ImGui.CalcTextSize(globalText);
        drawList.AddText(new Vector2(center.X - (globalTextSize.X / 2f), center.Y - (35f * scale)), ImGui.GetColorU32(new Vector4(1f, 1f, 1f, 1.0f)), globalText);

        string armorText = $"[Base: {baseArmorPercent:F0}%]";
        Vector2 armorTextSize = ImGui.CalcTextSize(armorText);
        drawList.AddText(new Vector2(center.X - (armorTextSize.X / 2f), center.Y + (20f * scale)), ImGui.GetColorU32(new Vector4(0.7f, 0.7f, 0.7f, 0.8f)), armorText);

        ImGui.Dummy(size);
    }
}