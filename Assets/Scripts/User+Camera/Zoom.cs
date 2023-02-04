using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    public int zoom;
    public bool stopZoom = false;

    private float defaultFieldOfView = 60F;

    void Update()
    {
        if (!stopZoom)
        {
            if (Camera.main.fieldOfView > defaultFieldOfView / zoom)
            {
                Camera.main.fieldOfView -= (20 * Time.deltaTime);
            }
        }
        else
        {
            if (Camera.main.fieldOfView < defaultFieldOfView)
            {
                Camera.main.fieldOfView += (20 * Time.deltaTime);
            }
        }
    }
}
