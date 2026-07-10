using UnityEngine;

public class EnemyHpBar : HpBar, IPoolable
{
    private EnemyBase target;
    private Camera mainCam;

    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, 0f);

    public PoolType PoolType => PoolType.EnemyHpBar;
    public GameObject GameObject => gameObject;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void LateUpdate()
    {
        if (target == null || !target.gameObject.activeSelf)
        {
            target = null;
            ObjectPool.instance.ReturnObject(this);
            return;
        }

        if (mainCam == null)
            mainCam = Camera.main;

        Vector3 worldPos = target.transform.position + offset;
        transform.position = mainCam.WorldToScreenPoint(worldPos);

        SetGage(target.CurrentHp / target.GetMaxHp());
    }

    public void SetTarget(EnemyBase monster)
    {
        target = monster;

        if (target == null)
            return;

        SetGage(target.CurrentHp / target.GetMaxHp());
    }
}