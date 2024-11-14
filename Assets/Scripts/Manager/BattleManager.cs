using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ItemAttribute;
using static MapGenerator;

public class BattleManager
{
    private Dictionary<GameObject, Coroutine> battleCoroutines = new Dictionary<GameObject, Coroutine>();

    public void StartBattle(GameObject player1, GameObject player2, int duration)
    {
        if (battleCoroutines.ContainsKey(player1))
            return;

        Coroutine battleCoroutine = CoroutineHelper.StartCoroutine(BattleCoroutine(duration, player1, player2));
        battleCoroutines.Add(player1, battleCoroutine);
        battleCoroutines.Add(player2, battleCoroutine);
    }

    IEnumerator BattleCoroutine(int duration, GameObject player1, GameObject player2)
    {
        //Debug.Log($"������ ���۵Ǿ����ϴ�. {player1.GetComponent<Player>().UserData.UserName} vs {player2.GetComponent<Player>().UserData.UserName}, ���� �ð�: {duration}��");

        yield return new WaitForSeconds(duration);

        // ���÷� �����ϰ� ���и� ����
        bool player1Won = Random.value > 0.5f;
        int survivingEnemyUnits = player1Won ? 0 : Random.Range(1, 5);

        //Debug.Log(player1Won ? $"{player1.name} �¸�" : $"{player1.name} �й�");

        EndBattle(player1, player2, player1Won, survivingEnemyUnits);
    }


    void EndBattle(GameObject player1, GameObject player2, bool player1Won, int survivingEnemyUnits)
    {
        if (battleCoroutines.ContainsKey(player1))
        {
            battleCoroutines.Remove(player1);
        }
        if (battleCoroutines.ContainsKey(player2))
        {
            battleCoroutines.Remove(player2);
        }

        // ��� è�Ǿ�, �������� ���� ��ġ�� ����
        RestoreOpponentPlayer(player2);
        RestoreOpponentChampions(player2);
        RestoreOpponentItems(player2);

        // StageManager�� ���� ���Ḧ �˸�
        Debug.Log(player1Won ? $"{player1.GetComponent<Player>().UserData.UserName} �¸�" : $"{player2.GetComponent<Player>().UserData.UserName} �¸�");
        Manager.Stage.OnBattleEnd(player1, player2, player1Won, survivingEnemyUnits);

        MergeScene.BatteStart = false;
    }

    public void MovePlayer(GameObject player1, GameObject player2)
    {
        // ��� �÷��̾��� è�Ǿ�, �������� player1�� ������ �̵�
        MoveOpponentPlayerToPlayerMap(player1, player2);
        MoveOpponentChampionsToPlayerMap(player1, player2);
        MoveOpponentItemsToPlayerMap(player1, player2);
    }


    #region �÷��̾� �̵�����
    private void MoveOpponentPlayerToPlayerMap(GameObject player1, GameObject player2)
    {
        Player playerComponent1 = player1.GetComponent<Player>();
        Player playerComponent2 = player2.GetComponent<Player>();


        playerComponent1.SetBattleStageIndex(playerComponent1.UserData.UserId);
        playerComponent2.SetBattleStageIndex(playerComponent1.UserData.UserId);

        // �÷��̾�2�� �÷��̾�1�� ������ �̵���ŵ�ϴ�.
        Transform player1MapTransform = playerComponent1.UserData.MapInfo.mapTransform;
        Vector3 opponentPlayerPosition = player1MapTransform.position + new Vector3(13.5f, 0.8f, 5f);

        player2.transform.position = opponentPlayerPosition;

        // �ʿ信 ���� �÷��̾��� ȸ���� �����մϴ�.
        player2.transform.LookAt(player1.transform.position);
    }
    private void RestoreOpponentPlayer(GameObject opponent)
    {
        Player opponentComponent = opponent.GetComponent<Player>();

        if (opponentComponent != null)
        {
            // �÷��̾ ���� ��ġ�� ����
            opponent.transform.position = opponentComponent.UserData.MapInfo.mapTransform.position + new Vector3(-13.5f, 0.8f, -5f);
        }
    }
    #endregion

