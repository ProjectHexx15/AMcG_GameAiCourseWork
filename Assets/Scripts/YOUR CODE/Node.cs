using UnityEngine;

public class Node
{
    // position on the grid
    public int x;
    public int y;

    // pathfinding
    public Node parent;
    public bool walkable;

    // pathfinding costs
    public float goalCost;
    public float heuristicCost;
    public float FinalCost => goalCost + heuristicCost;
    public float terainCost;

    public Node(int x, int y, bool walkable = true, float terainCost = 1f) // constructor to initialise node
    {
        // assigns values
        this.x = x;
        this.y = y;
        this.walkable = walkable;
        this.terainCost = terainCost;
        goalCost = float.MaxValue; // no path yet so initially max
        heuristicCost = 0f; // empty until calculated

    }


}
