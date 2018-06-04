using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

    private void LoadAllPrefabs() =>
        _prefabs = Resources.LoadAll("Prefabs/Old").Cast<GameObject>().ToList();


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
        try
        {
            InstantiateCoreElements();

            InstantiateObstacles();

            navMeshSurface.BuildNavMesh();

            InstantiateEnemies();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Build elements error: {ex}");
        }
    }

    private void InstantiateCoreElements()
    {
        try
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
        }
        catch (Exception ex)
        {
            Debug.LogError($"Load core elements error: {ex}");
            throw;
        }
    }

    private void InstantiateObstacles()
    {
        var obstacles = _mapData.gameObjects.Where(x => !x.type.ToLower().Contains("guard") && !x.type.ToLower().Contains("forestarea"));
        InstantiateObjects(obstacles);

        var forestAreas = _mapData.gameObjects.Where(x => x.type.ToLower().Contains("forestarea"));
        GenerateForests(forestAreas);
    }

    private void InstantiateEnemies()
    {
        var enemies = _mapData.gameObjects.Where(x => x.type.ToLower().Contains("guard"));
        InstantiateObjects(enemies);
    }

    private void GenerateForests(IEnumerable<MapData.GameObjectData> forestAreas)
    {
        var treePrefabsNames = _prefabs.FindAll(x => x.name.ToLower().Contains("tree")).Select(x => x.name);
        foreach (var forestArea in forestAreas)
        {
            GenerateTrees(forestArea, treePrefabsNames);
        }       
    }

    private void GenerateTrees(MapData.GameObjectData forestArea, IEnumerable<string> treePrefabsNames)
    {
        try
        {
            var random = new System.Random();
            RaycastHit[] hit = new RaycastHit[4];

            var field = forestArea.size[0]/80 * forestArea.size[1]/80;
            var density = 0.3f;
            var numberOfTrees = (int)(field * 0.37f * density); // 0.37 ≈ 1/pi -> pi - the area every tree takes(pi/r^2, r ≈ 1)

            var counter = 0;

            for (int i = 0; i < numberOfTrees; i++)
            {
                float[] point;
                var found = false;

                do
                {
                    point = new float[2]
                    {
                        forestArea.position[0] + random.Next(0, (int)forestArea.size[0]/80 + 1)*80,
                        forestArea.position[1] + random.Next(0, (int)forestArea.size[1]/80 + 1)*80
                    };

                    if (Physics.Raycast(new Vector3(point[0] / 80f + 0.5f, 10f, (_mapData.mapSize[1] - 80f - point[1]) / 80f + 0.5f), Vector3.down, out hit[0]))
                    {
                        if (hit[0].transform.CompareTag("Terrain"))
                        {
                            found = true;
                        }
                    }

                    if (++counter > 1000000)
                    {
                        Debug.LogError("Too many tries to generate random tree positions");
                        i = numberOfTrees;
                        break;
                    }

                } while (!found);
          
                var randomPrefabIndex = random.Next(0, treePrefabsNames.Count());
 
                var treeMapData = new MapData.GameObjectData()
                {
                    position = point,
                    size = new float[2] { 0, 0 },
                    type = treePrefabsNames.ElementAt(randomPrefabIndex)
                };

                InstantiateObjects(new[] { treeMapData });  
            }

            Debug.LogFormat("Number of [Trees]: {0} Number of all [RayCast Hits]: {1} ", numberOfTrees, counter);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Generate trees error: {ex}");
            throw;
        }
    }

    private void InstantiateObjects(IEnumerable<MapData.GameObjectData> mapElements, [CallerMemberName]string caller = null)
    {
        foreach (var mapElement in mapElements)
        {
            var prefabToBuild = _prefabs.FirstOrDefault(n => n.name.ToLower() == mapElement.type.ToLower());
            if (prefabToBuild == null)
            {
                Debug.LogErrorFormat("There's no {0} prefab", mapElement.type);
                break;
            }

            var createdObject = Instantiate(prefabToBuild);

            if (caller == "LoadEnemies")
            {
                createdObject.transform.position = new Vector3(mapElement.position[0] / 80f,
                                                               0,
                                                               (_mapData.mapSize[1] - 80f - mapElement.position[1]) / 80f);
            }
            else
            {
                createdObject.transform.position = new Vector3(mapElement.position[0] / 80f + 0.5f,
                                                               createdObject.transform.position.y,
                                                               (_mapData.mapSize[1] - 80f - mapElement.position[1]) / 80f + 0.5f);
            }
        }
    }

}
