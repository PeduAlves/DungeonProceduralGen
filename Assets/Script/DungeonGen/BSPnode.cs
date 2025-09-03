using UnityEngine;
using System.Collections.Generic;

public class BSPNode
{
    public RectInt bounds;
    public BSPNode left = null;
    public BSPNode right = null;
    public RectInt? room = null;
    // Configurações estáticas
    private static int minAreaSize = 10;
    
    public BSPNode(RectInt bounds) {
        this.bounds = bounds;
    }

    public bool IsLeaf()
    {
        return left == null && right == null;
    }
    
    // CORRIGIDO: Agora recebe System.Random para determinismo
    public bool Split(System.Random rng, int leafChance = 10)
    {
        // Se já foi dividido, não divide de novo
        if (!IsLeaf()) return false;

        // CORRIGIDO: Chance de parar dividindo (implementação correta)
        if (rng.Next(0, 100) < leafChance) return false;

        // CORRIGIDO: Usar rng ao invés de Random.value
        bool splitHorizontally = rng.NextDouble() > 0.5;

        // Lógica para evitar salas muito alongadas
        if (bounds.width > bounds.height && bounds.width / (float)bounds.height >= 1.25f)
            splitHorizontally = false;
        else if (bounds.height > bounds.width && bounds.height / (float)bounds.width >= 1.25f)
            splitHorizontally = true;

        int max = splitHorizontally ? bounds.height : bounds.width;

        if (max < minAreaSize * 2)
            return false; // Muito pequeno para dividir

        // CORRIGIDO: Usar rng.Next ao invés de Random.Range
        int split = rng.Next(minAreaSize, max - minAreaSize);

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
    
    // CORRIGIDO: Agora recebe System.Random
    public void CreateRoom(System.Random rng)
    {
        if (!IsLeaf()) return;

        // CORRIGIDO: Usar rng.Next ao invés de Random.Range
        int roomWidth = rng.Next(bounds.width / 2, bounds.width - 1);
        int roomHeight = rng.Next(bounds.height / 2, bounds.height - 1);

        int roomX = bounds.x + rng.Next(0, bounds.width - roomWidth);
        int roomY = bounds.y + rng.Next(0, bounds.height - roomHeight);

        room = new RectInt(roomX, roomY, roomWidth, roomHeight);
    }

    // Retorna todas as folhas da árvore
    public List<BSPNode> GetLeaves()
    {
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