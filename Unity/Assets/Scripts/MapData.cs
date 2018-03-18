using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapData
{
    [System.Serializable]
    public struct GameObjectData
    {
        public float[] size;
        public float[] position;
        public string type;
    }

    public float[] mapSize;
    public GameObjectData[] gameObjects;
}
