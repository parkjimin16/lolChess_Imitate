using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameDataBlueprint", menuName = "Blueprints/GameDataBlueprint")]
public class GameDataBlueprint : ScriptableObject
{
    [Header("Champion Data")]
    [SerializeField] private List<ChampionData> championDataList;
    [SerializeField] private List<ChampionRandomData> championRandomDataList;
    [SerializeField] private List<ChampionMaxCount> championMaxCount;

    [Header("UserData")]
    [SerializeField] private List<PlayerData> userData;

    [Header("MapData")]
    [SerializeField] private GameObject hexTilePrefab;
    [SerializeField] private GameObject rectTilePrefab; // ���簢�� Ÿ�� ������
    [SerializeField] private GameObject itemTilePrefab; // ������ Ÿ�� ������
    [SerializeField] private GameObject goldTilePrefeb; // ��� Ÿ�� ������
    [SerializeField] private GameObject goldPrefeb; // ��� ǥ�� ������
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int rectWidth; // ���簢�� Ÿ���� ����
    [SerializeField] private float desiredMapWidth; // ���ϴ� ���� ���� ũ�� (����: ����Ƽ ���� ��ǥ)
    [SerializeField] private GameObject championPrefeb;
    [SerializeField] private GameObject playerPrefeb;
    public List<ChampionData> ChampionDataList => championDataList;
    public List<ChampionRandomData> ChampionRandomDataList => championRandomDataList;
    public List<ChampionMaxCount> ChampionMaxCount => championMaxCount;
    public List<PlayerData> UserData => userData;
    public GameObject HexTilePrefab => hexTilePrefab;
    public GameObject RectTilePrefab => rectTilePrefab;
    public GameObject ItemTilePrefab => itemTilePrefab;
    public GameObject GoldTilePrefab => goldTilePrefeb;
    public GameObject GoldPrefab => goldPrefeb;
    public int Width => width;
    public int Height => height;
    public int RectWidth => rectWidth;
    public float DesiredMapWidth => desiredMapWidth;
    public GameObject ChampionPrefeb => championPrefeb;
    public GameObject PlayerPrefeb => playerPrefeb;
}

