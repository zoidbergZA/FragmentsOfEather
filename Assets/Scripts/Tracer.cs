using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class Tracer : MonoBehaviour
{
    public float fadeTime;
    public Color FromColor;
    public Color ToColor;

    private float fadeTimer;
    private LineRenderer lineRenderer;
    private bool inUse;
    private Color fadeColor = new Color();

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (!inUse)
            return;

        fadeTimer -= Time.deltaTime;

        if (fadeTimer <= 0)
        {
            Reset();
            return;
        }

        fadeColor = Color.Lerp(FromColor, ToColor, 1 - fadeTimer/fadeTime);
        lineRenderer.SetColors(fadeColor, fadeColor);
    }

    public void Activate(Vector3 start, Vector3 end)
    {
        inUse = true;
        fadeTimer = fadeTime;
        
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    public void Reset()
    {
        inUse = false;
        lineRenderer.SetColors(ToColor, ToColor);
    }
}
