using System;
using UnityEngine;
using System.Collections;

public class EraProgressor : MonoBehaviour
{
    [SerializeField] private GameObject[] eraModels;

    void Start()
    {
        SetModel(GameManager.Instance.EraManager.CurrentEra);
    }

    void OnEnable()
    {
        EraManager.NewEra += OnNewEra;
    }

    void OnDisable()
    {
        EraManager.NewEra -= OnNewEra;
    }

    private void OnNewEra(int newEra)
    {
        //Debug.Log(name + "listened to new era (era " + newEra + ") event at: " + Time.time);

        SetModel(newEra);
    }

    private void SetModel(int era)
    {
        for (int i = 0; i < eraModels.Length; i++)
        {
            if (i == GameManager.Instance.EraManager.CurrentEra)
                eraModels[i].SetActive(true);
            else
                eraModels[i].SetActive(false);
        }
    }
}
