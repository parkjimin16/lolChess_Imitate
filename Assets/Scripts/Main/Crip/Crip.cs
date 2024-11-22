using ChampionOwnedStates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class Crip : MonoBehaviour
{
    [SerializeField] private CripObjectData cripData;

    [SerializeField]private int currentHP;
    public HexTile CurrentTile;
    public MapGenerator.MapInfo PlayerMapInfo;
    public bool IsDie;


    public Animator anim;
    //[SerializeField] private ItemTile itemtile;


    public int CurrentHp
    {
        get { return currentHP; }
        set { currentHP = value; }
    }


    public void InitCrip()
    {
        IsDie = false;
        currentHP = cripData.HP;
        anim = GetComponentInChildren<Animator>();
    }


    void Start()
    {
        IsDie = false;
        currentHP = cripData.HP;
        anim = GetComponentInChildren<Animator>();
    }



    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            IsDie = true;

            Death();
        }
    }

    void OnDeath()
    {
        //itemtile.GenerateItem();
        // ������ ����
        //GenerateItem();

        // Ÿ�� ���� ������Ʈ
        if (CurrentTile != null)
        {
            //currentTile.isOccupied = false;
            CurrentTile.itemOnTile = null;
        }

        // �ڽ� �ı�
      
    }

    private void DestroyObj()
    {
        Destroy(this.gameObject);
    }

    public void Death()
    {
        //GenerateItem(); ������ ����

        if (CurrentTile != null)
        {
            CurrentTile = gameObject.GetComponent<CripMovement>().currentTile;
            //currentTile.isOccupied = false;
            CurrentTile.championOnTile.Remove(this.gameObject);
        }

        if (PlayerMapInfo != null)
        {
            Player playerComponent = PlayerMapInfo.playerData;
            if (playerComponent != null)
            {
                playerComponent.UserData.CripObjectList.Remove(this.gameObject);
            }
        }

        PlayAnimation("Die");
        Invoke("DestroyObj", 0.5f);
    }

    public void PlayAnimation(string animationName)
    {
        if (anim != null)
        {
            anim.Play(animationName);
        }
        else
        {
            Debug.LogError("Animator is not assigned.");
        }
    }
    void GenerateItem()
    {
        // ũ���� ���� ���� ItemTile�� ã���ϴ�.
        ItemTile itemTile = FindItemTileInMap();

        if (itemTile != null)
        {
            itemTile.GenerateItem();
        }
        else
        {
            Debug.LogWarning("ItemTile�� ã�� �� �����ϴ�.");
        }
    }

    ItemTile FindItemTileInMap()
    {
        if (PlayerMapInfo != null)
        {
            foreach (var itemTile in PlayerMapInfo.ItemTile)
            {
                if(itemTile.TileType1 == ItemOwner.Player)
                {
                    return itemTile;
                }
            }
            
        }
        return null;
    }
}
