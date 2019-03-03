using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public enum State {
        PAINTING,
        PICK_COLOR,
        FINISHED
    };
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
    public int paintCanvasIndex;
    public State currentState;
    [HideInInspector]
    public static GameManager Instance;

    public GameObject[] PaintCanvases;
    public GameObject[] PaintCanvasPair;

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
        paintCanvasIndex = 0;
        currentPaintColor = Color.red;
        sizeMultiplyer = 5;
        currentPaintCanvas = PaintCanvases[0];
        SetPaintCounters();
        currentState = State.PAINTING;

    }

    // Update is called once per frame
    void Update()
    {
        updateResolution();
        if (currPixelCount != 0)
        {
            percentComplete = (float)currPixelCount / (float)totalPixelCount;
        }

        if (percentComplete >= .9f)
        {
            if(paintCanvasIndex == PaintCanvases.Length-1)
            {
                currentState = State.FINISHED;
                currentPaintCanvas.GetComponent<Painting>().enabled = false;
                for(int i = 0; i < PaintCanvases.Length; i++)
                {
                    PaintCanvasPair[i].SetActive(false);
                    PaintCanvases[i].SetActive(true);
                }
            }
            else
            {
                currentPaintCanvas.GetComponent<Painting>().enabled = false;
                currentPaintCanvas.SetActive(false);
                PaintCanvasPair[paintCanvasIndex].SetActive(false);
                currentPaintCanvas = PaintCanvases[++paintCanvasIndex];
                PaintCanvasPair[paintCanvasIndex].SetActive(true);
                currentPaintCanvas.SetActive(true);
                SetPaintCounters();
            }
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

    private void SetPaintCounters()
    {
        percentComplete = 0f;
        currPixelCount = 0;
        totalPixelCount = 0;
        currentTexture = currentPaintCanvas.GetComponent<Image>().sprite.texture;
        Color[] colors = currentTexture.GetPixels();
        for (int i = 0; i < colors.Length; i++)
        {
            if (colors[i].a != 0 && !IsBasicallyBlack(colors[i]))
            {
                totalPixelCount++;
            }
        }
    }

}
