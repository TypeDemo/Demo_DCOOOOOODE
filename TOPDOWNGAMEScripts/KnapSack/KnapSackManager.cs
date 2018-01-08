using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnapSackManager : MonoBehaviour
{

    public GridBoxUI gridBox;

    public Dictionary<int, ItemData> itemBox;       //背包包含的道具种类

    public Dictionary<int, Transform> itemInGrid;   //物品在哪个格子

    public Dictionary<int, int> itemNum;        //道具数量

    public GameObject dragUI;

    public TipUI tip;

    [HideInInspector]
    public bool isOpen = false;

    public Camera playerCamera;
    public Camera UICamera;
    public Canvas miniCanvas;

    Vector2 mousePosition;

    bool isDraged = false;
    bool isShowDescription = false;

    //单例
    private static KnapSackManager _instance;
    private KnapSackManager()
    { }
    public static KnapSackManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindGameObjectWithTag("KnapSack").GetComponent<KnapSackManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        GridUI.beginDrag += StartDrag;
        GridUI.endDrag += endDrag;
        GridUI.onClick += OnClickItem;

    }

    // Use this for initialization
    void Start()
    {
        itemBox = new Dictionary<int, ItemData>();
        itemInGrid = new Dictionary<int, Transform>();
        itemNum = new Dictionary<int, int>();
        StoreItem(20001);
        StoreItem(20002);


    }

    // Update is called once per frame
    void Update()
    {
        //将鼠标坐标转化为UI界面坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GameObject.Find("KnapSack").transform as RectTransform, Input.mousePosition, UICamera, out mousePosition);
        if (isDraged)
        {
            dragUI.SetActive(true);
            dragUI.transform.localPosition = mousePosition;

        }
        else if (!isDraged)
        {
            dragUI.SetActive(false);
        }

    }

    //存储物品
    public void StoreItem(int itemId)
    {
        var item = DataTable.Get<ItemData>(itemId);

        if (!itemBox.ContainsKey(itemId))       //背包没有该物品
        {
            Transform emptyGrid = gridBox.GetEmptyGrid();

            if (emptyGrid == null)
            {
                return;
            }
            
            itemBox.Add(itemId, item);
            itemInGrid.Add(itemId, emptyGrid);
            if (itemId == 10002)
            {
                itemNum.Add(itemId, 10);
            }
            else
            {
                itemNum.Add(itemId, 1);

            }

            emptyGrid.GetComponent<GridUI>().idInGrid = itemId;

            CreatItemInKnapSack(item, emptyGrid);

        }
        else if (itemBox.ContainsKey(itemId))       //如果有该物品
        {
            if (itemId == 10002)
            {
                itemNum[itemId] += 10;
            }
            else
            {
                itemNum[itemId] += 1;

            }
            itemInGrid[itemId].GetComponentInChildren<Text>().text = itemNum[itemId].ToString();

        }



    }

    //生成UI
    private void CreatItemInKnapSack(ItemData item, Transform transformParent)
    {
        GameObject itemPrefab = Resources.Load<GameObject>("Prefabs/ItemIcon");
        var image = itemPrefab.GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>("Icon/" + item.icon) as Sprite;
        GameObject itemGo = Instantiate(itemPrefab, transformParent);
        itemGo.transform.localPosition = Vector3.zero;

        if (item.id == 10002)
        {
            itemGo.GetComponentInChildren<Text>().text = 10.ToString();
        }
        else
        {
            itemGo.GetComponentInChildren<Text>().text = 1.ToString();

        }

    }

    #region 拖拽
    //开始拖拽
    private void StartDrag(Transform gridTransform)
    {
        if (!isOpen)
            return;
        if (gridTransform.childCount == 0)
            return;
        isDraged = true;

        dragUI.GetComponent<Image>().sprite = gridTransform.GetChild(0).GetComponent<Image>().sprite;


    }

    //结束拖拽
    private void endDrag(Transform prevTransform, Transform enterTransform)
    {
        if (!isOpen)
            return;
        isDraged = false;


        if (prevTransform.childCount == 0)
            return;


        //删除物品
        if (enterTransform != null && enterTransform.CompareTag("DeleteGrid"))
        {
            if (prevTransform.CompareTag("WeaponGrid"))//如果当前格子是武器格子，则不能删除格子内的武器
                return;
            int id = prevTransform.GetComponent<GridUI>().idInGrid;
            itemBox.Remove(id);
            itemInGrid.Remove(id);
            itemNum.Remove(id);

            Destroy(prevTransform.GetChild(0).gameObject);

        }

        //交换物品
        else if (enterTransform != null && enterTransform.CompareTag("Grid"))
        {
            if (prevTransform.CompareTag("WeaponGrid"))
            {
                //如果当前格子是武器格子，目标格子内必须是武器才能交换
                if (enterTransform.GetComponent<GridUI>().idInGrid == 20001)
                {
                    var temp = GameObject.Find("PlayerWeaponSocket");
                    CloseAllWeapon(temp);
                    temp.transform.Find("Club").gameObject.SetActive(true);
                    PlayerCharacter.player.GetCurrentMeleeWeapon();
                    InterChangeItem(prevTransform, enterTransform);
                }
                else if (enterTransform.GetComponent<GridUI>().idInGrid == 20002)
                {
                    var temp = GameObject.Find("PlayerWeaponSocket");
                    CloseAllWeapon(temp);
                    temp.transform.Find("ShootWeapon").gameObject.SetActive(true);
                    PlayerCharacter.player.GetShootWeapon();
                    InterChangeItem(prevTransform, enterTransform);
                }
                //如果目标格子为空，则为卸下武器
                else if (enterTransform.childCount == 0)
                {
                    var temp = GameObject.Find("PlayerWeaponSocket");
                    CloseAllWeapon(temp);
                    temp.transform.Find("Fist").gameObject.SetActive(true);

                    PlayerCharacter.player.GetCurrentMeleeWeapon();

                    InterChangeItem(prevTransform, enterTransform);
                }
                return;
            }
            InterChangeItem(prevTransform, enterTransform);

        }
        else if (enterTransform != null && enterTransform.CompareTag("WeaponGrid"))
        {
            //如果目标格子是武器格子，则只有当前格子内保存的是武器才能交换

            if (prevTransform.GetComponent<GridUI>().idInGrid == 20001)
            {
                var temp = GameObject.Find("PlayerWeaponSocket");
                CloseAllWeapon(temp);
                temp.transform.Find("Club").gameObject.SetActive(true);
                PlayerCharacter.player.GetCurrentMeleeWeapon();
                InterChangeItem(prevTransform, enterTransform);
            }
            else if (prevTransform.GetComponent<GridUI>().idInGrid == 20002)
            {
                var temp = GameObject.Find("PlayerWeaponSocket");
                CloseAllWeapon(temp);
                temp.transform.Find("ShootWeapon").gameObject.SetActive(true);
                PlayerCharacter.player.GetShootWeapon();
                InterChangeItem(prevTransform, enterTransform);
            }

        }


    }

    //切换武器，本次未采用保留上一次武器的方法，直接隐藏所有武器然后激活需要的武器 
    private void CloseAllWeapon(GameObject root)
    {
        foreach (Transform n in root.transform)
        {
            n.gameObject.SetActive(false);
        }
    }

    //交换物品的UI与设置格子内的物品ID
    private void InterChangeItem(Transform prevTransform, Transform enterTransform)
    {
        if (prevTransform == enterTransform)
            return;

        int prevId;
        if (enterTransform.childCount == 0)
        {
            prevId = prevTransform.GetComponent<GridUI>().idInGrid;
            itemInGrid[prevId] = enterTransform;
            prevTransform.GetChild(0).SetParent(enterTransform);
            enterTransform.GetChild(0).localPosition = Vector3.zero;
            enterTransform.GetComponent<GridUI>().idInGrid = prevId;
            prevTransform.GetComponent<GridUI>().idInGrid = -1;

        }
        else
        {
            int temp = prevTransform.GetComponent<GridUI>().idInGrid;
            prevTransform.GetChild(0).SetParent(enterTransform);
            enterTransform.GetChild(1).localPosition = Vector3.zero;
            enterTransform.GetChild(0).SetParent(prevTransform);
            prevTransform.GetChild(0).localPosition = Vector3.zero;

            itemInGrid[temp] = enterTransform;
            itemInGrid[enterTransform.GetComponent<GridUI>().idInGrid] = prevTransform;

            prevTransform.GetComponent<GridUI>().idInGrid = enterTransform.GetComponent<GridUI>().idInGrid;
            enterTransform.GetComponent<GridUI>().idInGrid = temp;

        }
    }
    #endregion

    //显示描述
    private void OnClickItem(Transform gridTransform)
    {
        if (!isOpen)
            return;
        if (gridTransform.childCount == 0)
            return;

        if (!isShowDescription)
        {
            tip.gameObject.SetActive(true);
            int id = gridTransform.GetComponent<GridUI>().idInGrid;
            tip.UpdateText(itemBox[id].description);
            isShowDescription = true;
        }
        else
        {
            tip.gameObject.SetActive(false);
            isShowDescription = false;
        }

    }

    public bool CastItem(int itemID)
    {
        if (itemBox.ContainsKey(itemID))
        {
            if (itemNum[itemID] > 0)
            {
                itemNum[itemID] -= 1;
                if (itemNum[itemID] == 0)
                {
                    itemBox.Remove(itemID);
                    itemNum.Remove(itemID);

                    Destroy(itemInGrid[itemID].GetChild(0).gameObject);
                    itemInGrid.Remove(itemID);
                }
                else
                {
                    itemInGrid[itemID].GetComponentInChildren<Text>().text = itemNum[itemID].ToString();
                }

                return true;
            }
        }
        return false;
    }

    //关闭提示
    public void CloseTip()
    {
        isDraged = false;
        tip.gameObject.SetActive(false);
        isShowDescription = false;
    }


    //切换摄像机和画布
    public void CameraOpen(bool open)
    {
        miniCanvas.gameObject.SetActive(!open);
        playerCamera.gameObject.SetActive(open);
        UICamera.gameObject.SetActive(open);
    }



}
