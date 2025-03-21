using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Update()
    {
        Transform cameraTransform = Camera.main.transform;

        foreach (Transform child in transform)
        {
            child.LookAt(cameraTransform);
            child.rotation = Quaternion.Euler(0f, child.rotation.eulerAngles.y, 0f); // Keeps it upright
        }
    }
}
