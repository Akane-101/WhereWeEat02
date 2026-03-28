using System.Collections;
using UnityEngine;

public class DodgeCat : MonoBehaviour
{
    public static DodgeCat Instance;

    [Header("猫爪")]
    public Transform claw;
    public Animator clawAnimator;
    public GameObject clawObject;

    [Header("左右固定位置")]
    public float leftX = 2.82f;
    public float rightX = 8.77f;
    public float fixedY;
    public float fixedZ;

    [Header("节奏")]
    public float attackInterval = 2f;

    private bool isGameRunning = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (clawObject != null)
            clawObject.SetActive(false);

        // 记录初始Y/Z
        fixedY = claw.position.y;
        fixedZ = claw.position.z;
    }

    public void StartDodge()
    {
        if (clawObject != null)
            clawObject.SetActive(true);

        if (!isGameRunning)
            StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        isGameRunning = true;

        while (isGameRunning)
        {
            yield return new WaitForSeconds(attackInterval);
            yield return StartCoroutine(DoAttack());
        }
    }

    IEnumerator DoAttack()
    {
        bool isLeft = Random.value > 0.5f;

        Vector3 scale = claw.localScale;
        scale.x = isLeft ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        claw.localScale = scale;

        Vector3 pos = claw.position;
        pos.x = isLeft ? leftX : rightX;
        pos.y = fixedY;
        pos.z = fixedZ;
        claw.position = pos;

        // 动画
        if (clawAnimator != null)
            clawAnimator.SetTrigger("Extend");

        // 开启碰撞
        ClawHitbox hitbox = claw.GetComponent<ClawHitbox>();
        if (hitbox != null)
            hitbox.EnableHitbox();

        yield return new WaitForSeconds(0.2f);

        if (hitbox != null)
            hitbox.DisableHitbox();

        // 收回动画
        if (clawAnimator != null)
            clawAnimator.SetTrigger("Retract");

        yield return new WaitForSeconds(0.1f);
    }

    public void OnPlayerHit()
    {
        Debug.Log("被抓到了！");

        StopAllCoroutines();
        isGameRunning = false;

        StartDodge();
    }
}