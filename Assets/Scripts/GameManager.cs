using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public int screenHeight;
    [HideInInspector]
    public int screenWidth;
    public int sizeMultiplyer;
    public Color currentPaintColor;
    public GameObject currentPaintCanvas;
    public float percentComplete;
    public int totalPixelCount;
    public int currPixelCount;
    public Texture2D currentTexture;
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
        currentPaintColor = Color.red;
        sizeMultiplyer = 5;
        currPixelCount = 0;
        totalPixelCount = 0;
        currentTexture = currentPaintCanvas.GetComponent<Image>().sprite.texture;
        Color[] colors = currentTexture.GetPixels();
        for(int i = 0; i < colors.Length; i++)
        {
            if(colors[i].a != 0 && !IsBasicallyBlack(colors[i]))
            {
                totalPixelCount++;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        updateResolution();
        if(currPixelCount != 0)
        {
            percentComplete = (float)currPixelCount / (float)totalPixelCount;
        }
        
    }

    public bool IsBasicallyBlack(Color color, float epsilon = .039f)
    {
        if (color.r >= 0 && color.r <= epsilon)
        {
            if (color.g >= 0 && color.g <= epsilon)
            {
                if (color.b >= 0 && color.b <= epsilon)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void updateResolution()
    {
        screenHeight = Screen.height;
        screenWidth = Screen.width;
    }

}
