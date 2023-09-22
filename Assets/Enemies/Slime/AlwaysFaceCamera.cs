using UnityEngine;

public class AlwaysFaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField] private bool freezeXYAxis = false;

    void Start()
    {
        mainCamera = Camera.main;
    }

    //LateUpdate makes sure the Camera is always updated before the sprite
    void LateUpdate()
    {
        if (freezeXYAxis)
        {
            //Did this bcause it feels more flexible and intuitive than addition in "LookAt"
            transform.rotation = Quaternion.Euler(0f, mainCamera.transform.rotation.eulerAngles.y, 0f);

            //transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            //mainCamera.transform.rotation * Vector3.up);
        }
        else
        {
            transform.rotation = mainCamera.transform.rotation;
        }
    }
}
