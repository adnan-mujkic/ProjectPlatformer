using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
   public float speed;
   public float jumpForce;
   public float fallMultiplier = 2.5f;
   public float lowJumpMultiplier = 2f;
   public Transform groundCheck;
   public float checkRadius;
   public LayerMask whatIsGround;
   Animator animator;
   Rigidbody rb;
   SpriteRenderer spriteRenderer;
   Player player;

   float moveInput;
   public bool facingRight;
   bool moving;
   public bool isGrounded;
   public static int maxJumps = 2;
   public int jumps = 2;
   public static bool SprintEnabled;
   public static bool Sprinting;
   public bool jumping;

   Coroutine jumpingCoroutine;

   // Start is called before the first frame update
   void Start() {
      animator = GetComponent<Animator>();
      rb = GetComponent<Rigidbody>();
      spriteRenderer = GetComponent<SpriteRenderer>();
      facingRight = true;
      player = FindObjectOfType<Player>();
      //SprintEnabled = true;
      //maxJumps = 2;
   }
   // Update is called once per frame
   void FixedUpdate() {
      isGrounded = Physics.OverlapSphere(groundCheck.position, checkRadius, whatIsGround).Length > 0;
      if (isGrounded && !jumping)
      {
         jumps = maxJumps;
      }

      moveInput = Input.GetAxisRaw("Horizontal");
      rb.velocity = GetMovementVelocity(moveInput * speed * ((SprintEnabled && Sprinting) ? 2f : 1f), rb.velocity.y);

      if (!facingRight && moveInput > 0)
      {
         Flip();
      }
      else if (facingRight && moveInput < 0)
      {
         Flip();
      }

      if (rb.velocity.y < 0)
      {
         rb.velocity += GetGravityVelocity(fallMultiplier, Time.deltaTime);
      }
      else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
      {
         rb.velocity += GetGravityVelocity(lowJumpMultiplier, Time.deltaTime);
      }
   }
   void Update() {
      if (Input.GetKeyDown(KeyCode.Space) && jumps > 0)
      {
         rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
         jumps--;
         jumping = true;
         if (jumpingCoroutine != null)
            StopCoroutine(jumpingCoroutine);
         StartCoroutine(RestartJumping());
      }
      if (rb.velocity.y != 0)
      {
         animator.SetBool("Jumping", true);
      }
      else
      {
         animator.SetBool("Jumping", false);
      }
      Sprinting = Input.GetKey(KeyCode.LeftShift);
      if (rb.velocity.x != 0)
      {
         moving = true;
      }
      else
      {
         moving = false;
      }
      animator.SetBool("Walking", moving);
   }
   void Flip() {
      facingRight = !facingRight;
      GetComponent<SpriteRenderer>().flipX = !facingRight;
      player.Laptop.GetComponent<SpriteRenderer>().flipX = !facingRight;
   }

   public static Vector3 GetGravityVelocity(float multiplier, float deltaTime) {
      return Vector3.up * (Physics.gravity.y * (multiplier - 1) * deltaTime);
   }
   public static Vector2 GetMovementVelocity(float force, float yVelocity) {
      return new Vector2(force, yVelocity);
   }

   IEnumerator RestartJumping() {
      for (int i = 0; i < 5; i++)
      {
         yield return new WaitForFixedUpdate();
      }
      jumping = false;
   }
}
