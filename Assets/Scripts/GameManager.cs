using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public int screenHeight;
    [HideInInspector]
    public int screenWidth;
    [HideInInspector]
    public static GameManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        updateResolution();
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        updateResolution();
    }

    private void updateResolution()
    {
        screenHeight = Screen.height;
        screenWidth = Screen.width;
    }
}
