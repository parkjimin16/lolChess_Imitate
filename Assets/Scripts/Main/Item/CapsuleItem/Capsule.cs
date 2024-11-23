using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capsule : ObjectPoolable
{
    #region 변수 & 프로퍼티
    [SerializeField] private int gold;
    [SerializeField] private List<string> itemContainer;
    [SerializeField] private List<string> championContainer;
    [SerializeField] private int capsuleOwner;
    [SerializeField] private UserData ownerData;

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
    public int CapsuleOwner
    {
        get { return capsuleOwner; }
        set { capsuleOwner = value; }
    }

    public UserData OwnerData
    {
        get { return ownerData;}
        set { ownerData = value; }
    }
    #endregion

    #region 초기화

    public void InitCapsule(UserData owner, int goldAmount, List<string> itemList, List<string> championList)
    {
        OwnerData = owner;
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
            if (player.UserData == ownerData && player != null && player.UserData.MapInfo.ItemTile[0].EmptyTileCount() >= ItemContainer.Count 
                && Manager.Champion.GetEmptyTileCount(player.UserData) >= championContainer.Count)
            {
                Manager.User.UserCrushWithCapsule(player.UserData, gold, ItemContainer, championContainer);
                
                ReleaseObject();
                ObjectOff();
                //Destroy(gameObject);
            }
        }

    }
    #endregion
}
