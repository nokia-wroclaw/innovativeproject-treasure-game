using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MapDataController : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;

    private List<GameObject> _prefabs;
    private MapData _mapData;
    private string _fileName = "gameData.json";

    void Start()
    {
        LoadAllPrefabs();
        LoadDataFromJson();
        if(_mapData != null)
            BuildElements();
    }

    private void LoadAllPrefabs()
    {
        _prefabs = Resources.LoadAll("Prefabs").Cast<GameObject>().ToList();
    }

    private void LoadDataFromJson()
    {
        var filePath = Path.Combine(Application.streamingAssetsPath, _fileName);

        if (File.Exists(filePath))
        {
            var dataAsJson = File.ReadAllText(filePath);
            _mapData = JsonUtility.FromJson<MapData>(dataAsJson);
        }
        else
        {
            Debug.LogError("File doesn't exist");
        }
    }

    private void BuildElements()
    {
        var terrain = GetComponent<Terrain>();
        terrain.terrainData.size = new Vector3(_mapData.mapSize[0] / 80f, 
                                               0f,
                                               _mapData.mapSize[1] / 80f);

        var player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = new Vector3(_mapData.playerPosition[0] / 80f,
                                                0f,
                                                (_mapData.mapSize[1] - 80f - _mapData.playerPosition[1]) / 80f);

        var treasure = GameObject.FindGameObjectWithTag("Treasure");
        treasure.transform.position = new Vector3(_mapData.treasurePosition[0] / 80f, treasure.transform.position.y, (_mapData.mapSize[1] - 80f - _mapData.treasurePosition[1]) / 80f);
        // TODO: chest rotation

        var obstacles = _mapData.gameObjects.Where(x => !x.type.Contains("guard"));
        foreach (var obstacle in obstacles)
        {
            var prefabToBuild = _prefabs.FirstOrDefault(n => n.name == obstacle.type);
            if (prefabToBuild == null)
            {
                Debug.LogErrorFormat("There's no {0} prefab", obstacle.type);
                break;
            }

            var createdObject = Instantiate(prefabToBuild);

            //if (obstacle.type == "wall1")
            //{
            //    createdObject.transform.localScale = new Vector3(createdObject.transform.localScale.x, createdObject.transform.localScale.y, 2);
            //    createdObject.transform.position = new Vector3((obstacle.position[0]) / 80f + 0.5f,
            //                                                   2,
            //                                                   (_mapData.mapSize[1] - 80f - obstacle.position[1]) / 80f + 0.5f);
            //}
            //else
                createdObject.transform.position = new Vector3((obstacle.position[0]) / 80f + 0.5f,
                                                                createdObject.transform.position.y,
                                                                (_mapData.mapSize[1] - 80f - obstacle.position[1]) / 80f + 0.5f);
        }

        navMeshSurface.BuildNavMesh();

        var enemies = _mapData.gameObjects.Except(obstacles);
        foreach (var enemy in enemies)
        {
            var prefabToBuild = _prefabs.FirstOrDefault(n => n.name == enemy.type);
            if (prefabToBuild == null)
            {
                Debug.LogErrorFormat("There's no {0} prefab", enemy.type);
                break;
            }

            var createdObject = Instantiate(prefabToBuild);

            createdObject.transform.position = new Vector3((enemy.position[0]) / 80f,
                                                           0,
                                                           (_mapData.mapSize[1] - 80f - enemy.position[1]) / 80f);
        }
        
    }

}
