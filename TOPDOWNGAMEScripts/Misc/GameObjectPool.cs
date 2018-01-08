using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象池，本次工程对象较少，对象池也包括了AI的管理逻辑
/// </summary>

public class GameObjectPool : MonoBehaviour
{
    //该对象池仅适用于GameObject的对象
    public int randomArea = 30;

    public Transform parent;

    private Vector2 tempPosition;

    private static Dictionary<string, List<GameObject>> objectPool;

    private static List<GameObject> enemyPool = new List<GameObject>();
    private static List<GameObject> fireEffectPool = new List<GameObject>();
    private static List<GameObject> bloodEffectPool = new List<GameObject>();
    private static List<GameObject> towerEffectPool = new List<GameObject>();

    private static List<GameObject> builderPool = new List<GameObject>();
    private static List<GameObject> towerBloodLinePool = new List<GameObject>();
    private static List<GameObject> enemyBloodLinePool = new List<GameObject>();


    //单例
    private GameObjectPool() { }
    private static GameObjectPool _inventoryPool;
    public static GameObjectPool inventoryPool

    {
        get
        {
            if (_inventoryPool == null)
            {
                _inventoryPool = GameObject.Find("GameObjectPool").GetComponent<GameObjectPool>();
            }
            return _inventoryPool;
        }
    }

    private void Awake()
    {
        objectPool = new Dictionary<string, List<GameObject>>();
        objectPool.Add("Enemy", enemyPool);
        objectPool.Add("FireEffect", fireEffectPool);
        objectPool.Add("BloodEffect", bloodEffectPool);
        objectPool.Add("Builder", builderPool);
        objectPool.Add("TowerDestroyed", towerEffectPool);
        objectPool.Add("TowerBloodLine", towerBloodLinePool);
        objectPool.Add("EnemyBloodLine", enemyBloodLinePool);


    }

    private void Start()
    {
        
    }


    //获得对应对象
    public GameObject GetObject(GameObject prefabs)
    {

        //没有对应的对象池就生成一个
        if(!objectPool.ContainsKey(prefabs.tag))
        {
            List<GameObject> temp = new List<GameObject>();
            objectPool.Add(prefabs.tag, temp);
        }

        if (objectPool[prefabs.tag].Count == 0)
        {
            GameObject go = Instantiate(prefabs, Vector3.zero, Quaternion.identity);

            if (prefabs.tag == "Enemy")
            {
                SetPosition(go);
            }

            return go;
        }
        else
        {
            GameObject obj = objectPool[prefabs.tag][0];
            if (prefabs.tag == "Enemy")
            {
                prefabs.GetComponent<AICharacter>().ResetObject();
                SetPosition(obj);
            }
            objectPool[prefabs.tag].Remove(obj);
            return obj;
        }
    }

    //返还对象入池
    public void ReturnObject(GameObject prefabs)
    {
                    

        prefabs.SetActive(false);
        
        objectPool[prefabs.tag].Add(prefabs);

        if (prefabs.tag == "Enemy")     //入池的同时拿出新对象
        {
            GetObject(prefabs);

        }
    }

    

    private void SetPosition(GameObject obj)
    {
        obj.transform.SetParent(parent);
        tempPosition = Random.insideUnitCircle * randomArea;
        obj.transform.localPosition = new Vector3(tempPosition.x, 0, tempPosition.y);
        obj.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, randomArea);

    }

}
