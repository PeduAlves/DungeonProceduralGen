using UnityEngine;
using UnityEditor; // Para Handles (apenas no editor)

[ExecuteAlways] // Funciona mesmo sem rodar o Play Mode
public class DungeonVisualizer : MonoBehaviour
{
    public DungeonInitializer dungeonInitializer;
    public bool drawFloors = true;
    public bool drawRooms = true;
    public Color dungeonColor = Color.white;
    public Color floorColor = Color.green;
    public Color roomColor = Color.cyan;

    void OnDrawGizmos()
    {
        if (dungeonInitializer == null || dungeonInitializer.currentDungeon == null)
            return;

        var dungeon = dungeonInitializer.currentDungeon;

        // Desenhar "bounding box" da dungeon como um todo (se quiser)
        Gizmos.color = dungeonColor;
        // Se você quiser calcular totalSize, pode somar os tamanhos dos floors
        // Aqui vamos desenhar apenas um cubo de exemplo na origem
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(1,1,1));

        foreach (var floor in dungeon.floors)
        {
            if (drawFloors)
            {
                Gizmos.color = floorColor;
                Vector3 floorCenter = floor.position + new Vector3(floor.size.x / 2f, floor.size.y / 2f, floor.size.z / 2f);
                Gizmos.DrawWireCube(floorCenter, floor.size);
            }

            if (drawRooms)
            {
                foreach (var room in floor.rooms)
                {
                    Gizmos.color = roomColor;
                    // Converter bounds2D (RectInt) para 3D
                    Vector3 roomCenter = new Vector3(
                        room.bounds2D.x + room.bounds2D.width / 2f,
                        floor.position.y + 2, // meio do andar
                        room.bounds2D.y + room.bounds2D.height / 2f
                    );
                    Vector3 roomSize = new Vector3(room.bounds2D.width, 4, room.bounds2D.height);
                    Gizmos.DrawWireCube(roomCenter, roomSize);
                }
            }
        }
    }
}
