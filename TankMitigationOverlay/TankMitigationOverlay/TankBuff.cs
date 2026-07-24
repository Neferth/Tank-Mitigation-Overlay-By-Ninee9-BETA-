namespace TankMitigationOverlay;

public enum DamageType
{
    Both,
    Physical,
    Magic
}

public class TankBuff
{
    public uint StatusId { get; set; }
    public string Name { get; set; } = string.Empty;
    public float RemainingTime { get; set; }
    public float MaxDuration { get; set; } = 15f;
    public float PercentValue { get; set; } = 0f;
    public DamageType Type { get; set; } = DamageType.Both;

    public float DamageMultiplier => 1.0f - (PercentValue / 100.0f);

    public TankBuff(uint statusId, string name, float percentValue, DamageType type = DamageType.Both)
    {
        StatusId = statusId;
        Name = name;
        PercentValue = percentValue;
        Type = type;
        RemainingTime = 0;
    }
}