    #region è�Ǿ� �̵� ����
    private void MoveOpponentChampionsToPlayerMap(GameObject player1, GameObject player2)
    {
        Player playerComponent1 = player1.GetComponent<Player>();
        Player playerComponent2 = player2.GetComponent<Player>();

        MapInfo playerMapInfo1 = playerComponent1.UserData.MapInfo;
        MapInfo playerMapInfo2 = playerComponent2.UserData.MapInfo;

        if (playerMapInfo1 == null || playerMapInfo2 == null)
        {
            Debug.LogWarning("�� ������ ������ �� �����ϴ�.");
            return;
        }

        // ��� �÷��̾��� ���� ����Ʈ�� �����ɴϴ�.
        List<GameObject> opponentUnits = playerComponent2.UserData.BattleChampionObject;

        foreach (GameObject opponentUnit in opponentUnits)
        {
            // ������ ���� ���¸� �����մϴ�.
            ChampionOriginalState originalState = new ChampionOriginalState
            {
                originalPosition = opponentUnit.transform.position,
                originalParent = opponentUnit.transform.parent,
                originalTile = opponentUnit.GetComponentInParent<HexTile>(),
                wasActive = opponentUnit.activeSelf
            };


            // UserData�� ����
            playerComponent2.UserData.ChampionOriginState[opponentUnit] = originalState;

            // ��ǥ �����Ͽ� ���� �÷��̾��� �ʿ��� �����Ǵ� Ÿ���� ã���ϴ�.
            HexTile opponentTile = opponentUnit.GetComponentInParent<HexTile>();
            HexTile mirroredTile = GetMirroredTile(opponentTile, playerMapInfo1);

            if (mirroredTile != null)
            {
                opponentTile.championOnTile.Remove(opponentUnit);

                // ������ �̵���ŵ�ϴ�.
                opponentUnit.transform.position = mirroredTile.transform.position + new Vector3(0, 0.5f, 0);
                opponentUnit.transform.SetParent(mirroredTile.transform);

                // Ÿ�� ���� ������Ʈ
                mirroredTile.championOnTile.Add(opponentUnit);
            }

            // ������ ��Ȱ��ȭ�Ǿ� �ִٸ� Ȱ��ȭ
            if (!opponentUnit.activeSelf)
            {
                opponentUnit.SetActive(true);
            }
        }

        List<GameObject> opponentNonBattleUnits = playerComponent2.UserData.NonBattleChampionObject;

        foreach (GameObject opponentUnit in opponentNonBattleUnits)
        {
            // ������ ���� ���¸� �����մϴ�.
            ChampionOriginalState originalState = new ChampionOriginalState
            {
                originalPosition = opponentUnit.transform.position,
                originalParent = opponentUnit.transform.parent,
                originalTile = opponentUnit.GetComponentInParent<HexTile>(),
                wasActive = opponentUnit.activeSelf
            };

            // UserData�� ����
            playerComponent2.UserData.ChampionOriginState[opponentUnit] = originalState;

            // ������ ��ġ�� Ÿ�� ��������
            HexTile opponentTile = opponentUnit.GetComponentInParent<HexTile>();
            HexTile mirroredTile = GetMirroredRectTile(opponentTile, playerMapInfo1);

            // ���� �÷��̾��� �ʿ��� ������ ��ǥ�� �ش��ϴ� Ÿ���� ã���ϴ�.
            if (mirroredTile != null)
            {
                // ��� Ÿ�Ͽ��� ���� ����
                opponentTile.championOnTile.Remove(opponentUnit);

                // ������ �̵���ŵ�ϴ�.
                opponentUnit.transform.position = mirroredTile.transform.position + new Vector3(0, 0.5f, 0);
                opponentUnit.transform.SetParent(mirroredTile.transform);

                // Ÿ�� ���� ������Ʈ
                mirroredTile.championOnTile.Add(opponentUnit);
            }

            // ������ ��Ȱ��ȭ�Ǿ� �ִٸ� Ȱ��ȭ
            if (!opponentUnit.activeSelf)
            {
                opponentUnit.SetActive(true);
            }
        }

    }

