using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public float moveSpeed = -2f;  // �������� �̵�
    public float jumpForce = 5f;  // ���� ��

    private Rigidbody2D rb;

    private bool _isLeft_Truck = false;  // Ʈ���� ��Ҵ��� Ȯ��
    private bool _isLeft_Monster = false;  // ���Ϳ� ��Ҵ��� Ȯ��
    private bool _isUp_Monster = false; // ���� ���Ͱ� �ִ��� Ȯ��
    private float jumpCooldown = 2f;  // ���� ��ٿ�
    private float lastJumpTime = 0f;  // ������ ���� �ð� ����

    // �и��� ���� �ִ� �̵� �Ÿ� ����
    public float pushForce = 4f;  // �и��� ��
    public float maxPushDistance = 1.5f; // �ִ� �з��� �Ÿ�
    private Vector3 initialPosition; // ���� ��ġ ����

    void Start()
    {
        TryGetComponent<Rigidbody2D>(out rb);
        Collider2D col = GetComponent<Collider2D>();
        col.sharedMaterial = Resources.Load<PhysicsMaterial2D>("MonsterMaterial"); // ���� ���� ����

        initialPosition = transform.position; // ���� ��ġ ����
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
            DEF.LogError("Rigidbody2D ������Ʈ�� ���� �����մϴ�.");
        }
    }

    void Update()
    {

        if (CanJump())  // ���̳� �Ʒ��� ���Ͱ� ������ ���� �õ�
        {
            if ( _isLeft_Truck == false && _isLeft_Monster)  // Ʈ���� ���� �ʰ� ���Ϳ� ����ִٸ�
            {
                Jump();
            }
        }

        // ���� ���Ͱ� ������ ���������� �з����� ��
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
            if (_isLeft_Truck == false && _isLeft_Monster == false)  // Ʈ���� �浹�ϱ� ������ ��� �̵�
            {
                transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 collisionDirection = collision.contacts[0].point - (Vector2)transform.position;
        collisionDirection = collisionDirection.normalized; // ���� ���� ����ȭ
     if (collisionDirection.y < -0.5f) // �Ʒ����� �浹
        {

        }
        else if (collisionDirection.x > 0.5f) // �����ʿ��� �浹
        {

        }
        else if (collisionDirection.x < -0.5f) // ���ʿ��� �浹
        {
            if (collision.gameObject.CompareTag("Truck"))  // Ʈ���� �浹�ϸ� ����
            {
                DEF.Log(" Ʈ���� �浹");
                _isLeft_Truck = true;
                StopMoving();
            }
            if (collision.gameObject.CompareTag("Monster")) //���Ϳ� �浹
            {
                DEF.Log("LEFT : ���Ϳ� �浹");
                _isLeft_Monster = true;
                StopMoving();
            }
        }
        else if(collisionDirection.y > 0.5f) // ������ �浹
        {
            if (collision.gameObject.CompareTag("Monster")) //���Ϳ� �浹
            {
                DEF.Log(" UP : ���Ϳ� �浹");
                _isUp_Monster = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Truck"))  // Ʈ������ ����� �̵� �簳
        {
            _isLeft_Truck = false;
        }
        if (collision.gameObject.CompareTag("Monster")) //���Ϳ� �浹
        {
            _isLeft_Monster = false;
            _isUp_Monster = false;
        }
     
    }

    void StopMoving()
    {
        rb.velocity = Vector2.zero;  // �̵� ����
    }

    bool CanJump()
    {
        return GameManager.instance.customTime - lastJumpTime > jumpCooldown;  // ���� ��ٿ� ����
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);  // ���� ����
        lastJumpTime = GameManager.instance.customTime;  // ������ ���� �ð� ����
    }

    // ���� ���Ͱ� �ִ��� Ȯ���ϴ� �Լ�
    bool IsUnderAnotherMonster()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 1.5f, LayerMask.GetMask("Monster"));
        return hit.collider != null;
    }

    // ���� ���Ͱ� ���� ��� �з����� �ϴ� �Լ�
    void PushBack()
    {
        Vector3 targetPosition = new Vector3(initialPosition.x + maxPushDistance, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, pushForce * Time.deltaTime);
    }
}
