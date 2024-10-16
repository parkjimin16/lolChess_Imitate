public abstract class BaseState
{
    protected ChampionBase championBase;

    protected BaseState(ChampionBase championBase)
    {
        this.championBase = championBase;
    }

    public abstract void Enter(ChampionBase championBase);
    public abstract void Execute(ChampionBase championBase);
    public abstract void Exit(ChampionBase championBase);
}
