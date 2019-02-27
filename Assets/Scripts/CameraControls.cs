using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public float transitionDuration = .5f;

    private int heading = 0;
    private Vector2Int coordinates = new Vector2Int(0, 0);

    private float transitionRemaining = 0f;
    private int targetHeading = 0;
    private Vector2Int targetCoordinates = Vector2Int.zero;

    private Vector3 targetEulerAngles
    {
        get
        {
            return EulerAnglesFromHeading(targetHeading);
        }
    }

    private Vector3 targetPosition
    {
        get
        {
            return PositionFromCoordinates(targetCoordinates);
        }
    }

    // Start is called before the first frame update
    public void Start()
    {
    }

    // Update is called once per frame
    public void Update()
    {
        if (transitionRemaining > 0)
        {
            if (transitionRemaining - Time.deltaTime <= 0)
            {
                heading = targetHeading;
                coordinates = targetCoordinates;
                transform.eulerAngles = EulerAnglesFromHeading(heading);
                transform.position = PositionFromCoordinates(coordinates);
                transitionRemaining = 0;
            }
            else
            {
                float animateDelta = Time.deltaTime / transitionRemaining;
                Vector3 eulerAnglesDiff = targetEulerAngles - transform.eulerAngles;
                eulerAnglesDiff.y = ((eulerAnglesDiff.y + 540) % 360) - 180;
                transform.eulerAngles += eulerAnglesDiff * animateDelta;
                transform.position += (targetPosition - transform.position) * animateDelta;
                transitionRemaining -= Time.deltaTime;
            }
        }
        else
        {
            if (Input.GetButtonDown("Horizontal"))
            {
                targetHeading = GetNextHeading(heading, (int)Input.GetAxisRaw("Horizontal"));
                transitionRemaining = transitionDuration;
            }

            if (Input.GetButtonDown("Vertical"))
            {
                targetCoordinates = GetNextCoordinates(coordinates, heading, (int)Input.GetAxisRaw("Vertical"));
                transitionRemaining = transitionDuration;
            }
        }
    }

    private static Vector3 EulerAnglesFromHeading(int heading)
    {
        return 90 * heading * Vector3.up;
    }

    private static Vector3 PositionFromCoordinates(Vector2 coordinates)
    {
        return 10 * new Vector3(coordinates.x, 0, coordinates.y);
    }

    private static int GetNextHeading(int heading, int rotate)
    {
        return (heading + rotate + 4) % 4;
    }

    private static Vector2Int GetNextCoordinates(Vector2Int coordinates, int heading, int move)
    {
        switch (heading)
        {
            case 0:
                return coordinates + Vector2Int.right * move;
            case 1:
                return coordinates + Vector2Int.down * move;
            case 2:
                return coordinates - Vector2Int.right * move;
            case 3:
                return coordinates - Vector2Int.down * move;
        }
        return coordinates;
    }
}