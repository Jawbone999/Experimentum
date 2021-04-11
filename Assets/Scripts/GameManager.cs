using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<RobotController> robots;
    public PlayerController player;

    public LayerMask obstacleMask;

    public AudioSource backgroundMusicPlayer;
    public AudioSource stingPlayer;
    public float stingCooldown;
    float lastSting;

    void Awake()
    {
        // Remove Scene View Mask
        GameObject mask = GameObject.Find("Scene View Mask");
        if (mask)
        {
            mask.SetActive(false);
        }

        if (instance != null)
        {
            Destroy(gameObject);
        } 
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        player = GameObject.Find("ThePlayer").GetComponentInChildren<PlayerController>();

        GameObject robotParent = GameObject.Find("Robots");
        RobotController[] rcs = robotParent.GetComponentsInChildren<RobotController>();
        for (int i = 0; i < rcs.Length; i++)
        {
            robots.Add(rcs[i]);
        }

        lastSting = Time.time - stingCooldown;
    }

    void FixedUpdate()
    {
        for (int i = 0; i < robots.Count; i++)
        {
            robots[i].GetComponent<RobotController>().Hide();
        }
    }

    public void PlaySting()
    {
        if (Time.time - lastSting >= stingCooldown)
        {
            stingPlayer.Play();
            lastSting = Time.time;
        }
    }
}