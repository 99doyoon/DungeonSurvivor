using UnityEngine;

public class ExpItem : MonoBehaviour
{
    SpriteRenderer sr;

    int Exp;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void ChangeColor(Color color)
    {
        sr.color = color;
    }

    public void SetExp(int exp)
    {
        Exp = exp;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {

        }
    }
}
