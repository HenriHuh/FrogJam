using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterScrollEffect : MonoBehaviour
{
    RawImage waterRawImage;
    [SerializeField] float ScrollSpeed = 0.5f;

    void Awake()
    {
      waterRawImage = gameObject.GetComponent<RawImage>();
    }

    void Update()
    {
      Rect uvRect = waterRawImage.uvRect;
      uvRect.y -= ScrollSpeed * Time.deltaTime;
      waterRawImage.uvRect = uvRect;
    }
}
