using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScaleOnStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.one;
    }
}
