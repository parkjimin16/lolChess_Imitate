using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BattleManager
{
    private Dictionary<GameObject, Coroutine> battleCoroutines = new Dictionary<GameObject, Coroutine>();

    public void StartBattle(GameObject player1, GameObject player2, int duration)
    {
        // 이미 전투 중인 플레이어가 있으면 건너뜁니다.
        if (battleCoroutines.ContainsKey(player1))
            return;

        // 상대 플레이어의 유닛을 player1의 맵으로 이동
        MoveOpponentUnitsToPlayerMap(player1, player2);

        Coroutine battleCoroutine = CoroutineHelper.StartCoroutine(BattleCoroutine(duration, player1, player2));
        battleCoroutines.Add(player1, battleCoroutine);
        battleCoroutines.Add(player2, battleCoroutine); // player2도 추가하여 추적
    }

    IEnumerator BattleCoroutine(int duration, GameObject player1, GameObject player2)
    {
        //Debug.Log($"전투가 시작되었습니다. {player1.GetComponent<Player>().UserData.UserName} vs {player2.GetComponent<Player>().UserData.UserName}, 전투 시간: {duration}초");

        // 전투 진행 (전투 시간만큼 대기)
        yield return new WaitForSeconds(duration);

        // 예시로 랜덤하게 승패를 결정
        bool player1Won = Random.value > 0.5f;
        int survivingEnemyUnits = player1Won ? 0 : Random.Range(1, 5); // 패배 시 1~4개의 적 유닛이 생존

        //Debug.Log(player1Won ? $"{player1.name} 승리" : $"{player1.name} 패배");

        // 전투 종료 처리
        EndBattle(player1, player2, player1Won, survivingEnemyUnits);
    }

    private void MoveOpponentUnitsToPlayerMap(GameObject player1, GameObject player2)
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

            // 타일의 좌표 가져오기
            int x = opponentTile.x;
            //int y = opponentTile.y;

            // 좌표를 반전합니다.
            int mirroredX = 8 - x;
            int mirroredY = 8;

            // 현재 플레이어의 맵에서 반전된 좌표에 해당하는 타일을 찾습니다.
            if (playerMapInfo1.RectDictionary.TryGetValue((mirroredX, mirroredY), out HexTile mirroredTile))
            {
                // 상대 타일에서 유닛 제거
                opponentTile.championOnTile.Remove(opponentUnit);

                // 유닛을 이동시킵니다.
                opponentUnit.transform.position = mirroredTile.transform.position + new Vector3(0, 0.5f, 0);
                opponentUnit.transform.SetParent(mirroredTile.transform);

                // 타일 정보 업데이트
                mirroredTile.championOnTile.Add(opponentUnit);
            }
            else
            {
                Debug.LogWarning($"반전된 좌표 ({mirroredX}, {mirroredY})에 해당하는 타일을 찾을 수 없습니다.");
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

    void EndBattle(GameObject player1, GameObject player2, bool player1Won, int survivingEnemyUnits)
    {
        // 전투 코루틴 제거
        if (battleCoroutines.ContainsKey(player1))
        {
            battleCoroutines.Remove(player1);
        }
        if (battleCoroutines.ContainsKey(player2))
        {
            battleCoroutines.Remove(player2);
        }

        // 상대 유닛을 원래 위치로 복귀
        RestoreOpponentUnits(player2);

        // StageManager에 전투 종료를 알림
        Debug.Log(player1Won ? $"{player1.GetComponent<Player>().UserData.UserName} 승리" : $"{player2.GetComponent<Player>().UserData.UserName} 승리");
        Manager.Stage.OnBattleEnd(player1, player2, player1Won, survivingEnemyUnits);
    }
    private void RestoreOpponentUnits(GameObject opponent)
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
}
