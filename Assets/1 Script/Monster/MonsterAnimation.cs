using UnityEngine;
using UnityEngine.InputSystem;

public class MonsterAnimation : ChracterAnimation
{
    [SerializeField] MonsterMoveController monsterMoveController;

    int isHitHash = Animator.StringToHash("isHit");

    private Color originColor;

    private void Awake()
    {
        SettingAnimation();
        originColor = sr.color;
        monsterMoveController= GetComponentInChildren<MonsterMoveController>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckFlip();
        CheckMove();
    }


    //몬스터가 캐릭터를 바라보기위한 함수
    public override void CheckFlip()
    {
        sr.flipX = transform.position.x > monsterMoveController.player.position.x ? true : false;
    }


    //움직일경우 isMove를 통해 moveAnimation동작
    public override void CheckMove()
    {

        if (monsterMoveController.isTrace)
        {
            //움직일때 동작
            anim.SetBool(isMoveHash, true);
        }
        else
        {
            //안움직일 때 애니매이션 동작
            anim.SetBool(isMoveHash, false);
        }

    }
}
