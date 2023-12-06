using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMotion : MonoBehaviour
{
    public float moveTotalX = 10f;
    public float moveTotalY = 10f;
    public float speed = 0.05f;
    public bool startingDirectionHorizontal = true;
    public bool startingDirectionVertical = true;
    public bool loopBack = true;

    private float maxXRight;
    private float maxXLeft;
    private float maxYRight;
    private float maxYLeft;

    void Start()
    {
        maxXRight = transform.position.x + moveTotalX;
        maxXLeft = transform.position.x + -moveTotalX;
        maxYRight = transform.position.y + moveTotalY;
        maxYLeft = transform.position.y + -moveTotalY;
    }

    void FixedUpdate()
    {
        // Change directions
        if (loopBack) {
            if ((transform.position.x >= maxXRight || transform.position.x <= maxXLeft) && moveTotalX != 0) startingDirectionHorizontal = !startingDirectionHorizontal;
            if ((transform.position.y >= maxYRight || transform.position.y <= maxYLeft) && moveTotalY != 0) startingDirectionVertical = !startingDirectionVertical;
        }

        // Move right-left
        if (transform.position.x <= maxXRight && startingDirectionHorizontal && moveTotalX != 0) transform.Translate(Vector3.right * speed, Space.World);
        else if (transform.position.x >= maxXLeft && !startingDirectionHorizontal && moveTotalX != 0) transform.Translate(Vector3.left * speed, Space.World);

        // Move up-down
        if (transform.position.y <= maxYRight && startingDirectionVertical && moveTotalY != 0) transform.Translate(Vector3.up * speed, Space.World);
        else if (transform.position.y >= maxYLeft && !startingDirectionVertical && moveTotalY != 0) transform.Translate(Vector3.down * speed, Space.World);
    }
}
