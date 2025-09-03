using UnityEngine;
using System.Collections.Generic;

public class Dungeon3DBuilder : MonoBehaviour
{
    [Header("3D Generation Settings")]
    public Material wallMaterial;
    public Material floorMaterial;
    public Material ceilingMaterial;
    
    [Header("Dimensions")]
    public float wallHeight = 3f;
    public float wallThickness = 0.2f;
    public float floorThickness = 0.2f;
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    
    // Containers para organização na hierarquia
    private Transform dungeonRoot;
    private Dictionary<int, Transform> floorContainers = new Dictionary<int, Transform>();
    
    public void BuildDungeon3D(DungeonInstance dungeon)
    {
        if (dungeon == null)
        {
            Debug.LogError("No dungeon data provided!");
            return;
        }
        
        LogStep("=== STARTING 3D DUNGEON CONSTRUCTION ===");
        
        // Limpar dungeon anterior se existir
        ClearExistingDungeon();
        
        // Criar container raiz
        CreateDungeonRoot(dungeon);
        
        // Construir cada andar
        for (int i = 0; i < dungeon.floors.Count; i++)
        {
            BuildFloor3D(dungeon.floors[i]);
        }
        
        LogStep($"=== 3D CONSTRUCTION COMPLETE - {GetTotalObjectCount()} objects created ===");
    }
    
    private void ClearExistingDungeon()
    {
        if (dungeonRoot != null)
        {
            DestroyImmediate(dungeonRoot.gameObject);
        }
        floorContainers.Clear();
    }
    
    private void CreateDungeonRoot(DungeonInstance dungeon)
    {
        GameObject rootObj = new GameObject($"Dungeon_{dungeon.dungeonType.dungeonName}_Seed{dungeon.seed}");
        rootObj.transform.position = Vector3.zero;
        dungeonRoot = rootObj.transform;
        
        LogStep($"Created dungeon root: {rootObj.name}");
    }
    
    private void BuildFloor3D(FloorInstance floor)
    {
        LogStep($"Building Floor {floor.floorIndex}: {floor.floorType.floorName}");
        
        // Criar container para este andar
        Transform floorContainer = CreateFloorContainer(floor);
        
        // Construir piso do andar
        BuildFloorBase(floor, floorContainer);
        
        // Construir cada sala
        foreach (var room in floor.rooms)
        {
            BuildRoom3D(room, floorContainer, floor);
        }
        
        // Construir teto do andar
        BuildFloorCeiling(floor, floorContainer);
        
        LogStep($"Floor {floor.floorIndex} complete - {floor.rooms.Count} rooms built");
    }
    
    private Transform CreateFloorContainer(FloorInstance floor)
    {
        GameObject floorObj = new GameObject($"Floor_{floor.floorIndex}_{floor.floorType.floorName}");
        floorObj.transform.parent = dungeonRoot;
        floorObj.transform.localPosition = floor.position;
        
        Transform container = floorObj.transform;
        floorContainers[floor.floorIndex] = container;
        
        return container;
    }
    
    private void BuildFloorBase(FloorInstance floor, Transform parent)
    {
        // Criar piso completo do andar
        GameObject floorBase = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floorBase.name = "FloorBase";
        floorBase.transform.parent = parent;
        
        // Posicionar no "chão" do andar
        floorBase.transform.localPosition = new Vector3(
            floor.size.x / 2f, 
            -floorThickness / 2f, 
            floor.size.z / 2f
        );
        
        // Escalar para cobrir todo o andar
        floorBase.transform.localScale = new Vector3(
            floor.size.x, 
            floorThickness, 
            floor.size.z
        );
        
        // Aplicar material
        if (floorMaterial != null)
        {
            floorBase.GetComponent<Renderer>().material = floorMaterial;
        }
        
        LogStep($"Created floor base: {floor.size.x}x{floor.size.z}");
    }
    
