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
    private bool isHovering; // Tracks if the cursor is currently hovering over the image

    public void Start()
    {
        button = GetComponent<Button>();
    }

    public void Update()
    {
        UpdateImageState();
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
        isHovering = true; // Mark the image as being hovered over

        // Check if the image should be shown based on the 'on' state
        UpdateImageState();
    }

    public void OnHoverExitImage()
    {
        isHovering = false; // No longer hovering

        // Ensure the image is hidden when the cursor exits the hover
        if (selectedImage != null)
        {
            selectedImage.SetActive(false);
        }
    }

    // A helper method to check the 'on' state and update the image accordingly
    private void UpdateImageState()
    {
        if (isHovering && selectedImage != null)
        {
            if (on)
            {
                selectedImage.SetActive(true);
            }
            else
            {
                selectedImage.SetActive(false);
            }
        }
    }
}
