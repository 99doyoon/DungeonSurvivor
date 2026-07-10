using System.Collections;
using UnityEngine;

public class PlayerStatus : CharacterStatus, IHit
{
    PlayerAnimationController playerAnimationController;

    public int Level { get; private set; }
    public int CurrentExp { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MoveSpeed = 5;
        maxHp = 100;
        nowHp = maxHp;
        Level = 1;
        CurrentExp = 0;

        playerAnimationController = GetComponentInChildren<PlayerAnimationController>();

        isHit = true;
        takeDamageDelay = 1;
        wait = new WaitForSeconds(takeDamageDelay);
    }

    protected override void Die()
    {
#if UNITY_EDITOR
        Debug.Log("Player Die");
#endif
    }
    public bool Hit()
    {
        if(isHit)
        {
            StartCoroutine(SetIsHit());
            playerAnimationController.HitAnimation();
            return true;
        }
        else
        {
            return false;
        }
    }

    //takeDamageDelay동안 데미지를 입지않기위해 isHit을 false로 만든다
    public IEnumerator SetIsHit()
    {
        isHit = false;
        yield return wait;
        isHit = true;
    }

    public void AddExp(int exp)
    {
        CurrentExp += exp;
    }

    public void AddLevel(int level)
    {
        Level += level;
    }

    public void SetExp(int exp)
    {
        CurrentExp = exp;
    }

    public void SetLevel(int level)
    {
        Level = level;
    }
}
