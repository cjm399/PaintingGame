using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintBrushSize : MonoBehaviour
{
    public void ChangeSize()
    {
        GameManager.Instance.sizeMultiplyer = (int)this.GetComponent<Slider>().value;
    }
}
