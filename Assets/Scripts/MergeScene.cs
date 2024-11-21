using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeScene : MonoBehaviour
{
    /// <summary>
    /// 게임 시작 변수
    /// </summary>
    public static bool GameStart;

    /// <summary>
    /// 전투 시작 변수
    /// </summary>
    public static bool BatteStart;



    [SerializeField] private UISceneMain mainScene;
    [SerializeField] private GameDataBlueprint gameDataBlueprint;
    [SerializeField] private SymbolDataBlueprint symbolDataBlueprint;
    [SerializeField] private AugmenterBlueprint augmenterBlueprint;
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private Camera mainCam;
    [SerializeField] private User user;
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
                augmenterBlueprint = Manager.Asset.GetBlueprint("AugmenterBlueprint") as AugmenterBlueprint;
                GameStart = true;
                BatteStart = false;


                Manager.Game.InitGameManager();
                Manager.User.Init();
                Manager.ObjectPool.Initialize();

                mapGenerator.InitMapGenerator(gameDataBlueprint);
                Manager.User.InitMap(mapGenerator);
                mainScene.InitPanel(gameDataBlueprint, symbolDataBlueprint);
                
                Manager.Champion.Init(gameDataBlueprint);

                MinimapController minimapClickHandler = FindObjectOfType<MinimapController>();
                Manager.Cam.Init(mainCam, mapGenerator, mainScene, minimapClickHandler);

                Manager.User.SetShopUI(Manager.User.GetHumanUserData(), mainScene);
                Manager.Stage.InitStage(Manager.Game.PlayerListObject, mapGenerator, gameDataBlueprint, user);
                
                Manager.Item.Init();
                Manager.Synergy.Init(symbolDataBlueprint);
                Manager.Augmenter.Init(augmenterBlueprint);



                mainScene.UIShopPanel.UpdateChampionSlot(null);
                mainScene.UISynergyPanel.UpdateSynergy(Manager.User.GetHumanUserData());


            }
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            GameObject obj = Manager.ObjectPool.GetGo("Capsule");
            obj.transform.position = new Vector3(0, 2, 0);
            Capsule cap = obj.GetComponent<Capsule>();
            List<string> item = new List<string>();
            List<string> champion = new List<string>();

            item.Add("C017");
            item.Add("C018");

            
            string a = Manager.Champion.GetChampionInstantiateName("ChampionFrame_Olaf");
            string b = Manager.Champion.GetChampionInstantiateName("ChampionFrame_NorraYummi");

            champion.Add(a);
            champion.Add(b);

            cap.InitCapsule(0, item, champion);

            //Manager.Item.CreateItem("B020", new Vector3(0, 0, 0));
            //AugmenterData aData = augmenterBlueprint.GetAugmentByName("삼총사");
            //Manager.Augmenter.SetAugmenter(Manager.User.GetHumanUserData(), aData);
        }
        else if (Input.GetKeyDown(KeyCode.X)) // 증강 팝업
        {
            var augPopup = Manager.UI.ShowPopup<UIPopupAugmenter>();

            augPopup.InitAugmenterGoldPopup();
        }
        else if (Input.GetKeyDown(KeyCode.J)) // 증강 시작
        {
            //Manager.Augmenter.ApplyFirstAugmenter(Manager.User.GetHumanUserData());
            Manager.Augmenter.ApplyStartRoundAugmenter(Manager.User.GetHumanUserData());
        }
        else if (Input.GetKeyDown(KeyCode.K)) // 증강 종료
        {
            Manager.Augmenter.ApplyEndRoundAugmenter(Manager.User.GetHumanUserData());

            foreach(var obj in Manager.User.GetHumanUserData().TotalChampionObject)
            {
                ChampionBase cBase = obj.GetComponent<ChampionBase>();
                cBase.ResetChampionStats();
            }
        }
        else if (Input.GetKeyDown(KeyCode.L)) // 증강 원할때
        {
            Manager.Augmenter.ApplyWheneverAugmenter(Manager.User.GetHumanUserData());

        }
    }
}
