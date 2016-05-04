using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfluenceSlider : MonoBehaviour
{
    [SerializeField] private float sliderSpeed = 3f;
    [SerializeField] private float sliderOffset = 100f;
    [SerializeField] private Text influenceText;
    [SerializeField] private RectTransform mainlandHealthbar;

    private Hud hud;
    private RectTransform myRectTransform;
    private float healthbarWidth;

    void Awake()
    {
        healthbarWidth = mainlandHealthbar.rect.width;
    }

    void Start()
    {
        hud = GameManager.Instance.Hud;
        myRectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        float leadFrac = 0;
        if (GameManager.Instance.Mainland.TotalInfluence > 0)
        {
            if (GameManager.Instance.Leader == GameManager.Instance.player1)
                leadFrac = GameManager.Instance.player1.Influence / GameManager.Instance.Mainland.TotalInfluence;
            else
                leadFrac = -GameManager.Instance.player2.Influence / GameManager.Instance.Mainland.TotalInfluence;
        }

        float yPos = (hud.PlayerPanelGap - sliderOffset) * leadFrac * 0.5f;

        myRectTransform.anchoredPosition = Vector2.Lerp(myRectTransform.anchoredPosition, new Vector2(0, yPos), Time.deltaTime * sliderSpeed);

        if (GameManager.Instance.Leader)
        {
            influenceText.color = GameManager.Instance.Leader.color;

            //text
            int lead = GameManager.Instance.player1.Influence - GameManager.Instance.player2.Influence;
            if (GameManager.Instance.Leader != GameManager.Instance.player1)
                lead *= -1;

            influenceText.text = lead.ToString();
        }

        //healthbar slider
        float offset = healthbarWidth + (1f - GameManager.Instance.Mainland.Hitpoints/GameManager.Instance.Mainland.MaxHP*healthbarWidth);
        mainlandHealthbar.anchoredPosition = new Vector2(-offset, 0);
    }
}
