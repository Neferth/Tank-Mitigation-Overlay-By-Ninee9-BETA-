using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.Types;

namespace TankMitigationOverlay;

public class MitigationTracker
{
    private static readonly Dictionary<uint, (string Name, float MaxDuration, float Percent, DamageType Type)> KnownMitigations = new()
    {
        // ==========================================
        // 1. DEBUFFS ENNEMIS (Rétorsion, Addle, etc.)
        // ==========================================
        { 1193, ("Représailles", 15f, 10f, DamageType.Both) },
        { 1196, ("Manteau de ténèbres", 10f, 10f, DamageType.Magic) },
        { 1195, ("Feinte", 15f, 10f, DamageType.Physical) },
        { 2688, ("Emballement", 10f, 10f, DamageType.Both) },

        // ==========================================
        // 2. TANK MITIGATIONS (Niveau 1 à 100)
        // ==========================================
        { 1191, ("Rempart", 20f, 20f, DamageType.Both) },

        // --- PALADIN (PLD) ---
        { 74,   ("Sentinelle", 15f, 30f, DamageType.Both) },
        { 3825, ("Gardien", 15f, 40f, DamageType.Both) },
        { 3828, ("Gardien", 15f, 40f, DamageType.Both) },
        { 3833, ("Gardien", 15f, 40f, DamageType.Both) },
        { 72,   ("Sheltron", 6f, 15f, DamageType.Both) },
        { 1362, ("Holy Sheltron", 8f, 15f, DamageType.Both) },
        
        // PASSE D'ARMES (IDs de canalisation et de buff de zone)
        { 736,  ("Passe d'armes", 18f, 15f, DamageType.Both) },
        { 1174, ("Passe d'armes", 18f, 15f, DamageType.Both) },

        { 2674, ("Voile divin", 30f, 10f, DamageType.Both) },
        { 82,   ("Invulnérable", 10f, 100f, DamageType.Both) },

        // --- GUERRIER (WAR) ---
        { 1457, ("Débarrassage", 30f, 15f, DamageType.Both) },
        { 3826, ("Damnation", 15f, 40f, DamageType.Both) },
        { 3834, ("Damnation", 15f, 40f, DamageType.Both) },
        { 2677, ("Bravoure", 8f, 19f, DamageType.Both) },
        { 1178, ("Intuition brute", 6f, 10f, DamageType.Both) },
        { 1177, ("Frisson de la bataille", 10f, 10f, DamageType.Both) },
        { 1179, ("Cri de ralliement", 15f, 15f, DamageType.Both) },
        { 409,  ("Holmgang", 10f, 100f, DamageType.Both) },

        // --- CHEVALIER NOIR (DRK) ---
        { 747,  ("Rempart d'ombre", 15f, 30f, DamageType.Both) },
        { 3827, ("Vigilance d'ombre", 15f, 40f, DamageType.Both) },
        { 3835, ("Vigilance d'ombre", 15f, 40f, DamageType.Both) },
        { 2681, ("Mission sombre", 15f, 10f, DamageType.Magic) },
        { 2679, ("Oblation", 10f, 10f, DamageType.Both) },
        { 1308, ("Nuit noire", 7f, 25f, DamageType.Both) },
        { 810,  ("Mort-vivant", 10f, 100f, DamageType.Both) },
        { 2682, ("Oblation", 10f, 10f, DamageType.Both) },
        { 746,  ("Esprit ténébreux", 10f, 20f, DamageType.Magic) },
        { 1894,  ("Missionaire des ténèbres", 15f, 10f, DamageType.Physical) },

        // --- PISTOCEURS (GNB) ---
        { 1831, ("Nébuleuse", 15f, 30f, DamageType.Both) },
        { 3838, ("Grande nébuleuse", 15f, 40f, DamageType.Both) },
        { 3829, ("Grande nébuleuse", 15f, 40f, DamageType.Both) },
        { 2683, ("Cœur de corindon", 8f, 15f, DamageType.Both) },
        { 1833, ("Cœur de pierre", 7f, 15f, DamageType.Both) },
        { 1832, ("Camouflage", 20f, 10f, DamageType.Both) },
        { 1839, ("Cœur de lumière", 15f, 10f, DamageType.Physical) },
        { 1836, ("Bolide", 10f, 100f, DamageType.Both) },

        // ==========================================
        // 3. HEALERS & DPS (BUFFS EXTERNES)
        // ==========================================
        { 2708, ("Aquavoile", 15f, 15f, DamageType.Both) },
        { 2709, ("Cloche de joie", 15f, 10f, DamageType.Both) },
        { 1873, ("Tempérance", 20f, 10f, DamageType.Both) },  // Correction de l'ID (1873) pour les 10% de réduction
        { 3881, ("Faveur divine", 15f, 15f, DamageType.Both) },  // Ajout de la Faveur divine (Divine Benison - WHM)
        { 7433, ("Indulgence plénière", 10f, 10f, DamageType.Both) }, // ID exact du nouveau buff de mitigation (10%)
        { 299,  ("Loi de la fortification", 20f, 10f, DamageType.Both) },
        { 1896, ("Pacte féérique", 20f, 10f, DamageType.Magic) },
        { 2710, ("Protocole de guerre", 20f, 10f, DamageType.Both) },
        { 2717, ("Exaltation", 8f, 10f, DamageType.Both) },
        { 1881, ("Inconscience", 15f, 10f, DamageType.Both) },
        { 2716, ("Nébuleuse céleste", 15f, 10f, DamageType.Both) },
        { 2618, ("Taurochole", 15f, 10f, DamageType.Both) },
        { 2619, ("Kerachole", 15f, 10f, DamageType.Both) },
        { 2615, ("Haima", 15f, 10f, DamageType.Both) },
        { 2616, ("Panhaima", 15f, 10f, DamageType.Both) },
        { 1197, ("Troubadour", 15f, 15f, DamageType.Both) },
        { 1934, ("Tactique tactique", 15f, 10f, DamageType.Both) },
        { 1826, ("Samba de la nature", 15f, 10f, DamageType.Both) },
        { 2703, ("Magie défensive", 10f, 10f, DamageType.Both) },
        { 860, ("Brise Arme", 10, 10f, DamageType.Both) },
        { 2177, ("Tacticien", 15, 15f, DamageType.Both) },
        { 1951, ("Tacticien", 15, 15f, DamageType.Both) },

    };

