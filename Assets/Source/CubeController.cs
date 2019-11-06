using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CubeController : MonoBehaviour {
    int gridX = 0;
    int gridY = 0;
    int gridZ = 0;
    float currentFallSpeed = 0f;
    bool isFalling = true;

    CubeColor color;

    public float fallSpeed;
    public int startingX;
    public int startingY;
    public int startingZ;

    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;

    public void SetStartPosition(int x, int y , int z){
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

    void Start(){
        gridX = startingX;
        gridY = startingY;
        gridZ = startingZ;

        transform.position = new Vector3(gridX, 12f, gridZ);
    }

    void FixedUpdate() {
        if(isFalling){
            float currentY = transform.position.y;
            float newFallSpeed = currentFallSpeed + (fallSpeed * Time.deltaTime);
            float newY = currentY - newFallSpeed;

            if(newY < gridY){
                isFalling = false;
                transform.position = new Vector3(transform.position.x, gridY, transform.position.z);
            } else {
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                currentFallSpeed = newFallSpeed;
            }
        }

    }
}
