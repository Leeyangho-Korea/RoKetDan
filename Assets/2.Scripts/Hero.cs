using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public GameObject bulletPrefab; // 총알 프리팹
    public Transform firePoint; // 총알이 발사될 위치
    public float bulletSpeed = 10f; // 총알 속도
    public float fireRate = 10f; //  (초당 발사 간격)
    private float nextFireTime = 0f; // 다음 발사 가능 시간

    private Camera mainCamera; // 메인 카메라 참조
    private bool isMousePressed = false; // 마우스 입력 감지 (사용자가 조준 중인지 확인)
    private List<GameObject> bulletPool = new List<GameObject>(); // 총알 풀
    [SerializeField] private Transform bulletParent; // 총알의 부모 객체

    void Start()
    {
        mainCamera = Camera.main; // 카메라 가져오기
    }

    void Update()
    {
        if (GameManager.instance.customTime >= nextFireTime)
        {
            // 마우스 왼쪽 버튼이 눌린 경우 → 사용자가 조준 사격
            if (Input.GetMouseButton(0))
            {
                isMousePressed = true;
                ShootAtMouse(); // 마우스 방향으로 총알 발사
            }
            else
            {
                isMousePressed = false;
            }

            // 자동 공격 (마우스 클릭이 없을 때만 실행)
            if (!isMousePressed)
            {
                AutoShootAtNearestMonster(); // 가장 가까운 몬스터 공격
            }
        }
        
    }

    /// <summary>
    /// 마우스 클릭한 방향으로 총알을 발사하는 함수
    /// </summary>
    void ShootAtMouse()
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // 2D 게임이므로 Z축 제거

        // 플레이어 위치에서 마우스 위치 방향 계산
        Vector2 direction = (mousePosition - firePoint.position).normalized;

        // 총알 발사
        FireBullet(direction);
    }

    /// <summary>
    /// 가장 가까운 몬스터 방향으로 자동 공격하는 함수
    /// </summary>
    void AutoShootAtNearestMonster()
    { 
        GameObject nearestMonster = FindNearestMonster(); // 가장 가까운 몬스터 찾기
        if (nearestMonster != null)
        {
            Vector2 direction = (nearestMonster.transform.position - firePoint.position).normalized;
            FireBullet(direction); // 몬스터 방향으로 총알 발사
        }
    }


    /// <summary>
    /// 총알을 풀링 시스템을 통해 발사
    /// </summary>
    /// <param name="direction">총알이 날아갈 방향</param>
    void FireBullet(Vector2 direction)
    {
        GameObject bullet = GetPooledBullet(); // 풀에서 총알 가져오기

        if (bullet != null)
        {
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            bullet.SetActive(true);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * bulletSpeed;
            }
            nextFireTime = GameManager.instance.customTime + fireRate; // 다음 공격 시간 설정
        }
    }

    /// <summary>
    /// 비활성화된 총알을 가져오거나, 없으면 새로 생성
    /// </summary>
    GameObject GetPooledBullet()
    {
        foreach (GameObject bullet in bulletPool)
        {
            if (!bullet.activeSelf) // 비활성화된 총알 찾기
            {
                return bullet;
            }
        }

        // 풀에 남아있는 총알이 없으면 새로 생성 (동적 확장)
        GameObject newBullet = Instantiate(bulletPrefab);
        newBullet.transform.parent = bulletParent;
        newBullet.SetActive(false);
        bulletPool.Add(newBullet);
        return newBullet;
    }

    /// <summary>
    /// 가장 가까운 몬스터를 찾는 함수
    /// </summary>
    /// <returns>가장 가까운 몬스터 GameObject (없으면 null)</returns>
    GameObject FindNearestMonster()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster"); // "Monster" 태그가 붙은 모든 객체 찾기
        GameObject nearestMonster = null;
        float minDistance = Mathf.Infinity; // 초기 최소 거리 무한대로 설정

        foreach (GameObject monster in monsters)
        {
            float distance = Vector2.Distance(transform.position, monster.transform.position);
            if (distance < minDistance) // 가장 가까운 몬스터 찾기
            {
                minDistance = distance;
                nearestMonster = monster;
            }
        }
        return nearestMonster;
    }
}
