using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSpawnEffect : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
      anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void StartIdleTransition()
    {
      anim.SetTrigger("StartIdle");
    }

    void StopIdleTransition()
    {
      anim.ResetTrigger("StartIdle");
    }

}
