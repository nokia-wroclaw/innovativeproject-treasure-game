[System.Serializable]
public class MapData
{
    [System.Serializable]
    public class GameObjectData
    {
        public float[] position;
        public float[] size;
        public string type;

    }

    public GameObjectData[] gameObjects;
    public float[] mapSize,
                   playerPosition,
                   treasurePosition;

}
