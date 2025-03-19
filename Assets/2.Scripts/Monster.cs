using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public float moveSpeed = -2f;  // 왼쪽으로 이동
    public float jumpForce = 5f;  // 점프 힘

    private Rigidbody2D rb;

    private bool _isLeft_Truck = false;  // 트럭과 닿았는지 확인
    private bool _isLeft_Monster = false;  // 몬스터와 닿았는지 확인
    private bool _isUp_Monster = false; // 위에 몬스터가 있는지 확인
    private float jumpCooldown = 2f;  // 점프 쿨다운
    private float lastJumpTime = 0f;  // 마지막 점프 시간 저장

    // 밀리는 힘과 최대 이동 거리 설정
    public float pushForce = 4f;  // 밀리는 힘
    public float maxPushDistance = 1.5f; // 최대 밀려날 거리
    private Vector3 initialPosition; // 시작 위치 저장

    void Start()
    {
        TryGetComponent<Rigidbody2D>(out rb);
        Collider2D col = GetComponent<Collider2D>();
        col.sharedMaterial = Resources.Load<PhysicsMaterial2D>("MonsterMaterial"); // 물리 재질 적용

        initialPosition = transform.position; // 시작 위치 저장
    }

    public void Init()
    {
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.isKinematic = false;
            rb.simulated = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.freezeRotation = true;
            DEF.LogError("Rigidbody2D 컴포넌트를 새로 생성합니다.");
        }
    }

    void Update()
    {

        if (CanJump())  // 앞이나 아래에 몬스터가 있으면 점프 시도
        {
            if ( _isLeft_Truck == false && _isLeft_Monster)  // 트럭과 닿지 않고 몬스터와 닿아있다면
            {
                Jump();
            }
        }

        // 위에 몬스터가 있으면 점진적으로 밀려나게 함
        if (_isUp_Monster)
        {
            if(_isLeft_Monster)
            {
                _isUp_Monster = false;
            }
            PushBack();
        }
        else
        {
            if (_isLeft_Truck == false && _isLeft_Monster == false)  // 트럭과 충돌하기 전까지 계속 이동
            {
                transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 collisionDirection = collision.contacts[0].point - (Vector2)transform.position;
        collisionDirection = collisionDirection.normalized; // 방향 벡터 정규화
     if (collisionDirection.y < -0.5f) // 아래에서 충돌
        {

        }
        else if (collisionDirection.x > 0.5f) // 오른쪽에서 충돌
        {

        }
        else if (collisionDirection.x < -0.5f) // 왼쪽에서 충돌
        {
            if (collision.gameObject.CompareTag("Truck"))  // 트럭과 충돌하면 정지
            {
                DEF.Log(" 트럭과 충돌");
                _isLeft_Truck = true;
                StopMoving();
            }
            if (collision.gameObject.CompareTag("Monster")) //몬스터와 충돌
            {
                DEF.Log("LEFT : 몬스터와 충돌");
                _isLeft_Monster = true;
                StopMoving();
            }
        }
        else if(collisionDirection.y > 0.5f) // 위에서 충돌
        {
            if (collision.gameObject.CompareTag("Monster")) //몬스터와 충돌
            {
                DEF.Log(" UP : 몬스터와 충돌");
                _isUp_Monster = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Truck"))  // 트럭에서 벗어나면 이동 재개
        {
            _isLeft_Truck = false;
        }
        if (collision.gameObject.CompareTag("Monster")) //몬스터와 충돌
        {
            _isLeft_Monster = false;
            _isUp_Monster = false;
        }
     
    }

    void StopMoving()
    {
        rb.velocity = Vector2.zero;  // 이동 정지
    }

    bool CanJump()
    {
        return GameManager.instance.customTime - lastJumpTime > jumpCooldown;  // 점프 쿨다운 적용
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);  // 점프 실행
        lastJumpTime = GameManager.instance.customTime;  // 마지막 점프 시간 저장
    }

    // 위에 몬스터가 있는지 확인하는 함수
    bool IsUnderAnotherMonster()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 1.5f, LayerMask.GetMask("Monster"));
        return hit.collider != null;
    }

    // 위에 몬스터가 있을 경우 밀려나게 하는 함수
    void PushBack()
    {
        Vector3 targetPosition = new Vector3(initialPosition.x + maxPushDistance, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, pushForce * Time.deltaTime);
    }
}
