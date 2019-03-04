using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Intro : MonoBehaviour
{
    GameObject quote;
    Image fadeGraphic;
    Image colorPicker;
    RectTransform colorSelectionRect;
    bool shouldPickColor;
    bool isOpen = false;
    bool allowClose = false;


    void Start()
    {
        string quoteText = "I believe that imagination is stronger than knowledge. That myth is more potent than history. That dreams are more powerful than facts. That hope always triumphs over experience. That laughter is the only cure for grief. And I believe that love is stronger than death.\n\n- Robert Fulghum";
        quote = GameObject.Find("Quote");
        fadeGraphic = GameObject.Find("FadeGraphic").GetComponent<Image>();
        colorPicker = GameObject.Find("ColorPicker").GetComponent<Image>();
        StartCoroutine(FillText(quoteText, quote.GetComponent<TextMeshProUGUI>()));
        shouldPickColor = false;
        colorSelectionRect = GameObject.Find("ColorPicker").GetComponent<RectTransform>();
    }

    IEnumerator FillText(string text, TextMeshProUGUI area)
    {
        for(int i = 0; i < text.Length; i++)
        {
            area.text = text.Substring(0, i);
            yield return new WaitForSeconds(.06f);
        }
        yield return StartCoroutine(WaitForReadingTime());
    }

    IEnumerator WaitForReadingTime()
    {
        float timer = 0;
        float duration = 5f;
        while(timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        yield return StartCoroutine(FadeOutQuote());
    }

    IEnumerator FadeOutQuote()
    {
        float timer = 0;
        float duration = 2f;
        TextMeshProUGUI text = quote.GetComponent<TextMeshProUGUI>();
        Color blank = new Color(text.color.r, text.color.g, text.color.b, 0f);
        while (timer < duration)
        {
            text.color = Color.Lerp(text.color, blank, timer/duration);
            timer += Time.deltaTime;
            yield return null;
        }
        shouldPickColor = true;
        yield return null;
    }

    private void Update()
    {
        if(shouldPickColor)
        {
            if (!isOpen)
            {
                isOpen = true;
                StopAllCoroutines();
                StartCoroutine(FadeInCanvas());
            }
            if(!allowClose)
            {
                return;
            }
            int screenHeight = GameManager.Instance.screenHeight;
            int screenWidth = GameManager.Instance.screenWidth;
            RectTransform paintingRectTransform = colorSelectionRect.GetComponent<RectTransform>();
            Vector3 position;
            Texture2D colorSelection = colorPicker.sprite.texture;

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
                    Camera.main.backgroundColor = selectedColor;
                    GameManager.Instance.currentState = GameManager.State.PAINTING;
                    shouldPickColor = false;
                    StopAllCoroutines();
                    StartCoroutine(FadeOutCanvas());
                }
            }
        }
    }



    IEnumerator FadeInCanvas()
    {
        Debug.Log("FADEIN");
        float timer = 0;
        float duration = 2f;
        Color fadeFinal = new Color(fadeGraphic.color.r, fadeGraphic.color.g, fadeGraphic.color.b, 1f);
        Color colorPalletFinal = new Color(colorPicker.color.r, colorPicker.color.g, colorPicker.color.b, 1f);
        while (timer < duration)
        {
            fadeGraphic.color = Color.Lerp(fadeGraphic.color, fadeFinal, duration);
            colorPicker.color = Color.Lerp(colorPicker.color, colorPalletFinal, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        GameManager.Instance.PaintCanvases[0].SetActive(true);
        GameManager.Instance.PaintCanvasPair[0].SetActive(true);
        GameManager.Instance.PaintCanvases[0].gameObject.GetComponent<Painting>().enabled = false;
        allowClose = true;

        yield return null;
    }

    IEnumerator FadeOutCanvas()
    {
        GameObject.Find("HUD").GetComponent<Canvas>().enabled = true;
        Debug.Log("FADEOUT");
        float timer = 0;
        float duration = 1f;
        Color fadeFinal = new Color(fadeGraphic.color.r, fadeGraphic.color.g, fadeGraphic.color.b, 0f);
        Color colorPalletFinal = new Color(colorPicker.color.r, colorPicker.color.g, colorPicker.color.b, 0f);
        while (timer < duration)
        {
            fadeGraphic.color = Color.Lerp(fadeGraphic.color, fadeFinal, duration);
            colorPicker.color = Color.Lerp(colorPicker.color, colorPalletFinal, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        StartRealGame();
    }

    void StartRealGame()
    {
        Debug.Log("StartGame");
        GameManager.Instance.PaintCanvases[0].gameObject.GetComponent<Painting>().enabled = true;

        this.gameObject.SetActive(false);
    }
}
