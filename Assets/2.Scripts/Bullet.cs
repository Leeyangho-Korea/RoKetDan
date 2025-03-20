using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 몬스터 피격 시 데미지 적용
        if (collision.CompareTag("Monster"))
        {
            Monster monster = null;
            if (collision.TryGetComponent(out monster))
            {
                monster.Damage(DEF.Attack);
               gameObject.SetActive(false);
            }
            else
            {
                DEF.LogWarning($"collision : {collision.name} /  Monster Component is nulll");
            }
        }
    }

    private void OnEnable()
    {
        // 총알이 활성화될 때, 5초 후 자동 비활성화 예약
        Invoke(nameof(DeactivateBullet), 3f);
    }
    private void OnDisable()
    {
        //  비활성화될 때, `Invoke` 취소 (불필요한 실행 방지)
        CancelInvoke(nameof(DeactivateBullet));
    }

    /// <summary>
    /// 총알을 비활성화하여 오브젝트 풀로 반환
    /// </summary>
    private void DeactivateBullet()
    {
        gameObject.SetActive(false);
    }


}
