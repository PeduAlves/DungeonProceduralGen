using UnityEngine;

public class DungeonController : MonoBehaviour {
    [Header("Components")]
    public DungeonInitializer dungeonInitializer;
    public Dungeon3DBuilder dungeon3DBuilder;

    [Header("Controls")]
    public bool randomizeDungeon = false;
    public bool generateAndBuild = false;
    public bool buildOnly = false;
    public bool clearDungeon = false;

    [Header("Player Setup")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;
    private GameObject currentPlayer;

    void Update() {
        if (randomizeDungeon) {
            randomizeDungeon = false;
            RandomizeDungeon();
        }
        if (generateAndBuild) {
            generateAndBuild = false;
            GenerateAndBuild();
        }

        if (buildOnly) {
            buildOnly = false;
            BuildOnly();
        }

        if (clearDungeon) {
            clearDungeon = false;
            ClearDungeon();
        }
    }

    public void GenerateAndBuild() {
        Debug.Log("[DungeonController] Starting full generation + build process");

        // Verificar componentes
        if (!ValidateComponents()) return;

        // 1. Gerar dados da dungeon
        dungeonInitializer.DungeonInit();

        // 2. Aguardar um frame para garantir que geração completou
        StartCoroutine(BuildAfterGeneration());
    }
    public void RandomizeDungeon() {
        Debug.Log("[DungeonController] Randomizing dungeon parameters");

        if (dungeonInitializer == null) {
            Debug.LogError("DungeonInitializer not assigned!");
            return;
        }

        dungeonInitializer.seed = 0;
    }

    private System.Collections.IEnumerator BuildAfterGeneration() {
        yield return null; // Aguardar 1 frame

        // 3. Construir 3D
        dungeon3DBuilder.BuildDungeon3D(dungeonInitializer.currentDungeon);

        // 4. Posicionar player
        SpawnPlayer();
    }

    public void BuildOnly() {
        Debug.Log("[DungeonController] Building 3D from existing data");

        if (!ValidateComponents()) return;

        if (dungeonInitializer.currentDungeon == null) {
            Debug.LogError("No dungeon data exists! Generate a dungeon first.");
            return;
        }

        dungeon3DBuilder.BuildDungeon3D(dungeonInitializer.currentDungeon);
        SpawnPlayer();
    }

    public void ClearDungeon() {
        Debug.Log("[DungeonController] Clearing dungeon");

        // Limpar construções 3D
        if (dungeon3DBuilder != null) {
            // Encontrar e destruir dungeon root
            Transform[] children = dungeon3DBuilder.transform.GetComponentsInChildren<Transform>();
            foreach (Transform child in children) {
                if (child != dungeon3DBuilder.transform && child.name.StartsWith("Dungeon_")) {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        // Limpar dados
        if (dungeonInitializer != null) {
            dungeonInitializer.currentDungeon = null;
        }

        // Remover player
        if (currentPlayer != null) {
            DestroyImmediate(currentPlayer);
            currentPlayer = null;
        }
    }

    private void SpawnPlayer() {
        if (playerPrefab == null || dungeonInitializer.currentDungeon == null) return;

        // Remover player anterior
        if (currentPlayer != null) {
            DestroyImmediate(currentPlayer);
        }

        // Calcular posição de spawn (primeira sala do primeiro andar)
        Vector3 spawnPos = CalculateSpawnPosition();

        // Criar player
        currentPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        Debug.Log($"[DungeonController] Player spawned at {spawnPos}");
    }

    private Vector3 CalculateSpawnPosition() {
        var dungeon = dungeonInitializer.currentDungeon;

        if (dungeon.floors.Count == 0 || dungeon.floors[0].rooms.Count == 0) {
            return Vector3.zero;
        }

        // Primeira sala do primeiro andar
        var firstFloor = dungeon.floors[0];
        var firstRoom = firstFloor.rooms[0];

        // Posição no centro da primeira sala + altura do andar
        Vector3 spawnPos = new Vector3(
            firstRoom.bounds2D.center.x,
            firstFloor.position.y + 1f, // 1 metro acima do chão
            firstRoom.bounds2D.center.y
        );

        return spawnPos;
    }

    private bool ValidateComponents() {
        if (dungeonInitializer == null) {
            Debug.LogError("DungeonInitializer not assigned!");
            return false;
        }

        if (dungeon3DBuilder == null) {
            Debug.LogError("Dungeon3DBuilder not assigned!");
            return false;
        }

        return true;
    }

    // Métodos públicos para UI/botões
    [ContextMenu("Generate New Dungeon")]
    public void GenerateNewDungeon() {
        generateAndBuild = true;
    }

    [ContextMenu("Rebuild 3D")]
    public void Rebuild3D() {
        buildOnly = true;
    }

    [ContextMenu("Clear All")]
    public void ClearAll() {
        clearDungeon = true;
    }
    
}
