using UnityEngine;
using System.Collections.Generic;

public class BSPNode : MonoBehaviour
{
    public RectInt bounds;
    public BSPNode left = null;
    public BSPNode right = null;
    public RectInt? room = null;

    public int leafChance = 10;

    private static int minAreaSize = 10;

    public BSPNode(RectInt bounds){

        this.bounds = bounds;
    }

    public bool IsLeaf(){

        if (Random.Range(0, leafChance) == 10) return true;

        return left == null && right == null;
    }
    
     public bool Split(){

        // Se já foi dividido, não divide de novo
        if (!IsLeaf()) return false;

        bool splitHorizontally = Random.value > 0.5f;

        if (bounds.width > bounds.height && bounds.width / bounds.height >= 1.25f)
            splitHorizontally = false;
        else if (bounds.height > bounds.width && bounds.height / bounds.width >= 1.25f)
            splitHorizontally = true;

        int max = splitHorizontally ? bounds.height : bounds.width;

        if (max < minAreaSize * 2)
            return false; // muito pequeno para dividir

        int split = Random.Range(minAreaSize, max - minAreaSize);

        if (splitHorizontally)
        {
            RectInt top = new RectInt(bounds.x, bounds.y, bounds.width, split);
            RectInt bottom = new RectInt(bounds.x, bounds.y + split, bounds.width, bounds.height - split);
            left = new BSPNode(top);
            right = new BSPNode(bottom);
        }
        else
        {
            RectInt leftRect = new RectInt(bounds.x, bounds.y, split, bounds.height);
            RectInt rightRect = new RectInt(bounds.x + split, bounds.y, bounds.width - split, bounds.height);
            left = new BSPNode(leftRect);
            right = new BSPNode(rightRect);
        }

        return true;
    }
    
     public void CreateRoom(){
        
        if (!IsLeaf()) return;

        int roomWidth = Random.Range(bounds.width / 2, bounds.width - 1);
        int roomHeight = Random.Range(bounds.height / 2, bounds.height - 1);

        int roomX = bounds.x + Random.Range(0, bounds.width - roomWidth);
        int roomY = bounds.y + Random.Range(0, bounds.height - roomHeight);

        room = new RectInt(roomX, roomY, roomWidth, roomHeight);
    }

    // Retorna todas as folhas da árvore (útil para gerar salas)
    public List<BSPNode> GetLeaves(){

        List<BSPNode> leaves = new List<BSPNode>();
        if (IsLeaf())
            leaves.Add(this);
        else
        {
            if (left != null)
                leaves.AddRange(left.GetLeaves());
            if (right != null)
                leaves.AddRange(right.GetLeaves());
        }
        return leaves;
    }
}
