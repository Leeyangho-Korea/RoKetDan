using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP : MonoBehaviour
{
    public SpriteMask spriteMask;
    public float fillAmount = 1f; // 0 ~ 1 사이의 값 (HP 바 진행도)
    private Vector3 initialScale;
    private Vector3 initialPosition;
    private bool isInitialized = false; // 초기화 완료 여부 체크

    void Awake()
    {
        initialScale = spriteMask.transform.localScale;
        initialPosition = spriteMask.transform.localPosition;
        isInitialized = true; // 초기화 완료
    }

    private void OnEnable()
    {
        if (!isInitialized)
        {
            initialScale = spriteMask.transform.localScale;
            initialPosition = spriteMask.transform.localPosition;
            isInitialized = true; // 초기화 완료
        }
      //  Invoke("SetOffHP", 0.5f);
    }

    private void OnDisable()
    {
        CancelInvoke("SetOffHP");
    }

    public void SetHP(float amount)
    {
        fillAmount = Mathf.Clamp01(amount); // 0~1 값으로 제한

        // HP 바 크기 조절 (0보다 작아지거나 1보다 커지지 않도록 보정)
        float newScaleX = Mathf.Clamp(initialScale.x * fillAmount, 0.01f, initialScale.x);
        spriteMask.transform.localScale = new Vector3(newScaleX, initialScale.y, initialScale.z);

        // 마스크 위치 조정 (오른쪽에서 왼쪽으로 감소)
        float maskOffset = (1 - fillAmount) * initialScale.x * 0.5f;
        spriteMask.transform.localPosition = new Vector3(initialPosition.x - maskOffset, initialPosition.y, initialPosition.z);


        CancelInvoke("SetOffHP");
        Invoke("SetOffHP", 0.8f);
    }

    private void SetOffHP()
    {
        gameObject.SetActive(false);
    }
}
