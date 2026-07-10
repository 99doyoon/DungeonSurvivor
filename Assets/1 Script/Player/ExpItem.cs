using UnityEngine;

public class ExpItem : MonoBehaviour, IPoolable
{
    SpriteRenderer sr;

    int exp = 10;

    public PoolType PoolType => PoolType.ExpItem;

    public GameObject GameObject => gameObject;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void ChangeColor(Color color)
    {
        sr.color = color;
    }

    public void SetExp(int setExp)
    {
        exp = setExp;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collision.gameObject.GetComponent<ExpManger>().GetExp(exp);
            ObjectPool.instance.ReturnObject(this);
        }
    }
}
