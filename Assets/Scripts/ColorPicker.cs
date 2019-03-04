using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    Texture2D colorSelection;
    Image colorSelectionImage;
    Image fadeCanvas;
    RectTransform colorSelectionRect;
    bool isOpen = false;
    // Start is called before the first frame update
    void Start()
    {
        colorSelection = GameObject.Find("ColorPicker").GetComponent<Image>().sprite.texture;
        colorSelectionImage = GameObject.Find("ColorPicker").GetComponent<Image>();
        colorSelectionRect = GameObject.Find("ColorPicker").GetComponent<RectTransform>();
        fadeCanvas = GameObject.Find("FadeGraphic").GetComponent<Image>();

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.currentState == GameManager.State.PICK_COLOR)
        {
            int screenHeight = GameManager.Instance.screenHeight;
            int screenWidth = GameManager.Instance.screenWidth;
            RectTransform paintingRectTransform = colorSelectionRect.GetComponent<RectTransform>();
            Vector3 position;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                position = Camera.main.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, colorSelectionRect.transform.position.z));
                position.x *= screenWidth;
                position.y *= screenHeight;

                //Calculate the positions within the texture.
                Vector3 pictureCenter = Camera.main.ScreenToViewportPoint(paintingRectTransform.position);
                pictureCenter.x *= screenWidth;
                pictureCenter.y *= screenHeight;

                float denomY = ((float)screenHeight / paintingRectTransform.sizeDelta.y) * 2;
                float denomX = ((float)screenWidth / paintingRectTransform.sizeDelta.x) * 2;


                Vector3 FirstPixel = pictureCenter - new Vector3(screenWidth / denomX, -screenHeight / denomY, 0);

                Vector3 pixel = position - FirstPixel;
                pixel.x = pixel.x * colorSelection.width / paintingRectTransform.sizeDelta.x;
                pixel.y = (paintingRectTransform.sizeDelta.y + pixel.y) * colorSelection.height / paintingRectTransform.sizeDelta.y;

                pixel = new Vector3(pixel.x, pixel.y, pixel.z);

                if (pixel.x > 0 && pixel.y > 0 && pixel.x < colorSelection.width && pixel.y < colorSelection.height)
                {
                    Color selectedColor = colorSelection.GetPixel((int)pixel.x, (int)pixel.y);
                    GameManager.Instance.currentPaintColor = selectedColor;
                    GameManager.Instance.currentState = GameManager.State.PICKED;
                    StopAllCoroutines();
                    StartCoroutine(FadeOutCanvas());
                }
            }
        }
    }

    public void Selected()
    {
        StopAllCoroutines();
        if(!isOpen)
        {
            GameManager.Instance.currentState = GameManager.State.PICK_COLOR;
            StartCoroutine(FadeInCanvas());
        }
        else
        {
            GameManager.Instance.currentState = GameManager.State.PAINTING;
            StartCoroutine(FadeOutCanvas());
        }

    }

    IEnumerator FadeInCanvas()
    {
        isOpen = true;
        float timer = 0;
        float duration = 2f;
        Color fadeFinal = new Color(fadeCanvas.color.r, fadeCanvas.color.g, fadeCanvas.color.b, 1f);
        Color colorPalletFinal = new Color(colorSelectionImage.color.r, colorSelectionImage.color.g, colorSelectionImage.color.b, 1f);
        while (timer < duration)
        {
            fadeCanvas.color = Color.Lerp(fadeCanvas.color, fadeFinal, timer / duration);
            colorSelectionImage.color = Color.Lerp(colorSelectionImage.color, colorPalletFinal, duration);
            timer += Time.deltaTime;
            yield return null;
        }
        
        yield return null;
    }

    IEnumerator FadeOutCanvas()
    {
        isOpen = false;
        float timer = 0;
        float duration = 1f;
        Color fadeFinal = new Color(fadeCanvas.color.r, fadeCanvas.color.g, fadeCanvas.color.b, 0f);
        Color colorPalletFinal = new Color(colorSelectionImage.color.r, colorSelectionImage.color.g, colorSelectionImage.color.b, 0f);
        while (timer < duration)
        {
            fadeCanvas.color = Color.Lerp(fadeCanvas.color, fadeFinal, timer / duration);
            colorSelectionImage.color = Color.Lerp(colorSelectionImage.color, colorPalletFinal, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        GameManager.Instance.currentState = GameManager.State.PAINTING;
        yield return null;
    }
}
