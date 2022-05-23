
public abstract class UnitState
{
    protected Unit _unit;

    public UnitState(Unit unit)
    {
        _unit = unit;
    }

    public abstract void Apply();
    public abstract void MakeTransition(UnitState state);
}
