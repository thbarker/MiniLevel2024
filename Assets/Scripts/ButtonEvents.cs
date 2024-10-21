using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEvents : MonoBehaviour
{
    public TMP_Text buttonText;
    public GameObject selectedImage;
    private Button button;
    public bool on;

    public void Start()
    {
        button = GetComponent<Button>();
    }

    public void OnHoverEnter()
    {
        buttonText.color = Color.yellow;
    }

    public void OnHoverExit()
    {
        buttonText.color = Color.white;
    }
    public void OnHoverEnterImage()
    {
        if(on)
        {
            selectedImage.SetActive(true);
        }
    }

    public void OnHoverExitImage()
    {
        selectedImage.SetActive(false);

    }
}
