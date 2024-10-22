using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    public Transform player;
    public Vector3 cameraDistance;

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + cameraDistance;
    }
}
