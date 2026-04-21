using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator pAni;
    private bool isGrounded;
    private float moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pAni = GetComponent<Animator>();

        // 🛠️ 해결책 1: 씬이 시작될 때 멈춰있던 시간을 정상 속도(1)로 되돌려줍니다.
        Time.timeScale = 1f;
    }

    private void Update()
    {
        // 🛠️ 개선점: Update에 중복 작성되어 있던 코드를 하나로 정리했습니다.
        // 바닥 체크는 프레임마다 확인하는 것이 좋습니다.
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        // 캐릭터 방향 전환
        if (moveInput < 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput > 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    // 🛠️ 개선점: 이전 답변들에서 언급했듯, 물리적 이동은 FixedUpdate에서 처리해야 안정적입니다.
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveInput = input.x;
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            pAni.SetTrigger("Jump");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Respawn과 Enemy는 결과가 같으므로 하나로 합치면 코드가 더 짧아집니다.
        if (collision.CompareTag("Respawn") || collision.CompareTag("Enemy"))
        {
            Time.timeScale = 1f; // 혹시 몰라 재시작 전에도 시간을 초기화합니다.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (collision.CompareTag("Finish"))
        {
            // 🛠️ 해결책 2: 다음 레벨로 넘어가기 직전에 시간을 정상화하는 방어 코드를 추가합니다.
            Time.timeScale = 1f;
            collision.GetComponent<LevelObject>().MoveToNextLevel();
        }
    }
}