using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public enum State {
        INTRO,
        PAINTING,
        PICK_COLOR,
        FINISHED
    };
    [HideInInspector]
    public int screenHeight;
    [HideInInspector]
    public int screenWidth;
    [HideInInspector]
    public int oldScreenHeight = 0;
    [HideInInspector]
    public Button finishPictureButton;
    [HideInInspector]
    public int oldScreenWidth = 0;
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
    }

    void Start()
    {
        finishPictureButton = GameObject.Find("FinishPicture").GetComponent<Button>();
        paintCanvasIndex = 0;
        currentPaintColor = Color.red;
        sizeMultiplyer = 5;
        currentPaintCanvas = PaintCanvases[0];
        SetPaintCounters();
        currentState = State.INTRO;


        updateResolution();
    }

    // Update is called once per frame
    void Update()
    {
        updateResolution();
        if (currPixelCount != 0)
        {
            percentComplete = (float)currPixelCount / (float)totalPixelCount;
        }

        if (percentComplete >= .75f)
        {
            finishPictureButton.interactable = true;
        }
        else
        {
            finishPictureButton.interactable = false;
        }
    }

    public void NextPicture()
    {
        if (paintCanvasIndex == PaintCanvases.Length - 1)
        {
            currentState = State.FINISHED;
            currentPaintCanvas.GetComponent<Painting>().enabled = false;
            for (int i = 0; i < PaintCanvases.Length; i++)
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

    public void ExitGame()
    {
        Application.Quit();
    }

    private void updateResolution()
    {
        screenHeight = Screen.height;
        screenWidth = Screen.width;

        if(screenHeight != oldScreenHeight || screenWidth != oldScreenWidth)
        {
            SetPaintingResolutions();
        }
        oldScreenHeight = screenHeight;
        oldScreenWidth = screenWidth;
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

    private void SetPaintingResolutions()
    {
        Vector2 resolution = new Vector2(screenWidth, screenHeight);
        for (int i = 0; i < PaintCanvases.Length; i++)
        {
            PaintCanvases[i].GetComponent<RectTransform>().sizeDelta = resolution;
            PaintCanvasPair[i].GetComponent<RectTransform>().sizeDelta = resolution;
        }

        RectTransform FadeGraphic = GameObject.Find("FadeGraphic").GetComponent<RectTransform>();
        RectTransform ToolBar = GameObject.Find("ToolBar").GetComponent<RectTransform>();
        RectTransform DyeDropper = GameObject.Find("DyeDropper").GetComponent<RectTransform>();
        RectTransform ColorPicker = GameObject.Find("ColorPicker").GetComponent<RectTransform>();
        RectTransform Slider = GameObject.Find("Slider").GetComponent<RectTransform>();
        RectTransform finishPic = finishPictureButton.GetComponent<RectTransform>();
        RectTransform quoteArea = GameObject.Find("Quote").GetComponent<RectTransform>();
        RectTransform exit = GameObject.Find("Exit").GetComponent<RectTransform>();

        FadeGraphic.sizeDelta = resolution;
        ColorPicker.sizeDelta = resolution * .75f;
        ToolBar.sizeDelta = new Vector2(screenWidth, screenHeight / 10);
        ToolBar.localPosition = new Vector3(0, (screenHeight - ToolBar.sizeDelta.y) / 2, 0);

        DyeDropper.sizeDelta = new Vector2(screenHeight / 10, screenHeight / 10);
        DyeDropper.localPosition = new Vector3((screenWidth-DyeDropper.sizeDelta.x)/2, ToolBar.localPosition.y, 0);

        float sliderWidthDefault = 200f / 1920f;
        float sliderHeightDefault = 20f / 1080f;
        Slider.sizeDelta = new Vector2((float)screenWidth* sliderWidthDefault, (float)screenHeight* sliderHeightDefault);
        Slider.localPosition = new Vector3(((screenWidth - DyeDropper.sizeDelta.x) / 2)-Slider.sizeDelta.x - DyeDropper.sizeDelta.x, ToolBar.localPosition.y, 0);

        finishPic.sizeDelta = new Vector2(screenWidth / 10, (screenHeight / 10)/4);
        finishPic.localPosition = new Vector3(((-screenWidth+finishPic.sizeDelta.x)/2)+finishPic.sizeDelta.x, ToolBar.localPosition.y, 0);
        finishPic.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = screenWidth * .0125f;

        quoteArea.sizeDelta = new Vector2(screenWidth / 2, screenHeight);
        quoteArea.gameObject.GetComponent<TextMeshProUGUI>().fontSize = screenWidth * .01875f;

        exit.sizeDelta = new Vector2(screenWidth / 20, (screenHeight / 10) / 4);
        exit.localPosition = new Vector3(((-screenWidth + finishPic.sizeDelta.x) / 2) + (finishPic.sizeDelta.x*2)+exit.sizeDelta.x, ToolBar.localPosition.y, 0);
        exit.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = screenWidth * .0125f;
    }

}
