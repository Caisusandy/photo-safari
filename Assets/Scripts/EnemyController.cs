using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform movePoint;
    public Transform playerMovePoint;
    public Transform player;
    private bool playerWasMoving = false;

    private bool PlayerIsMoving
    {
        get
        {
            return Vector3.Distance(playerMovePoint.position, player.position) > 0.5f;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        // calculate and start next move
        if (Vector3.Distance(transform.position, movePoint.position) == 0 && (playerWasMoving && !PlayerIsMoving))
        {
            int moveBy = Random.Range(-1, 2);

            if (Random.Range(0, 2) == 0)
            {
                movePoint.position += new Vector3(moveBy, 0f, 0f);
            }
            else
            {
                movePoint.position += new Vector3(0f, moveBy, 0f);
            }
        }

        playerWasMoving = PlayerIsMoving;
    }
}
