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

    void Start()
    {
        paintingImage = paintingCanvas.GetComponent<Image>();
        paintingSprite = paintingImage.sprite;
        texture = paintingSprite.texture;
        Color[] colors = texture.GetPixels();
        Color white = Color.white;
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
            Debug.Log(pixel);
            pixel = new Vector3(pixel.x, pixel.y, pixel.z);

            int squareWidth = (int)Mathf.Pow(2, 6);

            if (pixel.x > 0 && pixel.y > 0 && pixel.x < texture.width && pixel.y < texture.height)
            {
                for (int i = 0; i < squareWidth; i++)
                {
                    for (int j = 0; j < squareWidth; j++)
                    {
                        Color color = old.GetPixel((int)pixel.x - (squareWidth / 2) + i, (int)pixel.y - (squareWidth / 2) + j);
                        if (color.a != 0 && (color.r != 0 && color.g != 0 && color.b != 0))
                            old.SetPixel((int)pixel.x - (squareWidth / 2) + i, (int)pixel.y - (squareWidth / 2) + j, Color.red);
                    }
                }
            }
        }

        texture.SetPixels(old.GetPixels(0,0, old.width, old.height));
        texture.Apply();
    }

}
