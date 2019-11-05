using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    // Start is called before the first frame update
    int gridX = 0;
    int gridY = 0;
    int gridZ = 0;
    float currentFallSpeed = 0f;
    bool isFalling = true;


    public float fallSpeed;
    public int startingX;
    public int startingY;
    public int startingZ;

    public void SetStartPosition(int x, int y , int z){
        startingX = x;
        startingY = y;
        startingZ = z;
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
