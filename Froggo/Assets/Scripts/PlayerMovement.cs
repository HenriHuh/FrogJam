using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    //Assign in editor
    public LayerMask aimLayer;
    public LineRenderer line;
    public Collider aimCollider;
    public Transform gravityObjectParent;

    //Other
    GameObject currentPlatform;
    Vector3 aimDir;
    Rigidbody rb;
    bool aiming = false, onPlanet, swapped;
    List<Transform> gravityObjectPool = new List<Transform>();


    void Start()
    {
        Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), aimCollider);
        rb = gameObject.GetComponent<Rigidbody>();
        line.gameObject.SetActive(false);

        foreach (Transform t in gravityObjectParent)
        {
            if (t.gameObject.GetComponent<Rigidbody>())
            {
                gravityObjectPool.Add(t);
            }
        }
    }



    void Update()
    {
        if (!onPlanet)
        {
            //Do planet gravity stuff
            //foreach (Transform t in gravityObjectPool)
            //{
            //    Vector3 dir = transform.position - t.position;
            //    dir.y = 0;
            //    float dist = dir.magnitude;
            //    float magnitude = (rb.mass * t.GetComponent<Rigidbody>().mass) / Mathf.Pow(dist, 2);
            //    Vector3 grav = dir.normalized * magnitude;
            //    rb.AddForce(grav);
            //}

            foreach (Transform t in gravityObjectPool)
            {
                Vector3 dir = (t.position - transform.position).normalized;
                float fixedMagnitude = (10 - (transform.position - t.position).magnitude);
                fixedMagnitude = fixedMagnitude < 0 ? 0 : fixedMagnitude;
                rb.AddForce(dir * fixedMagnitude);
            }

            return;
        }



        transform.position = currentPlatform.transform.position;

        if (aiming)
        {
            //Raycast
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 50, aimLayer);

            //Limit vector
            aimDir = Vector3.MoveTowards(transform.position, hit.point, 5);
            aimDir.y = transform.position.y;

            //Display line
            line.gameObject.SetActive(true);
            line.SetPosition(0, transform.position);
            line.SetPosition(line.positionCount - 1, aimDir);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            aiming = true;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0) && aiming)
        {
            aiming = false;
            LaunchPlayer();
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Base" && !onPlanet)
        {
            //Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), col.gameObject.GetComponent<Collider>());
            currentPlatform = col.gameObject;
            col.gameObject.GetComponent<Rigidbody>().AddForce(rb.velocity * 5);
            rb.velocity = Vector3.zero;
            transform.position = col.gameObject.transform.position;
            onPlanet = true;
        }

        if (col.gameObject.tag == "Edge" && !swapped)
        {
            swapped = true;
            StartCoroutine(SwapSide());
            Vector3 npos = transform.position;
            if (Mathf.Abs(transform.position.x) > Mathf.Abs(transform.position.z))
            {
                npos.x *= -1;
                transform.position = npos;
            }
            else
            {
                npos.z *= -1;
                transform.position = npos;
            }
        }
    }

    IEnumerator SwapSide()
    {
        yield return new WaitForSeconds(0.25f);
        swapped = false;
        yield return null;
    }


    void LaunchPlayer()
    {
        aimDir.y = 0;
        float magnitude = Vector3.Distance(transform.position, aimDir);
        aimDir = (transform.position - aimDir).normalized;
        rb.velocity = Vector3.zero;
        rb.AddForce(aimDir * 150 * magnitude);
        onPlanet = false;
        line.gameObject.SetActive(false);
        currentPlatform.GetComponent<Rigidbody>().AddForce(aimDir * -20 * magnitude);
        //StartCoroutine(DelayedColliderFix());
    }

    IEnumerator DelayedColliderFix()
    {
        yield return new WaitForSeconds(0.5f);
        Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), currentPlatform.GetComponent<Collider>(), false);
        yield return null;
    }
}
