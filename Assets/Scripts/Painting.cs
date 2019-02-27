using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Painting : MonoBehaviour
{
    Texture2D texture;
    GameObject paintingCanvas;
    Image paintingImage;
    Sprite paintingSprite;
    int screenWidth;
    int screenHeight;

    void Start()
    {
        paintingCanvas = GameObject.Find("PaintingCanvas");
        paintingImage = paintingCanvas.AddComponent<Image>();
        texture = new Texture2D(1920 / 4, 1080 / 4);
        Color[] colors = texture.GetPixels();
        Color white = Color.white;
        for (int i = 0; i < colors.Length; ++i)
        {
            colors[i] = white;
        }
        texture.SetPixels(colors);
        texture.Apply();

        paintingSprite = Sprite.Create(texture, new Rect(0, 0, 1920 / 4, 1080/4), new Vector2(0f, 0f), 72);
        
        paintingImage.sprite = paintingSprite;
    }

    // Update is called once per frame
    void Update()
    {
        screenHeight = GameManager.Instance.screenHeight;
        screenWidth = GameManager.Instance.screenWidth;
        RectTransform paintingRectTransform = paintingCanvas.GetComponent<RectTransform>();
        Vector3 position;
        Texture2D old = new Texture2D((int)(paintingRectTransform.sizeDelta.x), (int)(paintingRectTransform.sizeDelta.y));
        Color[] colors = texture.GetPixels(0, 0, (int)(paintingRectTransform.sizeDelta.x), (int)(paintingRectTransform.sizeDelta.y));
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

            Debug.Log(pictureCenter);

            float denomY = (screenHeight / paintingRectTransform.sizeDelta.y) * 2;
            float denomX = (screenWidth / paintingRectTransform.sizeDelta.x) * 2;

            Vector3 FirstPixel = pictureCenter - new Vector3(screenWidth / denomX,-screenHeight/denomY,0);
            
            Vector3 pixel = position - FirstPixel;
            Debug.Log(pixel);
            pixel = new Vector3(pixel.x, paintingRectTransform.sizeDelta.y + pixel.y, pixel.z);

            int squareWidth = (int)Mathf.Pow(2, 4);
            //Debug.Log(pixel);
            if (pixel.x > 0 && pixel.y > 0 && pixel.x < paintingRectTransform.sizeDelta.x && pixel.y < paintingRectTransform.sizeDelta.y)
            {
                for (int i = 0; i < squareWidth; i++)
                {
                    for (int j = 0; j < squareWidth; j++)
                    {
                        //old.SetPixel((int)position.x - (squareWidth / 2) + i, (int)position.y - (squareWidth / 2) + j, Color.red);
                        old.SetPixel((int)pixel.x - (squareWidth / 2) + i, (int)pixel.y - (squareWidth / 2) + j, Color.red);
                    }
                }
            }
        }

        texture.SetPixels(old.GetPixels(0,0, old.width, old.height));
        texture.Apply();
        paintingCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(1920/4, 1080/4);
    }

}
