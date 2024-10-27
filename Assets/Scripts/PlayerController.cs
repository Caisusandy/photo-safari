using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform movePoint;
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
        UpdateMovePoint();
    }

    private void UpdateMovePoint()
    {
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

            if (!Physics2D.OverlapCircle(finalMoveLocation, .2f, collisionLayer))
            {
                movePoint.position = finalMoveLocation;
            }
        }
    }
}
