using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capsule : MonoBehaviour
{
    #region ���� & ������Ƽ
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

    #region �ʱ�ȭ

    public void InitCapsule(int goldAmount, List<string> itemList, List<string> championList)
    {
        gold = goldAmount;
        itemContainer = itemList;
        championContainer = championList;
    }
    #endregion


    #region �浹 ó��
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("�浹 üũ ��");
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("�浹");
            Player player = other.GetComponent<Player>();
            if (player != null && player.UserData.MapInfo.ItemTile[0].EmptyTileCount() >= ItemContainer.Count 
                && Manager.Champion.GetEmptyTileCount(player.UserData) >= championContainer.Count)
            {
                Manager.User.UserCrushWithCapsule(player.UserData, gold, ItemContainer, championContainer);
                Destroy(gameObject);
            }
        }

    }
    #endregion
}
