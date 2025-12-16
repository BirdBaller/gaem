using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerBasics : MonoBehaviour
{
    public Rigidbody2D body;
    public BoxCollider2D steppers;
    public LayerMask ground;
    public Animator animator;
    public SpriteRenderer sprite;
    [SerializeField] private BoxCollider2D standing;


    [SerializeField] private float baseSpeed;
    [SerializeField] private float baseJump = 8;
    [SerializeField] private float accel;
    public float addSpeed = 0;
    public float addJump = 0;

    public bool steppin;


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
        
        
        if (Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(moveX) > 0){
            body.linearVelocity = new Vector2(moveX * speed * accel, body.linearVelocity.y);
            animator.speed = Mathf.Abs(moveX) * 1.5f;
        }
        else if (Mathf.Abs(moveX) > 0){
            body.linearVelocity = new Vector2(moveX * speed, body.linearVelocity.y);
            animator.speed = Mathf.Abs(moveX);
        }
        else{
            animator.speed = 1f;
        }

        
        if (steppin == true && moveY > 0.1 && addJump <= 4.5){
            addJump = addJump + .007f;
        }


        if (steppin == true && Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow) && moveY > 0){
            body.linearVelocity = new Vector2(body.linearVelocity.x, jump + addJump);
            addJump = 0f;
        }


        if (steppin == true && moveY < -.5 || animator.GetFloat("preppin") > 1.5f){
            standing.enabled = false;
        }
        else{
            standing.enabled = true;
        }
        
    }

    void FixedUpdate(){
        steppaChecka();
    }

    void steppaChecka(){
        steppin = Physics2D.OverlapAreaAll(steppers.bounds.min, steppers.bounds.max, ground).Length > 0;
    }
}
