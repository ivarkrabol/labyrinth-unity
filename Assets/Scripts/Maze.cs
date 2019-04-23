using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

public struct Wall {
    public Point parent1;
    public Point parent2;
    public bool removed;
}

public class Subset {
    public List<Point> _members;

    public Subset (Point point) {
        _members = new List<Point> ();
        _members.Add (point);
    }

    public void Combine (Subset other) {
        _members.AddRange (other._members);
    }

    public bool IsInSubset (Point point) {
        return _members.Contains (point);
    }
}
public class SingleMaze {
    private readonly int[, ] _cells;

    private List<Wall> _walls;

    public List<Wall> _checked;
    private readonly int _width;
    private readonly int _height;

    private readonly List<Subset> _subsets;
    private readonly Random _rng;
    private int[, ] _matrix;

    public SingleMaze (int width, int height, int seed) {
        _width = width;
        _height = height;
        _rng = new Random (seed);

        _subsets = new List<Subset> ();
        _checked = new List<Wall> ();
        _walls = new List<Wall> ();
        _cells = new int[width, height];

        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                _cells[x, y] = width * x + height * y;
                _subsets.Add (new Subset (new Point (x, y)));
            };
        };

        for (var x = 0; x < width - 1; x++) {
            for (var y = 0; y < height; y++) {
                Wall wall = new Wall ();
                wall.parent1 = new Point (x, y);
                wall.parent2 = new Point (x + 1, y);
                wall.removed = false;
                _walls.Add (wall);
            };
        };

        for (var y = 0; y < height - 1; y++) {
            for (var x = 0; x < width; x++) {
                Wall wall = new Wall ();
                wall.parent1 = new Point (x, y);
                wall.parent2 = new Point (x, y + 1);
                wall.removed = false;
                _walls.Add (wall);
            };
        };
        GenerateMaze ();
        ToNumericalMatrix ();
    }

    public void GenerateMaze () {
        Shuffle (_walls);
        while (_walls.Any ()) {
            Wall wall = _walls[0];
            _walls.RemoveAt (0);
            if (!BelongToSameSubset (wall.parent1, wall.parent2)) {
                wall.removed = true;
                _checked.Add (wall);
                CombineSubsets (wall.parent1, wall.parent2);
            }
        };
    }

    public void RemoveWall (Point p1, Point p2) {
        int x = p1.X - p2.X;
        int y = p1.Y - p2.Y;
        _matrix[p1.X * 2 + 1 - x, p1.Y * 2 + 1 - y] = 0;
    }

    public void ToNumericalMatrix () {
        var colLength = _height * 2 + 1;
        var rowLength = _width * 2 + 1;

        _matrix = new int[colLength, rowLength];

        for (var x = 0; x < _height; x += 1) {
            for (var y = 0; y < _width; y += 1) {
                _matrix[x * 2 + 1, y * 2 + 1] = 0;
                _matrix[x * 2 + 1, y * 2 + 1 - 1] = 1;
                _matrix[x * 2 + 1, y * 2 + 1 + 1] = 1;
                _matrix[x * 2 + 1 - 1, y * 2 + 1] = 1;
                _matrix[x * 2 + 1 + 1, y * 2 + 1] = 1;
            }
        }
        for (var x = 0; x < rowLength; x += 2) {
            for (var y = 0; y < colLength; y += 2) {
                _matrix[x, y] = 1;
            }
        }

        for (int i = 0; i < _checked.Count (); i += 1) {
            Wall wall = _checked[i];
            RemoveWall (wall.parent1, wall.parent2);
        }

    }

    public void CombineSubsets (Point point1, Point point2) {
        int index1 = BelongToSubset (point1);
        int index2 = BelongToSubset (point2);
        Subset sub1 = _subsets[index1];
        Subset sub2 = _subsets[index2];
        _subsets.RemoveAt (index2);
        sub1.Combine (sub2);
    }

    public bool BelongToSameSubset (Point point1, Point point2) {
        return BelongToSubset (point1) == BelongToSubset (point2);
    }

    public int BelongToSubset (Point point) {
        for (var x = 0; x < _subsets.Count; x++) {
            if (_subsets[x].IsInSubset (point)) {
                return x;
            }
        }
        return -1;
    }
    public void Shuffle<T> (IList<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = _rng.Next (n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    public void DisplayMatrix () {
        var rowLength = _matrix.GetLength (0);
        var colLength = _matrix.GetLength (1);
        Console.WriteLine ("Not failing");
        for (var i = 0; i < rowLength; i++) {
            for (var j = 0; j < colLength; j++) {
                Console.Write ($"{_matrix[i, j]}");
            }

            Console.Write (Environment.NewLine);
        }
    }

    public int[, ] GetNumericalMatrix () {
        return _matrix;
    }

}

public class CombinedMaze {
    private readonly SingleMaze[, ] _mazes;
    private readonly int _mazeWidth;
    private readonly int _mazeHeight;
    private readonly int _numberOfMazesX;
    private readonly int _numberOfMazesY;

    private readonly int[, ] _combinedMatrix;

    public CombinedMaze (int mazeWidth, int mazeHeight, int numberOfMazesX, int numberOfMazesY, int seed) {
        _mazeWidth = mazeWidth;
        _mazeHeight = mazeHeight;
        _numberOfMazesX = numberOfMazesX;
        _numberOfMazesY = numberOfMazesY;
        _mazes = new SingleMaze[numberOfMazesX, numberOfMazesY];
        var rng = new Random(seed);
        for (var x = 0; x < numberOfMazesX; x += 1) {
            for (var y = 0; y < numberOfMazesY; y += 1) {
                _mazes[x, y] = new SingleMaze (mazeWidth, mazeHeight, rng.Next());
            }
        }

        _combinedMatrix = new int[mazeWidth * numberOfMazesX * 2 + 1, mazeHeight * numberOfMazesY * 2 + 1];
        CombineMazes ();
        AddConnections ();

        //        _combinedMatrix[7, 28] = 1;
        //        _combinedMatrix[35, 14] = 1;
    }

    private void CombineMazes () {
        for (var x = 0; x < _numberOfMazesX; x += 1) {
            for (var y = 0; y < _numberOfMazesY; y += 1) {
                if (x == _numberOfMazesX / 2 && y == _numberOfMazesY / 2) continue;
                AddToMatrix (_mazes[x, y], x * _mazeWidth * 2, y * _mazeHeight * 2);
            }
        }
    }

    private void AddToMatrix (SingleMaze maze, int xPos, int yPos) {
        var matrix = maze.GetNumericalMatrix ();
        for (var x = 0; x < _mazeWidth * 2 + 1; x++) {
            for (var y = 0; y < _mazeHeight * 2 + 1; y++) {
                _combinedMatrix[xPos + x, yPos + y] = matrix[x, y];
            }
        }
    }

    private void AddConnections () {
        var width = _mazeWidth - 1 + _mazeWidth % 2; // "Rounded" down to nearest odd number
        var height = _mazeHeight - 1 + _mazeHeight % 2;
        for (var x = 0; x < _numberOfMazesX; x++) {
            for (var y = 0; y < _numberOfMazesY; y++) {
                if (x == _numberOfMazesX / 2 && y == _numberOfMazesY / 2) continue;

                if (x != 0 && (x != _numberOfMazesX / 2 + 1 || y != _numberOfMazesY / 2)) {
                    //                    Add connection to the left
                    _combinedMatrix[x * _mazeWidth * 2, y * _mazeHeight * 2 + height] = 0;
                }

                if (y != 0 && (x != _numberOfMazesX / 2 || y != _numberOfMazesY / 2 + 1)) {
                    //                    Add connection down
                    _combinedMatrix[x * _mazeWidth * 2 + width, y * _mazeHeight * 2] = 0;
                }
            }
        }
    }

    public int[, ] GetNumericalMatrix () {
        return _combinedMatrix;
    }
}