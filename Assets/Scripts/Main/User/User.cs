using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static MapGenerator;

public class User : MonoBehaviour
{
    [SerializeField] private UISceneMain uiMain;
    [SerializeField] private GameObject _movableObj;
    [SerializeField] private GameObject currentTile;
    [SerializeField] private MapGenerator _mapGenerator;
    [SerializeField] private GameObject test;

    private Vector2 _lastTouchPos = Vector2.zero;
    private Vector2 _currentTouchPos = Vector2.zero;
    private Vector3 _beforePosition;
    private Vector3 _offset;

    private bool _isReturning = false;
    private bool _objectMoved = false;
    private bool _isDragging = false;
    private bool _previousIsBattleOngoing = false;

    private MovableObjectType _movableObjectType;
    private ReturningObjectData _returningObjData;
    private ItemFrame _hoveredItem;

    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;

    private HexTile _previousHoveredTile = null;
    private UserData player1;
    private class ReturningObjectData
    {
        public GameObject obj;
        public Vector3 beforePosition;
        public GameObject currentTile;
        public MovableObjectType movableObjectType;
    }

    private void Start()
    {
        raycaster = uiMain.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
    }

    private void Update()
    {
        HandleBattleState();
        UpdateTouchPositions();
        HandleMouseInput();
        HandleRightClick();
        ObjectReturn();
        HandleItemHover();
        ActiveHoverTile();
        CheckChampionDrag();
    }

