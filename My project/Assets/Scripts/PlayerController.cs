using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Base Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator pAni;
    private bool isGrounded;
    private float moveInput;

    
    private bool isGiant = false;
    private float currentSpeed;
    private float currentJump; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pAni = GetComponent<Animator>();
        Time.timeScale = 1f;

       
        currentSpeed = moveSpeed;
        currentJump = jumpForce;
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 2f, groundLayer);

        
        if (isGiant)
        {
            if (moveInput < 0) transform.localScale = new Vector3(-10, 10, 10);
            else if (moveInput > 0) transform.localScale = new Vector3(10, 10, 10);
        }
        else
        {
            if (moveInput < 0) transform.localScale = new Vector3(-5, 5, 5);
            else if (moveInput > 0) transform.localScale = new Vector3(5, 5, 5);
        }
    }

    private void FixedUpdate()
    {
        
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Respawn")) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        if (collision.CompareTag("Finish")) collision.GetComponent<LevelObject>().MoveToNextLevel();

       
        if (collision.CompareTag("Enemy "))
        {
            if (isGiant) Destroy(collision.gameObject);
            else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        

        
        if (collision.CompareTag("Item"))
        {
            isGiant = true;
            CancelInvoke(nameof(ResetGiant));
            Invoke(nameof(ResetGiant), 3f);
            Destroy(collision.gameObject);
        }

        
        if (collision.CompareTag("banana"))
        {
            currentSpeed = moveSpeed * 2f; 
            CancelInvoke(nameof(ResetSpeed));
            Invoke(nameof(ResetSpeed), 3f);
            Destroy(collision.gameObject);
        }

        
        if (collision.CompareTag("apple"))
        {
            currentJump = jumpForce * 2f; 
            CancelInvoke(nameof(ResetJump));
            Invoke(nameof(ResetJump), 3f);
            Destroy(collision.gameObject);
        }
    }

    
    void ResetGiant() => isGiant = false;
    void ResetSpeed() => currentSpeed = moveSpeed;
    void ResetJump() => currentJump = jumpForce;

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
            // currentJump를 사용하여 점프
            rb.AddForce(Vector2.up * currentJump, ForceMode2D.Impulse);
            pAni.SetTrigger("Jump");
        }
    }
}