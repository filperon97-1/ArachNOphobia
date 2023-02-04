using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    /*public Transform Destination;
    public Transform oldPosition;
    private movimentoUser user;
    private bool grab = false;*/


    /*void OnMouseDown()
    {
        //turn off the gravity
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<BoxCollider>().enabled = false;
        this.transform.position = Destination.position;
        this.transform.localScale += new Vector3(500,500,500); 
        this.transform.parent = GameObject.Find("Destination").transform;
        grab = true;
    }

    void OnMouseUp()
    {
        this.transform.parent = null;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<BoxCollider>().enabled = true;
        //Return to original position
        this.transform.position = oldPosition.position;
        this.transform.localScale -= new Vector3(500, 500, 500);
        grab = false;
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
