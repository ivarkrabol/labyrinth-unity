using UnityEngine;

public class Instantiation : MonoBehaviour 
{
    public Transform wall;
    public Transform floor;

    public CombinedMaze maze;

    void Start() 
    {
        maze = new CombinedMaze(7,7, 3, 3);
        Debug.Log("instantiated");
        var matrix = maze.GetNumericalMatrix();

        var width = matrix.GetLength(0);
        var height = matrix.GetLength(1);
        
        int size = 3;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++) 
            {
                if(matrix[x,z] == 1)
                {
                    Instantiate(wall, new Vector3(x*size - size, 0, z*size - size), Quaternion.identity);
                } else {
                    Instantiate(floor, new Vector3(x*size - size, 0, z*size - size), Quaternion.identity);
                }
                
            }
        }
    }
}

