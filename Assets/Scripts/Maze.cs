using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

public static class Extensions
{
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
    {
        var e = source.ToArray();
        for (var i = e.Length - 1; i >= 0; i--)
        {
            var swapIndex = rng.Next(i + 1);
            yield return e[swapIndex];
            e[swapIndex] = e[i];
        }
    }

    public static CellState OppositeWall(this CellState orig)
    {
        return (CellState) (((int) orig >> 2) | ((int) orig << 2)) & CellState.Initial;
    }

    public static bool HasFlag(this CellState cs, CellState flag)
    {
        return ((int) cs & (int) flag) != 0;
    }
}

[Flags]
public enum CellState
{
    Top = 1,
    Right = 2,
    Bottom = 4,
    Left = 8,
    Visited = 128,
    Initial = Top | Right | Bottom | Left,
}

public struct RemoveWallAction
{
    public Point Neighbour;
    public CellState Wall;
}

public class SingleMaze
{
    private readonly CellState[,] _cells;
    private readonly int _width;
    private readonly int _height;
    private readonly Random _rng;
    private int[,] _matrix;

    public SingleMaze(int width, int height)
    {
        _width = width;
        _height = height;
        _cells = new CellState[width, height];
        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
            _cells[x, y] = CellState.Initial;
        _rng = new Random();
        VisitCell(_rng.Next(width), _rng.Next(height));
        ToNumericalMatrix();
    }

    private CellState this[int x, int y]
    {
        get => _cells[x, y];
        set => _cells[x, y] = value;
    }

    private IEnumerable<RemoveWallAction> GetNeighbours(Point p)
    {
        if (p.X > 0) yield return new RemoveWallAction {Neighbour = new Point(p.X - 1, p.Y), Wall = CellState.Left};
        if (p.Y > 0) yield return new RemoveWallAction {Neighbour = new Point(p.X, p.Y - 1), Wall = CellState.Top};
        if (p.X < _width - 1)
            yield return new RemoveWallAction {Neighbour = new Point(p.X + 1, p.Y), Wall = CellState.Right};
        if (p.Y < _height - 1)
            yield return new RemoveWallAction {Neighbour = new Point(p.X, p.Y + 1), Wall = CellState.Bottom};
    }

    private void VisitCell(int x, int y)
    {
        this[x, y] |= CellState.Visited;
        foreach (var p in GetNeighbours(new Point(x, y)).Shuffle(_rng)
            .Where(z => !this[z.Neighbour.X, z.Neighbour.Y].HasFlag(CellState.Visited)))
        {
            this[x, y] -= p.Wall;
            this[p.Neighbour.X, p.Neighbour.Y] -= p.Wall.OppositeWall();
            VisitCell(p.Neighbour.X, p.Neighbour.Y);
        }
    }

    public void DisplayMatrix()
    {
        var rowLength = _matrix.GetLength(0);
        var colLength = _matrix.GetLength(1);

        for (var i = 0; i < rowLength; i++)
        {
            for (var j = 0; j < colLength; j++)
            {
                Console.Write($"{_matrix[i, j]}");
            }

            Console.Write(Environment.NewLine);
        }
    }

    public void DisplayLabyrinth()
    {
        var firstLine = string.Empty;
        for (var y = 0; y < _height; y++)
        {
            var sbTop = new StringBuilder();
            var sbMid = new StringBuilder();
            for (var x = 0; x < _width; x++)
            {
                sbTop.Append(this[x, y].HasFlag(CellState.Top) ? "+--" : "+  ");
                sbMid.Append(this[x, y].HasFlag(CellState.Left) ? "|  " : "   ");
            }

            if (firstLine == string.Empty)
                firstLine = sbTop.ToString();
            Console.WriteLine(sbTop + "+");
            Console.WriteLine(sbMid + "|");
            Console.WriteLine(sbMid + "|");
        }

        Console.WriteLine(firstLine);
    }

    public int[,] GetNumericalMatrix()
    {
        return _matrix;
    }

