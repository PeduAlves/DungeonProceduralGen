using System;
using UnityEngine;

public class dungeonInicializer : MonoBehaviour {

    public bool generateDungeon;
    public int seed;
    private System.Random rng;

    public void Update() {
        if (generateDungeon) {
            DungeonInit();
        }
    }

    public void DungeonInit() {


    }
}
