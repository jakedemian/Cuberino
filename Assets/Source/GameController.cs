using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject cubePrefab;

    private InputState inputState = InputState.Game;

    void Start(){
        MasterGrid.AddCube(0,0,0, CubeColor.Red, cubePrefab);
        MasterGrid.AddCube(1,0,0, CubeColor.Green, cubePrefab);
        MasterGrid.AddCube(1,0,1, CubeColor.Blue, cubePrefab);
        MasterGrid.AddCube(0,1,0, CubeColor.Blue, cubePrefab);
    }

    private void Update() {
        ManageInputState();

        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // if you clicked an empty space, activate the camera
            if (!Physics.Raycast(ray, out hit)) {
                inputState = InputState.Camera;
            } else {
                // other?
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            inputState = InputState.Game;
        }
    }

    private void ManageInputState() {
        if(inputState == InputState.Game) {
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
}
