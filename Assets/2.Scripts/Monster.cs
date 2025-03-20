using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] HP hp;

    public float moveSpeed = 2f; // 왼쪽으로 이동 속도
    public float jumpForce = 5f; // 점프 힘
    public float jumpCooldown = 2f; // 점프 쿨타임
    private bool canJump = true;
    private bool pushBack = false;
    private Rigidbody2D rb;
    Coroutine moveCo = null; // SmoothMove 코루틴을 저장할 변수

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if(hp == null)
        {
            hp = GetComponentInChildren<HP>();
        }
    }

    private void OnDisable()
    {
        if (moveCo != null)
        {
            StopCoroutine(moveCo);
            moveCo = null;
        }
    }

    void Update()
    {
        if(pushBack == false)
        {
            // 왼쪽으로 이동
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
        }

#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Damage(10);
        }
#endif
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 트럭 또는 몬스터와 닿으면 점프 (단, 몬스터는 특정 레이어여야 함)
        if ((collision.gameObject.CompareTag("Truck") || IsTargetMonster(collision.gameObject)) && canJump)
        {
            Jump();
        }

        // 위에서 몬스터를 밟았을 때, 밟힌 몬스터는 뒤로 밀림
        if (IsTargetMonster(collision.gameObject))
        {
            if (collision.relativeVelocity.y < 0) // 위에서 밟았을 경우
            {
                Rigidbody2D otherRb = collision.gameObject.GetComponent<Rigidbody2D>();

                // 아래 공룡이 점프하는 중이고, 위 공룡이 있다면 위 공룡이 튀지 않도록 제한
                if (otherRb.velocity.y > 0 && rb.velocity.y > 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0); // 위 공룡 속도를 0으로 제한
                }
                else
                {
                    PushBack();
                }

            }
        }
    }

    void Jump()
    {
        if (!canJump) return; // 중복 실행 방지

        canJump = false;

        // 기존 속도를 초기화하여 예측 불가능한 반동 제거
        rb.velocity = new Vector2(rb.velocity.x, 0);

        // `AddForce()` 대신 `velocity`를 직접 설정하여 점프
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        StartCoroutine(JumpCooldown());
    }


    IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }
    void FixedUpdate()
    {
        // 과하게 높은 점프를 감지하면 강제로 제한
        if (rb.velocity.y > jumpForce)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // 강제 제한
        }
    }

    public float pushForce = 2f; // 뒤로 밀리는 힘

    public void PushBack()
    {
        if (pushBack) return;

        pushBack = true;

        // 몬스터 크기만큼만 이동하도록 변경 (자연스럽게 떨어지도록)
        float dinoWidth = 1.0f; // 몬스터 한 마리가 차지하는 너비
        Vector2 targetPosition = rb.position + new Vector2(dinoWidth, 0);

        if (moveCo != null)
        {
            StopCoroutine(moveCo);
            moveCo = null;
        }
        moveCo =  StartCoroutine(SmoothMove(targetPosition));

            // 뒤에 있는 몬스터도 같이 밀리도록 처리
            Vector2 checkPosition = transform.position + new Vector3(dinoWidth, 0, 0);
        Vector2 boxSize = new Vector2(dinoWidth, 1f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(checkPosition, boxSize, 0);
        foreach (var hit in hits)
        {
            if (IsTargetMonster(hit.gameObject))
            {
                Monster otherMonster = hit.gameObject.GetComponent<Monster>();
                if (otherMonster != null && !otherMonster.pushBack)
                {
                    otherMonster.PushBack();
                }
                break;
            }
        }

        StartCoroutine(ResetPushBack());
    }

    // 한 칸씩 이동하도록 부드럽게 이동
    IEnumerator SmoothMove(Vector2 targetPos)
    {
        float duration = 0.3f;
        float elapsedTime = 0f;
        Vector2 startPos = rb.position;

        while (elapsedTime < duration)
        {
            rb.MovePosition(Vector2.Lerp(startPos, targetPos, elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(targetPos);
    }

    // 일정 시간 후 pushBack 해제
    IEnumerator ResetPushBack()
    {
        yield return new WaitForSeconds(0.8f);
        pushBack = false;
    }

    // 특정 태그와 레이어를 동시에 확인하는 함수
    bool IsTargetMonster(GameObject obj)
    {
        return obj.CompareTag("Monster") && obj.layer == gameObject.layer && obj.activeSelf == true;
    }


    public void Damage(int i)
    {
        if(hp.gameObject.activeSelf == false)
        {
            hp.gameObject.SetActive(true);
        }
        DEF.Log($"[Monster] 데미지: {i}");
        hp.SetHP(hp.fillAmount - (float)i / DEF.MonsterHP);
        if (hp.fillAmount <=0)
        {
            Dead();
        }
    }

    public void HpInit()
    {
        if (hp.gameObject.activeSelf == false)
        {
            hp.gameObject.SetActive(true);
        }
        hp.SetHP(1.0f);
    }
    
 

    private void Dead()
    {
       gameObject.SetActive(false);
    }
 }
