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
    BossMonster,

    //MonsterBullet
    Axe,
    Fireball,
    Flask,
    MonsterEffect,
    ThrowRock,

    //Effect
    ExplosionEffect,
    DamageText
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
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
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
        if (!pools.TryGetValue(type, out Queue<GameObject> pool))
        {
            Debug.LogError($"{type} 타입의 풀이 등록되어 있지 않습니다.");
            return null;
        }

        GameObject go = null;

        // Queue 안에 남아 있는 파괴된 참조를 모두 건너뛴다.
        while (pool.Count > 0 && go == null)
        {
            go = pool.Dequeue();
        }

        // 사용할 수 있는 오브젝트가 없으면 새로 생성한다.
        if (go == null)
        {
            PoolData poolData = poolList.Find(x => x.type == type);

            if (poolData == null || poolData.prefab == null)
            {
                Debug.LogError($"{type} 타입의 프리팹이 Pool List에 등록되지 않았습니다.");
                return null;
            }

            go = Instantiate(poolData.prefab);
        }

        go.SetActive(true);
        return go;
    }

    public T GetObject<T>(PoolType type) where T : Component
    {
        GameObject go = GetObject(type);

        if (go == null)
        {
            Debug.LogError(
                $"{type} 타입의 오브젝트를 풀에서 가져오지 못했습니다. " +
                "ObjectPool의 Pool List 등록을 확인하세요.");

            return null;
        }

        if (!go.TryGetComponent(out T component))
        {
            Debug.LogError(
                $"{go.name} 프리팹에 {typeof(T).Name} 컴포넌트가 없습니다.");

            go.SetActive(false);
            pools[type].Enqueue(go);

            return null;
        }

        return component;
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
