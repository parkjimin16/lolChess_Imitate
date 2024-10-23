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
    public int Level = 1;

    private void Start()
    {
        Manager.Asset.LoadAllAsync((count, totalCount) =>
        {
            if (count >= totalCount)
            {
                GameStart = false;
            }
        });
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            gameDataBlueprint = Manager.Asset.GetBlueprint("GameDataBlueprint") as GameDataBlueprint;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            GameStart = true;
            mainScene.InitBtn(gameDataBlueprint);
            Manager.Item.Init();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            string itemId = Manager.Item.NormalItem[Random.Range(0, Manager.Item.NormalItem.Count)].ItemId;

            Manager.Item.CreateItem(itemId, new Vector3(0, 0, 0));
        }
    }

   
}
