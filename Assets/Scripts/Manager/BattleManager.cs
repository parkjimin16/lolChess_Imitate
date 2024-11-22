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

        Player p1 = player1.GetComponent<Player>();
        Player p2 = player2.GetComponent<Player>();

        Manager.Synergy.ApplySynergy(p1.UserData);
        Manager.Synergy.ApplySynergy(p2.UserData);

        Manager.Augmenter.ApplyStartRoundAugmenter(p1.UserData);
        Manager.Augmenter.ApplyStartRoundAugmenter(p2.UserData);

        SaveOriginalChampions(player1);
        SaveOriginalChampions(player2);

        Coroutine battleCoroutine = CoroutineHelper.StartCoroutine(BattleCoroutine(duration, player1, player2));
        battleCoroutines.Add(player1, battleCoroutine);
        battleCoroutines.Add(player2, battleCoroutine);
    }

    IEnumerator BattleCoroutine(int duration, GameObject player1, GameObject player2)
    {
        yield return new WaitForSeconds(duration);

        int player1AliveChampions = CountAliveChampions(player1);
        int player2AliveChampions = CountAliveChampions(player2);

        bool player1Won;
        int survivingEnemyUnits;

        if (player1AliveChampions > player2AliveChampions)
        {
            player1Won = true;
            survivingEnemyUnits = player1AliveChampions;
        }
        else if (player1AliveChampions < player2AliveChampions)
        {
            player1Won = false;
            survivingEnemyUnits = player2AliveChampions;
        }
        else
        {
            float player1TotalHp = GetTotalHp(player1);
            float player2TotalHp = GetTotalHp(player2);

            if (player1TotalHp >= player2TotalHp)
            {
                player1Won = true;
                survivingEnemyUnits = player1AliveChampions;
            }
            else
            {
                player1Won = false;
                survivingEnemyUnits = player2AliveChampions;
            }
        }

        EndBattle(player1, player2, player1Won, survivingEnemyUnits);
    }


    private void EndBattle(GameObject player1, GameObject player2, bool player1Won, int survivingEnemyUnits)
    {
        if (battleCoroutines.ContainsKey(player1))
        {
            battleCoroutines.Remove(player1);
        }
        if (battleCoroutines.ContainsKey(player2))
        {
            battleCoroutines.Remove(player2);
        }

        Player p1 = player1.GetComponent<Player>();
        Player p2 = player2.GetComponent<Player>();

        Manager.Synergy.UnApplySynergy(p1.UserData);
        Manager.Synergy.UnApplySynergy(p2.UserData);

        Manager.Augmenter.ApplyEndRoundAugmenter(p1.UserData);
        Manager.Augmenter.ApplyEndRoundAugmenter(p2.UserData);

        foreach (var champ in p1.UserData.BattleChampionObject)
        {
            ChampionBase cBase = champ.GetComponent<ChampionBase>();

            cBase.ChampionRotationReset();
            cBase.ChampionAttackController.EndBattle();
            cBase.ResetChampionStats();
        }

        foreach (var champ in p2.UserData.BattleChampionObject)
        {
            ChampionBase cBase = champ.GetComponent<ChampionBase>();

            cBase.ChampionRotationReset();
            cBase.ChampionAttackController.EndBattle();
            cBase.ResetChampionStats();
        }

        // Init player1
        RestoreOpponentChampions(player1);

        // Init player2
        RestoreOpponentPlayer(player2);
        RestoreOpponentChampions2(player2);
        RestoreOpponentItems(player2);

        Player playerComponent1 = player1.GetComponent<Player>();
        if (player1.GetComponent<Player>().UserData.MapInfo.goldDisplay != null)
        {
            player1.GetComponent<Player>().UserData.MapInfo.goldDisplay.SetEnemyGold(0);
        }

        SuccessiveWinLose(player1, player2, player1Won);

        if(p1.UserData == Manager.User.GetHumanUserData())
        {
            p1.UserData.UIMain.UIShopPanel.UpdateContinousBox(p1.UserData);
            p1.UserData.UIMain.UIRoundPanel.UpdateWinOrLose(Manager.Stage.currentStage, Manager.Stage.currentRound, player1Won);
        }
        else if (p2.UserData == Manager.User.GetHumanUserData())
        {
            bool player2Won = !player1Won;
            p2.UserData.UIMain.UIShopPanel.UpdateContinousBox(p2.UserData);
            p2.UserData.UIMain.UIRoundPanel.UpdateWinOrLose(Manager.Stage.currentStage, Manager.Stage.currentRound, player2Won);
        }

        Manager.Champion.OnBattleEnd(p1.UserData);
        Manager.Champion.OnBattleEnd(p2.UserData);


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

        if (playerComponent2.UserData == Manager.User.GetHumanUserData())
        {
            GameObject shop = GameObject.Find("ShopPanel");
            UIShopPanel uIShop = shop.GetComponent<UIShopPanel>();
            RectTile reverseRect = playerComponent1.UserData.MapInfo.mapTransform.GetComponent<RectTile>();
            uIShop.SetChampionPos(reverseRect.GetReversRectTileList());
            //Manager.Stage.user.ReturnUser(playerComponent1.UserData);
        }

        playerComponent1.SetBattleStageIndex(playerComponent1.UserData.UserId);
        playerComponent2.SetBattleStageIndex(playerComponent1.UserData.UserId);

        // �÷��̾�2�� �÷��̾�1�� ������ �̵���ŵ�ϴ�.
        Transform player1MapTransform = playerComponent1.UserData.MapInfo.mapTransform;
        Vector3 opponentPlayerPosition = player1MapTransform.position + new Vector3(13.5f, 0.8f, 5f);

        if(playerComponent2.UserData.PlayerType == PlayerType.Player1)
        {
            Manager.Cam.MoveCameraToPlayer(playerComponent1);
        }

        player2.transform.position = opponentPlayerPosition;
        player2.transform.LookAt(player1.transform.position);

        if (playerComponent1.UserData.MapInfo.goldDisplay != null)
        {
            playerComponent1.UserData.MapInfo.goldDisplay.SetEnemyGold(playerComponent2.UserData.UserGold);
        }
    }
    private void RestoreOpponentPlayer(GameObject opponent)
    {
        Player opponentComponent = opponent.GetComponent<Player>();

        if(opponentComponent.UserData == Manager.User.GetHumanUserData())
        {
            GameObject shop = GameObject.Find("ShopPanel");
            UIShopPanel uIShop = shop.GetComponent<UIShopPanel>();
            uIShop.InitChampionPos();
        }

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
                opponentChampion.transform.rotation = Quaternion.Euler(0, 180, 0);

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
                opponentChampion.transform.position = mirroredTile.transform.position;
                opponentChampion.transform.SetParent(mirroredTile.transform);
                opponentChampion.transform.rotation = Quaternion.Euler(0, 180, 0);

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
        if (opponentTile.y == -1)
        {
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
        else if (opponentTile.y == 8)
        {
            int mirroredX = 8 - x;
            int mirroredY = -1;
            Debug.Log($"{mirroredX}, {mirroredY}");
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
        else { return null; }
    }
    private void RestoreOpponentChampions(GameObject opponent)
    {
        Player opponentComponent = opponent.GetComponent<Player>();
        UserData opponentData = opponentComponent.UserData;

        foreach (var champ in opponentData.TotalChampionObject)
        {
            champ.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        foreach (var kvp in opponentData.ChampionOriginState.ToList()) // ToList()�� �÷��� ���� ����
        {
            GameObject champion = kvp.Key;
            ChampionOriginalState originalState = kvp.Value;

            ChampionBase cBase = champion.GetComponent<ChampionBase>();
            cBase.ChampionAttackController.EndBattle();
            cBase.ResetChampionStats();
            cBase.ChampionStateController.ChangeState(ChampionState.Idle, cBase);
            cBase.ChampionRotationReset();

            // è�Ǿ��� �̵��� ���� Ȯ��
            if (originalState.originalMapInfo != opponentData.MapInfo)
            {
                // è�Ǿ��� ���� �ʰ� �ٸ� �ʿ� ���� ��� �̵�
                MoveChampionToOriginalMap(champion, originalState);
            }
            else
            {
                // è�Ǿ��� ���� �ʿ� �ִ� ��� ��ġ ����
                MoveChampionToOriginalPosition(champion, originalState);
            }
        }

        // ���� ���� ���� �ʱ�ȭ
        opponentData.ChampionOriginState.Clear();
    }

    private void RestoreOpponentChampions2(GameObject opponent)
    {
        Player opponentComponent = opponent.GetComponent<Player>();
        UserData opponentData = opponentComponent.UserData;

        foreach (var champ in opponentData.TotalChampionObject)
        {
            champ.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        foreach (var kvp in opponentData.ChampionOriginState.ToList()) // ToList()�� �÷��� ���� ����
        {
            GameObject champion = kvp.Key;
            ChampionOriginalState originalState = kvp.Value;
            HexTile opponentTile = champion.GetComponentInParent<HexTile>();

            if(opponentTile == null)
            {
                Debug.Log("���̴�");
            }

            ChampionBase cBase = champion.GetComponent<ChampionBase>();
            cBase.ChampionAttackController.EndBattle();
            cBase.ResetChampionStats();
            cBase.ChampionStateController.ChangeState(ChampionState.Idle, cBase);
            cBase.ChampionRotationReset();

            // è�Ǿ��� �̵��� ���� Ȯ��
            if (originalState.originalMapInfo != opponentData.MapInfo)
            {
                // è�Ǿ��� ���� �ʰ� �ٸ� �ʿ� ���� ���, GetMirroredRectTile ����
                HexTile mirroredTile = GetMirroredRectTile(opponentTile, originalState.originalMapInfo);
                if (mirroredTile != null)
                {
                    // è�Ǿ��� ���� Ÿ�Ͽ��� ����
                    HexTile currentTile = champion.GetComponentInParent<HexTile>();
                    if (currentTile != null)
                    {
                        currentTile.championOnTile.Remove(champion);
                    }

                    // è�Ǿ��� ��ġ�� �θ� ������ Ÿ�Ϸ� ����
                    champion.transform.position = mirroredTile.transform.position;
                    champion.transform.SetParent(mirroredTile.transform);

                    // Ÿ�� ���� ������Ʈ
                    mirroredTile.championOnTile.Add(champion);
                }
            }
            else
            {
                // è�Ǿ��� ���� �ʿ� �ִ� ��� ��ġ ����
                MoveChampionToOriginalPosition(champion, originalState);
            }
        }

        // ���� ���� ���� �ʱ�ȭ
        opponentData.ChampionOriginState.Clear();
    }


    private void MoveChampionToOriginalMap(GameObject champion, ChampionOriginalState originalState)
    {
        // ���� ������ è�Ǿ� �̵�
        MapGenerator.MapInfo originalMap = originalState.originalMapInfo;
        HexTile originalTile = originalState.originalTile;

        // ���� Ÿ�Ͽ��� è�Ǿ� ����
        HexTile currentTile = champion.GetComponentInParent<HexTile>();
        if (currentTile != null)
        {
            currentTile.championOnTile.Remove(champion);
        }

        // è�Ǿ��� ��ġ�� �θ� ���� Ÿ�Ϸ� ����
        champion.transform.position = originalState.originalPosition;
        champion.transform.SetParent(originalTile.transform);

        // Ÿ�� ���� ������Ʈ
        originalTile.championOnTile.Add(champion);
    }

    private void MoveChampionToOriginalPosition(GameObject champion, ChampionOriginalState originalState)
    {
        // è�Ǿ��� ��ġ�� �θ� ���� ��ġ�� ����
        champion.transform.position = originalState.originalPosition;
        champion.transform.SetParent(originalState.originalParent);

        // Ÿ�� ���� ������Ʈ
        if (originalState.originalTile != null)
        {
            originalState.originalTile.championOnTile.Add(champion);
        }
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

    private int CountAliveChampions(GameObject player)
    {
        Player playerComponent = player.GetComponent<Player>();
        int aliveCount = 0;

        foreach (GameObject championObj in playerComponent.UserData.BattleChampionObject)
        {
            if (championObj.activeSelf)
            {
                ChampionHpMpController hpMpController = championObj.GetComponent<ChampionHpMpController>();
                if (hpMpController != null && !hpMpController.IsDie())
                {
                    aliveCount++;
                }
            }
        }

        return aliveCount;
    }
    private float GetTotalHp(GameObject player)
    {
        Player playerComponent = player.GetComponent<Player>();
        float totalHp = 0f;

        foreach (GameObject championObj in playerComponent.UserData.BattleChampionObject)
        {
            if (championObj.activeSelf)
            {
                ChampionBase cBase = championObj.GetComponent<ChampionBase>();
                ChampionHpMpController hpMpController = championObj.GetComponent<ChampionHpMpController>();
                if (hpMpController != null && !hpMpController.IsDie())
                {
                    totalHp += cBase.Champion_CurHp;
                }
            }
        }

        return totalHp;
    }
}
