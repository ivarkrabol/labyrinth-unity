using UnityEngine;

public class Instantiation : MonoBehaviour
{
    public int tileSize = 3;
    public int mazeWidth = 7;
    public int mazeHeight = 7;

    public Transform wall;
    public Transform floor;

    public CombinedMaze maze;

    public Instantiation()
    {
        // We use x and z here, and x and y in Maze.cs (just a reminder)
        maze = new CombinedMaze(mazeWidth, mazeHeight, Mood.NumberOfMoodsX, Mood.NumberOfMoodsY);
    }

    private void Start()
    {
        var matrix = maze.GetNumericalMatrix();

        var width = matrix.GetLength(0);
        var height = matrix.GetLength(1);

        MoodProfile.SetMazeSize(mazeWidth, mazeHeight);
        for (var x = 0; x < width; x++)
        {
            for (var z = 0; z < height; z++)
            {
                var profile = MoodProfile.Get(x, z);
                if (matrix[x, z] == 1) CreateWall(x, z, profile.WallMaterial);
                else CreateFloor(x, z, profile.FloorMaterial);
            }
        }
    }

    private void CreateFloor(int x, int z, Material floorMaterial)
    {
        Create(floor, x, z, floorMaterial);
    }

    private void CreateWall(int x, int z, Material wallMaterial)
    {
        Create(wall, x, z, wallMaterial);
    }

    private void Create(Transform tileType, int x, int z, Material material)
    {
        var newTile = Instantiate(tileType, new Vector3((x - 1) * tileSize, 0, (z - 1) * tileSize), Quaternion.identity);
        var cube = newTile.Find("Cube");
        ScaleCubeHorizontally(cube);
        cube.GetComponent<Renderer>().material = material;
    }

    private void ScaleCubeHorizontally(Transform cube)
    {
        cube.localScale = new Vector3(tileSize, cube.localScale.y, tileSize);
    }
}