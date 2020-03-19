using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollisions : MonoBehaviour
{
    public List<Collider> ignorable;

    void Start()
    {
        foreach (Collider c in ignorable)
        {
            Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), c);
        }
    }
}
