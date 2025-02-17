﻿using System;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public GameOverController gameoverController;
    public KeyScoreController keyscoreController;
    public ParticleSystem Particlesystem;
    public PlayerHealth playerHealth;
    public Animator animator;
    public float speed;
    private Rigidbody2D rb2D;
    public float jumpForce = 5f;
    private bool isJumping = false;
    private BoxCollider2D boxCollider;
    public float deathParticleMultiplier = 2f;
    private bool isGrounded;


    private void Awake()
    {
        Debug.Log("Player Controller Awake");
        rb2D = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        int currentHealth = playerHealth.health;
        
        if(currentHealth > 0)
        {   // PlayerMovement
                float horizontal = Input.GetAxisRaw("Horizontal");
                MoveCharacter(horizontal);
                PlayerMovementAnimation(horizontal);
            if (isGrounded)
            {
                //Player Jump
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    Jump();
                }
                //Crouch
                PlayerCrouchAnimation();
            }
        }        
    }

    private void MoveCharacter(float horizontal)
    {
        //RunMove
        Vector3 position = transform.position;
        position.x += horizontal * speed * Time.deltaTime;
        transform.position = position;
    }

    private void PlayerMovementAnimation(float horizontal)
    {
        // RUN
        animator.SetFloat("Speed", Mathf.Abs(horizontal));
        Vector3 scale = transform.localScale;

        if (horizontal < 0)
        {
            scale.x = -1f * Mathf.Abs(scale.x);
        }
        else if (horizontal > 0)
        {
            scale.x = Mathf.Abs(scale.x);
        }

        transform.localScale = scale;
    }

    private void Jump()
    {
        // Jump
        rb2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        animator.SetTrigger("Jump");
        SoundManager.Instance.Play(Sounds.PlayerMoveJump);
        isJumping = true;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (isJumping)
            {
                SoundManager.Instance.Play(Sounds.PlayerMoveJumpLand);
                isJumping = false;
            }
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void PlayerCrouchAnimation()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            animator.SetTrigger("Crouch");
            boxCollider.size = new Vector2(boxCollider.size.x, 1f);
            boxCollider.offset = new Vector2(boxCollider.offset.x, 0.5f);
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            boxCollider.size = new Vector2(boxCollider.size.x, 2f);
            boxCollider.offset = new Vector2(boxCollider.offset.x, 1f);
        }     
    }

    public void KillPlayer()
    {
        Debug.Log("Player hit by the Enemy");
        animator.SetBool("Death", true);

        ParticleSystem.MainModule mainModule = Particlesystem.main;
        mainModule.startColor = Color.black;

        Particlesystem.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

        ParticleSystem.EmissionModule emission = Particlesystem.emission;
        emission.rateOverTimeMultiplier *= deathParticleMultiplier;

        SoundManager.Instance.PlayMusic(Sounds.LevelFailed);
        gameoverController.Invoke("PlayerDied", 4f);        
    }

    public void PickUpKey()
    {
        Debug.Log("Player picked up the Key");
        keyscoreController.IncreaseScore(10);
    }
       
}
