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
        Debug.Log(maze);

    }

    public void Update()
    {
        var debugString = string.Format("f:{0}, l:{1}, r:{2}, h: {3}", Options()["f"], Options()["l"], Options()["r"], _heading);
        var debugString2 = string.Format("f:{0}, l:{1}, r:{2}", Direction()["f"], Direction()["l"], Direction()["r"]);
        var debugString3 = string.Format("x:{0}, z:{1}", autoDirections[_heading]["f"][0], autoDirections[_heading]["f"][1]);
   
        Debug.Log(debugString);
        Debug.Log(debugString2);
        Debug.Log(debugString3);
        Debug.Log(HasOptions());
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
        else if(!HasOptions()) {
            if(Direction()["f"]) {
                
                Debug.Log("forward");
                int forward;
                var nextCoord =GetNextCoordinates(_coordinates, _heading, 1);
                if((maze[nextCoord.x + 1, nextCoord.y + 1]) != 1){
                    _targetCoordinates = nextCoord;
                    _transitionRemaining = transitionDuration;
                }
            } else {
                if(Direction()["r"]) {
                    _targetHeading = GetNextHeading(_heading, 1);
                    _transitionRemaining = transitionDuration;
                    var nextCoord = GetNextCoordinates(_coordinates, _heading, 1);
                    if((maze[nextCoord.x + 1, nextCoord.y + 1]) != 1){
                        _targetCoordinates = nextCoord;
                        _transitionRemaining = transitionDuration;
                    }
                } else if(Direction()["l"]) {
                    _targetHeading = GetNextHeading(_heading, -1);
                    _transitionRemaining = transitionDuration;
                    var nextCoord = GetNextCoordinates(_coordinates, _heading, 1);
                    if((maze[nextCoord.x + 1, nextCoord.y + 1]) != 1){
                        _targetCoordinates = nextCoord;
                        _transitionRemaining = transitionDuration;
                    }
                } else {
                    _targetHeading = GetNextHeading(GetNextHeading(_heading, 1), 1);
                    _transitionRemaining = transitionDuration;
                }
                
            }
        } 
        else
        {
            if (Input.GetButton("Horizontal"))
            {
                if((Options()["r"] && (Input.GetAxisRaw("Horizontal") == 1)) || (Options()["l"] && (Input.GetAxisRaw("Horizontal") == -1))) {
                    _targetHeading = GetNextHeading(_heading, (int) Input.GetAxisRaw("Horizontal"));
                    _transitionRemaining = transitionDuration;
                    var nextCoord = GetNextCoordinates(_coordinates, _heading, 1);
                    if((maze[nextCoord.x + 1, nextCoord.y + 1]) != 1){
                    _targetCoordinates = nextCoord;
                    _transitionRemaining = transitionDuration;
                }
                }
                
            }

            if (Input.GetButton("Vertical"))
            {
                if(Options()["f"] && (Input.GetAxisRaw("Vertical") == 1)) {
                    var nextCoord = GetNextCoordinates(_coordinates, _heading, (int) Input.GetAxisRaw("Vertical"));
                    if((maze[nextCoord.x + 1, nextCoord.y + 1]) != 1){
                    _targetCoordinates = nextCoord;
                    _transitionRemaining = transitionDuration;
                }
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
        new Dictionary<string, int[]>(){{"f", new int[]{2, 0}}, {"l", new int[]{1, 1}},{"r", new int[]{1, -1}} },
        new Dictionary<string, int[]>(){{"f", new int[]{0, -2}}, {"l", new int[]{1, -1}},{"r", new int[]{-1, -1}} },
        new Dictionary<string, int[]>(){{"f", new int[]{-2, 0}}, {"l", new int[]{-1, -1}},{"r", new int[]{-1, 1}} },
        new Dictionary<string, int[]>(){{"f", new int[]{0, 2}}, {"l", new int[]{-1, 1}},{"r", new int[]{1, 1}} },
    };

    private Dictionary<string, bool> Direction() {
        var direction = new Dictionary<string, bool>();
        var directionOptions = autoDirections[_heading];
        foreach (KeyValuePair<string, int[]> item in directionOptions)
        {
            direction.Add(item.Key, maze[_coordinates.x + item.Value[0] + 1, _coordinates.y + item.Value[1] + 1] != 1);
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
            if(_coordinates.x + item.Value[0] + 1 < 0 || _coordinates.y + item.Value[1] + 1 < 0 ) {
                options.Add(item.Key, false);
            }else {
                options.Add(item.Key, maze[_coordinates.x + item.Value[0] + 1, _coordinates.y + item.Value[1] + 1] != 1);
            }
           
        }
        return options;
    }

    private static Vector3 EulerAnglesFromHeading(int heading)
    {
        return 90 * heading * Vector3.up;
    }

    private static Vector3 PositionFromCoordinates(Vector2 coordinates)
    {
        return 3 * new Vector3(coordinates.x, 0, coordinates.y);
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