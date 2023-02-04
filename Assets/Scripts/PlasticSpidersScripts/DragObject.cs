using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    /*private Vector3 mOffset;
    private float mZCoord;
    private float speed = 10;
    private bool grab = false;

    void OnMouseDown()

    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        // Store offset = gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
    }

    //Get mouse 3D position from 2D coordinates
    private Vector3 GetMouseAsWorldPoint()

    {
        Vector3 mousePoint = Input.mousePosition;
        // z coordinate of game object on screen
        mousePoint.z = mZCoord;
        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseDrag()

    {
        grab = true;
        Vector3 destination = new Vector3(GetMouseAsWorldPoint().x, this.transform.position.y, GetMouseAsWorldPoint().z);
        transform.position = Vector3.Lerp(transform.position, destination, speed * Time.deltaTime);
    }

    void Update()
    {
        if (grab == true)
        {
            //Rotate to the right
            if (Input.GetKeyUp(KeyCode.R))
            {
                this.transform.Rotate(0, 0, 90);
            }

            //Rotate to the left
            if (Input.GetKeyDown(KeyCode.L))
            {
                this.transform.Rotate(0, 0, -90);
            }
        }
    }*/
}
