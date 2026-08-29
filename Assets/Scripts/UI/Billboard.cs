using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        // Makes the warning face the camera, but keeps it upright
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                         Camera.main.transform.rotation * Vector3.up);
    }
}
