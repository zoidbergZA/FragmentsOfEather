using UnityEngine;
using System.Collections;

public class PlatherialUpgrade : Technology
{
    public override void DoUpgrade()
    {
        base.DoUpgrade();
        GameManager.Instance.AudioManager.playSFX("PlatherialSound2");
    }
}
