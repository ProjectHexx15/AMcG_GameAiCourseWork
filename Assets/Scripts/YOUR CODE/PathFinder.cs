using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    private GridManager grid;

    public PathFinder(GridManager grid)
    {
        // initialize the pathfinder with the grid
        this.grid = grid;
    }


    // A* path finding method
    public List<Node> FindPath(Node startNode, Node targetNode)
    {
        // open and closed list - hash set used for closed list as no duplicates - only needs to know if explored or not
        List<Node> openList = new List<Node> { startNode };
        HashSet<Node> closedList = new HashSet<Node>();

        // initialise the start node costs
        startNode.goalCost = 0;
        startNode.heuristicCost = Heuristic(startNode, targetNode);

        while (openList.Count > 0)
        {
            // current node is the lowestfCost node 
            Node current = GetLowestFCost(openList);

            if(current == targetNode) // reached target
            {
                return RetracePath(startNode, targetNode); // retract the path following parent links
            }

            // move current node into the closed list - has been explored
            openList.Remove(current);
            closedList.Add(current);

            // loops through the neighbours of the current node
            foreach (Node neighbour in GetNeighbours(current))
            {
                if(neighbour == null || !neighbour.walkable || closedList.Contains(neighbour))
                {
                    continue; // skip null neighbours,  obstacles, and nodes on closed list
                }

                // calculate cost to move to this neighbour
                float tenativeG = current.goalCost + 1 + neighbour.terainCost; // add the terrain cost also, so that best terrain is prioritised

                if(tenativeG < neighbour.goalCost) // update path if cheaper
                {
                    // update neighbours data
                    neighbour.parent = current;
                    neighbour.goalCost = tenativeG;
                    neighbour.heuristicCost = Heuristic(neighbour, targetNode);

                    if(!openList.Contains(neighbour))
                    {
                        // ensure neighbour is considered
                        openList.Add(neighbour);
                    }
                }

            }

        }

        return null; // no path is found
    }

    // HELPER METHODS
    private float Heuristic(Node a, Node b)
    {
        // calculate the manhattan distance between two nodes, no diagonals
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private Node GetLowestFCost(List<Node> nodes)
    {
        // find the node with the lowest fCost
        Node lowest = nodes[0];
        foreach (Node node in nodes)
        {
            if(node.FinalCost < lowest.FinalCost)
            {
                lowest = node;
            }
        }
        return lowest;
    }

    private List<Node> RetracePath(Node start, Node end)
    {
        // builds the path following the parents from start to end
        List<Node> path = new List<Node>();
        Node current = end;

        while (current != start)
        {
            path.Add(current);
            current = current.parent;
        }
        path.Add(start);
        // reserve the list so it goes from start node to end node
        path.Reverse();
        return path;
    }

    private List<Node> GetNeighbours(Node node)
    {
        // return the four neighbour nodes of this one (up down left right)
        List<Node> neighbours = new List<Node>();

        neighbours.Add(grid.GetNode(node.x + 1, node.y));
        neighbours.Add(grid.GetNode(node.x, node.y + 1));
        neighbours.Add(grid.GetNode(node.x - 1, node.y));
        neighbours.Add(grid.GetNode(node.x, node.y - 1));

        return neighbours;

    }

}
