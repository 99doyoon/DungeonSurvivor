using UnityEngine;

public class AttackTouch : MonoBehaviour
{
    [SerializeField] protected float damage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
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
}
