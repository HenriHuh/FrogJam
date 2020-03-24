using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    Rigidbody rb;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.tag == "PushAway")
        {
            Vector3 axis = Vector3.zero;

            if (Mathf.Abs(transform.position.x) > Mathf.Abs(transform.position.z))
            {
                axis = Vector3.right * -Mathf.Sign(transform.position.x);
                float magnitude = 2.5f - Mathf.Abs(transform.position.x - col.transform.position.x);
                magnitude = magnitude < 0 ? 0 : magnitude;
                rb.AddForce(axis * 5 * magnitude);
            }
            else
            {
                axis = Vector3.forward * -Mathf.Sign(transform.position.z);
                float magnitude = 2.5f - Mathf.Abs(transform.position.z - col.transform.position.z);
                magnitude = magnitude < 0 ? 0 : magnitude;
                rb.AddForce(axis * 5 * magnitude);
            }
        }
    }
}
