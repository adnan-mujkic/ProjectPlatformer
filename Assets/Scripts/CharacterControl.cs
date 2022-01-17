using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
   public float speed;
   public float jumpForce;
   public Transform groundCheck;
   public float checkRadius;
   public LayerMask whatIsGround;
   Animator animator;
   Rigidbody2D rb2d;
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
   void Start()
   {
      animator = GetComponent<Animator>();
      rb2d = GetComponent<Rigidbody2D>();
      spriteRenderer = GetComponent<SpriteRenderer>();
      facingRight = true;
      player = FindObjectOfType<Player>();
      //SprintEnabled = true;
      //maxJumps = 2;
   }
   // Update is called once per frame
   void FixedUpdate()
   {
      isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
      if(isGrounded && !jumping) {
         jumps = maxJumps;
      }

      moveInput = Input.GetAxisRaw("Horizontal");
      rb2d.velocity = GetMovementVelocity(moveInput * speed * ((SprintEnabled && Sprinting)? 2f : 1f), rb2d.velocity.y);

      if (!facingRight && moveInput > 0)
      {
         Flip();
      }
      else if (facingRight && moveInput < 0)
      {
         Flip();
      }
   }
   void Update()
   {
      if(Input.GetKeyDown(KeyCode.Space) && jumps > 0) {
         rb2d.velocity = GetJunmpingVelocity(jumpForce);
         jumps--;
         jumping = true;
         if(jumpingCoroutine != null)
            StopCoroutine(jumpingCoroutine);
         StartCoroutine(RestartJumping());
      }
      if(rb2d.velocity.y != 0){
         animator.SetBool("Jumping", true);
      }
      else{
         animator.SetBool("Jumping", false);
      }
      Sprinting = Input.GetKey(KeyCode.LeftShift);
      if (rb2d.velocity.x != 0)
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
   
   public static Vector2 GetJunmpingVelocity(float jumpForce) {
      return Vector2.up * jumpForce;
   }

   public static Vector2 GetMovementVelocity(float force, float yVelocity) {
      return new Vector2(force, yVelocity);
   }

   IEnumerator RestartJumping() {
      for(int i = 0; i < 5; i++) {
         yield return new WaitForFixedUpdate();
      }
      jumping = false;
   }
}
