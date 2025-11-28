using UnityEngine;

public class GridManager 
{
    public int width;
    public int height;
    // array of nodes - , means its a rectangle grid
    public Node[,] grid;

    public GridManager(int width, int height)
    {
        // stores the parameters in the values
        this.width = width;
        this.height = height;

        // new array of nodes
        grid =  new Node[width, height];


        // fill the grid with nodes
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                grid[i, j] = new Node(i, j, true);
            }
        }

    }

    // getter method to retireve node at a certain position
    public Node GetNode(int x, int y)
    {
        if(x < 0 || y < 0 || x >= width || y >= height) // only return valid nodes
        {
            return null;
        }

        return grid[x, y];

    }



}
