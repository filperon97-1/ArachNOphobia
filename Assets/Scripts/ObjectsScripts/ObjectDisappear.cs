using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDisappear : MonoBehaviour
{
    public Robottino _robottino;
    public bool disappear;
    // Start is called before the first frame update
    void Start()
    {
        /*if (_robottino != null)
            _robottino.onAnswer += Disappear;*/
        disappear = false;
    }

    void Update()
    {
        if (disappear)
        {
            Destroy(this.gameObject);
        }
    }

    /*void Disappear(object therapist, CustomEventArgs eventArgs)
    {
        Destroy(gameObject);
    }*/
}
