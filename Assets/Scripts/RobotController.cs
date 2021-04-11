using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(AudioSource))]
[RequireComponent(typeof(CircleCollider2D))]
public class RobotController : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;
    AudioSource audioSource;
    CircleCollider2D col;
    SpriteRenderer sr;
    MeshRenderer vision;
    FieldOfView fov;

    int xMovement;
    int yMovement;

    Vector3 lastSawPlayer;
    float chaseTimeRemaining;
    float hitWaitTime;
    public float hitCooldown;
    public Difficulty difficulty;

    public float distanceTolerance;
    public bool isTouchingPlayer;
    

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        col = GetComponent<CircleCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        vision = GetComponentInChildren<MeshRenderer>();
        fov = GetComponentInChildren<FieldOfView>();
    }

    void Start()
    {
        fov.SetObstacleMask(1 << 7 | 1 << 8);
        fov.SetTriggerMask(1 << 8);
        fov.viewRadius = difficulty.viewRange;
    }

    void LateUpdate()
    {
        Vector3 player = fov.DrawFieldOfView();

        if (player != Vector3.zero)
        {
            lastSawPlayer = player;
            chaseTimeRemaining = difficulty.chaseTime;
        }
    }

    void FixedUpdate()
    {
        MakeMove();
        Animate();

        hitWaitTime -= Time.deltaTime;
    }

    void MakeMove()
    {
        if (isTouchingPlayer)
        {
            return;
        }

        if (chaseTimeRemaining > 0)
        {
            chaseTimeRemaining -= Time.deltaTime;
            MoveTowards(lastSawPlayer);
        }
        else
        {
            
        }
    }

    void MoveTowards(Vector3 pos)
    {
        xMovement = 0;
        yMovement = 0;

        bool xNear = Mathf.Abs(transform.position.x - pos.x) < distanceTolerance;
        bool yNear = Mathf.Abs(transform.position.y - pos.y) < distanceTolerance;

        if (!xNear && transform.position.x - pos.x > 0)
        {
            xMovement = -1;
        }
        else if (!xNear && transform.position.x - pos.x < 0)
        {
            xMovement = 1;
        }

        if (!yNear && transform.position.y - pos.y > 0)
        {
            yMovement = -1;
        }
        else if (!yNear && transform.position.y - pos.y < 0)
        {
            yMovement = 1;
        }

        pos = new Vector3(pos.x, pos.y);

        Vector3 move = new Vector3(Time.deltaTime * difficulty.speed * xMovement, Time.deltaTime * difficulty.speed * yMovement, 0);
        Debug.DrawLine(transform.position, transform.position + move);
        rb.MovePosition(transform.position + move);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, pos - transform.position);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            isTouchingPlayer = true;
            if (hitWaitTime <= 0)
            {
                collision.gameObject.GetComponent<PlayerController>().Hurt();
                hitWaitTime = hitCooldown;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            isTouchingPlayer = false;
        }
    }

    void Animate()
    {
        animator.SetBool("isMoving", xMovement != 0 || yMovement != 0);
    }

    void PlayFootstepSound()
    {
        audioSource.pitch = Random.Range(1f, 1.1f);
        audioSource.volume = Random.Range(0.4f, 0.5f);

        audioSource.Play();
    }

    public void Hide()
    {
        sr.enabled = false;
        vision.enabled = false;
    }

    public void Show()
    {
        sr.enabled = true;
        vision.enabled = true;
    }

    [System.Serializable]
    public struct Difficulty
    {
        public float viewRange;
        public float speed;
        public float chaseTime;
    }
    
}
