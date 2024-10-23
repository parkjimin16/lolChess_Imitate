using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MergeScene : MonoBehaviour
{
    public static bool GameStart;

    [SerializeField] private UISceneMain mainScene;
    [SerializeField] private GameDataBlueprint gameDataBlueprint;
    [SerializeField] private MapGenerator mapGenerator;
    public int Level = 1;

    private void Start()
    {
        Manager.Asset.LoadAllAsync((count, totalCount) =>
        {
            if (count >= totalCount)
            {
                gameDataBlueprint = Manager.Asset.GetBlueprint("GameDataBlueprint") as GameDataBlueprint;
                //GameStart = false;
                //GameStart = true;
                mainScene.InitBtn(gameDataBlueprint);
                Manager.Item.Init();
                mapGenerator.InitMapGenerator(gameDataBlueprint);
            }
        });
    }
}
