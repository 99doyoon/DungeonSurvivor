using UnityEngine;

public class AttackTouch : MonoBehaviour
{
    [SerializeField] protected float damage;

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerStatus ps = collision.gameObject.GetComponent<PlayerStatus>();
            bool isHit = ps.Hit();
            if (isHit)
            {
                ps.TakeDamage(damage);
            }
        }
    }

    protected void SetDamage(float setDamage)
    {
        damage = setDamage;
    }
}
