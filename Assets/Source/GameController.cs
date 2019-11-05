using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject cubePrefab;

    void Start()
    {
        MasterGrid.AddCube(0,0,0,cubePrefab);
        MasterGrid.AddCube(1,0,0,cubePrefab);
        MasterGrid.AddCube(1,0,1,cubePrefab);
        MasterGrid.AddCube(0,1,0,cubePrefab);
    }
}
