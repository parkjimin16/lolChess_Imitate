using UnityEngine;

public class Manager : MonoBehaviour
{
    #region Singleton

    private static Manager instance;
    private static bool initialized;
    public static Manager Instance
    {
        get
        {
            if (!initialized)
            {
                initialized = true;

                GameObject obj = GameObject.Find("@Manager");
                if (obj == null)
                {
                    obj = new() { name = "@Manager" };
                    obj.AddComponent<Manager>();
                    DontDestroyOnLoad(obj);
                    instance = obj.GetComponent<Manager>();
                }
            }
            return instance;
        }
    }

    #endregion

    #region Manager

    // ´ë¿¬
    private readonly AssetManager asset = new();
    private readonly GameManager game = new();
    private readonly ItemManager item = new();
    private readonly ChampionManager champion = new();
    private readonly SkillManager skill = new();
    private readonly ObjectPoolManager objectPool = new();
    private readonly ClickManager click = new();
    private readonly UIManagerTemp ui = new();


    // Áö¹Î
    private readonly StageManager stage = new();
    private readonly BattleManager battle = new();
    private readonly UserHpManager userHp = new();

    // ´ë¿¬
    public static AssetManager Asset => Instance != null ? Instance.asset : null;
    public static GameManager Game => Instance != null ? Instance.game : null;
    public static ItemManager Item => Instance != null ? Instance.item : null;
    public static ChampionManager Champion => Instance != null ? Instance.champion : null;
    public static SkillManager Skill => Instance != null ? Instance.skill : null;
    public static ObjectPoolManager ObjectPool => Instance != null ? Instance.objectPool : null;
    public static ClickManager Click => Instance != null ? Instance.click : null;
    public static UIManagerTemp UI => Instance != null ? Instance.ui : null;


    // Áö¹Î

    public static StageManager Stage => Instance != null ? Instance.stage : null;
    public static BattleManager Battle => Instance != null ? Instance.battle : null;
    public static UserHpManager UserHp => Instance != null ? Instance.userHp : null;
    #endregion
}
