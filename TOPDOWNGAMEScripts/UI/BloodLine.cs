using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodLine : MonoBehaviour
{
    private Button cancelButton;

    private RectTransform lineTransform;
    private GameObject lineUI;
    private Transform parent;

    private Button finishBuild;
    Vector2 onScreenPos;
    private GameObject go;

    private Slider bloodLine;
    private Character myCharacter;

    public Vector2 offSet;

    private void Awake()
    {
        if (transform.CompareTag("Builder"))
        {

            go = Resources.Load<GameObject>("Prefabs/TowerBloodLine");
        }
        else if (transform.CompareTag("Enemy"))
        {

            go = Resources.Load<GameObject>("Prefabs/EnemyBloodLine");
        }
        parent = GameObject.Find("MiniMapCanvas").transform;
        lineUI = GameObjectPool.inventoryPool.GetObject(go);
        lineUI.transform.SetParent(parent);

        lineUI.SetActive(true);

        lineTransform = lineUI.GetComponent<RectTransform>();
        bloodLine = lineUI.GetComponent<Slider>();
        myCharacter = GetComponent<Character>();

        cancelButton = GameObject.Find("Canvases/MiniMapCanvas").transform.Find("BuildList/BuildCancelButton").GetComponent<Button>();
        cancelButton.onClick.AddListener(LineUIDisable);

    }
    // Use this for initialization

    private void OnEnable()
    {
        cancelButton = GameObject.Find("Canvases/MiniMapCanvas").transform.Find("BuildList/BuildCancelButton").GetComponent<Button>();
        cancelButton.onClick.AddListener(LineUIDisable);
    }

    void Start()
    {
        //if (transform.CompareTag("Builder"))
        //{

        //    go = Resources.Load<GameObject>("Prefabs/TowerBloodLine");
        //}
        //else if (transform.CompareTag("Enemy"))
        //{

        //    go = Resources.Load<GameObject>("Prefabs/EnemyBloodLine");
        //}
        //parent = GameObject.Find("MiniMapCanvas").transform;
        //lineUI = GameObjectPool.inventoryPool.GetObject(go);
        //lineUI.transform.SetParent(parent);

        //lineUI.SetActive(true);

        //lineTransform = lineUI.GetComponent<RectTransform>();
        //bloodLine = lineUI.GetComponent<Slider>();
        //myCharacter = GetComponent<Character>();

        //cancelButton = GameObject.Find("Canvases/MiniMapCanvas/BuildList").transform.Find("BuildCancelButton").GetComponent<Button>();
        //cancelButton.onClick.AddListener(LineUIDisable);
    }

    // Update is called once per frame
    void Update()
    {
        if (lineTransform == null)
        {
            return;

        }

        //世界坐标转换为Canvas坐标
        onScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
        lineTransform.position = onScreenPos + offSet;
        bloodLine.value = myCharacter.health;
        bloodLine.maxValue = myCharacter.maxHealth;

        if (lineTransform.position.x > Screen.width || lineTransform.position.x < 0.0f || lineTransform.position.y > Screen.height || lineTransform.position.y < 0.0f)
        {
            lineTransform.gameObject.SetActive(false);
        }
        else
        {
            lineTransform.gameObject.SetActive(true);
        }
        if (bloodLine.value <= 0.0f)
        {
            LineUIDisable();
        }
    }

    private void LineUIDisable()
    {
        lineUI.SetActive(false);
        cancelButton.onClick.RemoveListener(LineUIDisable);
    }
}
