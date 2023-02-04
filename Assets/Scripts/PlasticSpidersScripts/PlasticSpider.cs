using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasticSpider : MonoBehaviour

{
    public bool chosen;
    public string spiderType;
    public bool Rotate = true; //once on platform the spider can't be rotated
    public bool RightPosition = false;
    public Transform StartPosition;
    [SerializeField] private Material highlightMaterial;

    void Update()
    {
        if(RightPosition == true)
        {
            this.Rotate = false;
            //Plastic spider no longer selectable
            this.tag = "Untagged";
            //Change material into blue plastic
            this.GetComponent<Renderer>().material = highlightMaterial;
        }
    }
}
