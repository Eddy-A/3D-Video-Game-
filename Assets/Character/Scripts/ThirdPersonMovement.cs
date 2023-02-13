using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public Animator animator;
    public GameObject hitBox;

    public float speed = 6;
    public float gravity = -9.81f;
    public float jumpHeight = 3;
    Vector3 velocity;
    bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    public bool isAttacking = false;
    public bool isAttAnimation = false;

    [SerializeField]
    PlayerStats PlayerFunc;

    // Update is called once per frame
    void Update()
    {
        // jump
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            animator.SetBool("Jump", false);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            animator.SetBool("Jump", true);
        }

        if (Input.GetMouseButtonDown(0) && isAttAnimation == false && PlayerFunc.CheckMP(3))
        {
            animator.SetTrigger("isAttacking");
            PlayerFunc.LoseMP(3);
        }
        // gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // walk
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (horizontal != 0 || vertical != 0)
        {
            animator.SetFloat("Speed", 1);
        }
        else if (horizontal == 0 && vertical == 0)
        {
            animator.SetFloat("Speed", 0);
        }

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(speed * Time.deltaTime * moveDir.normalized);
        }
    }
    public void BulletOn() //assign Tag to the part of the monster that has the collider
    {
        hitBox.GetComponent<Collider>().enabled = true;
    }

    public void BulletOff()
    {
        hitBox.GetComponent<Collider>().enabled = false;
    }

    public void ToggleAttack()
    {
        isAttacking = !isAttacking;
    }

    public void ToggleAttAnimation()
    {
        isAttAnimation = !isAttAnimation;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Mage"))
        {
            PlayerFunc.LoseHP(5);
        }
        if (collision.gameObject.CompareTag("Skele"))
        {
            PlayerFunc.LoseHP(10);
        }
        if (collision.gameObject.CompareTag("Dragon"))
        {
            PlayerFunc.LoseHP(40);
        }
        if (collision.gameObject.CompareTag("Slime"))
        {
            PlayerFunc.LoseHP(5);
        }
        if (collision.gameObject.CompareTag("Spike"))
        {
            PlayerFunc.LoseHP(12);
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("FireBall"))
        {
            PlayerFunc.LoseHP(50);
        }
    }
}
