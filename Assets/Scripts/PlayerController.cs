using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private float moveInput;

    [Header("动画控制")]
    public Animator animator;

    private bool facingRight = true; // 记录当前朝向

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 自动获取名为 "Player" 的 Animator
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
        // 获取水平输入
        moveInput = 0f;
        if (Input.GetKey(KeyCode.A))
            moveInput = -1f;
        else if (Input.GetKey(KeyCode.D))
            moveInput = 1f;

        // 设置动画布尔值
        animator.SetBool("IsMoving", moveInput != 0f);

        // 翻转角色
        if (moveInput < 0 && !facingRight)
            Flip();
        else if (moveInput > 0 && facingRight)
            Flip();
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1; // 水平翻转
        transform.localScale = scale;
    }
}