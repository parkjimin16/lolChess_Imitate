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

        SaveOriginalChampions(player1);
        SaveOriginalChampions(player2);

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

        RestoreOpponentChampions(player1);

        // ��� è�Ǿ�, �������� ���� ��ġ�� ����
        RestoreOpponentPlayer(player2);
        RestoreOpponentChampions(player2);
        RestoreOpponentItems(player2);

        SuccessiveWinLose(player1, player2, player1Won);

        // StageManager�� ���� ���Ḧ �˸�
        Debug.Log(player1Won ? $"{player1.GetComponent<Player>().UserData.UserName} �¸�" : $"{player2.GetComponent<Player>().UserData.UserName} �¸�");
        Manager.Stage.OnBattleEnd(player1, player2, player1Won, survivingEnemyUnits);
        //Manager.Stage.DistributeGoldToPlayers();
        MergeScene.BatteStart = false;
    }

    public void MovePlayer(GameObject player1, GameObject player2)
    {
        // ��� �÷��̾��� è�Ǿ�, �������� player1�� ������ �̵�
        MoveOpponentPlayerToPlayerMap(player1, player2);
        MoveOpponentChampionsToPlayerMap(player1, player2);
        MoveOpponentItemsToPlayerMap(player1, player2);
    }

    private void SuccessiveWinLose(GameObject player1, GameObject player2, bool player1Won)
    {
        Player playerComponent1 = player1.GetComponent<Player>();
        Player playerComponent2 = player2.GetComponent<Player>();
        if (player1Won)
        {
            // �÷��̾�1 �¸�
            if (playerComponent1.UserData.UserSuccessiveWin > 0)
                playerComponent1.UserData.UserSuccessiveWin++;
            else
                playerComponent1.UserData.UserSuccessiveWin = 1;
            playerComponent1.UserData.UserSuccessiveLose = 0;

            if (playerComponent2.UserData.UserSuccessiveLose > 0)
                playerComponent2.UserData.UserSuccessiveLose++;
            else
                playerComponent2.UserData.UserSuccessiveLose = 1;
            playerComponent2.UserData.UserSuccessiveWin = 0;
        }
        else
        {
            // �÷��̾�2 �¸�
            if (playerComponent2.UserData.UserSuccessiveWin > 0)
                playerComponent2.UserData.UserSuccessiveWin++;
            else
                playerComponent2.UserData.UserSuccessiveWin = 1;
            playerComponent2.UserData.UserSuccessiveLose = 0;

            if (playerComponent1.UserData.UserSuccessiveLose > 0)
                playerComponent1.UserData.UserSuccessiveLose++;
            else
                playerComponent1.UserData.UserSuccessiveLose = 1;
            playerComponent1.UserData.UserSuccessiveWin = 0;
        }
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

        if(playerComponent2.UserData.PlayerType == PlayerType.Player1)
        {
            //Debug.Log("�־ȵ�");
            CameraManager.Instance.MoveCameraToPlayer(playerComponent1);
        }

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
    private void SaveOriginalChampions(GameObject player)
    {
        Player playerComponent = player.GetComponent<Player>();
        UserData userData = playerComponent.UserData;

        foreach (GameObject champion in userData.BattleChampionObject)
        {
            if (!userData.ChampionOriginState.ContainsKey(champion))
            {
                ChampionOriginalState originalState = new ChampionOriginalState
                {
                    originalPosition = champion.transform.position,
                    originalParent = champion.transform.parent,
                    originalTile = champion.GetComponentInParent<HexTile>(),
                    wasActive = champion.activeSelf
                };

                userData.ChampionOriginState[champion] = originalState;
            }
        }
    }

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
        List<GameObject> opponentChampions = playerComponent2.UserData.BattleChampionObject;

        foreach (GameObject opponentChampion in opponentChampions)
        {
            // ������ ���� ���¸� �����մϴ�.
            ChampionOriginalState originalState = new ChampionOriginalState
            {
                originalPosition = opponentChampion.transform.position,
                originalParent = opponentChampion.transform.parent,
                originalTile = opponentChampion.GetComponentInParent<HexTile>(),
                wasActive = opponentChampion.activeSelf
            };


            // UserData�� ����
            playerComponent2.UserData.ChampionOriginState[opponentChampion] = originalState;

            // ��ǥ �����Ͽ� ���� �÷��̾��� �ʿ��� �����Ǵ� Ÿ���� ã���ϴ�.
            HexTile opponentTile = opponentChampion.GetComponentInParent<HexTile>();
            HexTile mirroredTile = GetMirroredTile(opponentTile, playerMapInfo1);

            if (mirroredTile != null)
            {
                opponentTile.championOnTile.Remove(opponentChampion);

                // ������ �̵���ŵ�ϴ�.
                opponentChampion.transform.position = mirroredTile.transform.position + new Vector3(0, 0.5f, 0);
                opponentChampion.transform.SetParent(mirroredTile.transform);

                // Ÿ�� ���� ������Ʈ
                mirroredTile.championOnTile.Add(opponentChampion);
            }

            // ������ ��Ȱ��ȭ�Ǿ� �ִٸ� Ȱ��ȭ
            if (!opponentChampion.activeSelf)
            {
                opponentChampion.SetActive(true);
            }
        }

        List<GameObject> opponentNonBattleChampions = playerComponent2.UserData.NonBattleChampionObject;

        foreach (GameObject opponentChampion in opponentNonBattleChampions)
        {
            // ������ ���� ���¸� �����մϴ�.
            ChampionOriginalState originalState = new ChampionOriginalState
            {
                originalPosition = opponentChampion.transform.position,
                originalParent = opponentChampion.transform.parent,
                originalTile = opponentChampion.GetComponentInParent<HexTile>(),
                wasActive = opponentChampion.activeSelf
            };

            // UserData�� ����
            playerComponent2.UserData.ChampionOriginState[opponentChampion] = originalState;

            // ������ ��ġ�� Ÿ�� ��������
            HexTile opponentTile = opponentChampion.GetComponentInParent<HexTile>();
            HexTile mirroredTile = GetMirroredRectTile(opponentTile, playerMapInfo1);

            // ���� �÷��̾��� �ʿ��� ������ ��ǥ�� �ش��ϴ� Ÿ���� ã���ϴ�.
            if (mirroredTile != null)
            {
                // ��� Ÿ�Ͽ��� ���� ����
                opponentTile.championOnTile.Remove(opponentChampion);

                // ������ �̵���ŵ�ϴ�.
                opponentChampion.transform.position = mirroredTile.transform.position + new Vector3(0, 0.5f, 0);
                opponentChampion.transform.SetParent(mirroredTile.transform);

                // Ÿ�� ���� ������Ʈ
                mirroredTile.championOnTile.Add(opponentChampion);
            }

            // ������ ��Ȱ��ȭ�Ǿ� �ִٸ� Ȱ��ȭ
            if (!opponentChampion.activeSelf)
            {
                opponentChampion.SetActive(true);
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
            GameObject champion = kvp.Key;
            ChampionOriginalState originalState = kvp.Value;

            // ������ ����Ͽ� ��Ȱ��ȭ�� ��� Ȱ��ȭ
            if (!champion.activeSelf && originalState.wasActive)
            {
                champion.SetActive(true);
            }

            // ���� Ÿ�Ͽ��� ���� ����
            HexTile currentTile = champion.GetComponentInParent<HexTile>();

            if (currentTile != null)
            {
                currentTile.championOnTile.Remove(champion);
            }

            // ���� Ÿ�Ϸ� ����
            champion.transform.position = originalState.originalPosition;
            champion.transform.SetParent(originalState.originalParent);

            // Ÿ�� ���� ������Ʈ
            currentTile = originalState.originalTile;

            if (originalState.originalTile != null)
            {
                originalState.originalTile.championOnTile.Add(champion);
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
