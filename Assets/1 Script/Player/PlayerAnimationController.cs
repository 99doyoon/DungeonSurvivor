using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationController : ChracterAnimation
{
    [SerializeField] PlayerMoveController playerMoveController;

    int isHitHash = Animator.StringToHash("isHit");

    private Color originColor;

    private void Awake()
    {
        SettingAnimation();
        originColor = sr.color;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        CheckFlip();
        CheckMove();
    }

    //마우스커서의 x축에따라 캐릭터가 왼쪽을 볼지 오른쪽을 볼지 선택
    public override void CheckFlip()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        if (transform.position.x < worldPos.x)
        {
            sr.flipX = false;
        }
        else
        {
            sr.flipX = true;
        }
    }

    //움직일경우 isMove를 통해 moveAnimation동작
    public override void CheckMove()
    {

        if (playerMoveController.Dir==Vector2.zero)
        {
            //안움직일 때 애니매이션 동작
            anim.SetBool(isMoveHash, false);
        }
        else
        {
            //움직일때 동작
            anim.SetBool(isMoveHash, true);
        }

    }

    public void HitAnimation()
    {
        anim.SetTrigger(isHitHash);
        sr.DOColor(Color.red, 0.5f)
      .OnComplete(() => sr.DOColor(originColor, 0.5f));
    }
}
