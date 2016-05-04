using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TechnologyPanel : MonoBehaviour
{
    [SerializeField] private Text techText;
    [SerializeField] private Sprite[] techSprites;
    [SerializeField] private Image techIcon;

    void OnEnable()
    {
        TechnologyManager.NewTechnologyReached += OnNewTechnologyReached;
    }

    void OnDisable()
    {
        TechnologyManager.NewTechnologyReached -= OnNewTechnologyReached;
    }

    void Update()
    {
        techText.text = GameManager.Instance.TechnologyManager.CurrentTechnologyPoints + "/" +
                           GameManager.Instance.TechnologyManager.NexTechnologyUpgrade.Steps;
    }

    private void OnNewTechnologyReached(int newEra)
    {
        if (newEra + 1 >= techSprites.Length)
        {
            enabled = false;
        }
		else
	        techIcon.sprite = techSprites[newEra + 1];
    }
}
