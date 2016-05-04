using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private RectTransform healthSlider;
    [SerializeField] private Image twirlIcon;
    [SerializeField] private Image bombIcon;

    public Player TargetPlayer { get; set; }
    public RectTransform RectTransform { get { return GetComponent<RectTransform>(); } }
    public Image HealthImage { get { return healthSlider.GetComponent<Image>(); } }

    void Start()
    {
        
    }

    void Update()
    {
        if (!TargetPlayer)
            return;

        if (TargetPlayer.Ship)
        {
            //health
            float healthFrac = 1f - TargetPlayer.Ship.Hitpoints / TargetPlayer.Ship.MaxHP;
            healthSlider.anchoredPosition = Vector2.left*healthSlider.rect.width*healthFrac;

            //twirl
            if (TargetPlayer.Ship.IsTwirlReady)
                twirlIcon.enabled = true;
            else
                twirlIcon.enabled = false;

            //bomb
            Weapon bombWeapon = TargetPlayer.Ship.WeaponManager.Weapons[1];
            if (bombWeapon.IsActivated && bombWeapon.IsReady)
                bombIcon.enabled = true;
            else
                bombIcon.enabled = false;
        }
    }

    public void SetColors(Color newColor)
    {
        HealthImage.color = newColor;
        twirlIcon.color = newColor;
        bombIcon.color = newColor;
    }
}
