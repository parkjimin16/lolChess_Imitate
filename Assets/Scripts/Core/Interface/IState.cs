public interface IState
{
    public void Enter(ChampionBase champBase);    // ���� ���� �� ȣ��
    public void Execute(ChampionBase champBase);  // ���� ���� �� ȣ��
    public void Exit(ChampionBase champBase);     // ���� ���� �� ȣ��
}
