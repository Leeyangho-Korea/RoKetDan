using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] HP hp; // HP 스크립트
    public float moveSpeed = 2f; // 몬스터 이동 속도
    public float jumpForce = 5f; // 몬스터 점프 힘
    public float jumpCooldown = 2f; // 점프 쿨타임

    private bool canJump = true; // 점프 가능 여부
    private bool pushBack = false; // 뒤로 밀리는 중인지 여부
    private Rigidbody2D rb;
    private Coroutine moveCo = null; // 밀릴 때 실행되는 코루틴 저장 변수

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hp ??= GetComponentInChildren<HP>(); // HP가 없으면 자식 오브젝트에서 찾기
    }

    private void OnEnable()
    {
        canJump = true;
        pushBack = false;
    }

    private void OnDisable()
    {
        // 몬스터가 비활성화될 때 실행 중이던 밀림 코루틴 정리
        if (moveCo != null)
        {
            StopCoroutine(moveCo);
        }
    }

    void FixedUpdate()
    {
        if (!pushBack)
        {
            // 일정한 속도로 왼쪽으로 이동
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
        }

        // 점프 높이 제한 (불필요한 높이 상승 방지)
        if (rb.velocity.y > jumpForce)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 트럭 또는 몬스터를 만나면 점프 (단, 특정 레이어의 몬스터만 가능)
        if ((collision.gameObject.CompareTag("Truck") || IsTargetMonster(collision.gameObject)) && canJump)
        {
            Jump();
        }

        // 위에서 몬스터를 밟았을 경우 밑에 있는 몬스터를 뒤로 밀기
        if (IsTargetMonster(collision.gameObject) && collision.relativeVelocity.y < 0)
        {
            Rigidbody2D otherRb = collision.gameObject.GetComponent<Rigidbody2D>();

            // 아래 몬스터가 점프하는 중이라면 위에 있는 몬스터는 밀리지 않도록 제한
            if (otherRb.velocity.y > 0 && rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
            else
            {
                PushBack();
            }
        }
    }

    /// <summary>
    /// 몬스터가 점프하는 동작
    /// </summary>
    void Jump()
    {
        if (!canJump) return;

        canJump = false;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        StartCoroutine(JumpCooldown());
    }

    /// <summary>
    /// 일정 시간이 지나면 다시 점프 가능하도록 설정
    /// </summary>
    IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    /// <summary>
    /// 몬스터가 뒤로 밀리는 동작
    /// </summary>
    public void PushBack()
    {
        if (pushBack) return;

        pushBack = true;
        float dinoWidth = 1.0f; // 몬스터 한 마리의 너비
        Vector2 targetPosition = rb.position + new Vector2(dinoWidth, 0); // 뒤로 이동할 위치

        if (moveCo != null)
        {
            StopCoroutine(moveCo);
        }
        moveCo = StartCoroutine(SmoothMove(targetPosition));

        // 뒤에 있는 몬스터도 같이 밀리도록 처리
        Vector2 checkPosition = transform.position + new Vector3(dinoWidth, 0, 0);
        Collider2D[] hits = Physics2D.OverlapBoxAll(checkPosition, new Vector2(dinoWidth, 1f), 0);

        foreach (var hit in hits)
        {
            if (IsTargetMonster(hit.gameObject))
            {
                hit.gameObject.GetComponent<Monster>()?.PushBack();
                break; // 한 마리만 밀리도록 설정
            }
        }

        StartCoroutine(ResetPushBack());
    }

    /// <summary>
    /// 몬스터가 부드럽게 한 칸씩 이동하는 연출
    /// </summary>
    IEnumerator SmoothMove(Vector2 targetPos)
    {
        float duration = 0.3f; // 이동 시간
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

    /// <summary>
    /// 일정 시간이 지나면 밀림 상태 해제
    /// </summary>
    IEnumerator ResetPushBack()
    {
        yield return new WaitForSeconds(0.8f);
        pushBack = false;
    }

    /// <summary>
    /// 특정 오브젝트가 몬스터인지 확인 (태그 + 레이어 체크)
    /// </summary>
    bool IsTargetMonster(GameObject obj)
    {
        return obj.CompareTag("Monster") && obj.layer == gameObject.layer && obj.activeSelf;
    }

    /// <summary>
    /// 몬스터가 피해를 입었을 때 HP 감소 처리
    /// </summary>
    public void Damage(int damage)
    {
        if (!hp.gameObject.activeSelf)
        {
            hp.gameObject.SetActive(true);
        }

        hp.SetHP(hp.fillAmount - (float)damage / DEF.MonsterHP);
        if (hp.fillAmount <= 0) Dead();
    }

    /// <summary>
    /// 몬스터 HP 초기화 (최대 HP)
    /// </summary>
    public void HpInit()
    {
        if (!hp.gameObject.activeSelf)
        {
            hp.gameObject.SetActive(true);
        }
        hp.SetHP(1.0f);
    }

    /// <summary>
    /// 몬스터가 사망하면 비활성화
    /// </summary>
    private void Dead()
    {
        gameObject.SetActive(false);
    }
}
