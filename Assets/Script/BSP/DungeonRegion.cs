using UnityEngine;

public enum RegionType { Hall, Library, Storage, Empty }

public class DungeonRegion : MonoBehaviour
{

    public RectInt room;
    public RegionType type;

}
