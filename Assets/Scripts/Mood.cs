using UnityEngine;

public abstract class Mood : MonoBehaviour
{
    public const int NumberOfMoodsX = 3;
    public const int NumberOfMoodsY = 3;

    public bool ExecuteInEditMode { get; protected set; }

    public abstract void SetWeight(float weight);
}

public class MoodProfile
{
    public static MoodProfile Get(int x, int y)
    {
        if (!_profilesInitialized) InitProfiles();
        return (x % (2 * _mazeWidth)) * (y % (2 * _mazeHeight)) == 0
            ? _profiles[1, 1]
            : _profiles[x / (_mazeWidth * 2), y / (_mazeHeight * 2)];
    }

    public static void SetMazeSize(int mazeWidth, int mazeHeight)
    {
        _mazeWidth = mazeWidth;
        _mazeHeight = mazeHeight;
    }

    private static void InitProfiles()
    {
        var defaultWall = LoadWall("Brick");
        var defaultFloor = LoadFloor("Floor");

        _profiles = new MoodProfile[3, 3];

        // Dramatic
        _profiles[0, 0] = new MoodProfile(
            LoadWall("Forest"),
            LoadFloor("Grass 2")
        );

        // Concrete
        _profiles[0, 1] = new MoodProfile(
            LoadWall("Hedge"),
            LoadFloor("Sand")
        );

        // Harmonic
        _profiles[0, 2] = new MoodProfile(
            LoadWall("Water"),
            LoadFloor("Sand")
        );

        // Scary
        _profiles[1, 0] = new MoodProfile(
            LoadWall("Tree"),
            LoadFloor("Blood")
        );

        // Neutral
        _profiles[1, 1] = new MoodProfile(
            defaultWall,
            defaultFloor
        );

        // Merry
        _profiles[1, 2] = new MoodProfile(
            LoadWall("ButterfliesAndBubbles"),
            LoadFloor("Grass")
        );

        // Glitchy
        _profiles[2, 0] = new MoodProfile(
            LoadWall("Faces"),
            LoadFloor("Brown")
        );

        // Abstract
        _profiles[2, 1] = new MoodProfile(
            LoadWall("Abstrakt"),
            LoadFloor("Checkers")
        );

        // Trippy
        var trippyMaterial = new TrippyMaterial();
        _profiles[2, 2] = new MoodProfile(
            trippyMaterial.Material,
            LoadFloor("Stream")
        );

        _profilesInitialized = true;
    }

    private static Material LoadWall(string name)
    {
        return Resources.Load<Material>("Materials/Walls/" + name);
    }

    private static Material LoadFloor(string name)
    {
        return Resources.Load<Material>("Materials/Floors/" + name);
    }

    private static int _mazeWidth;
    private static int _mazeHeight;
    private static MoodProfile[,] _profiles;
    private static bool _profilesInitialized;

    public readonly Material WallMaterial;
    public readonly Material FloorMaterial;

    private MoodProfile(Material wallMaterial, Material floorMaterial)
    {
        WallMaterial = wallMaterial;
        FloorMaterial = floorMaterial;
    }
}