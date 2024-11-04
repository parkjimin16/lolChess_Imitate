using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeScene : MonoBehaviour
{
    public static bool GameStart;

    [SerializeField] private UISceneMain mainScene;
    [SerializeField] private GameDataBlueprint gameDataBlueprint;
    [SerializeField] private SymbolDataBlueprint symbolDataBlueprint;
    [SerializeField] private MapGenerator mapGenerator;
    public int Level = 1;

    private void Start()
    {
        Manager.Asset.LoadAllAsync((count, totalCount) =>
        {
            GameStart = false;

            if (count >= totalCount)
            {
                gameDataBlueprint = Manager.Asset.GetBlueprint("GameDataBlueprint") as GameDataBlueprint;
                symbolDataBlueprint = Manager.Asset.GetBlueprint("SymbolDataBlueprint") as SymbolDataBlueprint;
                GameStart = true;
                mainScene.InitPanel(gameDataBlueprint, symbolDataBlueprint);
                Manager.Item.Init();
                Manager.User.Init();
                Manager.Synerge.Init(symbolDataBlueprint);
                Manager.Game.InitGameManager();
                mapGenerator.InitMapGenerator(gameDataBlueprint);
            }
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //Manager.Item.CreateItem("B020", new Vector3(0, 0, 0));
        }
        else if(Input.GetKeyDown(KeyCode.X)) 
        {
            ChampionBlueprint cBlueprint = Manager.Asset.GetBlueprint("ChampionBlueprint_Seraphine") as ChampionBlueprint;

            GameObject newChampionObject = Manager.Asset.InstantiatePrefab(cBlueprint.ChampionInstantiateName);
            newChampionObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            newChampionObject.tag = "Enemy";

            GameObject frame = Manager.Asset.InstantiatePrefab("ChampionFrame");

            frame.transform.SetParent(newChampionObject.transform, false);
            newChampionObject.transform.position = new Vector3(0, 5, 0);

            ChampionBase cBase = newChampionObject.GetComponent<ChampionBase>();
            ChampionFrame cFrame = frame.GetComponentInChildren<ChampionFrame>();

            cBase.SetChampion(cBlueprint);
            cBase.InitChampion(cFrame);
        }
        else if(Input.GetKeyDown(KeyCode.N)) //전투 시작 
        {
            Manager.Synerge.ApplySynergy(Manager.User.User1_Data);
        }
        else if(Input.GetKeyDown(KeyCode.M)) // 전투 종료
        {
            Manager.Synerge.UnApplySynergy(Manager.User.User1_Data);
        }
    }
}
