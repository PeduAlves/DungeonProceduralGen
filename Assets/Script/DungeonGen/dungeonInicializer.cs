using System;
using UnityEngine;

public class dungeonInicializer : MonoBehaviour {
    
    public bool generateDungeon;

    public void Update() {
        if (generateDungeon) {
            DungeonInit();
        }
    }

    public void DungeonInit() {


    }

}
