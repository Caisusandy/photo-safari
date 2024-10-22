using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform movePoint;
    public double moveInterval;
    private double timeLastChecked = 0.0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) == 0 && (Time.timeAsDouble - moveInterval > timeLastChecked))
        {
            timeLastChecked = Time.timeAsDouble;
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
    }
}
