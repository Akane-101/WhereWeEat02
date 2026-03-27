using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private float moveInput;

    public bool canMove = true;

    [Header("动画控制")]
    public Animator animator;
    private bool facingRight = true;
    private bool isEating = false;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            GameObject playerAnimObj = GameObject.Find("Player");
            if (playerAnimObj != null)
                animator = playerAnimObj.GetComponent<Animator>();
            else
                Debug.LogError("找不到名为 'Player' 的 Animator 对象！");
        }
    }

    void Update()
    {
        if (!canMove)
        {
            moveInput = 0f;

            // 强制待机动画
            if (animator != null)
                animator.SetBool("IsMoving", false);

            return;
        }

        // 正常输入
        moveInput = 0f;
        if (Input.GetKey(KeyCode.A))
            moveInput = -1f;
        else if (Input.GetKey(KeyCode.D))
            moveInput = 1f;

        // 动画控制
        if (animator != null)
            animator.SetBool("IsMoving", moveInput != 0f);

        // 翻转（只在能移动时才允许）
        if (moveInput < 0 && !facingRight)
            Flip();
        else if (moveInput > 0 && facingRight)
            Flip();
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            return;
        }

        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void PlayEat()
    {
        if (animator == null) return;

        // 防止重复触发（很关键）
        if (isEating) return;

        isEating = true;

        animator.SetTrigger("Eat");

        // 自动解除状态（假设动画0.5秒，可调整）
        StartCoroutine(EatRoutine());
    }

    IEnumerator EatRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        isEating = false;
    }
}