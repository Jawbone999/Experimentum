using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(AudioSource))]
[RequireComponent(typeof(CircleCollider2D))]
public class PlayerController : MonoBehaviour
{
    public CameraController camController;

    Animator animator;
    Rigidbody2D rb;
    AudioSource audioSource;
    CircleCollider2D col;

    public AudioClip footstepClip;

    public float walkSpeed;
    public float runSpeed;

    public bool canMove;
    public bool isRunning;
    public int xMovement; // 0 for none, -1 for left, +1 for right
    public int yMovement; // 0 for none, -1 for down, +1 for up

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        col = GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        camController.Setup(() => transform.position);
    }

    void Update()
    {
        MovementInput();
        TurnInput();
        Animate();
    }

    void ResetMovement()
    {
        xMovement = 0;
        yMovement = 0;
        isRunning = false;
    }

    void MovementInput()
    {
        if (!canMove)
        {
            return;
        }

        ResetMovement();

        if (Input.GetKey(KeyCode.W))
        {
            yMovement += 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            xMovement -= 1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            yMovement -= 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            xMovement += 1;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRunning = true;
        }
    }

    void TurnInput()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, camController.cameraZoom);
        Vector3 worldPos = camController.cam.ScreenToWorldPoint(mousePos);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, worldPos - transform.position);
    }

    void Animate()
    {
        bool isMoving = xMovement != 0 || yMovement != 0;
        animator.SetBool("moving", isMoving);
        animator.SetBool("running", isRunning);
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            return;
        }

        float speed = isRunning ? runSpeed : walkSpeed;
        Vector3 move = new Vector3(speed * Time.deltaTime * xMovement, speed * Time.deltaTime * yMovement, 0);
        rb.MovePosition(transform.position + move);
    }
}
