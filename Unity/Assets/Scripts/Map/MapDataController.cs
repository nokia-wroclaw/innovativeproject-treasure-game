using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MapDataController : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;

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
        terrain.terrainData.size = new Vector3(mapData.mapSize[0] / 80f, 0f, mapData.mapSize[1] / 80f);

        var player = GameObject.FindGameObjectWithTag("Player");
        player.transform.localPosition = new Vector3(mapData.playerPosition[0] / 80f, 0f, mapData.playerPosition[1] / 80f);

        var treasure = GameObject.FindGameObjectWithTag("Treasure");
        treasure.transform.localPosition = new Vector3(mapData.treasurePosition[0] / 80f, -0.4f, mapData.treasurePosition[1] / 80f);
        // TODO: chest rotation

        var obstacles = mapData.gameObjects.Where(x => !x.type.Contains("guard"));
        foreach (var obstacle in obstacles)
        {
            var prefabToBuild = prefabs.FirstOrDefault(n => n.name == obstacle.type);
            if (prefabToBuild == null)
            {
                Debug.LogErrorFormat("There's no {0} prefab", obstacle.type);
                break;
            }

            var createdObject = Instantiate(prefabToBuild);

            createdObject.transform.localScale = new Vector3(obstacle.size[0] / 80f, 3f, obstacle.size[1] / 80f);
            createdObject.transform.localPosition = new Vector3((obstacle.position[0]) / 80f + createdObject.transform.localScale.x / 2f,
                                                                createdObject.transform.localScale.y / 2f,
                                                                (mapData.mapSize[1] - 80f - obstacle.position[1]) / 80f + createdObject.transform.localScale.z / 2f);
        }

        navMeshSurface.BuildNavMesh();

        var enemies = mapData.gameObjects.Except(obstacles);
        foreach (var enemy in enemies)
        {
            var prefabToBuild = prefabs.FirstOrDefault(n => n.name == enemy.type);
            if (prefabToBuild == null)
            {
                Debug.LogErrorFormat("There's no {0} prefab", enemy.type);
                break;
            }

            var createdObject = Instantiate(prefabToBuild);

            createdObject.transform.localPosition = new Vector3((enemy.position[0]) / 80f,
                                                                0,
                                                                (mapData.mapSize[1] - 80f - enemy.position[1]) / 80f);
        }
        
    }

}
