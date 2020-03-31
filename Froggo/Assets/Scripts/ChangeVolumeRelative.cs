using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeVolumeRelative : MonoBehaviour
{
    public Transform target;

    private AudioSource source;

    void Start()
    {
        source = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        float FixedVol = Mathf.Lerp(source.volume, 0.75f - Vector3.Distance(transform.position, target.position) / 7.5f, Time.deltaTime * 2);
        source.volume = FixedVol;
    }
}
