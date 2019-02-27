using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Painting : MonoBehaviour
{
    Texture2D texture;
    public GameObject paintingCanvas;
    Image paintingImage;
    Sprite paintingSprite;
    int screenWidth;
    int screenHeight;
    Texture2D originalTexture;

    void Start()
    {
        paintingImage = paintingCanvas.GetComponent<Image>();
        paintingSprite = paintingImage.sprite;
        texture = paintingSprite.texture;
        originalTexture = new Texture2D(texture.width, texture.height);
        originalTexture.SetPixels(texture.GetPixels());
        texture.Apply();

    }

    // Update is called once per frame
    void Update()
    {
        screenHeight = GameManager.Instance.screenHeight;
        screenWidth = GameManager.Instance.screenWidth;
        RectTransform paintingRectTransform = paintingCanvas.GetComponent<RectTransform>();
        Vector3 position;
        Texture2D old = new Texture2D((int)texture.width, (int)texture.height);
        Color[] colors = texture.GetPixels(0, 0, (int)texture.width, (int)texture.height);
        old.SetPixels(colors);
        Color currentPaintColor = GameManager.Instance.currentPaintColor;
        int sizeMultiplyer = GameManager.Instance.sizeMultiplyer;
        int count = 0;

        if (Input.GetKey(KeyCode.Mouse0))
        {
            position = Camera.main.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, paintingCanvas.transform.position.z));
            position.x *= screenWidth;
            position.y *= screenHeight;

            //Calculate the positions within the texture.
            Vector3 pictureCenter = Camera.main.ScreenToViewportPoint(paintingRectTransform.position);
            pictureCenter.x *= screenWidth;
            pictureCenter.y *= screenHeight;

            float denomY = ((float)screenHeight / paintingRectTransform.sizeDelta.y) * 2;
            float denomX = ((float)screenWidth / paintingRectTransform.sizeDelta.x) * 2;


            Vector3 FirstPixel = pictureCenter - new Vector3(screenWidth / denomX,-screenHeight/denomY,0);
            
            Vector3 pixel = position - FirstPixel;
            pixel.x = pixel.x * texture.width / paintingRectTransform.sizeDelta.x;
            pixel.y = (paintingRectTransform.sizeDelta.y + pixel.y) * texture.height / paintingRectTransform.sizeDelta.y;

            pixel = new Vector3(pixel.x, pixel.y, pixel.z);

            int squareWidth = 2 * sizeMultiplyer;

            if (pixel.x > 0 && pixel.y > 0 && pixel.x < texture.width && pixel.y < texture.height)
            {
                for (int i = 0; i < squareWidth; i++)
                {
                    for (int j = 0; j < squareWidth; j++)
                    {
                        Color color = old.GetPixel((int)pixel.x - (squareWidth / 2) + i, (int)pixel.y - (squareWidth / 2) + j);
                        if (color.a != 0 && !IsBasicallyBlack(color))
                        {
                            old.SetPixel((int)pixel.x - (squareWidth / 2) + i, (int)pixel.y - (squareWidth / 2) + j, currentPaintColor);
                            if(color == originalTexture.GetPixel((int)pixel.x - (squareWidth / 2) + i, (int)pixel.y - (squareWidth / 2) + j))
                                count++;
                        }
                    }
                }
            }
        }
        GameManager.Instance.currPixelCount += count;

        texture.SetPixels(old.GetPixels(0,0, old.width, old.height));
        texture.Apply();
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

}
