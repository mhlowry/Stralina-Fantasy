using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;
    [SerializeField] private float globalScreenshake = 1f;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public void ShakeCamera(CinemachineImpulseSource impulseSource)
    {
        impulseSource.GenerateImpulseWithForce(globalScreenshake);
    }
}