    #region Mouse Input Handling

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Manager.UI.CheckPopupStack())
                Manager.UI.CloseAllPopupUIExcept("UIPopupAugmenter");

            TouchBeganEvent();
            FindCurrentTile();
        }

        if (Input.GetMouseButton(0))
        {
            if (Input.mousePosition != _beforePosition)
                TouchMovedEvent();
        }

        if (Input.GetMouseButtonUp(0))
        {
            TouchEndedEvent();
            InActiveTile();
        }
    }

    private void TouchBeganEvent()
    {
        if (_isReturning) return;

        _objectMoved = false;
        _movableObj = GetObjectUnderMouse("Item") ?? GetObjectUnderMouse("Champion");

        if (_movableObj != null)
        {
            SetMovableObjectType(_movableObj.CompareTag("Item") ? MovableObjectType.Item : MovableObjectType.Champion);
            currentTile = _movableObj.transform.parent?.gameObject;
            _beforePosition = _movableObj.transform.position;

            if (currentTile != null)
            {
                HexTile tile = currentTile.GetComponent<HexTile>();
                if (_movableObjectType == MovableObjectType.Item)
                {
                    tile.isItemTile = false;
                    tile.itemOnTile = null;
                }
            }
        }
    }

    private void TouchMovedEvent()
    {
        if (_movableObj != null)
        {
            _objectMoved = true;
            _isDragging = true;

            Vector3 touchPos = Input.mousePosition;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, 10));
            _movableObj.transform.position = worldPos;
        }
    }

    private void TouchEndedEvent()
    {
        if (_movableObj != null)
        {
            _isDragging = false;
            if (_objectMoved && currentTile != null)
            {
                HexTile tile = currentTile.GetComponent<HexTile>();
                if (_movableObjectType == MovableObjectType.Item)
                {
                    tile.isItemTile = false;
                    tile.itemOnTile = null;
                }
            }

            RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));
            foreach (RaycastHit hit in hits)
            {
                GameObject hitObj = hit.collider.gameObject;
                if (hitObj == _movableObj) continue;

                string expectedTileTag = _movableObjectType == MovableObjectType.Item ? "ItemTile" : "PlayerTile";
                if (hitObj.CompareTag(expectedTileTag) || hitObj.transform.root.CompareTag(expectedTileTag))
                {
                    HexTile hitTile = hitObj.GetComponent<HexTile>() ?? hitObj.transform.root.GetComponent<HexTile>();
                    if (hitTile != null)
                    {
                        if (_movableObjectType == MovableObjectType.Item)
                            HandleItemDrop(hitTile);
                        else if (_movableObjectType == MovableObjectType.Champion)
                            HandleChampionDrop(hitTile);
                        return;
                    }
                }
                else
                {
                    HandleChampionDrop();
                }

                if (_movableObjectType == MovableObjectType.Item)
                {
                    if (hitObj.CompareTag("Champion"))
                    {
                        if(hitObj.GetComponent<ChampionBase>().CanGetItem())
                        {
                            HandleGiveItemToChampion(hitObj);
                            return;
                        }
                    }
                }
            }

            StartReturningObject();
        }
    }

    #endregion

    #region Battle State Handling

    private void HandleBattleState()
    {
        if (!_previousIsBattleOngoing && Manager.Stage.IsBattleOngoing)
        {
            if (_isDragging && _movableObj != null)
            {
                // 반환 과정을 즉시 완료
                StartReturningObject();
                CompleteObjectReturnImmediately();
                _isDragging = false; // 드래그 상태를 false로 설정
                InActiveTile();
            }
        }
        _previousIsBattleOngoing = Manager.Stage.IsBattleOngoing;
    }

    #endregion

    #region Helper Methods

    private void UpdateTouchPositions()
    {
        _lastTouchPos = _currentTouchPos;
        _currentTouchPos = Input.mousePosition;
    }

    private void HandleRightClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                GameObject hitObject = hitInfo.collider.gameObject;
                if (hitObject.CompareTag("Champion"))
                {
                    ShowChampionInfo(hitObject);
                }
                else if (hitObject.CompareTag("Item"))
                {
                    ItemFrame iFrame = hitObject.GetComponent<ItemFrame>();

                    if(iFrame.ItemBlueprint.ItemType == ItemType.Normal && !(iFrame.ItemBlueprint.ItemName == "뒤집개")
                        && !(iFrame.ItemBlueprint.ItemName == "프라이팬"))
                        ShowItemCombinePopup(hitObject);
                }
            }
        }
    }

    private GameObject GetObjectUnderMouse(string tag, bool ignoreBattleCheck = false)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            GameObject hitObject = hitInfo.collider.gameObject;
            if (hitObject.CompareTag(tag))
            {
                UserData user1 = Manager.User.GetHumanUserData();
                if (tag == "Champion")
                {
                    ActiveTile();
                    if (Manager.Stage.IsBattleOngoing && !ignoreBattleCheck)
                    {
                        if (user1.BattleChampionObject.Contains(hitObject))
                        {
                            return null;
                        }
                        else if (user1.NonBattleChampionObject.Contains(hitObject))
                        {
                            return hitObject;
                        }
                        else if (user1.TotalChampionObject.Contains(hitObject))
                        {

                            return hitObject;
                        }
                    }
                    else
                    {
                        if (user1.TotalChampionObject.Contains(hitObject))
                        {
                            return hitObject;
                        }
                    }
                }
                if (tag == "Item")
                {
                    if (user1.UserItemObject.Contains(hitObject))
                        return hitObject;
                    else
                        return null;
                }
            }
        }
        return null;
    }

    private void ShowChampionInfo(GameObject championObj)
    {
        ChampionBase cBase = championObj.GetComponent<ChampionBase>();
        if (cBase != null)
            uiMain.UIChampionExplainPanel.UpdateChampionExplainPanel(cBase);
    }

    private void ShowItemCombinePopup(GameObject item)
    {
        var itemCombine = Manager.UI.ShowPopup<UIPopupItemCombineDesk>();

        Vector2 mousePosition = Input.mousePosition;
        itemCombine.SetPosition(mousePosition);

        UIPopupItemCombineDesk uiItemCombine = itemCombine.GetComponent<UIPopupItemCombineDesk>();
        ItemFrame iFrame = item.GetComponent<ItemFrame>();

        var list = Manager.Item.GetItemCombineDesk(iFrame.ItemBlueprint.ItemId);
        uiItemCombine.InitItemCombinePopup(list, iFrame.ItemBlueprint.ItemId);
    }

    private void SetMovableObjectType(MovableObjectType type)
    {
        _movableObjectType = type;
        _offset = type == MovableObjectType.Item ? new Vector3(0.0f, 0.3f, 0.0f) : Vector3.zero;
    }

    private void FindCurrentTile()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject hitObj = hit.collider.gameObject;
            if (hitObj.CompareTag("PlayerTile") || hitObj.CompareTag("ItemTile"))
                currentTile = hitObj;
        }
    }

    private void StartReturningObject()
    {
        if (!_isReturning)
        {
            _isReturning = true;
            _returningObjData = new ReturningObjectData
            {
                obj = _movableObj,
                beforePosition = _beforePosition,
                currentTile = currentTile,
                movableObjectType = _movableObjectType
            };
            _movableObj = null;
            currentTile = null;
        }
    }

    #endregion

    #region Object Handling

    private void HandleItemDrop(HexTile hitTile)
    {
        if (!hitTile.isItemTile)
        {
            PlaceObjectOnTile(hitTile);
        }
        else
        {
            SwapItemsOnTiles(hitTile);
        }
        _movableObj = null;
    }

    private void HandleChampionDrop(HexTile hitTile)
    {
        UserData userData = Manager.User.GetHumanUserData();

        bool isDraggingChampionOnBattleGrid = userData.BattleChampionObject.Contains(_movableObj);

        if (hitTile != null && !hitTile.isRectangularTile && !Manager.Stage.IsBattleOngoing)
        {
            if (userData.CurrentPlaceChampion >= userData.MaxPlaceChampion)
            {
                if (hitTile.HasChampion)
                {
                    SwapChampions(hitTile);
                    /*
                    if (!isDraggingChampionOnBattleGrid)
                    {
                        SwapChampions(hitTile);
                    }
                    else
                    {
                        StartReturningObject();
                    }
                    */
                }
                else
                {
                    if (isDraggingChampionOnBattleGrid)
                    {
                        PlaceObjectOnTile(hitTile);
                    }
                    else
                    {
                        StartReturningObject();
                    }
                }
                return;
            }
        }

        if (hitTile == null || (Manager.Stage.IsBattleOngoing && !hitTile.isRectangularTile))
        {
            StartReturningObject();
            return;
        }

        if (hitTile.HasChampion)
        {
            SwapChampions(hitTile);
        }
        else
        {
            PlaceObjectOnTile(hitTile);
        }

        if (!Manager.Stage.IsBattleOngoing)
        {
            UpdateSynergy(Manager.User.GetHumanUserData());
        }
        InActiveTile();
        _movableObj = null;
    }

    private void HandleChampionDrop()
    {
        if (IsMouseOverUI("SellPanel"))
        {
            uiMain.UIShopPanel.SellChampion(_movableObj);
            currentTile = null;
        }
    }

    private void HandleItemCombination(GameObject targetItemObj)
    {
        ItemFrame draggedItemFrame = _movableObj.GetComponent<ItemFrame>();
        ItemFrame targetItemFrame = targetItemObj.GetComponent<ItemFrame>();

        if (draggedItemFrame != null && targetItemFrame != null)
        {
            string combinedItemName = Manager.Item.ItemCombine(draggedItemFrame.ItemBlueprint.ItemId, targetItemFrame.ItemBlueprint.ItemId);

            if (combinedItemName == "error")
            {
                StartReturningObject();
                return;
            }

            GameObject newItem = Manager.Item.CreateItem(combinedItemName, targetItemObj.transform.position);

            Destroy(targetItemFrame.gameObject);
            Destroy(draggedItemFrame.gameObject);

            HexTile tile = currentTile?.GetComponent<HexTile>() ?? targetItemObj.transform.parent.GetComponent<HexTile>();
            if (tile != null)
            {
                tile.itemOnTile = newItem;
                newItem.transform.SetParent(tile.transform);
            }
        }
        else
        {
            StartReturningObject();
        }
    }

    private void HandleGiveItemToChampion(GameObject championObj)
    {
        ChampionBase cBase = championObj.GetComponent<ChampionBase>();
        if (cBase == null)
        {
            StartReturningObject();
            return;
        }

        ItemFrame draggedItemFrame = _movableObj.GetComponent<ItemFrame>();
        if (draggedItemFrame != null)
        {
            cBase.GetItem(draggedItemFrame.ItemBlueprint);
            UserData user1 = Manager.User.GetHumanUserData();
            user1.UserItemObject.Remove(_movableObj);
            Destroy(draggedItemFrame.gameObject);

            if (currentTile != null)
            {
                HexTile tile = currentTile.GetComponent<HexTile>();
                tile.isItemTile = false;
            }
        }
        else
        {
            StartReturningObject();
        }
    }

    private void PlaceObjectOnTile(HexTile hitTile)
    {
        _movableObj.transform.position = hitTile.transform.position + _offset;
        _movableObj.transform.SetParent(hitTile.transform);

        if (_movableObjectType == MovableObjectType.Item)
        {
            hitTile.isItemTile = true;
            hitTile.itemOnTile = _movableObj;
        }
        else if (_movableObjectType == MovableObjectType.Champion)
        {
            hitTile.championOnTile.Add(_movableObj);
            ChampionBase cBase = _movableObj.GetComponent<ChampionBase>();
            cBase.BattleChampionStat();

            if (currentTile != null && currentTile != hitTile.gameObject)
            {
                HexTile previousTile = currentTile.GetComponent<HexTile>();
                previousTile?.championOnTile.Remove(_movableObj);
            }
            currentTile = hitTile.gameObject;
        }
        _movableObj = null;
    }

    private void SwapItemsOnTiles(HexTile hitTile)
    { 
        GameObject otherItem = hitTile.itemOnTile;

        if (currentTile != null)
        {
            otherItem.transform.position = currentTile.transform.position + _offset;
            otherItem.transform.SetParent(currentTile.transform);

            HexTile previousTile = currentTile.GetComponent<HexTile>();
            previousTile.isItemTile = true;
            previousTile.itemOnTile = otherItem;
        }
        else
        {
            otherItem.transform.position = _beforePosition;
            otherItem.transform.SetParent(null);
        }

        _movableObj.transform.position = hitTile.transform.position + _offset;
        _movableObj.transform.SetParent(hitTile.transform);
        hitTile.itemOnTile = _movableObj;
        _movableObj = null;
    }
    private void SwapChampions(HexTile hitTile)
    {
        // 현재 드래그 중인 챔피언과 타일에 있는 챔피언을 가져옵니다.
        GameObject tileChampion = hitTile.championOnTile[0]; // 해당 타일의 챔피언 (리스트의 첫 번째 요소)
        HexTile currentChampionTile = currentTile.GetComponent<HexTile>(); // 드래그 중인 챔피언의 현재 타일

        // 위치 교환을 위해 임시 변수에 위치 정보를 저장합니다.
        Vector3 tempPosition = tileChampion.transform.position;
        GameObject tempTile = hitTile.gameObject;

        // 드래그 중인 챔피언을 대상 타일로 이동
        _movableObj.transform.position = hitTile.transform.position + _offset;
        _movableObj.transform.SetParent(hitTile.transform);

        // 대상 타일의 챔피언을 원래 드래그 중인 챔피언의 타일로 이동
        tileChampion.transform.position = currentChampionTile.transform.position + _offset;
        tileChampion.transform.SetParent(currentChampionTile.transform);

        // 타일의 챔피언 정보를 업데이트합니다.
        // 대상 타일에 드래그 중인 챔피언을 등록
        hitTile.championOnTile.Clear();
        hitTile.championOnTile.Add(_movableObj);

        // 원래 타일에 대상 챔피언을 등록
        currentChampionTile.championOnTile.Clear();
        currentChampionTile.championOnTile.Add(tileChampion);

        // 현재 타일 정보를 업데이트합니다.
        currentTile = hitTile.gameObject;
        _movableObj = null;
        InActiveTile();
        UpdateSynergy(Manager.User.GetHumanUserData());
    }
    private void ObjectReturn()
    {
        if (_isReturning && _returningObjData != null && _returningObjData.obj != null)
        {
            if (_returningObjData.movableObjectType == MovableObjectType.Item)
            {
                _returningObjData.obj.transform.position = Vector3.MoveTowards(
                    _returningObjData.obj.transform.position,
                    _returningObjData.beforePosition,
                    Time.deltaTime * 30f);
            }
            else
            {
                InActiveTile();
                _returningObjData.obj.transform.position = _returningObjData.beforePosition;
            }

            if (Vector3.Distance(_returningObjData.obj.transform.position, _returningObjData.beforePosition) < 0.01f)
            {
                _isReturning = false;

                if (_returningObjData.currentTile != null)
                {
                    _returningObjData.obj.transform.SetParent(_returningObjData.currentTile.transform);
                    HexTile previousTile = _returningObjData.currentTile.GetComponent<HexTile>();
                    if (previousTile != null)
                    {
                        if (_returningObjData.movableObjectType == MovableObjectType.Item)
                        {
                            previousTile.isItemTile = true;
                            previousTile.itemOnTile = _returningObjData.obj;
                        }
                        else if (_returningObjData.movableObjectType == MovableObjectType.Champion)
                        {
                            if (!previousTile.championOnTile.Contains(_returningObjData.obj))
                                previousTile.championOnTile.Add(_returningObjData.obj);
                        }
                    }
                }
                _returningObjData = null;
            }
        }
    }

    private void CompleteObjectReturnImmediately()
    {
        if (_isReturning && _returningObjData != null && _returningObjData.obj != null)
        {
            // 즉시 위치를 원래 위치로 변경
            _returningObjData.obj.transform.position = _returningObjData.beforePosition;

            // 부모 설정 및 타일 정보 업데이트
            if (_returningObjData.currentTile != null)
            {
                _returningObjData.obj.transform.SetParent(_returningObjData.currentTile.transform);
                HexTile previousTile = _returningObjData.currentTile.GetComponent<HexTile>();
                if (previousTile != null)
                {
                    if (_returningObjData.movableObjectType == MovableObjectType.Item)
                    {
                        previousTile.isItemTile = true;
                        previousTile.itemOnTile = _returningObjData.obj;
                    }
                    else if (_returningObjData.movableObjectType == MovableObjectType.Champion)
                    {
                        if (!previousTile.championOnTile.Contains(_returningObjData.obj))
                            previousTile.championOnTile.Add(_returningObjData.obj);
                    }
                }
            }
            _isReturning = false;
            _returningObjData = null;
        }
    }

    #endregion

    #region UI Handling

    private void HandleItemHover()
    {
        if (_movableObj != null)
        {
            _hoveredItem?.HideItemInfoUI();
            _hoveredItem = null;
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject hoveredObject = hit.collider.gameObject;

            if (hoveredObject.CompareTag("Item"))
            {
                ItemFrame itemFrame = hoveredObject.GetComponent<ItemFrame>();
                if (_hoveredItem != itemFrame)
                {
                    _hoveredItem?.HideItemInfoUI();
                    _hoveredItem = itemFrame;
                    _hoveredItem.ShowItemInfoUI();
                }
            }
            else
            {
                _hoveredItem?.HideItemInfoUI();
                _hoveredItem = null;
            }
        }
        else
        {
            _hoveredItem?.HideItemInfoUI();
            _hoveredItem = null;
        }
    }

    private void CheckChampionDrag()
    {
        if (_movableObj != null && _movableObjectType == MovableObjectType.Champion)
        {
            if (IsMouseOverUI("ShopBG"))
            {
                uiMain.UIShopPanel.SellPanel.SetActive(true);
                uiMain.UIShopPanel.InitSellPanel(_movableObj);
            }
            else
            {
                uiMain.UIShopPanel.SellPanel.SetActive(false);
            }
        }
        else
        {
            uiMain.UIShopPanel.SellPanel.SetActive(false);
        }
    }

    private bool IsMouseOverUI(string name)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.name == name && result.gameObject.activeSelf)
                return true;
        }
        return false;
    }
    private void UpdateSynergy(UserData userdata)
    {
        //player1 = Manager.User.GetHumanUserData();
        Manager.User.ClearSynergy(userdata);
        //Manager.Champion.SettingNonBattleChampion(userdata);
        Manager.Champion.SettingAllChampion(userdata);
        Manager.Champion.SettingBattleChampion(userdata);


        uiMain?.UISynergyPanel.UpdateSynergy(userdata);
    }

    #region 챔피언 자동배치
    public void AutoPlaceUserChampions(UserData userData)
    {
        // 현재 배틀필드에 배치된 챔피언 수 확인
        int currentBattleChampions = userData.CurrentPlaceChampion;

        // 플레이어의 최대 배치 가능 챔피언 수
        int maxBattleChampions = userData.MaxPlaceChampion;

        // 배치 가능한 슬롯 수 계산
        int availableSlots = maxBattleChampions - currentBattleChampions;

        if (availableSlots <= 0)
        {
            // 배치 가능한 슬롯이 없으면 반환
            return;
        }

        // NonBattleChampionObject 리스트에서 챔피언을 가져옵니다.
        List<GameObject> championsOnBench = new List<GameObject>(userData.NonBattleChampionObject);

        // 배치 가능한 수만큼 챔피언 선택
        for (int i = 0; i < availableSlots && championsOnBench.Count > 0; i++)
        {
            // 랜덤 챔피언 선택
            int randomIndex = Random.Range(0, championsOnBench.Count);
            GameObject championToPlace = championsOnBench[randomIndex];

            PlaceChampionOnHexTileForUser(championToPlace, userData);
            championsOnBench.RemoveAt(randomIndex);
        }
        UpdateSynergy(userData);
    }
    private void PlaceChampionOnHexTileForUser(GameObject champion, UserData userData)
    {
        // 플레이어의 맵 정보를 가져옵니다.
        MapGenerator.MapInfo mapInfo = userData.MapInfo;

        if (mapInfo == null)
        {
            Debug.LogWarning($"플레이어 {userData.UserName}의 MapInfo를 찾을 수 없습니다.");
            return;
        }

        // 빈 HexTile 찾기
        HexTile emptyTile = FindEmptyHexTile(mapInfo);

        if (emptyTile != null)
        {
            // 챔피언의 현재 타일 정보 가져오기
            HexTile currentTile = champion.transform.parent.GetComponent<HexTile>();
            if (currentTile != null)
            {
                currentTile.championOnTile.Remove(champion);

                // 현재 타일이 RectTile인지 확인하여 리스트 업데이트
                if (currentTile.isRectangularTile)
                {
                    userData.NonBattleChampionObject.Remove(champion);
                }
                else
                {
                    userData.BattleChampionObject.Remove(champion);
                }
            }

            // 챔피언의 위치와 부모를 업데이트
            champion.transform.position = emptyTile.transform.position;
            champion.transform.SetParent(emptyTile.transform);

            // 타일 상태 업데이트
            emptyTile.championOnTile.Add(champion);

            // 새로운 타일이 RectTile인지 확인하여 리스트 업데이트
            if (emptyTile.isRectangularTile)
            {
                userData.NonBattleChampionObject.Add(champion);
            }
            else
            {
                userData.BattleChampionObject.Add(champion);
            }
        }
        else
        {
            Debug.LogWarning($"플레이어 {userData.UserName}의 HexTile에 빈 타일이 없습니다.");
        }
    }
    private HexTile FindEmptyHexTile(MapGenerator.MapInfo mapInfo)
    {
        List<HexTile> availableTiles = new List<HexTile>();

        // 조건에 맞는 타일들을 리스트에 추가합니다.
        foreach (var tileEntry in mapInfo.HexDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (!tile.isOccupied && tile.CompareTag("PlayerTile"))
            {
                availableTiles.Add(tile);
            }
        }

        // 리스트가 비어있지 않으면 랜덤한 타일을 반환합니다.
        if (availableTiles.Count > 0)
        {
            int randomIndex = Random.Range(0, availableTiles.Count);
            return availableTiles[randomIndex];
        }

        // 조건에 맞는 타일이 없으면 null을 반환합니다.
        return null;
    }
    #endregion
    
    private void ActiveTile()
    {
        UserData user = Manager.User.GetHumanUserData();
        MapGenerator.MapInfo mapinfo = user.MapInfo;

        if(Manager.Stage.IsBattleOngoing)
        {
            foreach (var tileEntry in mapinfo.RectDictionary)
            {
                HexTile tile = tileEntry.Value;
                if (tile.y == -1)
                {
                    tile.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }
        else
        {
            foreach (var tileEntry in mapinfo.HexDictionary)
            {
                HexTile tile = tileEntry.Value;
                if (tile.CompareTag("PlayerTile"))
                {
                    tile.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
            foreach (var tileEntry in mapinfo.RectDictionary)
            {
                HexTile tile = tileEntry.Value;
                if(tile.y == -1)
                {
                    tile.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }
    }

    private void InActiveTile()
    {
        UserData user = Manager.User.GetHumanUserData();
        MapGenerator.MapInfo mapinfo = user.MapInfo;
        foreach (var tileEntry in mapinfo.HexDictionary)
        {
            HexTile tile = tileEntry.Value;
            tile.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        foreach (var tileEntry in mapinfo.RectDictionary)
        {
            HexTile tile = tileEntry.Value;
            tile.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void ActiveHoverTile()
    {
        // 챔피언을 드래그 중인지 확인
        if (!_isDragging || _movableObjectType != MovableObjectType.Champion || _isReturning)
        {
            // 이전에 활성화된 하이라이트를 비활성화
            if (_previousHoveredTile != null)
            {
                Transform highlightTransform = _previousHoveredTile.transform.Find("Highlight");
                if (highlightTransform != null)
                {
                    highlightTransform.gameObject.SetActive(false);
                }
                _previousHoveredTile = null;
            }
            return;
        }

        // 이전에 활성화된 하이라이트를 비활성화
        if (_previousHoveredTile != null)
        {
            Transform highlightTransform = _previousHoveredTile.transform.Find("Highlight");
            if (highlightTransform != null)
            {
                highlightTransform.gameObject.SetActive(false);
            }
            _previousHoveredTile = null;
        }

        int layerMask = 1 << LayerMask.NameToLayer("PlayerTile");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            Transform hoveredTransform = hit.collider.transform;
            while (hoveredTransform != null)
            {
                UserData user = Manager.User.GetHumanUserData();
                MapGenerator.MapInfo mapinfo = user.MapInfo;
                HexTile hexTile = hoveredTransform.GetComponent<HexTile>();

                if (hexTile != null)
                {
                    bool isRectTile = mapinfo.RectDictionary.ContainsValue(hexTile);
                    bool isHexTile = mapinfo.HexDictionary.ContainsValue(hexTile);

                    if (Manager.Stage.IsBattleOngoing)
                    {
                        // 전투 중일 때는 RectTile만 고려
                        if (isRectTile)
                        {
                            Transform highlightTransform = hoveredTransform.Find("Highlight");
                            if (highlightTransform != null)
                            {
                                highlightTransform.gameObject.SetActive(true);
                                _previousHoveredTile = hexTile;
                            }
                            break;
                        }
                    }
                    else
                    {
                        // 전투 중이 아닐 때는 HexTile만 고려
                        if (isHexTile)
                        {
                            Transform highlightTransform = hoveredTransform.Find("Highlight");
                            if (highlightTransform != null)
                            {
                                highlightTransform.gameObject.SetActive(true);
                                _previousHoveredTile = hexTile;
                            }
                            break;
                        }
                        if (isRectTile)
                        {
                            Transform highlightTransform = hoveredTransform.Find("Highlight");
                            if (highlightTransform != null)
                            {
                                highlightTransform.gameObject.SetActive(true);
                                _previousHoveredTile = hexTile;
                            }
                            break;
                        }
                    }
                }
                hoveredTransform = hoveredTransform.parent;
            }
        }
    }

    public UserData ReturnUser(UserData user)
    {
        return player1 = user;
    }
    #endregion
}
