using UnityEngine;
using System.Collections.Generic;

// Estruturas de dados RUNTIME (o que é gerado em tempo de execução)
// Diferentes dos SOs (que são configurações)

[System.Serializable]
public class DungeonInstance
{
    public DungeonSO dungeonType;           // Referência para o SO de configuração
    public List<FloorInstance> floors = new List<FloorInstance>();
    public int seed;
    public Vector3Int totalSize;            // Tamanho total da dungeon (largura, altura, profundidade)

    public DungeonInstance(DungeonSO type, int seed)
    {
        this.dungeonType = type;
        this.seed = seed;
    }
}

[System.Serializable]
public class FloorInstance
{
    public FloorDungeonSO floorType;        // Referência para o SO de configuração
    public List<RoomInstance> rooms = new List<RoomInstance>();
    public Vector3Int position;             // Posição no mundo (x, y=altura, z)
    public Vector3Int size;                 // Tamanho deste andar
    public int floorIndex;                  // Índice do andar (0=primeiro, 1=segundo, etc.)
    
    // Dados específicos do layout (gerados pelo algoritmo)
    public BSPNode bspRoot;                 // Se usar BSP
    public FloorLayoutType layoutType;

    public FloorInstance(FloorDungeonSO type, int index)
    {
        this.floorType = type;
        this.floorIndex = index;
    }
}

[System.Serializable]
public class RoomInstance
{
    public RoomDungeonSO roomType;          // Referência para o SO de configuração
    public RectInt bounds2D;               // Posição e tamanho no andar (2D)
    public Vector3Int worldPosition;       // Posição final no mundo 3D
    public Vector3Int size;                // Tamanho 3D final
    public int roomId;                      // ID único na dungeon

    // Conectividade
    public List<ConnectionPoint> connections = new List<ConnectionPoint>();
    public bool hasStairs;                 // Se tem escada para próximo andar

    public RoomInstance(RoomDungeonSO type, RectInt bounds)
    {
        this.roomType = type;
        this.bounds2D = bounds;
    }
}

[System.Serializable]
public class ConnectionPoint
{
    public Vector2Int position;            // Posição da conexão no grid 2D
    public ConnectionType type;            // Porta, corredor, escada, etc.
    public int targetRoomId;               // ID da sala conectada (-1 se corredor)
    
    public ConnectionPoint(Vector2Int pos, ConnectionType connType)
    {
        this.position = pos;
        this.type = connType;
        this.targetRoomId = -1;
    }
}

// Enums para organização
public enum FloorLayoutType 
{ 
    BSP,           // Múltiplas salas usando BSP
    SingleRoom,    // Uma sala dominando o andar (Maze, Boss)
    Grid,          // Salas em grid regular
    Organic        // Layout orgânico/irregular
}

public enum ConnectionType 
{ 
    Door,          // Porta normal entre salas
    Corridor,      // Corredor
    StairUp,       // Escada para andar superior  
    StairDown,     // Escada para andar inferior
    SecretDoor,    // Passagem secreta
    Teleporter     // Teletransporte (especial)
}