using UnityEngine;

public class MonsterMoveController : MonoBehaviour
{
    public Transform player;

    public bool isTrace {  get; private set; }

    float attackDistance;
    float speed;

    Rigidbody2D rb;

    SpriteRenderer sr;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        attackDistance = 3;
        speed = 3;
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistance();
    }

    //특정 거리보다 가까우면 추적 아니면 공격
    void CheckDistance()
    {
        float distance;
        distance = Vector2.Distance(transform.position, player.position);
        isTrace = distance > attackDistance;
        //공격거리보다 멀면 따라간다.
        if(isTrace)
        {
            Trace();
        }
        else
        {
            //공격시 행동
        }        
    }

    void Trace()
    {
        Move();
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }
}
