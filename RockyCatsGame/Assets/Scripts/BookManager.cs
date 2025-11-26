using UnityEngine;
using UnityEngine.UI;

public class BookManager : MonoBehaviour
{
    
    public Image panelImage;
    public Sprite[] images;
    public Button buttonUp;
    public Button buttonDown;

    private int currentIndex = 0;

    private void OnEnable()
    {
        // Reinicia a la primera imagen cuando se active el panel
        currentIndex = 0;

        if (panelImage != null && images.Length > 0)
            panelImage.sprite = images[currentIndex];
        UpdateButtons();
    }

    public void ChangeUp()
    {
        if (images.Length == 0 || panelImage == null) return;

        currentIndex++;
        if (currentIndex >= images.Length)
            currentIndex = 0;

        panelImage.sprite = images[currentIndex];
        UpdateButtons();
    }
    public void ChangeDown()
    {
        if (images.Length == 0 || panelImage == null) return;

        currentIndex--;
        if (currentIndex < 0)
            currentIndex = images.Length - 1;

        panelImage.sprite = images[currentIndex];
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        if (buttonUp != null)
            buttonUp.interactable = currentIndex < images.Length - 1;

        if (buttonDown != null)
            buttonDown.interactable = currentIndex > 0;
    }
}
