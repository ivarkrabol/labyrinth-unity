using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;

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
        return (CellState)(((int) orig >> 2) | ((int) orig << 2)) & CellState.Initial;
    }

    public static bool HasFlag(this CellState cs,CellState flag)
    {
        return ((int)cs & (int)flag) != 0;
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
        for(var x=0; x<width; x++)
            for(var y=0; y<height; y++)
                _cells[x, y] = CellState.Initial;
        _rng = new Random();
        VisitCell(_rng.Next(width), _rng.Next(height));
        ToNumericalMatrix();
    }

    public CellState this[int x, int y]
    {
        get { return _cells[x,y]; }
        set { _cells[x,y] = value; }
    }

    public IEnumerable<RemoveWallAction> GetNeighbours(Point p)
    {
        if (p.X > 0) yield return new RemoveWallAction {Neighbour = new Point(p.X - 1, p.Y), Wall = CellState.Left};
        if (p.Y > 0) yield return new RemoveWallAction {Neighbour = new Point(p.X, p.Y - 1), Wall = CellState.Top};
        if (p.X < _width-1) yield return new RemoveWallAction {Neighbour = new Point(p.X + 1, p.Y), Wall = CellState.Right};
        if (p.Y < _height-1) yield return new RemoveWallAction {Neighbour = new Point(p.X, p.Y + 1), Wall = CellState.Bottom};
    }

    public void VisitCell(int x, int y)
    {
        this[x,y] |= CellState.Visited;
        foreach (var p in GetNeighbours(new Point(x, y)).Shuffle(_rng).Where(z => !(this[z.Neighbour.X, z.Neighbour.Y].HasFlag(CellState.Visited))))
        {
            this[x, y] -= p.Wall;
            this[p.Neighbour.X, p.Neighbour.Y] -= p.Wall.OppositeWall();
            VisitCell(p.Neighbour.X, p.Neighbour.Y);
        }
    }

    public void DisplayMatrix()
    {
        int rowLength = _matrix.GetLength(0);
        int colLength = _matrix.GetLength(1);

        for (int i = 0; i < rowLength; i++)
        {
            for (int j = 0; j < colLength; j++)
            {
                Console.Write(string.Format("{0}", _matrix[i, j]));
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

    public int[,] GetNumericalMatrix() {
        return _matrix;
    }

    public void  ToNumericalMatrix() {

        int colLength = _height * 2 + 1;
        int rowLength = _width * 2 +1;

        _matrix = new int[colLength, rowLength];
        
        for( var x = 0; x < _height; x += 1) {
            for( var y = 0; y < _width; y += 1) {
                _matrix[(x * 2 + 1), (y * 2 + 1)] = 0;
                _matrix[(x * 2 + 1), (y * 2 + 1) - 1] = this[x, y].HasFlag(CellState.Top) ? 1 : 0;
                _matrix[(x * 2 + 1), (y * 2 + 1) + 1] = this[x, y].HasFlag(CellState.Bottom) ? 1 : 0;
                _matrix[(x * 2 + 1) - 1, (y * 2 + 1)] = this[x, y].HasFlag(CellState.Left) ? 1 : 0;
                _matrix[(x * 2 + 1) + 1, (y * 2 + 1)] = this[x, y].HasFlag(CellState.Right) ? 1 : 0;                    
            }
        }
        
        for( var x = 0; x < rowLength; x += 2) {
            for( var y = 0; y < colLength; y += 2) {
                _matrix[x, y] = 1;
            }
        }
    }
}

public class CombinedMaze {

    private SingleMaze[,] _mazes;
    private int _mazeWidth;
    private int _mazeHeight;
    private int _mazeColSize;
    private int _mazeRowSize;

    private int[,] _combinedMatrix;

    public CombinedMaze(int mazeWidth, int mazeHeight, int mazeColSize, int mazeRowSize)
    {
    
        _mazeWidth = mazeWidth;
        _mazeHeight = mazeHeight;
        _mazeColSize = mazeColSize;
        _mazeRowSize = mazeRowSize;
        _mazes = new SingleMaze[mazeWidth, mazeHeight];
        for(var x = 0; x < mazeColSize; x += 1) {
            for(var y = 0; y < mazeRowSize; y += 1){
                _mazes[x,y] = new SingleMaze(mazeWidth, mazeHeight);
            }
        }
        _combinedMatrix = new int[mazeWidth * mazeColSize * 2 + 1, mazeHeight * mazeRowSize * 2 +1];
        CombineMazes();
        AddConnections();
    }

    private void CombineMazes() {
        for(var x = 0; x < _mazeColSize; x += 1) {
            for(var y = 0; y < _mazeRowSize; y += 1){
                addToMatrix(_mazes[x,y], x * _mazeWidth*2, y * _mazeHeight*2);
            }
        }
    }

    private void addToMatrix(SingleMaze maze, int xPos, int yPos) {
        int[,] matrix = maze.GetNumericalMatrix();
        for( var x = 0; x < _mazeWidth * 2 + 1; x += 1) {
            for( var y = 0; y < _mazeHeight * 2 + 1; y += 1){
               _combinedMatrix[ xPos + x,yPos + y] = matrix[x,y];
            }
        }
    }

    private void AddConnections() {
        var width = _mazeWidth % 2 == 1 ? _mazeWidth : _mazeWidth - 1;
        var height = _mazeHeight % 2 == 1 ? _mazeHeight : _mazeHeight - 1;
        for(var x = 0; x < _mazeColSize; x += 1) {
            for(var y = 1; y < _mazeRowSize ; y += 1){
                _combinedMatrix[x*_mazeWidth * 2 + width, y * _mazeHeight * 2] = 0; 
            }
        }
        for(var y = 0; y < _mazeRowSize; y += 1) {
            for(var x = 1; x < _mazeColSize ; x += 1){
                _combinedMatrix[x*_mazeWidth * 2, y * _mazeHeight * 2 + height] = 0; 
            }
        }
    }

    public int[,] GetNumericalMatrix() {
        return _combinedMatrix;
    }
}
