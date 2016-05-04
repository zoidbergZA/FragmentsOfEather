using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Text healthText;

    private Vulnerable target;
    private RectTransform myRectTransform;
    [SerializeField] private Image backImage;
    [SerializeField] private Image frontImage;

    void Start()
    {
        myRectTransform = GetComponent<RectTransform>();
        myRectTransform.SetParent(GameManager.Instance.Hud.GetComponent<RectTransform>());
    }
    	
	void Update ()
    {
	    if (target)
	    {
			if (target.Hitpoints <= 0 || target.Invulnerable)
				ToggleVissible (false);
			else
				ToggleVissible (true);

			float frac = target.Hitpoints / target.MaxHP;

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, target.transform.position) + Vector2.down * 25f;
            myRectTransform.anchoredPosition = screenPoint - GameManager.Instance.Hud.CanvasRect.sizeDelta / 2f;

            healthText.text = (int)(target.Hitpoints) + "/" + target.MaxHP;
			frontImage.GetComponent<RectTransform> ().localScale = new Vector3 (frac, 1, 1);
			frontImage.color = Color.Lerp (Color.red, Color.green, frac);
        }

	    else
	    {
	        Destroy(gameObject);
	    }
	}

    public void SetTarget(Vulnerable targetVulnrerable)
    {
        target = targetVulnrerable;
    }

	private void ToggleVissible(bool show)
	{
		if (show) {
			frontImage.enabled = true;
			backImage.enabled = true;
		} else {
			frontImage.enabled = false;
			backImage.enabled = false;
		}
	}
}
