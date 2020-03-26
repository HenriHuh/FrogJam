using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Handles input, gravity and collisions


    //Assign in editor
    public LayerMask aimLayer;
    public LineRenderer line;
    public Collider aimCollider;
    public Transform gravityObjectParent;
    public List<TrailRenderer> trails;
    public Renderer playerRenderer;
    public Material idle, jump;
    //Other
    GameObject currentPlatform;
    Vector3 aimDir;
    Rigidbody rb;
    bool aiming = false, onPlanet, swapped;
    List<Transform> gravityObjectPool = new List<Transform>();
    int trailIndex = 0;

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
        trails[trailIndex].transform.position = transform.position + Vector3.down * 0.5f;

        if (!onPlanet)
        {
            Vector3 moveDirection = rb.velocity;
            transform.LookAt(moveDirection);

            ApplyGravitation();

            Wrap();

            return;
        }


        transform.position = currentPlatform.transform.position;

        GetInput();

    }

    void Wrap()
    {
        if (transform.position.x > 12)
        {
            Vector3 pos = transform.position;
            pos.x = -12;
            transform.position = pos;
            ChangeTrail();
        }
        else if (transform.position.x < -12)
        {
            Vector3 pos = transform.position;
            pos.x = 12;
            transform.position = pos;
            ChangeTrail();
        }
        if (transform.position.z > 12)
        {
            Vector3 pos = transform.position;
            pos.z = -12;
            transform.position = pos;
            ChangeTrail();
        }
        else if (transform.position.z < -12)
        {
            Vector3 pos = transform.position;
            pos.z = 12;
            transform.position = pos;
            ChangeTrail();
        }
    }

    void ChangeTrail()
    {
        trailIndex++;
        if (trailIndex == trails.Count)
        {
            trailIndex = 0;
        }
        trails[trailIndex].gameObject.SetActive(false);

        StartCoroutine(SwapSide());
    }

    public void StartGame()
    {
        Time.timeScale = 1;
    }

    void ApplyGravitation()
    {
        foreach (Transform t in gravityObjectPool)
        {
            Vector3 dir = (t.position - transform.position).normalized;
            float fixedMagnitude = (10 - (transform.position - t.position).magnitude);
            fixedMagnitude = fixedMagnitude < 0 ? 0 : fixedMagnitude;
            rb.AddForce(dir * fixedMagnitude * 0.8f);
        }
    }
    
    void GetInput()
    {
        if (aiming)
        {
            //Raycast
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 50, aimLayer);

            //Limit vector
            aimDir = Vector3.MoveTowards(transform.position, hit.point, 5);
            aimDir.y = transform.position.y;

            transform.LookAt(aimDir);
            transform.Rotate(Vector3.right * 180);
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
            PointManager.instance.EndDrift();
            playerRenderer.material = idle;
            SoundManager.instance.EndDistort();
            SoundManager.instance.PlaySound(SoundManager.instance.land);

        }

        //if (col.gameObject.tag == "Edge")
        //{
        //    if (!swapped)
        //    {

        //        //Swap player to opposite side
        //        swapped = true;
        //        StartCoroutine(SwapSide());
        //        Vector3 npos = transform.position;
        //        if (Mathf.Abs(transform.position.x) > Mathf.Abs(transform.position.z))
        //        {
        //            npos.x *= -1;
        //            transform.position = npos;
        //        }
        //        else
        //        {
        //            npos.z *= -1;
        //            transform.position = npos;
        //        }

        //        //Swap trail to prevent glitching
        //        trailIndex++;
        //        if (trailIndex == trails.Count)
        //        {
        //            trailIndex = 0;
        //        }
        //        trails[trailIndex].gameObject.SetActive(false);
        //    }
        //    else
        //    {
        //        swapped = false;
        //    }
        //}

        if (col.tag == "Bubble")
        {
            GameManager.instance.EatBubble();
            col.gameObject.SetActive(false);
            PointManager.instance.driftMultiplier++;
            SoundManager.instance.PlaySound(SoundManager.instance.bubbleEat);
        }
            
    }



    IEnumerator SwapSide()
    {
        trails[trailIndex].Clear();
        yield return new WaitForEndOfFrame();
        trails[trailIndex].gameObject.SetActive(true);

        //yield return new WaitForSeconds(0.25f);
        //swapped = false;
        yield return null;
    }


    void LaunchPlayer()
    {
        aimDir.y = 0;
        float magnitude = Vector3.Distance(transform.position, aimDir);
        magnitude = magnitude < 2 ? 2 : magnitude;
        aimDir = (transform.position - aimDir).normalized;
        rb.velocity = Vector3.zero;
        rb.AddForce(aimDir * 200 * magnitude);
        onPlanet = false;
        line.gameObject.SetActive(false);
        currentPlatform.GetComponent<Rigidbody>().AddForce(aimDir * -20 * magnitude);
        PointManager.instance.StartDrift();
        //StartCoroutine(DelayedColliderFix());
        playerRenderer.material = jump;
        SoundManager.instance.MusicDistort();

    }

    IEnumerator DelayedColliderFix()
    {
        yield return new WaitForSeconds(0.5f);
        Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), currentPlatform.GetComponent<Collider>(), false);
        yield return null;
    }
}
