using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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
}