    private void BuildFloorCeiling(FloorInstance floor, Transform parent)
    {
        // Criar teto do andar
        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceiling.name = "Ceiling";
        ceiling.transform.parent = parent;
        
        // Posicionar no "teto" do andar
        ceiling.transform.localPosition = new Vector3(
            floor.size.x / 2f, 
            wallHeight + floorThickness / 2f, 
            floor.size.z / 2f
        );
        
        // Escalar para cobrir todo o andar
        ceiling.transform.localScale = new Vector3(
            floor.size.x, 
            floorThickness, 
            floor.size.z
        );
        
        // Aplicar material
        if (ceilingMaterial != null)
        {
            ceiling.GetComponent<Renderer>().material = ceilingMaterial;
        }
    }
    
    private void BuildRoom3D(RoomInstance room, Transform floorContainer, FloorInstance floor)
    {
        // Criar container para a sala
        GameObject roomObj = new GameObject($"Room_{room.roomId}_{room.roomType?.roomName ?? "Unknown"}");
        roomObj.transform.parent = floorContainer;
        roomObj.transform.localPosition = new Vector3(room.bounds2D.x, 0, room.bounds2D.y);
        
        // Construir paredes da sala
        BuildRoomWalls(room, roomObj.transform);
        
        LogStep($"Built room {room.roomId}: {room.bounds2D.size} at {room.bounds2D.position}");
    }
    
    private void BuildRoomWalls(RoomInstance room, Transform roomParent)
    {
        var bounds = room.bounds2D;
        
        // Parede Norte (top)
        CreateWall(
            "Wall_North",
            new Vector3(bounds.width / 2f, wallHeight / 2f, bounds.height - wallThickness / 2f),
            new Vector3(bounds.width, wallHeight, wallThickness),
            roomParent
        );
        
        // Parede Sul (bottom)  
        CreateWall(
            "Wall_South",
            new Vector3(bounds.width / 2f, wallHeight / 2f, wallThickness / 2f),
            new Vector3(bounds.width, wallHeight, wallThickness),
            roomParent
        );
        
        // Parede Leste (right)
        CreateWall(
            "Wall_East", 
            new Vector3(bounds.width - wallThickness / 2f, wallHeight / 2f, bounds.height / 2f),
            new Vector3(wallThickness, wallHeight, bounds.height),
            roomParent
        );
        
        // Parede Oeste (left)
        CreateWall(
            "Wall_West",
            new Vector3(wallThickness / 2f, wallHeight / 2f, bounds.height / 2f),
            new Vector3(wallThickness, wallHeight, bounds.height),
            roomParent
        );
    }
    
    private void CreateWall(string wallName, Vector3 position, Vector3 scale, Transform parent)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = wallName;
        wall.transform.parent = parent;
        wall.transform.localPosition = position;
        wall.transform.localScale = scale;
        
        // Aplicar material de parede
        if (wallMaterial != null)
        {
            wall.GetComponent<Renderer>().material = wallMaterial;
        }
        
        // Adicionar collider para caminhada
        // (CreatePrimitive já adiciona BoxCollider automaticamente)
    }
    
    // Métodos utilitários
    private void LogStep(string message)
    {
        if (showDebugInfo)
        {
            Debug.Log($"[3DBuilder] {message}");
        }
    }
    
    private int GetTotalObjectCount()
    {
        if (dungeonRoot == null) return 0;
        return dungeonRoot.GetComponentsInChildren<Transform>().Length - 1; // -1 para não contar o root
    }
    
    // Método público para integração com DungeonInitializer
    public void RequestBuild3D()
    {
        // Procurar DungeonInitializer na cena
        DungeonInitializer initializer = FindFirstObjectByType<DungeonInitializer>();

        if (initializer == null)
        {
            Debug.LogError("DungeonInitializer not found in scene!");
            return;
        }
        
        if (initializer.currentDungeon == null)
        {
            Debug.LogError("No dungeon generated yet! Generate a dungeon first.");
            return;
        }
        
        BuildDungeon3D(initializer.currentDungeon);
    }
}