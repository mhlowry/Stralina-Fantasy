using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreParentRotation : MonoBehaviour
{
    Quaternion initialRotation;

    // Start is called before the first frame update
    void Start()
    {
        initialRotation = gameObject.transform.rotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        gameObject.transform.rotation = initialRotation;
    }
}
