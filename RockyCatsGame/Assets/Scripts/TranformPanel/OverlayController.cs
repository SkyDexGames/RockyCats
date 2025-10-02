using UnityEngine;

public class OverlayController : MonoBehaviour
{
    [SerializeField] GameObject overlayRoot;

    void Awake()
    {
        if (overlayRoot != null) overlayRoot.SetActive(false); // empieza cerrado
    }

    public void Toggle()
    {
        if (overlayRoot == null) return;
        overlayRoot.SetActive(!overlayRoot.activeSelf);
    }

    public void Open() { if (overlayRoot) overlayRoot.SetActive(true); }
    public void Close() { if (overlayRoot) overlayRoot.SetActive(false); }
}
