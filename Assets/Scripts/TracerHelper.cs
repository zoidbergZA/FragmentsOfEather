using UnityEngine;
using System.Collections;

public class TracerHelper : MonoBehaviour 
{
    [SerializeField] private Tracer TracerPrefab;

    private Tracer[] tracers = new Tracer[20];
    private int tracerIndex;

    void Awake()
    {
        for (int i = 0; i < tracers.Length; i++)
        {
            tracers[i] = (Tracer)Instantiate(TracerPrefab, Vector3.zero, Quaternion.identity);
            tracers[i].transform.SetParent(transform);
            tracers[i].Reset();
        }
    }

    public void ShowTracer(Vector3 start, Vector3 end)
    {
        tracers[tracerIndex].Activate(start, end);

        tracerIndex++;

        if (tracerIndex >= tracers.Length)
            tracerIndex = 0;
    }
}
