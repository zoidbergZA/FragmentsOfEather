using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class ContactDestroyer : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Players"))
            return;

        Destroy(coll.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(other.gameObject);
    }
}
