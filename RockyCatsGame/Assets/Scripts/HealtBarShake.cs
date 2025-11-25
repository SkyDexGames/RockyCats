using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBarShake : MonoBehaviour
{
    public RectTransform barTransform; // El rect transform de la barra
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 10f;

    private Vector3 originalPosition;

    private void Start()
    {
        if (barTransform == null) 
            barTransform = GetComponent<RectTransform>();

        originalPosition = barTransform.anchoredPosition;
    }

    public void Shake()
    {
        
        StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            barTransform.anchoredPosition = new Vector2(originalPosition.x + x, originalPosition.y + y);

            elapsed += Time.deltaTime;
            yield return null;
        }

        barTransform.anchoredPosition = originalPosition;
    }
}