    private HexTile GetMirroredTile(HexTile opponentTile, MapInfo playerMapInfo)
    {
        int q = opponentTile.q;
        int r = opponentTile.r;

        // ��ǥ�� �����մϴ�.
        int mirroredQ = 6 - q;
        int mirroredR = 7 - r;

        // ���� �÷��̾��� �ʿ��� ������ ��ǥ�� �ش��ϴ� Ÿ���� ã���ϴ�.
        if (playerMapInfo.HexDictionary.TryGetValue((mirroredQ, mirroredR), out HexTile mirroredTile))
        {
            return mirroredTile;
        }
        else
        {
            Debug.LogWarning($"������ ��ǥ ({mirroredQ}, {mirroredR})�� �ش��ϴ� Ÿ���� ã�� �� �����ϴ�.");
            return null;
        }
    }

    private HexTile GetMirroredRectTile(HexTile opponentTile, MapInfo playerMapInfo)
    {
        int x = opponentTile.x;
        //int y = opponentTile.y;

        // ��ǥ�� �����մϴ�.
        int mirroredX = 8 - x;
        int mirroredY = 8;
        if (playerMapInfo.RectDictionary.TryGetValue((mirroredX, mirroredY), out HexTile mirroredTile))
        {
            return mirroredTile;
        }
        else
        {
            Debug.LogWarning($"������ ��ǥ ({mirroredX}, {mirroredY})�� �ش��ϴ� Ÿ���� ã�� �� �����ϴ�.");
            return null;
        }
    }
    private void RestoreOpponentChampions(GameObject opponent)
    {
        Player opponentComponent = opponent.GetComponent<Player>();
        UserData opponentData = opponentComponent.UserData;

        foreach (var kvp in opponentData.ChampionOriginState)
        {
            GameObject unit = kvp.Key;
            ChampionOriginalState originalState = kvp.Value;

            // ������ ����Ͽ� ��Ȱ��ȭ�� ��� Ȱ��ȭ
            if (!unit.activeSelf && originalState.wasActive)
            {
                unit.SetActive(true);
            }

            // ���� Ÿ�Ͽ��� ���� ����
            HexTile currentTile = unit.GetComponentInParent<HexTile>();

            if (currentTile != null)
            {
                currentTile.championOnTile.Remove(unit);
            }

            // ���� Ÿ�Ϸ� ����
            unit.transform.position = originalState.originalPosition;
            unit.transform.SetParent(originalState.originalParent);

            // Ÿ�� ���� ������Ʈ
            currentTile = originalState.originalTile;

            if (originalState.originalTile != null)
            {
                originalState.originalTile.championOnTile.Add(unit);
            }
        }

        // ���� ���� ���� �ʱ�ȭ
        opponentData.ChampionOriginState.Clear();
    }
    #endregion

