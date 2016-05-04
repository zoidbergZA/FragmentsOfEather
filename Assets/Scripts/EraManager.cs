using UnityEngine;
using System.Collections;

public delegate void NewEraHandler(int newEra);

public class EraManager : MonoBehaviour
{
    public static event NewEraHandler NewEra;

    [SerializeField] private int eraCount = 3;

    public int CurrentEra { get; private set; }
    public float NextEraIn { get; private set; }
    public float EraInterval { get { return GameManager.Instance.level.levelTime/eraCount; } }
    public int EraCount { get { return eraCount; } }

    void Awake()
    {
        NextEraIn = EraInterval;
    }

    void Update()
    {
        if (GameManager.Instance.GameState != GameManager.GameStates.RUNNING)
            return;

        EraProgressChecker();
    }

    private void EraProgressChecker()
    {
        if (NextEraIn <= 0 && CurrentEra < eraCount)
        {
            CurrentEra++;
            if(CurrentEra == 1)
            {
                GameManager.Instance.AudioManager.EnterEraOne();
            }
            else if (CurrentEra == 2)
            {
                GameManager.Instance.AudioManager.EnterEraTwo();
            }

            NextEraIn += EraInterval;

            if (NewEra != null)
                NewEra(CurrentEra);
            
        }

        NextEraIn -= Time.deltaTime;
    }
}
