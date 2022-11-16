using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(10f, 10f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    [SerializeField] AudioClip bulletSoundEffect; // bullet sound effect
    [SerializeField] AudioClip jumpSoundEffect; // jump sound effect
    [SerializeField] AudioClip runSoundEffect; // run sound effect

    Vector2 moveInput;
    Rigidbody2D rigidBody;
    Animator animator;
    CapsuleCollider2D playerBodyCollider;
    BoxCollider2D playerFeetCollider;
    float gravityScaleAtStart;
    bool isJumping;
    bool isAlive = true;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerBodyCollider = GetComponent<CapsuleCollider2D>();
        playerFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = rigidBody.gravityScale;
    }
    // Update is called once per frame
    void Update()
    {
        if (!isAlive) { return; }
        Run();
        flipSprite();
        climbLadder();
        Die();
        if (Mathf.Abs(rigidBody.velocity.x) < Mathf.Epsilon) //If the player isn't moving
        {
            animator.SetBool("isRunning", false);
        }
    }

    //Function for when the player fires a bullet
    void OnFire(InputValue value)
    {
        if (!isAlive) { return; } //Checks if player is alive
        if (!PauseMenu.isPaused)
        {
            Instantiate(bullet, gun.position, transform.rotation); //Creates a bullet at the gun position
        }
        AudioSource.PlayClipAtPoint(bulletSoundEffect, Camera.main.transform.position); //Plays the bullet sound effect at the position of where the camera is.
    }

    //Function for when the player is moving
    void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>(); //Gets the move input from the player when they go left,right, up, or down.
    }

    //Function for when the player is running
    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, rigidBody.velocity.y); //Sets the velocity of the player to the move input multiplied by the run speed.
        rigidBody.velocity = playerVelocity; //Sets the rigidbody velocity to the player velocity.
        bool hasHorizontalSpeed = Mathf.Abs(rigidBody.velocity.x) > Mathf.Epsilon; //Checks if the player is moving horizontally.
        animator.SetBool("isRunning", hasHorizontalSpeed); //Sets the isRunning animation to true if the player is moving horizontally.
    }

    //Function for when the player is jumping
    void OnJump(InputValue value)
    {
        if (!isAlive) { return; }
        if (!playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && !playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"))) { return; } //Checks if the player is touching the ground or ladder.
        if (value.isPressed) //Checks to see if the player is pressing the jump button.
        {
            rigidBody.velocity += new Vector2(0f, jumpSpeed); //Adds the jump speed to the player velocity.
            isJumping = true;
            animator.SetBool("isClimbing", false);
        }
        if (rigidBody.velocity.y > Mathf.Epsilon && playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"))) //Checks if the player is moving up and touching the ladder.
        {
            rigidBody.gravityScale = gravityScaleAtStart; //Sets the gravity scale to the gravity scale at the start.
            animator.SetBool("isClimbing", false);
        }
        if (playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"))) //Checks if the player is touching the Ladder.
        {
            animator.SetBool("isClimbing", true);
        }
    }
    //Function for when the sprite has to flip.
    void flipSprite()
    {
        bool hasHorizontalSpeed = Mathf.Abs(rigidBody.velocity.x) > Mathf.Epsilon; //Checks if the player is moving horizontally.
        if (hasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rigidBody.velocity.x), 1f); //Flips the player sprite depending on which direction they are moving.
        }
    }
    void climbLadder()
    {
        if (!playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            isJumping = false;
            rigidBody.gravityScale = gravityScaleAtStart;
            animator.SetBool("isClimbing", false);
            animator.SetBool("isRunning", true);
            animator.enabled = true; //Enables the animator.
            return;
        }
        //Exiting ladder
        if (!isJumping && playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"))) //Checks if the player is not jumping and touching the ladder. 
        {
            animator.SetBool("isClimbing", true);
            Vector2 climbVelocity = new Vector2(rigidBody.velocity.x, moveInput.y * climbSpeed); //Sets the climb velocity to the move input multiplied by the climb speed.
            rigidBody.velocity = climbVelocity;
            rigidBody.gravityScale = 0f;
        }
        if (Mathf.Abs(rigidBody.velocity.y) < Mathf.Epsilon)
        {
            animator.SetBool("isClimbing", false);
        }
        if (rigidBody.velocity.y < Mathf.Epsilon && isJumping && playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            isJumping = false;
            Vector2 climbVelocity = new Vector2(rigidBody.velocity.x, moveInput.y * climbSpeed);
            rigidBody.velocity = climbVelocity;
            rigidBody.gravityScale = 0f;
        }
    }
    void Die()
    {
        if (playerBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards")))
        {
            isAlive = false;
            animator.SetTrigger("Dying");
            rigidBody.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }
}
