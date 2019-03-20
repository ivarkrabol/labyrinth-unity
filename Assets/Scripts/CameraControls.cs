using UnityEngine;
using System.Collections.Generic;

public class CameraControls : MonoBehaviour
{
    public float transitionDuration = .5f;

    private int _heading;
    private Vector2Int _coordinates = new Vector2Int(1, 1);

    private float _transitionRemaining;
    private int _targetHeading;
    private Vector2Int _targetCoordinates = Vector2Int.zero;

    private Vector3 TargetEulerAngles => EulerAnglesFromHeading(_targetHeading);

    private Vector3 TargetPosition => PositionFromCoordinates(_targetCoordinates);

    private int[,] maze;

    public void Start()
    {
        maze = GameObject.Find("Maze").GetComponent<Instantiation>().maze.GetNumericalMatrix();
        Debug.Log(maze[0,0]);

    }

    public void Update()
    {
        /*Debug.Log(Direction()[0]);
        Debug.Log(Direction()[1]);*/
        if (_transitionRemaining > 0)
        {
            if (_transitionRemaining - Time.deltaTime <= 0)
            {
                _heading = _targetHeading;
                _coordinates = _targetCoordinates;
                transform.eulerAngles = EulerAnglesFromHeading(_heading);
                transform.position = PositionFromCoordinates(_coordinates);
                _transitionRemaining = 0;
                /*var debugString = string.Format("f:{0}, l:{1}, r:{2}, h: {3}", Options()["f"], Options()["l"], Options()["r"], _heading);
                Debug.Log(debugString);*/
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
        else if(!HasOptions()) {
            Debug.Log(autoDirections[_heading]["f"][0]);
            Debug.Log(Direction()[0]);
            Debug.Log(autoDirections[_heading]["f"][1]);
            Debug.Log(Direction()[1]);
            if(autoDirections[_heading]["f"] == Direction()) {
                
                Debug.Log("forward");
                var nextCoord = GetNextCoordinates(_coordinates, _heading, (int) Input.GetAxisRaw("Vertical"));
                if((maze[nextCoord.x + 1, nextCoord.y + 1]) != 1){
                    _targetCoordinates = nextCoord;
                    _transitionRemaining = transitionDuration;
                }
            } else {
                Debug.Log("happens");
                var heading = 1;
                if(autoDirections[_heading]["l"] == Direction()) {
                    Debug.Log("left");
                    heading = -1;
                } else if (autoDirections[_heading]["r"] == Direction()) {
                    Debug.Log("right");
                    heading = 1;
                };
                var nextCoord = GetNextCoordinates(_coordinates, _heading, (int) Input.GetAxisRaw("Vertical"));
                if((maze[nextCoord.x + 1, nextCoord.y + 1]) != 1){
                    _targetCoordinates = nextCoord;
                    _transitionRemaining = transitionDuration;
                }
                _targetHeading = GetNextHeading(_heading, heading);
                _transitionRemaining = transitionDuration;
            }
        } 
        else
        {
            if (Input.GetButton("Horizontal"))
            {
                _targetHeading = GetNextHeading(_heading, (int) Input.GetAxisRaw("Horizontal"));
                _transitionRemaining = transitionDuration;
            }

            if (Input.GetButton("Vertical"))
            {
                var nextCoord = GetNextCoordinates(_coordinates, _heading, (int) Input.GetAxisRaw("Vertical"));
                if((maze[nextCoord.x + 1, nextCoord.y + 1]) != 1){
                    _targetCoordinates = nextCoord;
                    _transitionRemaining = transitionDuration;
                }
                
            }
        }
    }

    private Dictionary<string, int[]>[] choiceDirections = new Dictionary<string, int[]>[]{
        new Dictionary<string, int[]>(){{"f", new int[]{2, 0}}, {"l", new int[]{1, 1}},{"r", new int[]{1, -1}} },
        new Dictionary<string, int[]>(){{"f", new int[]{0, -2}}, {"l", new int[]{1, -1}},{"r", new int[]{-1, -1}} },
        new Dictionary<string, int[]>(){{"f", new int[]{-2, 0}}, {"l", new int[]{-1, -1}},{"r", new int[]{-1, 1}} },
        new Dictionary<string, int[]>(){{"f", new int[]{0, 2}}, {"l", new int[]{-1, 1}},{"r", new int[]{1, 1}} },
    };

    private Dictionary<string, int[]>[] autoDirections = new Dictionary<string, int[]>[]{
        new Dictionary<string, int[]>(){{"f", new int[]{1, 0}}, {"l", new int[]{0, 1}},{"r", new int[]{0, -1}} },
        new Dictionary<string, int[]>(){{"f", new int[]{0, -1}}, {"l", new int[]{1, 0}},{"r", new int[]{-1, 0}} },
        new Dictionary<string, int[]>(){{"f", new int[]{-1, 0}}, {"l", new int[]{0, -1}},{"r", new int[]{0, 1}} },
        new Dictionary<string, int[]>(){{"f", new int[]{0, 1}}, {"l", new int[]{-1, 0}},{"r", new int[]{1, 0}} },
    };

    private int[] Direction() {
        var direction = new int[2];
        var directionOptions = autoDirections[_heading];
        foreach (KeyValuePair<string, int[]> item in directionOptions)
        {
            if(maze[_coordinates.x + item.Value[0] + 1, _coordinates.y + item.Value[1] + 1] != 1) {
                direction[0] = item.Value[0];
                direction[1] = item.Value[1];
            }
        }
        return direction;
    }

    private bool HasOptions() {
        var total = 0;
        foreach (KeyValuePair<string, bool> item in Options())
        {
            if(item.Value) {
                total += 1;
            }
        }
        return total >= 2;
        
    }

    private Dictionary<string, bool> Options() {
        
        Dictionary<string, bool> options = new Dictionary<string, bool>();
        var directionOptions = choiceDirections[_heading];
        foreach (KeyValuePair<string, int[]> item in directionOptions)
        {
            /*var debugString = string.Format("Cord: (x:{0}, y: {1}), Options: (x:{2}, y:{3}), Key: {4}",_coordinates.x, _coordinates.y, _coordinates.x + item.Value[0], _coordinates.y + item.Value[1], item.Key);
            Debug.Log(debugString);*/
            //options.Add(item.Key, maze[_coordinates.x + item.Value[0] + 1, _coordinates.y + item.Value[1] + 1] != 1);
        }
        return options;
    }

    private static Vector3 EulerAnglesFromHeading(int heading)
    {
        return 90 * heading * Vector3.up;
    }

    private static Vector3 PositionFromCoordinates(Vector2 coordinates)
    {
        return 4 * new Vector3(coordinates.x, 0, coordinates.y);
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