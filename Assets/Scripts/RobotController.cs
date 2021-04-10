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

    public float speed;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        col = GetComponent<CircleCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        vision = GetComponentInChildren<MeshRenderer>();
    }

    void Update()
    {
        
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
    
}
