using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class Safezone : MonoBehaviour
{
    private Mainland mainland;
    [SerializeField] private Transform visualTransform;

    public float Range { get { return GetComponent<CircleCollider2D>().radius; } } 

    void Awake()
    {
        mainland = transform.root.GetComponent<Mainland>();
    }

    public void IncreaseSize(float amount, float asFraction)
    {
        GetComponent<CircleCollider2D>().radius += amount;
        visualTransform.localScale *= asFraction;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Island island = other.GetComponent<Island>();

        if (island)
        {
            island.ConnectToMainland();
            GameManager.Instance.AudioManager.playSFX("PlatherialPlanetCreated");
        }
    }
}
