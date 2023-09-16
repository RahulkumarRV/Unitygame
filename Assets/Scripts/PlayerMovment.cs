using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] public float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallGoUpPower  = 6.0f;
    private Rigidbody2D body;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown = 0;
    private float horizontalInput;

    private void Awake()
    {
        // help to access the rigid body and animator object of the player
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }


    private void Update()
    {

        horizontalInput = Input.GetAxis("Horizontal");

        // flep the player on x axis
        if (horizontalInput > 0.01f){
            transform.localScale = Vector3.one;
        }
        else if (horizontalInput < -0.01f){
            transform.localScale = new Vector3( -1, 1, 1); 
        }

        

        animator.SetBool("run", horizontalInput != 0);
        animator.SetBool("grounded", isGrounded());

        // each jump should have some gap between them
        if(wallJumpCooldown > 0.2f){
           
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if(isOnWall() && !isGrounded()){
                body.gravityScale = 0;
                body.velocity = new Vector2(body.velocity.x, 0);
            }else{
                body.gravityScale = 7f;
            }

             if (Input.GetKey(KeyCode.Space))
                Jump();
            
        }
        else {
            wallJumpCooldown += Time.deltaTime;
        }
        
    }

    // trigger jump from any state to jump state
    private void Jump() {
        // Normal jump if player on the ground
        if(isGrounded()){
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            animator.SetTrigger("jump");
            
        }
        // Jump if player is on the wall but not on the ground
        else if(isOnWall() && !isGrounded()){
            // if player is on the wall not want to go up and only jump then pull player down
            if(horizontalInput == 0){
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x)*10, 0);
                transform.localScale = new Vector2(-Mathf.Sign(transform.localScale.x), transform.localScale.y);
            }
            //used for delay in two jump
            wallJumpCooldown = 0;
            // throw player away from the wall and upwords
            body.velocity = new Vector2(
                -Mathf.Sign(transform.localScale.x)*3,
                wallGoUpPower
            );
        }
    }

    // check the player is on the ground or not
    private bool isGrounded() {

        RaycastHit2D rayCastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0,
            Vector2.down,
            0.1f,
            groundLayer
        );
        
        return rayCastHit.collider != null;
    }

    // check the player is on the wall collision or not
    private bool isOnWall() {

        RaycastHit2D rayCastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0,
            new Vector2(transform.position.x, 0),
            0.1f,
            wallLayer
        );
        return rayCastHit.collider != null;
    }












}
