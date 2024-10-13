public interface IState
{
    public void Enter(ChampionBase champBase);    // 상태 진입 시 호출
    public void Execute(ChampionBase champBase);  // 상태 유지 중 호출
    public void Exit(ChampionBase champBase);     // 상태 종료 시 호출
}
