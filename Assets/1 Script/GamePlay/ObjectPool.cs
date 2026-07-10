using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    PoolType PoolType { get; }

    GameObject GameObject { get; }
}

public enum PoolType
{
    None,

    Bullet,
    Enemy,
    Effect,
    Item,
    Arrow,
    Axe,
    ExpItem,

    //UI
    EnemyHpBar,

    //Monster
    BigDemon,
    BigZombie,
    Chort,
    Doc,
    Goblin,
    Imp,
    Lizard,
    MaskedOrc,
    Ogre,
    PumpkinDude,
    Skelet,
    Wizzard,
    Wogol,
}

public class ObjectPool : MonoBehaviour
{
    Dictionary<PoolType, Queue<GameObject>> pools = new Dictionary<PoolType, Queue<GameObject>>();

    Queue<GameObject> allObjectPool = new Queue<GameObject>();

    //처럼 Enum과 Prefab을 연결해주는 역할입니다.
    [System.Serializable]
    public class PoolData
    {
        public PoolType type;
        public GameObject prefab;
        public int poolSize;//몇개를 미리 생성할것이냐
    }

    [SerializeField]
    List<PoolData> poolList = new();

    public static ObjectPool instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        foreach (PoolData data in poolList)
        {
            pools[data.type] = new Queue<GameObject>();

            GameObject parent = new GameObject(data.type + "Pool");

            for (int i = 0; i < data.poolSize; i++)
            {
                GameObject go = Instantiate(data.prefab, parent.transform);
                go.SetActive(false);
                pools[data.type].Enqueue(go);

                allObjectPool.Enqueue(go);
            }
        }
    }

    public GameObject GetObject(PoolType type)
    {
        if (!pools.ContainsKey(type))
            return null;

        GameObject go;

        if (pools[type].Count > 0)
        {
            go = pools[type].Dequeue();
        }
        else
        {
            GameObject prefab = poolList.Find(x => x.type == type).prefab;
            go = Instantiate(prefab);
        }

        go.SetActive(true);

        return go;
    }

    public T GetObject<T>(PoolType type) where T : Component
    {
        return GetObject(type).GetComponent<T>();
    }

    //이 함수의 목적은"IPoolable을 구현한 오브젝트를 다시 풀에 넣는다."

    public void ReturnObject(GameObject go, PoolType type)
    {
        if (type == PoolType.None)
        {
            Debug.LogError($"{go.name}의 PoolType이 None입니다. 프리팹 Inspector에서 PoolType을 설정해주세요.");
            go.SetActive(false);
            return;
        }

        if (!pools.ContainsKey(type))
        {
            Debug.LogError($"풀에 등록되지 않은 PoolType입니다: {type}, Object: {go.name}");
            go.SetActive(false);
            return;
        }

        go.SetActive(false);
        pools[type].Enqueue(go);
    }

    public void ReturnObject(IPoolable obj)
    {
        ReturnObject(obj.GameObject, obj.PoolType);
    }

    public void ReturnObject<T>(PoolType type, T component) where T : Component
    {
        ReturnObject(component.gameObject, type);
    }

    public void GameOverAllPools()
    {

        foreach (var ao in allObjectPool)
        {
            ao.SetActive(false);
        }
    }
}
