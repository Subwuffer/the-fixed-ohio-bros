using UnityEngine;
using UnityEngine.InputSystem;

public class rbPlayerController : MonoBehaviour
{
    [SerializeField] private float camDistance = 7.5f;
    public Rigidbody2D rb;

    public Animator animator;

    public enum PlayerState
    {
        Idle,
        Walking,
        Running,
        Jumping,
        SprintJumping
    }
    public PlayerState State;
    [SerializeField] private float playerSpeed = 1.0f;
    [SerializeField] private float jumpHeight = 0.0f;
    [SerializeField] private bool grounded;
    private Vector2 movementInput = Vector2.zero;
    private bool jumped = false;
    private bool sprintin = false;
    private bool attacked = false;
    private float stockCounter = 3;
    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        jumped = context.action.triggered;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        sprintin = context.action.triggered;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        attacked = context.action.triggered;
    }

    /*void OnCollisionEnter(Collision col)
     {
         if(col.gameObject.tag == "PlayerTarget") // Do not forget assign tag to the field
         {
             rb2 = col.gameobject.GetComponent<Rigidbody>();    
             rb2.AddForce(transform.right * kickForce);
         }
     }*/

    void Update()
    {
        if (!grounded && sprintin)
        {
            State = PlayerState.SprintJumping;
            animator.SetBool("Run", true);
        }
        else if (!grounded)
        {
            State = PlayerState.Jumping;
            animator.SetBool("Run", true);
        }
        else if (Mathf.Abs(movementInput.x) < 0.1f)
        {
            State = PlayerState.Idle;
            animator.SetBool("Run", false);
        }
        else if (movementInput.x != 0 && grounded && !sprintin)
        {
            State = PlayerState.Walking;
            animator.SetBool("Run", true);
        }
        else if (movementInput.x != 0 && grounded && sprintin)
        {
            State = PlayerState.Running;
            animator.SetBool("Run", true);
        }
        Vector3 move = new Vector2(movementInput.x, 0);
        float speedMultiplier = sprintin ? 4 : 2;
        rb.AddForce(move * Time.deltaTime * playerSpeed * speedMultiplier);
        if (jumped && grounded)
        {
            rb.AddForce(Vector3.up * jumpHeight, ForceMode2D.Impulse);
            grounded = false;
        }

        if (transform.position.x >= 30 || transform.position.x <= -30 || transform.position.y >= 30 || transform.position.y <= -30)
        {
            transform.position = new Vector3(0, 0, 0);
            stockCounter = stockCounter - 1;
        }

        if (stockCounter == 0)
        {
            Destroy(gameObject);
        }

        /*if (attacked)
        {

        }*/
        sr.flipX = movementInput.x < 0 ? false : (movementInput.x > 0 ? true : sr.flipX);
    }

    void FixedUpdate()
    {
        if (movementInput == Vector2.zero)
        {
            rb.velocity = new Vector2(rb.velocity.x / 1.1f, rb.velocity.y);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "PlayerTarget")
        {
            grounded = true;
        }
        if (collision.gameObject.tag == "Die")
        {
            Destroy(gameObject);
        }
    }
}