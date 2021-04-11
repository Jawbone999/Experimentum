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
    public SpriteRenderer body;
    SpriteRenderer legs;
    FieldOfView fov;
    GameManager gm;

    public float walkSpeed;
    public float runSpeed;
    public int hitPoints;

    public bool canMove;
    public bool isRunning;
    public bool isMoving;
    public int xMovement; // 0 for none, -1 for left, +1 for right
    public int yMovement; // 0 for none, -1 for down, +1 for up

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        col = GetComponent<CircleCollider2D>();
        legs = GetComponent<SpriteRenderer>();
        fov = GetComponentInChildren<FieldOfView>();
        gm = GameObject.FindObjectOfType<GameManager>();
    }

    void Start()
    {
        camController.Setup(() => transform.position);
        LayerMask enemies = 1 << 10;
        fov.SetTriggerMask(enemies);

        LayerMask walls = 1 << 7 | 1 << 10;
        fov.SetObstacleMask(walls);
    }

    void Update()
    {
        MovementInput();
        TurnInput();
        Animate();
    }

    void LateUpdate()
    {
        Vector3 enemy = fov.DrawFieldOfView();

        if (enemy != Vector3.zero)
        {
            gm.PlaySting();
        }
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

        isMoving = xMovement != 0 || yMovement != 0;
    }

    void TurnInput()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, camController.cameraZoom);
        Vector3 worldPos = camController.cam.ScreenToWorldPoint(mousePos);
        body.transform.rotation = Quaternion.LookRotation(Vector3.forward, worldPos - transform.position);
    }

    void Animate()
    {
        animator.SetBool("moving", isMoving);
        animator.SetBool("running", isRunning);
    }
    
    public void PlayFootstepSound()
    {
        audioSource.pitch = Random.Range(0.9f, 1);
        audioSource.volume = Random.Range(0.4f, 0.5f);

        audioSource.Play();
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            return;
        }

        float speed = isRunning ? runSpeed : walkSpeed;
        Vector3 move = new Vector3(speed * Time.deltaTime * xMovement, speed * Time.deltaTime * yMovement, 0);

        if (move != Vector3.zero)
        {
            rb.MovePosition(transform.position + move);
            legs.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(move.y, move.x) * Mathf.Rad2Deg + 90, Vector3.forward);
        }
    }

    public void Hurt()
    {
        Debug.Log("ow!");
        hitPoints -= 1;
    }
}