    #region ������ �̵� ����
    private void MoveOpponentItemsToPlayerMap(GameObject player1, GameObject player2)
    {
        Player playerComponent1 = player1.GetComponent<Player>();
        Player playerComponent2 = player2.GetComponent<Player>();

        MapInfo playerMapInfo1 = playerComponent1.UserData.MapInfo;
        MapInfo playerMapInfo2 = playerComponent2.UserData.MapInfo;

        if (playerMapInfo1 == null || playerMapInfo2 == null)
        {
            Debug.LogWarning("�� ������ ������ �� �����ϴ�.");
            return;
        }

        // ��� �÷��̾��� ������ ����Ʈ�� �����ɴϴ�.
        List<GameObject> opponentItems = playerComponent2.UserData.UserItemObject;

        // ��� �÷��̾��� ItemTile�� �����ɴϴ�.
        ItemTile opponentItemTile = playerMapInfo2.ItemTile.FirstOrDefault(tile => tile.TileType1 == ItemOwner.Another);
        if (opponentItemTile == null)
        {
            Debug.LogWarning("��� �÷��̾��� ItemOwner.Another ������ Ÿ���� ã�� �� �����ϴ�.");
            return;
        }

        // ���� �÷��̾��� ItemTile�� �����ɴϴ�.
        ItemTile playerItemTile = playerMapInfo1.ItemTile.FirstOrDefault(tile => tile.TileType1 == ItemOwner.Another);
        if (playerItemTile == null)
        {
            Debug.LogWarning("���� �÷��̾��� ItemOwner.Another ������ Ÿ���� ã�� �� �����ϴ�.");
            return;
        }

        foreach (GameObject opponentItem in opponentItems)
        {
            // �������� ���� ���¸� �����մϴ�.
            Transform originalParent = opponentItem.transform.parent;
            int originalTileIndex = opponentItem.transform.parent.GetSiblingIndex();

            ItemOriginalState originalState = new ItemOriginalState
            {
                originalPosition = opponentItem.transform.position,
                originalParent = originalParent,
                originalItemTile = opponentItemTile,
                originalTileIndex = originalTileIndex,
                wasActive = opponentItem.activeSelf
            };

            // UserData�� ����
            playerComponent2.UserData.ItemOriginState[opponentItem] = originalState;

            // ��� ������ Ÿ�Ͽ��� ������ ����
            HexTile opponentHexTile = originalParent.GetComponent<HexTile>();
            if (opponentHexTile != null)
            {
                opponentHexTile.isItemTile = false;
                opponentHexTile.itemOnTile = null;
            }

            // ���� �÷��̾��� ������ Ÿ�Ͽ��� ������ �ε����� Ÿ���� �����ɴϴ�.
            if (playerItemTile._Items.Count > originalTileIndex)
            {
                GameObject playerTileObj = playerItemTile._Items[originalTileIndex];
                HexTile playerHexTile = playerTileObj.GetComponent<HexTile>();

                // �������� �̵���ŵ�ϴ�.
                opponentItem.transform.SetParent(playerTileObj.transform);
                opponentItem.transform.position = playerTileObj.transform.position + new Vector3(0, 0.3f, 0);

                // Ÿ���� ���� ������Ʈ
                playerHexTile.isItemTile = true;
                playerHexTile.itemOnTile = opponentItem;
            }
            else
            {
                Debug.LogWarning("���� �÷��̾��� ������ Ÿ�Ͽ� �ش� �ε����� Ÿ���� �����ϴ�.");
            }

            // �������� ��Ȱ��ȭ�Ǿ� �ִٸ� Ȱ��ȭ
            if (!opponentItem.activeSelf)
            {
                opponentItem.SetActive(true);
            }
        }
    }
    private void RestoreOpponentItems(GameObject opponent)
    {
        Player opponentComponent = opponent.GetComponent<Player>();
        UserData opponentData = opponentComponent.UserData;

        foreach (var kvp in opponentData.ItemOriginState)
        {
            GameObject item = kvp.Key;
            ItemOriginalState originalState = kvp.Value;

            // �������� ��Ȱ��ȭ�� ��� ���� ���¿� ���� Ȱ��ȭ
            if (!item.activeSelf && originalState.wasActive)
            {
                item.SetActive(true);
            }

            // ���� ������ Ÿ�Ͽ��� ������ ����
            Transform currentParent = item.transform.parent;
            HexTile currentHexTile = currentParent.GetComponent<HexTile>();
            if (currentHexTile != null)
            {
                currentHexTile.isItemTile = false;
                currentHexTile.itemOnTile = null;
            }

            // �������� ���� ��ġ�� ����
            item.transform.SetParent(originalState.originalParent);
            item.transform.position = originalState.originalPosition;

            // ���� Ÿ���� ���� ������Ʈ
            HexTile originalHexTile = originalState.originalParent.GetComponent<HexTile>();
            if (originalHexTile != null)
            {
                originalHexTile.isItemTile = true;
                originalHexTile.itemOnTile = item;
            }
        }

        // ���� ���� ���� �ʱ�ȭ
        opponentData.ItemOriginState.Clear();
    }
    #endregion
}
