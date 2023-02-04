using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step_Play : MonoBehaviour
{
    public AudioSource audioData;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void OnEnable()
    {
        audioData.Play(0);
        //Debug.Log("step");
    }
}
