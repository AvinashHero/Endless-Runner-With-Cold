using System;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private const float LANE_DISTANCE = 2.5f;
    private const float Turn_Speed = 0.05f;

    //
    private bool isRunning = false;

    //Movement
    private CharacterController controller;
    private float jumpForce = 6f;
    private float gravity = 12.0f;
    private float verticalVelocity;
    

    //Animation
    private Animator animator;

    private int desiredLane = 1; // 0 = Left, 1 = Middle, 2 = Right

    //Speed Modifier
    private float originalSpeed = 7.0f;
    private float speed;
    private float speedIncreaseLastTick;
    private float speedIncreaseTime = 2.5f;
    private float speedIncreaseAmount = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        speed = originalSpeed;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        SetAnimator();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRunning)
            return;

        if(Time.time - speedIncreaseLastTick > speedIncreaseTime)
        {
            speedIncreaseLastTick = Time.time;
            speed += speedIncreaseAmount;
            GameManager.Instance.UpdateModifier(speed - originalSpeed);

        }
        //Gather the inputs on which lane we should be
        if (MobileInput.Instance.SwipeLeft)
            MoveLane(false);

        if (MobileInput.Instance.SwipeRight)
            MoveLane(true);

        //Calculate where we should be in the future
        Vector3 targetPosition = transform.position.z * Vector3.forward;
        
        if(desiredLane == 0)
              targetPosition += Vector3.left * LANE_DISTANCE;
       
        else if(desiredLane == 2)
              targetPosition += Vector3.right * LANE_DISTANCE;
    
        
        // let's calculate our move delta

        Vector3 moveVector = Vector3.zero;

        moveVector.x = (targetPosition - transform.position).normalized.x * speed;

        bool isGrounded = IsGrounded();
        animator.SetBool("isGrounded", isGrounded);
        if(isGrounded)//isGrounded is true
        {
            verticalVelocity = -0.1f;

            if(MobileInput.Instance.SwipeUp)
            {
                //jump
                animator.SetTrigger("Jump");
                verticalVelocity = jumpForce;
            }
            else if(MobileInput.Instance.SwipeDown)
            {
                //Slide
                StartSlide();
                Invoke("StopSlide", 1.0f);
            }
        }
        else
        {
            verticalVelocity -= (gravity * Time.deltaTime);

            //Fast falling down
            if(MobileInput.Instance.SwipeDown)
            {
                verticalVelocity = -jumpForce;
            }
        }
        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        //Move the Character
        controller.Move(moveVector * Time.deltaTime);

        //Rotate the player where he is going
        Vector3 dir = controller.velocity;
        if(dir != Vector3.zero)
        {
            dir.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, dir, Turn_Speed);
        }
       
    }

    private void StartSlide()
    {
        animator.SetBool("isSliding", true);
        controller.height /= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y / 2, controller.center.z);
    }

    private void StopSlide()
    {
        animator.SetBool("isSliding", false);
        controller.height *= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y * 2, controller.center.z);

    }

    private void MoveLane(bool goingRight)
    {
        desiredLane += (goingRight) ? 1 : -1;
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);
    }

    private bool IsGrounded()
    {
        Ray groundRay = new Ray(new Vector3(controller.bounds.center.x,
            (controller.bounds.center.y - controller.bounds.extents.y) + 0.2f,
            controller.bounds.center.z),
            Vector3.down);

        return Physics.Raycast(groundRay, 0.2f + 0.1f);
    }

    //Set the Avatar for Player
    void SetAnimator()
    {
        Animator[] animators = GetComponentsInChildren<Animator>();
        if (animators.Length > 0)
        {
            for (int i = 0; i < animators.Length; i++)
            {
                Animator anim = animators[i];
                Avatar av = anim.avatar;

                if (anim != animator)
                {
                    animator.avatar = av;
                    Destroy(anim);
                }
            }
        }
    }


    //For Start Game
    public void StartGame()
    {
        isRunning = true;
        animator.SetTrigger("startRunning");
    }

    private void Crash()
    {
        animator.SetTrigger("Death");
        isRunning = false;
        GameManager.Instance.OnDeath();
    }

    private void SideCrash(bool goingRight)
    {
        desiredLane += (goingRight) ? -1 : 1;
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        switch(hit.gameObject.tag)
        {
            case "Obstacle":
                Crash();
                break;

            case "sideObstacle":
                SideCrash(desiredLane!=0);
                break;
        }
    }
}
