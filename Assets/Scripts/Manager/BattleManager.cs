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
        //Debug.Log($"전투가 시작되었습니다. {player1.GetComponent<Player>().UserData.UserName} vs {player2.GetComponent<Player>().UserData.UserName}, 전투 시간: {duration}초");

        yield return new WaitForSeconds(duration);

        // 예시로 랜덤하게 승패를 결정
        bool player1Won = Random.value > 0.5f;
        int survivingEnemyUnits = player1Won ? 0 : Random.Range(1, 5);

        //Debug.Log(player1Won ? $"{player1.name} 승리" : $"{player1.name} 패배");

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

        // 상대 챔피언, 아이템을 원래 위치로 복귀
        RestoreOpponentPlayer(player2);
        RestoreOpponentChampions(player2);
        RestoreOpponentItems(player2);

        // StageManager에 전투 종료를 알림
        Debug.Log(player1Won ? $"{player1.GetComponent<Player>().UserData.UserName} 승리" : $"{player2.GetComponent<Player>().UserData.UserName} 승리");
        Manager.Stage.OnBattleEnd(player1, player2, player1Won, survivingEnemyUnits);

        MergeScene.BatteStart = false;
    }

    public void MovePlayer(GameObject player1, GameObject player2)
    {
        // 상대 플레이어의 챔피언, 아이템을 player1의 맵으로 이동
        MoveOpponentPlayerToPlayerMap(player1, player2);
        MoveOpponentChampionsToPlayerMap(player1, player2);
        MoveOpponentItemsToPlayerMap(player1, player2);
    }


    #region 플레이어 이동로직
    private void MoveOpponentPlayerToPlayerMap(GameObject player1, GameObject player2)
    {
        Player playerComponent1 = player1.GetComponent<Player>();
        Player playerComponent2 = player2.GetComponent<Player>();


        playerComponent1.SetBattleStageIndex(playerComponent1.UserData.UserId);
        playerComponent2.SetBattleStageIndex(playerComponent1.UserData.UserId);

        // 플레이어2를 플레이어1의 맵으로 이동시킵니다.
        Transform player1MapTransform = playerComponent1.UserData.MapInfo.mapTransform;
        Vector3 opponentPlayerPosition = player1MapTransform.position + new Vector3(13.5f, 0.8f, 5f);

        player2.transform.position = opponentPlayerPosition;

        // 필요에 따라 플레이어의 회전을 조정합니다.
        player2.transform.LookAt(player1.transform.position);
    }
    private void RestoreOpponentPlayer(GameObject opponent)
    {
        Player opponentComponent = opponent.GetComponent<Player>();

        if (opponentComponent != null)
        {
            // 플레이어를 원래 위치로 복귀
            opponent.transform.position = opponentComponent.UserData.MapInfo.mapTransform.position + new Vector3(-13.5f, 0.8f, -5f);
        }
    }
    #endregion

    #region 챔피언 이동 로직
    private void MoveOpponentChampionsToPlayerMap(GameObject player1, GameObject player2)
    {
        Player playerComponent1 = player1.GetComponent<Player>();
        Player playerComponent2 = player2.GetComponent<Player>();

        MapInfo playerMapInfo1 = playerComponent1.UserData.MapInfo;
        MapInfo playerMapInfo2 = playerComponent2.UserData.MapInfo;

        if (playerMapInfo1 == null || playerMapInfo2 == null)
        {
            Debug.LogWarning("맵 정보를 가져올 수 없습니다.");
            return;
        }

        // 상대 플레이어의 유닛 리스트를 가져옵니다.
        List<GameObject> opponentUnits = playerComponent2.UserData.BattleChampionObject;

        foreach (GameObject opponentUnit in opponentUnits)
        {
            // 유닛의 원래 상태를 저장합니다.
            ChampionOriginalState originalState = new ChampionOriginalState
            {
                originalPosition = opponentUnit.transform.position,
                originalParent = opponentUnit.transform.parent,
                originalTile = opponentUnit.GetComponentInParent<HexTile>(),
                wasActive = opponentUnit.activeSelf
            };


            // UserData에 저장
            playerComponent2.UserData.ChampionOriginState[opponentUnit] = originalState;

            // 좌표 반전하여 현재 플레이어의 맵에서 대응되는 타일을 찾습니다.
            HexTile opponentTile = opponentUnit.GetComponentInParent<HexTile>();
            HexTile mirroredTile = GetMirroredTile(opponentTile, playerMapInfo1);

            if (mirroredTile != null)
            {
                opponentTile.championOnTile.Remove(opponentUnit);

                // 유닛을 이동시킵니다.
                opponentUnit.transform.position = mirroredTile.transform.position + new Vector3(0, 0.5f, 0);
                opponentUnit.transform.SetParent(mirroredTile.transform);

                // 타일 정보 업데이트
                mirroredTile.championOnTile.Add(opponentUnit);
            }

            // 유닛이 비활성화되어 있다면 활성화
            if (!opponentUnit.activeSelf)
            {
                opponentUnit.SetActive(true);
            }
        }

        List<GameObject> opponentNonBattleUnits = playerComponent2.UserData.NonBattleChampionObject;

        foreach (GameObject opponentUnit in opponentNonBattleUnits)
        {
            // 유닛의 원래 상태를 저장합니다.
            ChampionOriginalState originalState = new ChampionOriginalState
            {
                originalPosition = opponentUnit.transform.position,
                originalParent = opponentUnit.transform.parent,
                originalTile = opponentUnit.GetComponentInParent<HexTile>(),
                wasActive = opponentUnit.activeSelf
            };

            // UserData에 저장
            playerComponent2.UserData.ChampionOriginState[opponentUnit] = originalState;

            // 유닛이 위치한 타일 가져오기
            HexTile opponentTile = opponentUnit.GetComponentInParent<HexTile>();
            HexTile mirroredTile = GetMirroredRectTile(opponentTile, playerMapInfo1);

            // 현재 플레이어의 맵에서 반전된 좌표에 해당하는 타일을 찾습니다.
            if (mirroredTile != null)
            {
                // 상대 타일에서 유닛 제거
                opponentTile.championOnTile.Remove(opponentUnit);

                // 유닛을 이동시킵니다.
                opponentUnit.transform.position = mirroredTile.transform.position + new Vector3(0, 0.5f, 0);
                opponentUnit.transform.SetParent(mirroredTile.transform);

                // 타일 정보 업데이트
                mirroredTile.championOnTile.Add(opponentUnit);
            }

            // 유닛이 비활성화되어 있다면 활성화
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

        // 좌표를 반전합니다.
        int mirroredQ = 6 - q;
        int mirroredR = 7 - r;

        // 현재 플레이어의 맵에서 반전된 좌표에 해당하는 타일을 찾습니다.
        if (playerMapInfo.HexDictionary.TryGetValue((mirroredQ, mirroredR), out HexTile mirroredTile))
        {
            return mirroredTile;
        }
        else
        {
            Debug.LogWarning($"반전된 좌표 ({mirroredQ}, {mirroredR})에 해당하는 타일을 찾을 수 없습니다.");
            return null;
        }
    }

    private HexTile GetMirroredRectTile(HexTile opponentTile, MapInfo playerMapInfo)
    {
        int x = opponentTile.x;
        //int y = opponentTile.y;

        // 좌표를 반전합니다.
        int mirroredX = 8 - x;
        int mirroredY = 8;
        if (playerMapInfo.RectDictionary.TryGetValue((mirroredX, mirroredY), out HexTile mirroredTile))
        {
            return mirroredTile;
        }
        else
        {
            Debug.LogWarning($"반전된 좌표 ({mirroredX}, {mirroredY})에 해당하는 타일을 찾을 수 없습니다.");
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

            // 유닛이 사망하여 비활성화된 경우 활성화
            if (!unit.activeSelf && originalState.wasActive)
            {
                unit.SetActive(true);
            }

            // 현재 타일에서 유닛 제거
            HexTile currentTile = unit.GetComponentInParent<HexTile>();

            if (currentTile != null)
            {
                currentTile.championOnTile.Remove(unit);
            }

            // 원래 타일로 복귀
            unit.transform.position = originalState.originalPosition;
            unit.transform.SetParent(originalState.originalParent);

            // 타일 정보 업데이트
            currentTile = originalState.originalTile;

            if (originalState.originalTile != null)
            {
                originalState.originalTile.championOnTile.Add(unit);
            }
        }

        // 원래 상태 정보 초기화
        opponentData.ChampionOriginState.Clear();
    }
    #endregion

    #region 아이템 이동 로직
    private void MoveOpponentItemsToPlayerMap(GameObject player1, GameObject player2)
    {
        Player playerComponent1 = player1.GetComponent<Player>();
        Player playerComponent2 = player2.GetComponent<Player>();

        MapInfo playerMapInfo1 = playerComponent1.UserData.MapInfo;
        MapInfo playerMapInfo2 = playerComponent2.UserData.MapInfo;

        if (playerMapInfo1 == null || playerMapInfo2 == null)
        {
            Debug.LogWarning("맵 정보를 가져올 수 없습니다.");
            return;
        }

        // 상대 플레이어의 아이템 리스트를 가져옵니다.
        List<GameObject> opponentItems = playerComponent2.UserData.UserItemObject;

        // 상대 플레이어의 ItemTile을 가져옵니다.
        ItemTile opponentItemTile = playerMapInfo2.ItemTile.FirstOrDefault(tile => tile.TileType1 == ItemOwner.Another);
        if (opponentItemTile == null)
        {
            Debug.LogWarning("상대 플레이어의 ItemOwner.Another 아이템 타일을 찾을 수 없습니다.");
            return;
        }

        // 현재 플레이어의 ItemTile을 가져옵니다.
        ItemTile playerItemTile = playerMapInfo1.ItemTile.FirstOrDefault(tile => tile.TileType1 == ItemOwner.Another);
        if (playerItemTile == null)
        {
            Debug.LogWarning("현재 플레이어의 ItemOwner.Another 아이템 타일을 찾을 수 없습니다.");
            return;
        }

        foreach (GameObject opponentItem in opponentItems)
        {
            // 아이템의 원래 상태를 저장합니다.
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

            // UserData에 저장
            playerComponent2.UserData.ItemOriginState[opponentItem] = originalState;

            // 상대 아이템 타일에서 아이템 제거
            HexTile opponentHexTile = originalParent.GetComponent<HexTile>();
            if (opponentHexTile != null)
            {
                opponentHexTile.isItemTile = false;
                opponentHexTile.itemOnTile = null;
            }

            // 현재 플레이어의 아이템 타일에서 동일한 인덱스의 타일을 가져옵니다.
            if (playerItemTile._Items.Count > originalTileIndex)
            {
                GameObject playerTileObj = playerItemTile._Items[originalTileIndex];
                HexTile playerHexTile = playerTileObj.GetComponent<HexTile>();

                // 아이템을 이동시킵니다.
                opponentItem.transform.SetParent(playerTileObj.transform);
                opponentItem.transform.position = playerTileObj.transform.position + new Vector3(0, 0.3f, 0);

                // 타일의 상태 업데이트
                playerHexTile.isItemTile = true;
                playerHexTile.itemOnTile = opponentItem;
            }
            else
            {
                Debug.LogWarning("현재 플레이어의 아이템 타일에 해당 인덱스의 타일이 없습니다.");
            }

            // 아이템이 비활성화되어 있다면 활성화
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

            // 아이템이 비활성화된 경우 원래 상태에 따라 활성화
            if (!item.activeSelf && originalState.wasActive)
            {
                item.SetActive(true);
            }

            // 현재 아이템 타일에서 아이템 제거
            Transform currentParent = item.transform.parent;
            HexTile currentHexTile = currentParent.GetComponent<HexTile>();
            if (currentHexTile != null)
            {
                currentHexTile.isItemTile = false;
                currentHexTile.itemOnTile = null;
            }

            // 아이템을 원래 위치로 복귀
            item.transform.SetParent(originalState.originalParent);
            item.transform.position = originalState.originalPosition;

            // 원래 타일의 상태 업데이트
            HexTile originalHexTile = originalState.originalParent.GetComponent<HexTile>();
            if (originalHexTile != null)
            {
                originalHexTile.isItemTile = true;
                originalHexTile.itemOnTile = item;
            }
        }

        // 원래 상태 정보 초기화
        opponentData.ItemOriginState.Clear();
    }
    #endregion
}
