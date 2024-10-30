using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform movePoint;
    public PlayerHealth manager;

    public Transform enemyTranform;

    public LayerMask collisionLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, movePoint.position) <= .05f)
        {
            Vector3 finalMoveLocation = movePoint.position;
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            {
                finalMoveLocation += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
            }
            else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            {
                finalMoveLocation += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
            }

            if (Vector3.Distance(finalMoveLocation, movePoint.position) != 0)
            {
                // check for walls
                if (!Physics2D.OverlapCircle(finalMoveLocation, .2f, collisionLayer))
                {
                    // check for enemy pos
                    if (Vector3.Distance(finalMoveLocation, enemyTranform.position) == 0)
                    {
                        // take damage and don't move
                        manager.currentHealth--;
                        Debug.Log("Player moved onto enemy. Current Health: " + manager.currentHealth);
                    }
                    else
                    {
                        movePoint.position = finalMoveLocation;
                    }
                }
            }
        }
    }
}
