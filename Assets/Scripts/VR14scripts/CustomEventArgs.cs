using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CustomEventArgs : EventArgs
{
    public String eventType;
    public String texture;
        
    public CustomEventArgs (String enteringType, String enteringTexture)
    {
       eventType= enteringType;
       texture = enteringTexture;

    } 
}
