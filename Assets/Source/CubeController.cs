using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CubeController : MonoBehaviour {
    int gridX = 0;
    int gridY = 0;
    int gridZ = 0;
    float currentFallSpeed = 0f;
    bool isFalling = true;
    bool isSelected = false;

    float moveTimer;
    bool isBeingMoved = false;
    Vector3 previousMovingGridPosition = Vector3.zero;
    Vector3 targetMovingGridPosition = Vector3.zero;

    List<GameObject> neighborCubes = new List<GameObject>();

    CubeColor color;

    public float fallSpeed;
    public int startingX;
    public int startingY;
    public int startingZ;

    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;
    public Material selectedMaterial;

    public float moveAnimationTime;

    public void SetStartPosition(int x, int y, int z) {
        startingX = x;
        startingY = y;
        startingZ = z;
    }

    public void SetColor(CubeColor c) {
        color = c;

        if (color == CubeColor.Red) {
            GetComponent<Renderer>().sharedMaterial = redMaterial;
        } else if (color == CubeColor.Green) {
            GetComponent<Renderer>().sharedMaterial = greenMaterial;
        } else if (color == CubeColor.Blue) {
            GetComponent<Renderer>().sharedMaterial = blueMaterial;
        }
    }

    public void SetSelected(bool selected) {
        this.isSelected = selected;
    }

    public bool GetSelected() {
        return this.isSelected;
    }

    public void SetCubeHighlight(bool isHighlighted) {
        var matsArray = GetComponent<Renderer>().materials;
        var newMatsArray = new Material[2];
        newMatsArray[0] = matsArray[0];
        newMatsArray[1] = isHighlighted ? selectedMaterial : null;
        GetComponent<Renderer>().materials = newMatsArray;
    }

    public void MoveTo(Vector3 newGridPos) {
        isBeingMoved = true;
        targetMovingGridPosition = newGridPos;
        previousMovingGridPosition = MasterGrid.GetCubeGridPosition(transform.gameObject);
        moveTimer = 0f;
    }

    public void MoveBack() {
        MoveTo(previousMovingGridPosition);
    }

    public bool isMovingOrFalling() {
        return isBeingMoved || isFalling;
    }

    public CubeColor GetColor() {
        return color;
    }

    public void SetFalling() {
        isFalling = true;
    }

    public void UpdateGridY(int y) {
        gridY = y;
    }

    void Start(){
        gridX = startingX;
        gridY = startingY;
        gridZ = startingZ;

        transform.position = new Vector3(gridX, 12f, gridZ);
    }

    private void Update() {
        if (isSelected) {
            SetCubeHighlight(true);
        } else {
            SetCubeHighlight(false);
        }
    }

    void FixedUpdate() {
        if(isFalling){
            float currentY = transform.position.y;
            float newFallSpeed = currentFallSpeed + (fallSpeed * Time.deltaTime);
            float newY = currentY - newFallSpeed;

            if(newY <= gridY){
                isFalling = false;
                transform.position = new Vector3(transform.position.x, gridY, transform.position.z);
            } else {
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                currentFallSpeed = newFallSpeed;
            }
        }

        if (isBeingMoved) {
            moveTimer += Time.deltaTime;
            float percentComplete = moveTimer / moveAnimationTime;
            transform.position = Vector3.Lerp(previousMovingGridPosition, targetMovingGridPosition, percentComplete);

            if(moveTimer >= moveAnimationTime) {
                isBeingMoved = false;
            }
        }
    }
}
