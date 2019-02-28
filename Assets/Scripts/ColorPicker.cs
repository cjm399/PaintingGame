using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    Texture2D colorSelection;
    RectTransform colorSelectionRect;
    // Start is called before the first frame update
    void Start()
    {
        colorSelection = GameObject.Find("ColorPicker").GetComponent<Image>().sprite.texture;
        colorSelectionRect = GameObject.Find("ColorPicker").GetComponent<RectTransform>();
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
            Texture2D old = new Texture2D((int)colorSelection.width, (int)colorSelection.height);
            Color[] colors = colorSelection.GetPixels(0, 0, (int)colorSelection.width, (int)colorSelection.height);
            old.SetPixels(colors);
            Color currentPaintColor = GameManager.Instance.currentPaintColor;
            int sizeMultiplyer = GameManager.Instance.sizeMultiplyer;
            int count = 0;

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
                    GameManager.Instance.currentState = GameManager.State.PAINTING;
                }
            }
        }
    }

    public void Selected()
    {
        GameManager.Instance.currentState = GameManager.State.PICK_COLOR;
    }
}
