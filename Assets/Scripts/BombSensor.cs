using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class BombSensor : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemies"))
            return;
        
        transform.root.GetComponent<Bomb>().SetTarget(other.gameObject);
    }
}
