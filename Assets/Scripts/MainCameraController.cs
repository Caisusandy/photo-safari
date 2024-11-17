using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    public Transform player;
    public Vector3 cameraDistance;

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            transform.position = player.transform.position + cameraDistance;
        }
    }
}
