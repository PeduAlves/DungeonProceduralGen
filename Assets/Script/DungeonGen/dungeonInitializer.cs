using System;
using UnityEngine;
using System.Collections.Generic;

public class DungeonInitializer : MonoBehaviour 
{
    [Header("Generation Settings")]
    public bool generateDungeon;
    public int seed = 0;

    [Header("Debug")]
    public bool logGeneration = true;

    [HideInInspector] public DungeonInstance currentDungeon;
    [HideInInspector] public System.Random rng;
    public BSPNode bsp;

    [Header("Available SOs - Assign manually for now")]
    public DungeonSO[] availableDungeonTypes;
    public FloorDungeonSO[] availableFloorTypes; 
    public RoomDungeonSO[] availableRoomTypes;

    void Update() 
    {
        if (generateDungeon) 
        {
            generateDungeon = false;
            DungeonInit();
        }
    }

    public void DungeonInit() 
    {
        LogStep("=== STARTING DUNGEON GENERATION ===");
 
        SetupSeed();
        
        DungeonSO chosenDungeonType = ChooseDungeonType();
        
        if (chosenDungeonType == null) {
            Debug.LogError("No dungeon types available!");
            return;
        }
        
        currentDungeon = new DungeonInstance(chosenDungeonType, seed);

        LogStep($"Created dungeon: {chosenDungeonType.dungeonName}");

        GenerateFloors();
        GenerateFloorLayouts();
        
        LogStep("=== DUNGEON GENERATION COMPLETE ===");
        LogDungeonSummary();
    }

    private void SetupSeed()
    {
        if (seed == 0) 
        {
            seed = Environment.TickCount;
        }
        rng = new System.Random(seed);
        LogStep($"Using seed: {seed}");
    }

    private DungeonSO ChooseDungeonType()
    {
        if (availableDungeonTypes == null || availableDungeonTypes.Length == 0)
        {
            Debug.LogError("No DungeonSO assigned! Please assign availableDungeonTypes in Inspector.");
            return null;
        }

        // Por enquanto: escolha aleatória simples
        // TODO: Implementar WeightedRandom quando tiver pesos nos SOs
        int randomIndex = rng.Next(0, availableDungeonTypes.Length);
        return availableDungeonTypes[randomIndex];
    }

    private void GenerateFloors()
    {
        var dungeonType = currentDungeon.dungeonType;
        int numFloors = rng.Next(dungeonType.minFloors, dungeonType.maxFloors + 1);

        LogStep($"Generating {numFloors} floors (range: {dungeonType.minFloors}-{dungeonType.maxFloors})");
        
        for (int i = 0; i < numFloors; i++)
        {
            FloorDungeonSO floorType = ChooseFloorType(dungeonType);
            if (floorType != null)
            {
                var floorInstance = new FloorInstance(floorType, i);
                
                floorInstance.position = new Vector3Int(0, -i * 4, 0);
                floorInstance.size = new Vector3Int(
                    rng.Next(floorType.minSize.x, floorType.maxSize.x + 1),
                    rng.Next(floorType.minSize.y, floorType.maxSize.y + 1),
                    rng.Next(floorType.minSize.z, floorType.maxSize.z + 1)
                );
                
                currentDungeon.floors.Add(floorInstance);

                LogStep($"Floor {i}: {floorType.floorName} (size: {floorInstance.size})");
            }
        }
    }

    private FloorDungeonSO ChooseFloorType(DungeonSO dungeonType)
    {
        // TODO: Usar BuildListWithQuotas quando implementar
        // Por enquanto: escolha aleatória dos floors permitidos
        
        if (availableFloorTypes == null || availableFloorTypes.Length == 0) return null;
            
        var validFloors = new List<FloorDungeonSO>();
        
        foreach (var floor in availableFloorTypes)
        {

            if (IsFloorAllowedInDungeon(floor, dungeonType))
            {
                validFloors.Add(floor);
            }
        }
        
        if (validFloors.Count == 0)
        {
            LogStep("Warning: No valid floors found, using first available floor");

            return availableFloorTypes[0];
        }
        
        return validFloors[rng.Next(0, validFloors.Count)];
    }

    private bool IsFloorAllowedInDungeon(FloorDungeonSO floor, DungeonSO dungeon)
    {
        if (floor.allowedDungeonTypes == null || floor.allowedDungeonTypes.Length == 0)
            return true;
            
        foreach (var allowedDungeon in floor.allowedDungeonTypes)
        {
            if (allowedDungeon == dungeon)
                return true;
        }
        
        return false;
    }

    private void GenerateFloorLayouts()
    {
        for (int i = 0; i < currentDungeon.floors.Count; i++) {
            var floor = currentDungeon.floors[i];
            LogStep($"Generating layout for floor {i}: {floor.floorType.floorName}");

            GenerateBSPLayout(floor);//TODO Alterar para funcao de escolha de algoritmo
        }
    }
    
    private void GenerateBSPLayout(FloorInstance floor) {
        // Usar seed derivada para este andar
        var floorRng = new System.Random(rng.Next());

        // Criar área inicial baseada no tamanho do andar
        RectInt floorArea = new RectInt(0, 0, floor.size.x, floor.size.z);

        // Criar BSP
        floor.bspRoot = new BSPNode(floorArea);
        floor.layoutType = FloorLayoutType.BSP;

        // Dividir BSP (máximo 4 iterações para ter poucas salas)
        var leaves = new List<BSPNode> { floor.bspRoot };

        for (int iteration = 0; iteration < 4; iteration++) {
            var newLeaves = new List<BSPNode>();

            foreach (var leaf in leaves) {
                if (leaf.Split(floorRng, leafChance: 30)) // 30% chance de parar
                {
                    newLeaves.Add(leaf.left);
                    newLeaves.Add(leaf.right);
                }
                else {
                    newLeaves.Add(leaf);
                }
            }

            leaves = newLeaves;
        }

        // Criar salas nas folhas
        foreach (var leaf in leaves) {
            leaf.CreateRoom(floorRng);

            if (leaf.room.HasValue) {
                // TODO: Escolher tipo de sala baseado nas RoomEntries do FloorSO
                // Por enquanto: usar primeira sala disponível
                RoomDungeonSO roomType = availableRoomTypes?[0];

                var roomInstance = new RoomInstance(roomType, leaf.room.Value);
                roomInstance.roomId = floor.rooms.Count;

                floor.rooms.Add(roomInstance);
            }
        }

        LogStep($"Floor {floor.floorIndex}: Generated {floor.rooms.Count} rooms using BSP");
    }

    public void LogStep(string message) {
        if (logGeneration) {
            Debug.Log($"[DungeonGen] {message}");
        }
    }

    private void LogDungeonSummary()
    {
        if (currentDungeon == null) return;
        
        LogStep($"SUMMARY - Seed: {currentDungeon.seed}, Type: {currentDungeon.dungeonType.dungeonName}");
        
        for (int i = 0; i < currentDungeon.floors.Count; i++)
        {
            var floor = currentDungeon.floors[i];
            LogStep($"  Floor {i}: {floor.floorType.floorName} - {floor.rooms.Count} rooms");
        }
    }
}
