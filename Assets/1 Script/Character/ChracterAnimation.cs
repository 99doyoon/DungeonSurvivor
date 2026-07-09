using UnityEngine;

public abstract class ChracterAnimation : MonoBehaviour
{
    protected Animator anim;
    protected SpriteRenderer sr;
    protected Camera camera;
    protected readonly int isMoveHash = Animator.StringToHash("isMove");

    public abstract void CheckFlip();

    public abstract void CheckMove();

    public virtual void SettingAnimation()
    {
        camera = Camera.main;
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }
}
