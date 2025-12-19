using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerBasics : MonoBehaviour
{
    public LayerMask stuf;
    public LayerMask everything;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private BoxCollider2D steppers;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private BoxCollider2D standing;
    [SerializeField] private BoxCollider2D standCheck;
    [SerializeField] private BoxCollider2D wallCheck;

    private float moveX;
    private float moveY;
    [SerializeField] private float baseSpeed;
    [SerializeField] private float baseJump = 10;
    [SerializeField] private float accel;
    public float addSpeed = 0;
    public float addJump = 0;

    [SerializeField] private bool steppin;
    [SerializeField] private bool cantStand;
    [SerializeField] private bool colliding;
    private bool hanging;


    [SerializeField] private int health;
    public int playerHealth;
    public int maxHealth;
    public int armor;
    public bool playerDeath;

    [SerializeField] private float stamina;
    public float playerStamina;



    // Update is called once per frame
    void Update(){
        animator.SetBool("hang", hanging);
        animator.SetBool("standing", standing.enabled);
        animator.SetBool("onGround", steppin);
        animator.SetFloat("running", Mathf.Abs(body.linearVelocity.x));
        animator.SetFloat("inAir", body.linearVelocity.y);
        animator.SetFloat("preppin", addJump);

        
        float speed = baseSpeed + addSpeed;
        float jump = baseJump + addJump;
        
  
        if (steppin == true && moveY > 0.1 && addJump <= 6.7){
            addJump = addJump + .032f;
        } // charges jump on hold key

        if (steppin == true && Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow)){
            body.linearVelocity = new Vector2(body.linearVelocity.x, jump + addJump);
            addJump = 0f;
        } // jumps and resets add jump


        if (steppin == true && moveY < -.5 || addJump > 1.5f){
            standing.enabled = false;
        }
        else{
            standing.enabled = true;
        } // for crouching and crouching when charging jump

        
        if (Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(moveX) > 0){
            body.linearVelocity = new Vector2(moveX * speed * accel, body.linearVelocity.y);
            animator.speed = Mathf.Abs(moveX) * 1.5f;
        }
        else if (Mathf.Abs(moveX) > 0){
            body.linearVelocity = new Vector2(moveX * speed, body.linearVelocity.y);
            animator.speed = Mathf.Abs(moveX) + .3f;
        }
        else{
            animator.speed = 1f;
        } // for running/walking


        if (steppin == false && colliding == true && Mathf.Abs(moveX) > 0.5f && moveY > 0f){
            hanging = true;
        }
        else if (steppin == false && colliding == true && Mathf.Abs(moveX) > 0.1f){
            body.linearVelocity = new Vector2(0f, body.linearVelocity.y);
            hanging = false;
        }
        else{
            hanging = false;
        } // for wall hanging
    }

    void FixedUpdate(){
        Checka();
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");
        
        health = playerHealth + armor;
        playerStamina = stamina;

        animator.SetBool("CantStand", cantStand);

        if (playerDeath == false && health <= 0){
            playerDeath = true;
        }

        
        if (cantStand == true){
            standing.enabled = false;
        } // makes sur your still crouched if you let go of s/down arrow without enough headspace

        if (sprite.flipX == false && moveX < -.1){
            sprite.flipX = true;
        }
        else if(sprite.flipX == true && moveX > .1){
            sprite.flipX = false;
        } // for flipping sprite when turning

        if (standing.enabled == false || animator.GetCurrentAnimatorStateInfo(0).IsName("CrouchMov")){
            animator.speed = Mathf.Abs(moveX) * .8f;
            baseSpeed = 5.5f;
            accel = 1.5f;
        }
        else{
            baseSpeed = 12f;
            accel = 2f;
        } // slows down while crouching
    }

    void Checka(){
        steppin = Physics2D.OverlapAreaAll(steppers.bounds.min, steppers.bounds.max, stuf).Length > 0; // you be steppin not floatin
        cantStand = Physics2D.OverlapAreaAll(standCheck.bounds.min, standCheck.bounds.max, stuf).Length > 0; // bro dont hit your head
        colliding = Physics2D.OverlapAreaAll(wallCheck.bounds.min, wallCheck.bounds.max, everything).Length > 0; // wow, you can hang on a wall
    }
}
