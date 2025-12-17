using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerBasics : MonoBehaviour
{
    public Rigidbody2D body;
    public BoxCollider2D steppers;
    public BoxCollider2D aaabody;
    public LayerMask ground;
    public LayerMask everything;
    public Animator animator;
    public SpriteRenderer sprite;
    [SerializeField] private BoxCollider2D standing;


    [SerializeField] private float baseSpeed;
    [SerializeField] private float baseJump = 8;
    [SerializeField] private float accel;
    public float addSpeed = 0;
    public float addJump = 0;

    public bool steppin;
    private bool aabody;


    // Update is called once per frame
    void Update(){
        animator.SetBool("standing", standing.enabled);
        animator.SetBool("onGround", steppin);
        animator.SetFloat("running", Mathf.Abs(body.linearVelocity.x));
        animator.SetFloat("inAir", body.linearVelocity.y);
        animator.SetFloat("preppin", addJump);

        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        float speed = baseSpeed + addSpeed;
        float jump = baseJump + addJump;


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

        
        if (steppin == true && moveY > 0.1 && addJump <= 4.5){
            addJump = addJump + .007f;
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
            animator.speed = Mathf.Abs(moveX);
        }
        else{
            animator.speed = 1f;
        }

        
        if (steppin == true && Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow) && moveY > 0){
            body.linearVelocity = new Vector2(body.linearVelocity.x, jump + addJump);
            addJump = 0f;
        }
    }

    void FixedUpdate(){
        Checka();

        animator.SetBool("StandCheck", aabody);

    }

    void Checka(){
        steppin = Physics2D.OverlapAreaAll(steppers.bounds.min, steppers.bounds.max, ground).Length > 0;
        aabody = Physics2D.OverlapAreaAll(aaabody.bounds.min, aaabody.bounds.max, everything).Length > 0;
    }
}
