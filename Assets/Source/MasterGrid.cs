using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MasterGrid
{
    private static Dictionary<string, GameObject> masterGrid = new Dictionary<string, GameObject>();

    public static void ClearGrid(){
        // TODO
    }

    private static string GetMapKey(int x, int y, int z){
        return x.ToString() + "," + y.ToString() + "," + z.ToString();
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
}
