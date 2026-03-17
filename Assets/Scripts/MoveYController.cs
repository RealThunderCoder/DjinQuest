using UnityEngine;

public class MoveYController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float targetY = 1f;

    private bool moveUp = false;
    private bool moveDown = false;

    private float originalY;

    void Start()
    {
        // IMPORTANT: Use localPosition because wall is a child object
        originalY = transform.localPosition.y;
    }

    void Update()
    {
        Vector3 pos = transform.localPosition;

        if (moveUp)
        {
            pos.y = Mathf.MoveTowards(pos.y, targetY, moveSpeed * Time.deltaTime);
            transform.localPosition = pos;

            if (Mathf.Approximately(pos.y, targetY))
                moveUp = false;
        }

        if (moveDown)
        {
            pos.y = Mathf.MoveTowards(pos.y, originalY, moveSpeed * Time.deltaTime);
            transform.localPosition = pos;

            if (Mathf.Approximately(pos.y, originalY))
                moveDown = false;
        }
    }

    public void MoveUp()
    {
        moveDown = false;
        moveUp = true;
    }

    public void MoveDown()
    {
        moveUp = false;
        moveDown = true;
    }
}


