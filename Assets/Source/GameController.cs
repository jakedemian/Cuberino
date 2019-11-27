using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject cubePrefab;

    private InputState inputState = InputState.Game;
    private List<GameObject> selectedCubes = new List<GameObject>();
    private List<GameObject> lastMovedCubes = new List<GameObject>();
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

    // TODO maybe make every cube manage its own moving state.  have a static list of gameObjects and when a cube is moving,
    // it adds itself to that list and when it's done moving it removes itself.  then you can just check the list.Count
    // to see if any blocks are moving
    private bool AnyCubeIsMoving() {
        // continually check if everything has stopped falling/moving
        bool anyBlockIsMoving = false;
        List<GameObject> allCubes = MasterGrid.GetAllCubes();

        // check if any cubes are moving or falling
        for (int i = 0; i < allCubes.Count; i++) {
            if (allCubes[i] == null) {
                continue;
            }

            CubeController cc = allCubes[i].GetComponent<CubeController>();
            if (cc.isMovingOrFalling()) {
                anyBlockIsMoving = true;
                break;
            }
        }

        return anyBlockIsMoving;
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

    private void MoveCubesBack() {
        inputLocked = true;

        lastMovedCubes[0].GetComponent<CubeController>().MoveBack();
        lastMovedCubes[1].GetComponent<CubeController>().MoveBack();
    }

    private bool CubeChainShouldDelete(List<GameObject> cubeChain) {
        return cubeChain.Count > 2;
    }

    private IEnumerator TriggerCubeDeletionChain(List<GameObject> cubesToTest, bool isFirstMove) {
        //grab all of the cube chains we will be checking
        List<List<GameObject>> cubeChains = new List<List<GameObject>>();
        foreach (GameObject cube in cubesToTest) {

            //if this cube is in an existing chain that we have, skip it.
            bool cubeAlreadyInChain = false;
            foreach(var chain in cubeChains) {
                foreach(var c in chain) {
                    if(c.GetInstanceID() == cube.GetInstanceID()){
                        cubeAlreadyInChain = true;
                        break;
                    }
                }
                if (cubeAlreadyInChain) {
                    break;
                }
            }
            if (cubeAlreadyInChain) {
                continue;
            }

            // add this unique cube chain
            cubeChains.Add(MasterGrid.GetColorChain(cube));
        }

        Debug.Log(cubeChains.Count);

        // delete the cubes in each chain that needs it
        bool chainsWereDeleted = false;
        foreach (List<GameObject> cubeChain in cubeChains) {
            if (CubeChainShouldDelete(cubeChain)) {
                chainsWereDeleted = true;
                foreach (GameObject c in cubeChain) {
                    // does this need a null check?
                    DestroyImmediate(c.gameObject);
                }
            }
        }


        // if any cubes were removed, we need to clean the grid of null values
        // and update "floating" cubes to their new positions and make them fall.
        if (chainsWereDeleted) {
            MasterGrid.Clean();

            int highestCubeYValue = 0;
            foreach(var cube in MasterGrid.GetAllCubes()) {
                if(cube.GetComponent<CubeController>().GetGridPosition().y > highestCubeYValue) {
                    highestCubeYValue = cube.GetComponent<CubeController>().GetGridPosition().y;
                }
            }
            // TODO should be able to start with 1 i think 0 height cubes should never be set to fall right?
            for(int i = 1; i <= highestCubeYValue; i++) {
                foreach(var cube in MasterGrid.GetAllCubes()) {
                    if(cube.GetComponent<CubeController>().GetGridPosition().y == i) {

                        int cubesBelow = MasterGrid.GetCubesBelow(cube);

                        if (cube.GetComponent<CubeController>().GetGridPosition().y != cubesBelow) {
                            cube.GetComponent<CubeController>().UpdateGridY(cubesBelow);
                            cube.GetComponent<CubeController>().SetFalling();
                        }
                    }
                }
            }

            yield return new WaitUntil(() => AnyCubeIsMoving() == false);
            Debug.Log("freedom!");

            // recursion!
            StartCoroutine(TriggerCubeDeletionChain(MasterGrid.GetAllCubes(), false));
        } else if (isFirstMove) {
            MoveCubesBack();

            yield return new WaitUntil(() => AnyCubeIsMoving() == false);

            MasterGrid.UpdateSelectedCubePositions(lastMovedCubes[0], lastMovedCubes[1]);
            lastMovedCubes = new List<GameObject>();
        }
    }

    void Start(){

        MasterGrid.AddCube(0, 0, 0, CubeColor.Blue, cubePrefab);
        MasterGrid.AddCube(0, 1, 0, CubeColor.Red, cubePrefab);
        MasterGrid.AddCube(0, 2, 0, CubeColor.Red, cubePrefab);
        MasterGrid.AddCube(0, 3, 0, CubeColor.Blue, cubePrefab);
        MasterGrid.AddCube(1, 0, 0, CubeColor.Red, cubePrefab);
        MasterGrid.AddCube(2, 0, 0, CubeColor.Blue, cubePrefab);
        MasterGrid.AddCube(3, 0, 0, CubeColor.Gray, cubePrefab);
        MasterGrid.AddCube(4, 0, 0, CubeColor.Gray, cubePrefab);
        MasterGrid.AddCube(2, 1, 0, CubeColor.Gray, cubePrefab);


        // 5x3 test wall ////////////////////////////////////////////
        /////////////////////////////////////////////////////////////
        //MasterGrid.AddCube(0, 0, 0, CubeColor.Red, cubePrefab);
        //MasterGrid.AddCube(-1, 0, 0, CubeColor.Gray, cubePrefab);
        //MasterGrid.AddCube(-2, 0, 0, CubeColor.Red, cubePrefab);
        //MasterGrid.AddCube(1, 0, 0, CubeColor.Green, cubePrefab);
        //MasterGrid.AddCube(2, 0, 0, CubeColor.Gray, cubePrefab);

        //MasterGrid.AddCube(0, 1, 0, CubeColor.Blue, cubePrefab);
        //MasterGrid.AddCube(-1, 1, 0, CubeColor.Blue, cubePrefab);
        //MasterGrid.AddCube(-2, 1, 0, CubeColor.Red, cubePrefab);
        //MasterGrid.AddCube(1, 1, 0, CubeColor.Gray, cubePrefab);
        //MasterGrid.AddCube(2, 1, 0, CubeColor.Red, cubePrefab);

        //MasterGrid.AddCube(0, 2, 0, CubeColor.Green, cubePrefab);
        //MasterGrid.AddCube(-1, 2, 0, CubeColor.Red, cubePrefab);
        //MasterGrid.AddCube(-2, 2, 0, CubeColor.Blue, cubePrefab);
        //MasterGrid.AddCube(1, 2, 0, CubeColor.Green, cubePrefab);
        //MasterGrid.AddCube(2, 2, 0, CubeColor.Red, cubePrefab);
    }

    void Update() {
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
            if (!AnyCubeIsMoving() && selectedCubes.Count > 0) {

                GameObject cube1 = selectedCubes[0];
                GameObject cube2 = selectedCubes[1];
                lastMovedCubes = selectedCubes;
                selectedCubes = new List<GameObject>();

                MasterGrid.UpdateSelectedCubePositions(cube1, cube2);

                StartCoroutine(TriggerCubeDeletionChain(new List<GameObject> { cube1, cube2 }, true));
            } else if (!AnyCubeIsMoving()) {
                inputLocked = false;
            }
        }
    }
}
