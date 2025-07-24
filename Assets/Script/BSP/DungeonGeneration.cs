using UnityEngine;
using System.Collections.Generic;

public class DungeonGeneration : MonoBehaviour
{
    public bool generate = false;
    public Vector2Int dungeonSize = new Vector2Int(64, 64);
    public int maxIterations = 5;
    public int minRoomSize = 6;
    public BSPNode rootNode;
    public List<BSPNode> leaves;


    void Update()
    {
        if (generate)
        {
            generate = false;
            Generate();
        }
    }

    public void Generate()
    {
        RectInt startArea = new RectInt(0, 0, dungeonSize.x, dungeonSize.y);
        rootNode = new BSPNode(startArea);

        leaves = new List<BSPNode> { rootNode };

        for (int i = 0; i < maxIterations; i++)
        {
            List<BSPNode> newLeaves = new List<BSPNode>();

            foreach (var leaf in leaves)
            {
                if (leaf.Split())
                {
                    newLeaves.Add(leaf.left);
                    newLeaves.Add(leaf.right);
                }
                else
                {
                    newLeaves.Add(leaf);
                }
            }

            leaves = newLeaves;
        }

        foreach (var leaf in leaves)
        {
            leaf.CreateRoom();
        }
    }
}
