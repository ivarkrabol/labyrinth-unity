using UnityEngine;

public class Instantiation : MonoBehaviour 
{
    public Transform wall;
    public Transform floor;

    void Start() 
    {
        var maze = new CombinedMaze(11,11, 3, 3);

        var matrix = maze.GetNumericalMatrix();

        var width = matrix.GetLength(0);
        var height = matrix.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++) 
            {
                if(matrix[x,z] == 1)
                {
                    Instantiate(wall, new Vector3(x*4 - 4, 0, z*4 - 4), Quaternion.identity);
                } else {
                    Instantiate(floor, new Vector3(x*4 - 4, 0, z*4 - 4), Quaternion.identity);
                }
                
            }
        }
    }
}

