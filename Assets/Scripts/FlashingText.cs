using UnityEngine;
using TMPro;

public class FlashingText : MonoBehaviour
{
    private TMP_Text textMesh;
    [SerializeField] private float speed = 3f;

    private void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (textMesh != null)
        {
            // Calculate a slow sinusoidal oscillation between 0 and 1
            float t = (Mathf.Sin(Time.time * speed) + 1f) / 2f;
            // Smoothly interpolate between black and white
            textMesh.color = Color.Lerp(Color.black, Color.white, t);
        }
    }
}
