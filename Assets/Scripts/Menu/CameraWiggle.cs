using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWiggle : MonoBehaviour
{
    [SerializeField]
    float frequency = 1;
    [SerializeField]
    float amplitude = 1;

    Vector3 originalPosition;
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = originalPosition + (Vector3.up * Mathf.Sin(Time.time * frequency * Mathf.PI * 2) * amplitude);
    }
}
