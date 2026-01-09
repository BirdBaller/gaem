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


    [SerializeField] private float health;
    public int playerHealth;
    public int maxHealth;
    public int armor;
    public bool playerDeath;
    public float stamina;
    public bool StaminaUse;


    void Start(){
        playerHealth = 100;
        stamina = 100;
    }

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
        
  
        if (steppin == true && moveY > 0.1 && addJump < 7 && stamina > 0){
            StaminaUse = true;
            InvokeRepeating("repeat", 0, 1f);
        } // charges jump on hold key

        if (steppin == true && Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow)){
            CancelInvoke("repeat");
            body.linearVelocity = new Vector2(body.linearVelocity.x, jump + addJump);
            StaminaUse = false;
            addJump = 0;
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


        if (steppin == false && colliding == true && Mathf.Abs(moveX) > 0.5f && moveY > 0f && stamina > 0){
            hanging = true;
            StaminaUse = true;
            InvokeRepeating("repeat", 0, .67f);
        }
        else if (steppin == false && colliding == true && Mathf.Abs(moveX) > 0.5f && moveY > 0f && stamina < 1){
            CancelInvoke("repeat");
            hanging = false;
            StaminaUse = false;
            body.linearVelocity = new Vector2(-moveX, body.linearVelocity.y);
        }
        else if (steppin == false && colliding == true && Mathf.Abs(moveX) > 0.1f || stamina < .02f){
            body.linearVelocity = new Vector2(0f, body.linearVelocity.y);
            hanging = false;
            CancelInvoke("repeat");
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

        if (StaminaUse == false && stamina < 100){
            InvokeRepeating("repeat", 3f, .2f);
        }
        else if (StaminaUse == false && stamina > 99){
            CancelInvoke("repeat");
        }
    }

    void Checka(){
        steppin = Physics2D.OverlapAreaAll(steppers.bounds.min, steppers.bounds.max, stuf).Length > 0; // you be steppin not floatin
        cantStand = Physics2D.OverlapAreaAll(standCheck.bounds.min, standCheck.bounds.max, stuf).Length > 0; // bro dont hit your head
        colliding = Physics2D.OverlapAreaAll(wallCheck.bounds.min, wallCheck.bounds.max, everything).Length > 0; // wow, you can hang on a wall
    }

    private void repeat(){
        if (StaminaUse == true){
            stamina = stamina - .2f;
        }
        else if (StaminaUse == false){
            stamina = stamina + .2f;
        }

        if(steppin == true && moveY > 0.1){
            addJump = addJump + .2f;
        }
    }
}
