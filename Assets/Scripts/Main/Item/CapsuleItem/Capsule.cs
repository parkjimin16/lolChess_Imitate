using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capsule : MonoBehaviour
{
    #region 변수 & 프로퍼티
    [SerializeField] private int gold;
    [SerializeField] private List<string> itemContainer;
    [SerializeField] private List<string> championContainer;


    public int Gold
    {
        get { return gold; }
        set { gold = value; }
    }

    public List<string> ItemContainer
    {
        get { return itemContainer; }
        set { itemContainer = value; }
    }

    public List<string> ChampionContainer
    {
        get { return championContainer; }
        set { championContainer = value; }
    }
    #endregion

    #region 초기화

    public void InitCapsule(int goldAmount, List<string> itemList, List<string> championList)
    {
        gold = goldAmount;
        itemContainer = itemList;
        championContainer = championList;
    }
    #endregion

    #region 충돌 처리
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null && player.UserData.MapInfo.ItemTile[0].EmptyTileCount() >= ItemContainer.Count 
                && Manager.Champion.GetEmptyTileCount(player.UserData) >= championContainer.Count)
            {
                Manager.User.UserCrushWithCapsule(player.UserData, gold, ItemContainer, championContainer);
                
                UISceneMain main = GameObject.Find("UISceneMain").GetComponent<UISceneMain>();
                main.UIShopPanel.UpdatePlayerGold(Manager.User.GetHumanUserData());

                Destroy(gameObject);
            }
        }

    }
    #endregion
}
