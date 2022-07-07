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
   Vector3 defaultPlayerScale;
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
   public bool dashing;

   Coroutine jumpingCoroutine;

   // Start is called before the first frame update
   void Start() {
      animator = GetComponent<Animator>();
      rb = GetComponent<Rigidbody>();
      spriteRenderer = GetComponent<SpriteRenderer>();
      facingRight = true;
      player = FindObjectOfType<Player>();
      SprintEnabled = true;
      defaultPlayerScale = player.PlayerModel.transform.localScale;
   }

   // Update is called once per frame
   void FixedUpdate() {
      isGrounded = Physics.OverlapSphere(groundCheck.position, checkRadius, whatIsGround).Length > 0;
      if (isGrounded && !jumping)
      {
         jumps = maxJumps;
      }
      animator.SetBool("IsGrounded",isGrounded);

      moveInput = Input.GetAxisRaw("Horizontal");
      if(!dashing)
         rb.velocity = GetMovementVelocity(moveInput * speed * ((SprintEnabled && Sprinting) ? 2f : 1f), rb.velocity.y);

      if (!facingRight && moveInput > 0)
      {
         Flip();
      }
      else if (facingRight && moveInput < 0)
      {
         Flip();
      }

      if (rb.velocity.y < 0 && !dashing)
      {
         rb.velocity += GetGravityVelocity(fallMultiplier, Time.deltaTime);
         animator.SetBool("Floating", true);
      }
      else if (rb.velocity.y > 0 && !jumping && !dashing)
      {
         rb.velocity += GetGravityVelocity(lowJumpMultiplier, Time.deltaTime);
         animator.Play("Warrior_JumpStart");
      }

      if(isGrounded)
         animator.SetBool("Floating", false);
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
         animator.SetTrigger("JumpUp");
      }
      Sprinting = Input.GetKey(KeyCode.LeftShift);
      moving = (rb.velocity.x != 0);
      animator.SetBool("Moving", moving);

      if(Input.GetKeyDown(KeyCode.LeftShift) && !isGrounded) {
         dashing = true;
         StartCoroutine(InitiateDash());
      }
   }
   void Flip() {
      player.PlayerModel.transform.localScale = new Vector3(
         facingRight ? -1 * defaultPlayerScale.x : 1 * defaultPlayerScale.x,
         defaultPlayerScale.y,
         1);
      facingRight = !facingRight;
   }

   public static Vector3 GetGravityVelocity(float multiplier, float deltaTime) {
      var fallingVector = Vector3.up * (Physics.gravity.y * (multiplier - 1) * deltaTime);
      return fallingVector;
   }
   public static Vector2 GetMovementVelocity(float force, float yVelocity) {
      return new Vector2(force, yVelocity);
   }

   public bool IsFalling() {
      return rb.velocity.y < 0;
   }

   IEnumerator RestartJumping() {
      for (int i = 0; i < 5; i++)
      {
         yield return new WaitForFixedUpdate();
      }
      jumping = false;
   }
   IEnumerator InitiateDash() {
      animator.SetBool("Dashing", true);
      float seconds = 0.3f;
      rb.useGravity = false;
      rb.velocity = Vector3.zero;
      rb.velocity += (facingRight ? Vector3.right * 20f : Vector3.left * 20f);
      while(seconds > 0) {
         seconds -= Time.deltaTime;
         yield return new WaitForEndOfFrame();
      }
      dashing = false;
      rb.useGravity = true;
      animator.SetBool("Dashing", false);
   }
}
