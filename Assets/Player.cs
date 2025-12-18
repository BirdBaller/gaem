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


    [SerializeField] private float baseSpeed;
    [SerializeField] private float baseJump = 10;
    [SerializeField] private float accel;
    public float addSpeed = 0;
    public float addJump = 0;

    [SerializeField] private bool steppin;
    [SerializeField] private bool cantStand;
    [SerializeField] private bool colliding;
    private bool hanging;



    // Update is called once per frame
    void Update(){
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        float speed = baseSpeed + addSpeed;
        float jump = baseJump + addJump;
        

        animator.SetBool("standing", standing.enabled);
        animator.SetBool("onGround", steppin);
        animator.SetFloat("running", Mathf.Abs(body.linearVelocity.x));
        animator.SetFloat("inAir", body.linearVelocity.y);
        animator.SetFloat("preppin", addJump);
        animator.SetBool("hang", hanging);


        if (sprite.flipX == false && moveX < -.1){
            sprite.flipX = true;
        }
        else if(sprite.flipX == true && moveX > .1){
            sprite.flipX = false;
        }

        if (standing.enabled == false){
            animator.speed = Mathf.Abs(moveX) * .8f;
            baseSpeed = 5.5f;
            accel = 1.5f;
        }
        else{
            baseSpeed = 12f;
            accel = 2f;
        }

                
        if (steppin == true && moveY > 0.1 && addJump <= 6.7){
            addJump = addJump + .032f;
        }

        if (steppin == true && moveY < -.5 || animator.GetFloat("preppin") > 1.5f){
            standing.enabled = false;
        }
        else{
            standing.enabled = true;
        }

        
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
        }

        
        if (steppin == true && Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow)){
            body.linearVelocity = new Vector2(body.linearVelocity.x, jump + addJump);
            addJump = 0f;
        }

        if (steppin == false && colliding == true && Mathf.Abs(moveX) > 0.5f && moveY > 0f){
            hanging = true;
        }
        else if (steppin == false && colliding == true && Mathf.Abs(moveX) > 0.3f){
            body.linearVelocity = new Vector2(0f, body.linearVelocity.y);
            hanging = false;
        }
        else{
            hanging = false;
        }
    }

    void FixedUpdate(){
        Checka();
        animator.SetBool("CantStand", cantStand);
        
        if (cantStand == true){
            standing.enabled = false;
        }
    }

    void Checka(){
        steppin = Physics2D.OverlapAreaAll(steppers.bounds.min, steppers.bounds.max, stuf).Length > 0;
        cantStand = Physics2D.OverlapAreaAll(standCheck.bounds.min, standCheck.bounds.max, stuf).Length > 0;
        colliding = Physics2D.OverlapAreaAll(wallCheck.bounds.min, wallCheck.bounds.max, everything).Length > 0;
    }
}
