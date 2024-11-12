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
        // �̹� ���� ���� �÷��̾ ������ �ǳʶݴϴ�.
        if (battleCoroutines.ContainsKey(player1))
            return;

        // ��� �÷��̾��� ������ player1�� ������ �̵�
        MoveOpponentUnitsToPlayerMap(player1, player2);

        Coroutine battleCoroutine = CoroutineHelper.StartCoroutine(BattleCoroutine(duration, player1, player2));
        battleCoroutines.Add(player1, battleCoroutine);
        battleCoroutines.Add(player2, battleCoroutine); // player2�� �߰��Ͽ� ����
    }

    IEnumerator BattleCoroutine(int duration, GameObject player1, GameObject player2)
    {
        //Debug.Log($"������ ���۵Ǿ����ϴ�. {player1.GetComponent<Player>().UserData.UserName} vs {player2.GetComponent<Player>().UserData.UserName}, ���� �ð�: {duration}��");

        // ���� ���� (���� �ð���ŭ ���)
        yield return new WaitForSeconds(duration);

        // ���÷� �����ϰ� ���и� ����
        bool player1Won = Random.value > 0.5f;
        int survivingEnemyUnits = player1Won ? 0 : Random.Range(1, 5); // �й� �� 1~4���� �� ������ ����

        //Debug.Log(player1Won ? $"{player1.name} �¸�" : $"{player1.name} �й�");

        // ���� ���� ó��
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

            // Ÿ���� ��ǥ ��������
            int x = opponentTile.x;
            //int y = opponentTile.y;

            // ��ǥ�� �����մϴ�.
            int mirroredX = 8 - x;
            int mirroredY = 8;

            // ���� �÷��̾��� �ʿ��� ������ ��ǥ�� �ش��ϴ� Ÿ���� ã���ϴ�.
            if (playerMapInfo1.RectDictionary.TryGetValue((mirroredX, mirroredY), out HexTile mirroredTile))
            {
                // ��� Ÿ�Ͽ��� ���� ����
                opponentTile.championOnTile.Remove(opponentUnit);

                // ������ �̵���ŵ�ϴ�.
                opponentUnit.transform.position = mirroredTile.transform.position + new Vector3(0, 0.5f, 0);
                opponentUnit.transform.SetParent(mirroredTile.transform);

                // Ÿ�� ���� ������Ʈ
                mirroredTile.championOnTile.Add(opponentUnit);
            }
            else
            {
                Debug.LogWarning($"������ ��ǥ ({mirroredX}, {mirroredY})�� �ش��ϴ� Ÿ���� ã�� �� �����ϴ�.");
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

    void EndBattle(GameObject player1, GameObject player2, bool player1Won, int survivingEnemyUnits)
    {
        // ���� �ڷ�ƾ ����
        if (battleCoroutines.ContainsKey(player1))
        {
            battleCoroutines.Remove(player1);
        }
        if (battleCoroutines.ContainsKey(player2))
        {
            battleCoroutines.Remove(player2);
        }

        // ��� ������ ���� ��ġ�� ����
        RestoreOpponentUnits(player2);

        // StageManager�� ���� ���Ḧ �˸�
        Debug.Log(player1Won ? $"{player1.GetComponent<Player>().UserData.UserName} �¸�" : $"{player2.GetComponent<Player>().UserData.UserName} �¸�");
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
}
