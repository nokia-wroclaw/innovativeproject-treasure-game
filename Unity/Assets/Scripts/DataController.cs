using System.IO;
using UnityEngine;

public class DataController : MonoBehaviour
{
    //will be used in the near future
    //public GameObject prefab;

    private MapData mapData;
    private string fileName = "gameData.json";

    void Start()
    {
        LoadDataFromJson();
        BuildElements();
	}
	
    private void LoadDataFromJson()
    {
        var filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(filePath))
        {
            var dataAsJson = File.ReadAllText(filePath);
            mapData = JsonUtility.FromJson<MapData>(dataAsJson);
        }
        else
        {
            Debug.LogError("File doesn't exist");
            print("File doesn't exist");
        }
    }

    private void BuildElements()
    {
        foreach (var gameObjectData in mapData.gameObjects)
        {
            //var block = Instantiate(prefab);// - will be used in the near future
            var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var sizeV3 = new Vector3(gameObjectData.size[0], gameObjectData.size[1], gameObjectData.size[2]);
            var positionV3 = new Vector3(gameObjectData.position[0] + sizeV3.x / 2,
                                         gameObjectData.position[1] + sizeV3.y / 2,
                                         gameObjectData.position[2] + sizeV3.z / 2);
            block.transform.localScale = sizeV3;
            block.transform.localPosition = positionV3;
        }
    }

}
