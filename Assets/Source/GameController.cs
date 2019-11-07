using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject cubePrefab;

    private InputState inputState = InputState.Game;
    private List<GameObject> selectedCubes = new List<GameObject>();
    private bool inputLocked = true;


    private void ManageInputState() {
        if (inputState == InputState.Game) {
            // TODO unsub from UI
            // unsub from cam
            Camera.main.GetComponent<CameraController>().IsCameraActive(false);

            // TODO sub to game
        } else if (inputState == InputState.Camera) {
            // TODO unsub from game
            // TODO unsub from UI

            // connect to camera
            Camera.main.GetComponent<CameraController>().IsCameraActive(true);
        } else if (inputState == InputState.UI) {
            // TODO unsub from game
            // unsub from cam
            Camera.main.GetComponent<CameraController>().IsCameraActive(false);

            // TODO connect to UI

        }
    }

    private void UpdateSelectedCubeArray(GameObject cube) {
        for(int i = 0; i < selectedCubes.Count; i++) {
            if(selectedCubes[i].GetInstanceID() == cube.GetInstanceID()) {
                cube.GetComponent<CubeController>().SetSelected(false);
                selectedCubes.RemoveAt(i);
                return;
            }
        }

        if (selectedCubes.Count >= 2) {
            Debug.LogError("You can't have more than two selected cubes.");
            return;
        }

        selectedCubes.Add(cube);
        cube.GetComponent<CubeController>().SetSelected(true);
    }

    private void StartMove() {
        inputLocked = true;
        Vector3 posA = selectedCubes[0].transform.position;
        Vector3 posB = selectedCubes[1].transform.position;

        selectedCubes[0].GetComponent<CubeController>().MoveTo(posB);
        selectedCubes[1].GetComponent<CubeController>().MoveTo(posA);

        selectedCubes[0].GetComponent<CubeController>().SetSelected(false);
        selectedCubes[1].GetComponent<CubeController>().SetSelected(false);
    }

    void Start(){
        MasterGrid.AddCube(0, 0, 0, CubeColor.Red, cubePrefab);
        MasterGrid.AddCube(1, 0, 0, CubeColor.Blue, cubePrefab);
        MasterGrid.AddCube(1, 0, 1, CubeColor.Blue, cubePrefab);
        MasterGrid.AddCube(0, 1, 0, CubeColor.Blue, cubePrefab);

        MasterGrid.AddCube(0, 1, 0, CubeColor.Red, cubePrefab);
        MasterGrid.AddCube(1, 1, 0, CubeColor.Red, cubePrefab);
        MasterGrid.AddCube(1, 1, 1, CubeColor.Green, cubePrefab);
        MasterGrid.AddCube(0, 2, 0, CubeColor.Green, cubePrefab);

        MasterGrid.AddCube(0, 2, 0, CubeColor.Green, cubePrefab);
        MasterGrid.AddCube(1, 2, 0, CubeColor.Red, cubePrefab);
        MasterGrid.AddCube(1, 2, 1, CubeColor.Blue, cubePrefab);
        MasterGrid.AddCube(0, 3, 0, CubeColor.Green, cubePrefab);
    }

    private void Update() {
        ManageInputState();

        if (!inputLocked) {
            if (Input.GetMouseButtonDown(0)) {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // if you clicked an empty space, activate the camera
                if (!Physics.Raycast(ray, out hit)) {
                    inputState = InputState.Camera;
                } else {
                    GameObject cube = hit.transform.gameObject;
                    UpdateSelectedCubeArray(cube);
                    if (selectedCubes.Count == 2) {
                        StartMove();
                    }
                }
            } else if (Input.GetMouseButtonUp(0)) {
                inputState = InputState.Game;
            }
        }
        else {
            // continually check if everything has stopped falling/moving
            bool anyBlockIsMoving = false;
            List<GameObject> allCubes = MasterGrid.GetAllCubes();

            // check if any cubes are moving or falling
            for (int i = 0; i < allCubes.Count; i++) {
                if(allCubes[i] == null) {
                    continue;
                }

                CubeController cc = allCubes[i].GetComponent<CubeController>();
                if (cc.isMovingOrFalling()) {
                    anyBlockIsMoving = true;
                    break;
                }
            }

            if (!anyBlockIsMoving && selectedCubes.Count > 0) {

                GameObject cube1 = selectedCubes[0];
                GameObject cube2 = selectedCubes[1];
                selectedCubes = new List<GameObject>();

                // update grid position of two moved blocks
                MasterGrid.UpdateTwoCubePositions(cube1, cube2);




                // TODO NEEDS TO BE A LOOP
                // while there are still chains {
                //


                // check for any cube deletions
                List<GameObject> cubeChain1 = MasterGrid.GetColorChain(cube1);
                List<GameObject> cubeChain2 = MasterGrid.GetColorChain(cube2);

                if(cubeChain1.Count > 2 || cubeChain2.Count > 2) {
                    // delete the chains that are large enough and clear out selections

                    if (cubeChain1.Count > 2) {
                        foreach(var cube in cubeChain1) {
                            DestroyImmediate(cube.gameObject);
                        }
                    }
                    if(cubeChain2.Count > 2) {
                        foreach (var cube in cubeChain2) {
                            DestroyImmediate(cube.gameObject);
                        }
                    }

                    // clear out deleted cubes
                    MasterGrid.Clean();

                    //
                    //
                    //
                    // TODO NEED TO CHAIN THIS SOMEHOW!!!
                    //
                    //
                    //

                    // trigger block falling
                    allCubes = MasterGrid.GetAllCubes();
                    Debug.Log(allCubes.Count);
                    foreach(var cube in allCubes) {
                        int cubesBelow = MasterGrid.GetCubesBelow(cube);
                        cube.GetComponent<CubeController>().UpdateGridY(cubesBelow);
                        cube.GetComponent<CubeController>().SetFalling();

                        // TODO update position in grid as well
                    }

                }

                //
                // } end of chain loop
                //
            } else if (!anyBlockIsMoving) {
                inputLocked = false;
            }
        }
    }
}
