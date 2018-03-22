using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class MapDataController : MonoBehaviour
{
    private List<GameObject> prefabs;
    private MapData mapData;
    private string fileName = "gameData.json";

    void Start()
    {
        LoadAllPrefabs();
        LoadDataFromJson();
        if(mapData != null)
            BuildElements();
    }

    private void LoadAllPrefabs()
    {
        prefabs = Resources.LoadAll("Prefabs").Cast<GameObject>().ToList();
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
        }
    }

    private void BuildElements()
    {
        var terrain = GetComponent<Terrain>();
        terrain.terrainData.size = new Vector3(mapData.mapSize[0], 0, mapData.mapSize[1]);
 
        foreach (var gameObject in mapData.gameObjects)
        {
            var prefabToBuild = prefabs.FirstOrDefault(n => n.name == gameObject.type);
            if(prefabToBuild == null)
            {
                Debug.LogErrorFormat("There's no such thing as \"{0}\" GameObject type", gameObject.type);
                return;
            }

            var createdObject = Instantiate(prefabToBuild);
                
            var sizeV3 = new Vector3(gameObject.size[0], gameObject.size[1], gameObject.size[2]);
            var positionV3 = new Vector3(gameObject.position[0] + sizeV3.x / 2,
                                         gameObject.position[1] + sizeV3.y / 2,
                                         gameObject.position[2] + sizeV3.z / 2);
            createdObject.transform.localScale = sizeV3;
            createdObject.transform.localPosition = positionV3;
        }
    }

}
