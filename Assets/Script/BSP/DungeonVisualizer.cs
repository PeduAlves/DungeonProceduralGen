using UnityEngine;

public class DungeonVisualizer : MonoBehaviour
{
    public DungeonGeneration generator;

    void OnDrawGizmos()
    {
        if (generator == null || generator.leaves == null) return;

        Gizmos.color = Color.gray;
        foreach (var leaf in generator.leaves)
        {
            Gizmos.DrawWireCube((Vector2)leaf.bounds.center, (Vector2)leaf.bounds.size);
        }

        Gizmos.color = Color.green;
        foreach (var leaf in generator.leaves)
        {
            if (leaf.room.HasValue)
            {
                RectInt room = leaf.room.Value;
                Gizmos.DrawCube((Vector2)room.center, (Vector2)room.size);
            }
        }
    }

}
