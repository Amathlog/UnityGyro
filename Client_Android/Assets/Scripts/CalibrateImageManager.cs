using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalibrateImageManager : MonoBehaviour {

    public Sprite center;
    public Sprite topLeft;
    public Sprite botRight;

	public void SetCenter() {
        Color color = GetComponent<Image>().color;
        GetComponent<Image>().color = new Color(color.r, color.g, color.b, 1.0f);
        GetComponent<Image>().sprite = center;
    }

    public void SetTopLeft() {
        Color color = GetComponent<Image>().color;
        GetComponent<Image>().color = new Color(color.r, color.g, color.b, 1.0f);
        GetComponent<Image>().sprite = topLeft;
    }

    public void SetBotRight() {
        Color color = GetComponent<Image>().color;
        GetComponent<Image>().color = new Color(color.r, color.g, color.b, 1.0f);
        GetComponent<Image>().sprite = botRight;
    }

    public void Disable() {
        Color color = GetComponent<Image>().color;
        GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.0f);
    }
}
