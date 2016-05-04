using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private Text textBox;

    private Transform target;
    private RectTransform rectTransform;
    private Vector2 offset;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetText(Transform targeTransform, string msg, Color color)
    {
        target = targeTransform;
        textBox.text = msg;
        textBox.color = color;
    }

    void Update()
    {
        if (target)
        {
            rectTransform.anchoredPosition = (Vector2)Camera.main.WorldToScreenPoint(target.position) + offset;
        }

        offset.y += speed*Time.deltaTime;
//        speed *= 0.95f;
    }
}
