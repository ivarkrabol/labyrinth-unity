﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using System.Text;

public class CameraControls : MonoBehaviour
{
    public float transitionDuration = .5f;
    public Vector2Int startingRegion;

    private int _heading;
    private Vector2Int _coordinates = new Vector2Int(1, 1);

    private float _transitionRemaining;
    private int _targetHeading;
    private Vector2Int _targetCoordinates = Vector2Int.zero;

    private Vector3 TargetEulerAngles => EulerAnglesFromHeading(_targetHeading);

    private Vector3 TargetPosition => PositionFromCoordinates(_targetCoordinates);

    private int[,] _maze;
    private int _tileSize;

    public void Start()
    {
        var mazeInstantiation = GameObject.Find("Maze").GetComponent<Instantiation>();
        _maze = mazeInstantiation.maze.GetNumericalMatrix();
        _tileSize = mazeInstantiation.tileSize;
        _targetCoordinates = _coordinates = startingRegion * 14 + new Vector2Int(6, 6);
        _transitionRemaining = .1f;
    }

    public void Update()
    {
        SendLocation();
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
                var transform1 = transform;
                var eulerAngles = transform1.eulerAngles;
                var eulerAnglesDiff = TargetEulerAngles - eulerAngles;
                eulerAnglesDiff.y = (eulerAnglesDiff.y + 540) % 360 - 180;
                eulerAngles += eulerAnglesDiff * animateDelta;
                transform1.eulerAngles = eulerAngles;
                var position = transform1.position;
                position += (TargetPosition - position) * animateDelta;
                transform1.position = position;
                _transitionRemaining -= Time.deltaTime;
            }
        }
        else if (!HasOptions())
        {
            if (Direction()["f"])
            {
                var nextCoord = GetNextCoordinates(_coordinates, _heading, 1);
                if (_maze[nextCoord.x + 1, nextCoord.y + 1] == 1) return;
                _targetCoordinates = nextCoord;
                _transitionRemaining = transitionDuration;
            }
            else
            {
                if (Direction()["r"])
                {
                    _targetHeading = GetNextHeading(_heading, 1);
                    _transitionRemaining = transitionDuration;
                    var nextCoord = GetNextCoordinates(_coordinates, _heading, 1);
                    if (_maze[nextCoord.x + 1, nextCoord.y + 1] == 1) return;
                    _targetCoordinates = nextCoord;
                    _transitionRemaining = transitionDuration;
                }
                else if (Direction()["l"])
                {
                    _targetHeading = GetNextHeading(_heading, -1);
                    _transitionRemaining = transitionDuration;
                    var nextCoord = GetNextCoordinates(_coordinates, _heading, 1);
                    if (_maze[nextCoord.x + 1, nextCoord.y + 1] == 1) return;
                    _targetCoordinates = nextCoord;
                    _transitionRemaining = transitionDuration;
                }
                else
                {
                    _targetHeading = GetNextHeading(GetNextHeading(_heading, 1), 1);
                    _transitionRemaining = transitionDuration;
                }
            }
        }
        else
        {
            if (Input.GetButton("Horizontal"))
            {
                if (Options()["r"] && Math.Abs(Input.GetAxisRaw("Horizontal") - 1) < 0.01
                    || Options()["l"] && Math.Abs(Input.GetAxisRaw("Horizontal") + 1) < 0.01)
                {
                    _targetHeading = GetNextHeading(_heading, (int) Input.GetAxisRaw("Horizontal"));
                    _transitionRemaining = transitionDuration;
                    var nextCoord = GetNextCoordinates(_coordinates, _heading, 1);
                    if ((_maze[nextCoord.x + 1, nextCoord.y + 1]) != 1)
                    {
                        _targetCoordinates = nextCoord;
                        _transitionRemaining = transitionDuration;
                    }
                }
            }

            if (!Input.GetButton("Vertical")) return;
            {
                if (!Options()["f"] || Math.Abs(Input.GetAxisRaw("Vertical") - 1) > 0.01) return;
                var nextCoord = GetNextCoordinates(
                    _coordinates,
                    _heading,
                    (int) Input.GetAxisRaw("Vertical")
                );
                if (_maze[nextCoord.x + 1, nextCoord.y + 1] == 1) return;
                _targetCoordinates = nextCoord;
                _transitionRemaining = transitionDuration;
            }
        }
    }

    private readonly Dictionary<string, int[]>[] _choiceDirections =
    {
        new Dictionary<string, int[]> {{"f", new[] {2, 0}}, {"l", new[] {1, 1}}, {"r", new[] {1, -1}}},
        new Dictionary<string, int[]> {{"f", new[] {0, -2}}, {"l", new[] {1, -1}}, {"r", new[] {-1, -1}}},
        new Dictionary<string, int[]> {{"f", new[] {-2, 0}}, {"l", new[] {-1, -1}}, {"r", new[] {-1, 1}}},
        new Dictionary<string, int[]> {{"f", new[] {0, 2}}, {"l", new[] {-1, 1}}, {"r", new[] {1, 1}}},
    };

    private readonly Dictionary<string, int[]>[] _autoDirections =
    {
        new Dictionary<string, int[]> {{"f", new[] {2, 0}}, {"l", new[] {1, 1}}, {"r", new[] {1, -1}}},
        new Dictionary<string, int[]> {{"f", new[] {0, -2}}, {"l", new[] {1, -1}}, {"r", new[] {-1, -1}}},
        new Dictionary<string, int[]> {{"f", new[] {-2, 0}}, {"l", new[] {-1, -1}}, {"r", new[] {-1, 1}}},
        new Dictionary<string, int[]> {{"f", new[] {0, 2}}, {"l", new[] {-1, 1}}, {"r", new[] {1, 1}}},
    };

    private Dictionary<string, bool> Direction()
    {
        var direction = new Dictionary<string, bool>();
        var directionOptions = _autoDirections[_heading];
        foreach (var item in directionOptions)
        {
            direction.Add(item.Key, _maze[_coordinates.x + item.Value[0] + 1, _coordinates.y + item.Value[1] + 1] != 1);
        }

        return direction;
    }

    private bool HasOptions()
    {
        var total = 0;
        foreach (var item in Options())
        {
            if (item.Value)
            {
                total += 1;
            }
        }

        return total >= 2;
    }

    private Dictionary<string, bool> Options()
    {
        var options = new Dictionary<string, bool>();
        var directionOptions = _choiceDirections[_heading];
        foreach (var item in directionOptions)
        {
            if (_coordinates.x + item.Value[0] + 1 < 0 || _coordinates.y + item.Value[1] + 1 < 0)
            {
                options.Add(item.Key, false);
            }
            else
            {
                var optionX = _coordinates.x + item.Value[0] + 1;
                var optionY = _coordinates.y + item.Value[1] + 1;
                options.Add(item.Key, _maze[optionX, optionY] != 1);
            }
        }

        return options;
    }

    private static Vector3 EulerAnglesFromHeading(int heading)
    {
        return 90 * heading * Vector3.up;
    }

    private Vector3 PositionFromCoordinates(Vector2 coordinates)
    {
        return _tileSize * new Vector3(coordinates.x, 0, coordinates.y);
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

    public void SendLocation() {


        
        float x = calculatePosition(_coordinates.x);
        float y = calculatePosition(_coordinates.y);
        string temp = "{\"x\":" + x.ToString() + ", \"y\":" + y.ToString() + "}";
        Debug.Log(temp);
        StartCoroutine(Send("http://eit3014.jowies.com/location", temp));
    }

    public float calculatePosition(int pos) {
        return (float)pos/(float)40.0;
    }

    public IEnumerator Send(string url, string json)
    {
     var uwr = new UnityWebRequest(url, "POST");
     byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
     uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
     uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
     uwr.SetRequestHeader("Content-Type", "application/json");

     //Send the request then wait here until it returns
     yield return uwr.SendWebRequest();

     if (uwr.isNetworkError)
         {
             Debug.Log("Error While Sending: " + uwr.error);
         }
     else
         {
             Debug.Log("Received: " + uwr.downloadHandler.text);
         }
    }
}