    private void ToNumericalMatrix()
    {
        var colLength = _height * 2 + 1;
        var rowLength = _width * 2 + 1;

        _matrix = new int[colLength, rowLength];

        for (var x = 0; x < _height; x += 1)
        {
            for (var y = 0; y < _width; y += 1)
            {
                _matrix[x * 2 + 1, y * 2 + 1] = 0;
                _matrix[x * 2 + 1, y * 2 + 1 - 1] = this[x, y].HasFlag(CellState.Top) ? 1 : 0;
                _matrix[x * 2 + 1, y * 2 + 1 + 1] = this[x, y].HasFlag(CellState.Bottom) ? 1 : 0;
                _matrix[x * 2 + 1 - 1, y * 2 + 1] = this[x, y].HasFlag(CellState.Left) ? 1 : 0;
                _matrix[x * 2 + 1 + 1, y * 2 + 1] = this[x, y].HasFlag(CellState.Right) ? 1 : 0;
            }
        }

        for (var x = 0; x < rowLength; x += 2)
        {
            for (var y = 0; y < colLength; y += 2)
            {
                _matrix[x, y] = 1;
            }
        }
    }
}

public class CombinedMaze
{
    private readonly SingleMaze[,] _mazes;
    private readonly int _mazeWidth;
    private readonly int _mazeHeight;
    private readonly int _numberOfMazesX;
    private readonly int _numberOfMazesY;

    private readonly int[,] _combinedMatrix;

    public CombinedMaze(int mazeWidth, int mazeHeight, int numberOfMazesX, int numberOfMazesY)
    {
        _mazeWidth = mazeWidth;
        _mazeHeight = mazeHeight;
        _numberOfMazesX = numberOfMazesX;
        _numberOfMazesY = numberOfMazesY;
        _mazes = new SingleMaze[numberOfMazesX, numberOfMazesY];
        for (var x = 0; x < numberOfMazesX; x += 1)
        {
            for (var y = 0; y < numberOfMazesY; y += 1)
            {
                _mazes[x, y] = new SingleMaze(mazeWidth, mazeHeight);
            }
        }

        _combinedMatrix = new int[mazeWidth * numberOfMazesX * 2 + 1, mazeHeight * numberOfMazesY * 2 + 1];
        CombineMazes();
        AddConnections();
    }

    private void CombineMazes()
    {
        for (var x = 0; x < _numberOfMazesX; x += 1)
        {
            for (var y = 0; y < _numberOfMazesY; y += 1)
            {
                if (x == _numberOfMazesX / 2 && y == _numberOfMazesY / 2) continue;
                AddToMatrix(_mazes[x, y], x * _mazeWidth * 2, y * _mazeHeight * 2);
            }
        }
    }

    private void AddToMatrix(SingleMaze maze, int xPos, int yPos)
    {
        var matrix = maze.GetNumericalMatrix();
        for (var x = 0; x < _mazeWidth * 2 + 1; x++)
        {
            for (var y = 0; y < _mazeHeight * 2 + 1; y++)
            {
                _combinedMatrix[xPos + x, yPos + y] = matrix[x, y];
            }
        }
    }

    private void AddConnections()
    {
        var width = _mazeWidth - 1 + _mazeWidth % 2; // "Rounded" down to nearest odd number
        var height = _mazeHeight - 1 + _mazeHeight % 2;
        for (var x = 0; x < _numberOfMazesX; x++)
        {
            for (var y = 0; y < _numberOfMazesY; y++)
            {
                if (x == _numberOfMazesX / 2 && y == _numberOfMazesY / 2) continue;

                if (x != 0 && (x != _numberOfMazesX / 2 + 1 || y != _numberOfMazesY / 2))
                {
//                    Add connection to the left
                    _combinedMatrix[x * _mazeWidth * 2, y * _mazeHeight * 2 + height] = 0;
                }
                
                if (y != 0 && (x != _numberOfMazesX / 2 || y != _numberOfMazesY / 2 + 1))
                {
//                    Add connection down
                    _combinedMatrix[x * _mazeWidth * 2 + width, y * _mazeHeight * 2] = 0;
                }
            }
        }
    }

    public int[,] GetNumericalMatrix()
    {
        return _combinedMatrix;
    }
}