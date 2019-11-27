using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MasterGrid
{
    private static Dictionary<string, GameObject> masterGrid = new Dictionary<string, GameObject>();

    public static void ClearGrid(){
        // TODO
    }

    public static void Clean() {
        Dictionary<string, GameObject> cleanGrid = new Dictionary<string, GameObject>();
        foreach(var cube in masterGrid) {
            if(cube.Value != null) {
                cleanGrid.Add(cube.Key, cube.Value);
            }
        }

        masterGrid = cleanGrid;
    }

    private static string GetMapKey(int x, int y, int z) {
        return x.ToString() + "," + y.ToString() + "," + z.ToString();
    }

    private static string GetMapKey(Vector3Int pos) {
        return pos.x.ToString() + "," + pos.y.ToString() + "," + pos.z.ToString();
    }

    private static Vector3 MapKeyToVector(string key) {
        Vector3 res = Vector3.zero;

        string[] points = key.Split(',');

        res = new Vector3(int.Parse(points[0]), int.Parse(points[1]), int.Parse(points[2]));

        return res;
    }

    public static GameObject GetCube(int x, int y, int z){
        GameObject res = null;

        string mapKey = GetMapKey(x,y,z);
        if(masterGrid.ContainsKey(mapKey)){
            res = masterGrid[mapKey];
        }

        return res;
    }

    public static GameObject AddCube(int x, int y, int z, CubeColor color, GameObject cubePrefab){
        GameObject newCube = null;

        if(GetCube(x,y,z) == null){
            newCube = GameObject.Instantiate(cubePrefab, Vector3.zero, Quaternion.identity);
            newCube.GetComponent<CubeController>().SetStartPosition(x,y,z);
            newCube.GetComponent<CubeController>().SetColor(color);
            masterGrid.Add(GetMapKey(x,y,z), newCube);
        }

        return newCube;
    }

    public static Vector3 GetCubeGridPosition(GameObject cube) {
        Vector3 res = Vector3.zero;

        foreach(var item in masterGrid) {
            if(item.Value.GetInstanceID() == cube.GetInstanceID()) {
                res = MapKeyToVector(item.Key);
            }
        }

        return res;
    }

    public static List<GameObject> GetAllCubes() {
        List<GameObject> allCubes = new List<GameObject>();

        foreach(var cube in masterGrid) {
            allCubes.Add(cube.Value);
        }

        return allCubes;
    }

    public static int GetCubesBelow(GameObject originCube) {
        int cubesBelow = 0;

        foreach(var cube in masterGrid) {
            Vector3Int cubeGridPos = Vector3Int.RoundToInt(cube.Value.transform.position);
            Vector3Int originCubeGridPos = Vector3Int.RoundToInt(originCube.transform.position);

            if(cubeGridPos.x == originCubeGridPos.x &&
                cubeGridPos.z == originCubeGridPos.z &&
                cubeGridPos.y < originCubeGridPos.y) {
                cubesBelow++;
            }
        }

        return cubesBelow;
    }

    public static void UpdateCubePosition(GameObject cube1, Vector3Int newPos) {
        var physicalPos = cube1.transform.position;
        var gridPos = cube1.GetComponent<CubeController>().GetGridPosition();

        Dictionary<string, GameObject> masterGridCopy = new Dictionary<string, GameObject>();
        foreach (var cube in masterGrid) {
            if (cube.Value.GetInstanceID() != cube1.GetInstanceID()) {
                masterGridCopy.Add(cube.Key, cube.Value);
            }
        }

        // now just hand over the new cube position
        masterGridCopy.Add(GetMapKey(newPos), cube1);
        masterGrid = masterGridCopy;

        Debug.Log(physicalPos);
        Debug.Log(gridPos);
    }

    public static void UpdateSelectedCubePositions(GameObject cube1, GameObject cube2) {
        // NOTE: we can't just call UpdateCubePosition twice, since these need to be deleted at the same exact time
        Dictionary<string, GameObject> masterGridCopy = new Dictionary<string, GameObject>();
        // remove them
        foreach (var cube in masterGrid) {
            if (cube.Value.GetInstanceID() != cube1.GetInstanceID()
                && cube.Value.GetInstanceID() != cube2.GetInstanceID())
            {
                masterGridCopy.Add(cube.Key, cube.Value);
            }
        }

        //readd them
        Vector3Int c1pos = Vector3Int.RoundToInt(cube1.transform.position);
        Vector3Int c2pos = Vector3Int.RoundToInt(cube2.transform.position);

        masterGridCopy.Add(GetMapKey(c1pos.x, c1pos.y, c1pos.z), cube1);
        masterGridCopy.Add(GetMapKey(c2pos.x, c2pos.y, c2pos.z), cube2);

        // relay this change back to each cube
        cube1.GetComponent<CubeController>().UpdateGridPositionFromMasterGrid(c1pos.x, c1pos.y, c1pos.z);
        cube2.GetComponent<CubeController>().UpdateGridPositionFromMasterGrid(c2pos.x, c2pos.y, c2pos.z);

        masterGrid = masterGridCopy;
    }

    private static bool CubesAreAdjacent(GameObject cube1, GameObject cube2) {
        bool adjacent = false;

        Vector3Int cube1GridPos = Vector3Int.RoundToInt(cube1.transform.position);
        Vector3Int cube2GridPos = Vector3Int.RoundToInt(cube2.transform.position);

        if(Mathf.Approximately(Vector3Int.Distance(cube1GridPos, cube2GridPos), 1f)) {
            adjacent = true;
        }

        return adjacent;
    }

    public static List<GameObject> GetColorChain(GameObject originCube) {
        CubeColor cubeColor = originCube.GetComponent<CubeController>().GetColor();
        List<GameObject> chain = new List<GameObject>();
        chain.Add(originCube);

        //begin global cube scans
        for(int i = 0; i < 100; i++) {
            int cubesAddedToChain = 0;
            foreach(var cube in masterGrid) {
                // go to next cube in grid if this one is already a part of the chain
                if (chain.Contains(cube.Value)) {
                    continue;
                }

                // if this cube is directly next to any existing cubes
                if(cube.Value.GetComponent<CubeController>().GetColor() == cubeColor) {
                    // loop through current chain and see if this one is adjacent to any in the chain
                    List<GameObject> cubesToAddToChain = new List<GameObject>();
                    foreach(GameObject cubeInChain in chain) {
                        if(CubesAreAdjacent(cubeInChain, cube.Value)) {
                            cubesToAddToChain.Add(cube.Value);
                        }
                    }
                    foreach(var cubeToAdd in cubesToAddToChain) {
                        chain.Add(cubeToAdd);
                        cubesAddedToChain++;
                    }
                }
            }

            if(cubesAddedToChain == 0) {
                break;
            }
        }

        return chain;
    }
}
