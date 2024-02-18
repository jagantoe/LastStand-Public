namespace LastStand.Game;
public class Resources(uint grain = 0, uint wood = 0, uint stone = 0, uint steel = 0)
{
    public uint Grain { get; set; } = grain;
    public uint Wood { get; set; } = wood;
    public uint Stone { get; set; } = stone;
    public uint Steel { get; set; } = steel;

    public uint Limit { get; set; }

    public bool Full(ResourceType type) => type switch
    {
        ResourceType.Grain => Grain >= Limit,
        ResourceType.Wood => Wood >= Limit,
        ResourceType.Stone => Stone >= Limit,
        ResourceType.Steel => Steel >= Limit,
        _ => true
    };
    public bool HasSufficientResources(Resources resources)
    {
        if (Grain < resources.Grain
            || Wood < resources.Wood
            || Stone < resources.Stone
            || Steel < resources.Steel) return false;
        return true;
    }

    public void Add(Resources resources)
    {
        AddGrain(resources.Grain);
        AddWood(resources.Wood);
        AddStone(resources.Stone);
        AddSteel(resources.Steel);
    }

    public void Add(ResourceType type, uint amount)
    {
        switch (type)
        {
            case ResourceType.Grain:
                AddGrain(amount);
                break;
            case ResourceType.Wood:
                AddWood(amount);
                break;
            case ResourceType.Stone:
                AddStone(amount);
                break;
            case ResourceType.Steel:
                AddSteel(amount);
                break;
            default:
                break;
        }
    }

    private void AddGrain(uint amount)
    {
        if (Limit > 0 && Grain + amount > Limit) Grain = Limit;
        else Grain += amount;
    }
    private void AddWood(uint amount)
    {
        if (Limit > 0 && Wood + amount > Limit) Wood = Limit;
        else Wood += amount;
    }
    private void AddStone(uint amount)
    {
        if (Limit > 0 && Stone + amount > Limit) Stone = Limit;
        else Stone += amount;
    }
    private void AddSteel(uint amount)
    {
        if (Limit > 0 && Steel + amount > Limit) Steel = Limit;
        else Steel += amount;
    }

    public void Clear()
    {
        Grain = 0;
        Wood = 0;
        Stone = 0;
        Steel = 0;
    }

    public void Reset()
    {
        Clear();
        Limit = Constants.BaseLimit;
    }

    public static Resources operator *(Resources resources, uint amount)
    {
        return new Resources()
        {
            Grain = resources.Grain * amount,
            Wood = resources.Wood * amount,
            Stone = resources.Stone * amount,
            Steel = resources.Steel * amount
        };
    }

    public static Resources operator -(Resources resources, Resources cost)
    {
        resources.Grain -= cost.Grain;
        resources.Wood -= cost.Wood;
        resources.Stone -= cost.Stone;
        resources.Steel -= cost.Steel;
        return resources;
    }
}
