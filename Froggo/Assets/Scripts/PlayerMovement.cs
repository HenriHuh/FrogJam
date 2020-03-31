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
    public float gravityBlackHole, gravityPlanet;
    public ParticleSystem splashParticle;
    public float jumpForce;
    public GameObject blackHole;
    //Other
    GameObject currentPlatform;
    Vector3 aimDir;
    Rigidbody rb;
    bool aiming = false, onPlanet, swapped;
    List<Transform> gravityObjectPool = new List<Transform>();
    int trailIndex = 0;
    float waterWarningCD = 1;
    bool waterWarningPlayed;

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
        if (!GameManager.instance.started)
        {
            return;
        }

        //Change material color and play warning sound if below 1/3 health
        if (GameManager.instance.waterRemaining < GameManager.instance.maxWater / 3)
        {
            if (!waterWarningPlayed)
            {
                waterWarningCD = 0;
                //SoundManager.instance.PlaySound(SoundManager.instance.waterWarning);

                waterWarningPlayed = true;
            }

            float r = Mathf.Abs(Mathf.Sin(Time.time * 4)) / 2;
            waterWarningCD += Time.deltaTime;
            if (waterWarningCD > 0.5f)
            {
                waterWarningCD = 0;
                SoundManager.instance.PlaySound(SoundManager.instance.waterLow);
            }
            playerRenderer.material.color = new Color(0.5f + r, 1-r, 1-r, 1);
        }
        else
        {
            waterWarningPlayed = false;
            playerRenderer.material.color = Color.white;
        }


        //Set trail pos
        trails[trailIndex].transform.position = transform.position + Vector3.down * 0.5f;


        if (!onPlanet)
        {
            Vector3 moveDirection = rb.velocity;
            transform.LookAt(moveDirection);

            ApplyGravitation();

            Wrap();

            //if (Input.GetKeyDown(KeyCode.Mouse0))
            //{
            //    blackHole.transform.position = GetMousePos();
            //}

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
            float fixedMagnitude = (16 - (transform.position - t.position).magnitude);
            fixedMagnitude = fixedMagnitude < 0 ? 0 : fixedMagnitude;
            float multiplier;
            if (t.tag == "BlackHole")
            {
                multiplier = gravityBlackHole;
            }
            else
            {
                multiplier = gravityPlanet;
            }
            rb.AddForce(dir * fixedMagnitude * multiplier);
        }
        
    }

    Vector3 GetMousePos()
    {
        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 50, aimLayer);
        return hit.point;
    }

    void GetInput()
    {
        if (aiming)
        {

            //Limit vector
            aimDir = Vector3.MoveTowards(transform.position, GetMousePos(), 5);
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
            SoundManager.instance.PlaySound(SoundManager.instance.stretch);
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
            splashParticle.transform.position = col.gameObject.transform.position;
            GameManager.instance.EatBubble();
            col.gameObject.SetActive(false);
            PointManager.instance.driftMultiplier++;
            SoundManager.instance.PlaySplash();
            SoundManager.instance.PlaySound(SoundManager.instance.waterDing);
            splashParticle.Play();
        }

        if (col.tag == "BlackHole" && !onPlanet)
        {
            //PointManager.instance.ResetDrift();
            SoundManager.instance.PlaySound(SoundManager.instance.blackHole);

            rb.velocity *= 1.25f;
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
        rb.AddForce(aimDir * 250 * magnitude);
        onPlanet = false;
        line.gameObject.SetActive(false);
        currentPlatform.GetComponent<Rigidbody>().AddForce(aimDir * -jumpForce * magnitude);
        PointManager.instance.StartDrift();
        //StartCoroutine(DelayedColliderFix());
        playerRenderer.material = jump;
        SoundManager.instance.PlaySound(SoundManager.instance.jump);
        SoundManager.instance.MusicDistort();

    }

    IEnumerator DelayedColliderFix()
    {
        yield return new WaitForSeconds(0.5f);
        Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), currentPlatform.GetComponent<Collider>(), false);
        yield return null;
    }
}