    public List<TankBuff> GetActiveMitigations(IGameObject localPlayer, IGameObject? target)
    {
        var activeBuffs = new List<TankBuff>();

        if (localPlayer is IBattleChara playerChara)
        {
            foreach (var status in playerChara.StatusList)
            {
                if (status.StatusId == 0) continue;

                if (KnownMitigations.TryGetValue(status.StatusId, out var info))
                {
                    if (!activeBuffs.Exists(b => b.Name == info.Name))
                    {
                        activeBuffs.Add(new TankBuff(status.StatusId, info.Name, info.Percent, info.Type)
                        {
                            RemainingTime = status.RemainingTime,
                            MaxDuration = info.MaxDuration > 0 ? info.MaxDuration : 15f
                        });
                    }
                }
            }
        }

        if (target is IBattleChara targetChara)
        {
            foreach (var status in targetChara.StatusList)
            {
                if (status.StatusId == 0) continue;

                if (KnownMitigations.TryGetValue(status.StatusId, out var info))
                {
                    if (!activeBuffs.Exists(b => b.Name == info.Name))
                    {
                        activeBuffs.Add(new TankBuff(status.StatusId, info.Name + " (Cible)", info.Percent, info.Type)
                        {
                            RemainingTime = status.RemainingTime,
                            MaxDuration = info.MaxDuration > 0 ? info.MaxDuration : 15f
                        });
                    }
                }
            }
        }

        return activeBuffs;
    }

    public (float Physical, float Magic, float Global) CalculateMitigations(List<TankBuff> buffs)
    {
        if (buffs.Count == 0) return (0f, 0f, 0f);

        float physMultiplier = 1.0f;
        float magicMultiplier = 1.0f;
        float globalMultiplier = 1.0f;

        foreach (var buff in buffs)
        {
            globalMultiplier *= buff.DamageMultiplier;

            if (buff.Type == DamageType.Both || buff.Type == DamageType.Physical)
                physMultiplier *= buff.DamageMultiplier;

            if (buff.Type == DamageType.Both || buff.Type == DamageType.Magic)
                magicMultiplier *= buff.DamageMultiplier;
        }

        float physMitig = (1.0f - physMultiplier) * 100.0f;
        float magicMitig = (1.0f - magicMultiplier) * 100.0f;
        float globalMitig = (1.0f - globalMultiplier) * 100.0f;

        return (physMitig, magicMitig, globalMitig);
    }
}