using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public float transitionDuration = .5f;

    private int _heading;
    private Vector2Int _coordinates = new Vector2Int(0, 0);

    private float _transitionRemaining;
    private int _targetHeading;
    private Vector2Int _targetCoordinates = Vector2Int.zero;

    private Vector3 TargetEulerAngles => EulerAnglesFromHeading(_targetHeading);

    private Vector3 TargetPosition => PositionFromCoordinates(_targetCoordinates);

    public void Update()
    {
        if (_transitionRemaining > 0)
        {
            if (_transitionRemaining - Time.deltaTime <= 0)
            {
                _heading = _targetHeading;
                _coordinates = _targetCoordinates;
                transform.eulerAngles = EulerAnglesFromHeading(_heading);
                transform.position = PositionFromCoordinates(_coordinates);
                _transitionRemaining = 0;
            }
            else
            {
                var animateDelta = Time.deltaTime / _transitionRemaining;
                var eulerAnglesDiff = TargetEulerAngles - transform.eulerAngles;
                eulerAnglesDiff.y = (eulerAnglesDiff.y + 540) % 360 - 180;
                transform.eulerAngles += eulerAnglesDiff * animateDelta;
                transform.position += (TargetPosition - transform.position) * animateDelta;
                _transitionRemaining -= Time.deltaTime;
            }
        }
        else
        {
            if (Input.GetButtonDown("Horizontal"))
            {
                _targetHeading = GetNextHeading(_heading, (int) Input.GetAxisRaw("Horizontal"));
                _transitionRemaining = transitionDuration;
            }

            if (Input.GetButtonDown("Vertical"))
            {
                _targetCoordinates = GetNextCoordinates(_coordinates, _heading, (int) Input.GetAxisRaw("Vertical"));
                _transitionRemaining = transitionDuration;
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
            default:
                return coordinates;
        }
    }
}