using UnityEngine;
using System.Collections;

public class BackgroundBlock : MonoBehaviour
{
	[SerializeField] private float speed = 20f;
    [SerializeField] private GameObject[] eras;

    void Awake()
    {
        SetEra(0);
    }

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }

    public void SetEra(int era)
    {
//        Debug.Log("CHANGE ERA ON BLOCK: " + name + ", at " + Time.time);

        for (int i = 0; i < eras.Length; i++)
        {
            if (i == era)
                eras[i].SetActive(true);
            else
                eras[i].SetActive(false);
        }
    }
}
