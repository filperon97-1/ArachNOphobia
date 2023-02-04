using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class firstDoorTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _door;

    [SerializeField] private Therapist therapist;

    [SerializeField] private AudioClip _doorSound;

    private int enterCounter;
    private OcclusionPortal _portal;


    private void Start()
    {
        enterCounter = 0;
        _portal = GetComponent<OcclusionPortal>();
        _portal.open = false;
    }


    private void OnTriggerEnter(Collider other)
    {
            enterCounter++;

            if (_door != null && enterCounter == 1)
            {
                openDoor();
            }
    
    }

    private void OnTriggerExit(Collider other)
    {
       
            enterCounter--;
           

            if (_door != null && enterCounter == 0)
            {
                closeDoor();
                playDoorSound();
            }

    }

    private void openDoor()
    {
        _door.transform.DOMoveZ(-3.49f, 0.5f);
        playDoorSound();
        _portal.open = true;
    }

    private void closeDoor()
    {
        _door.transform.DOMoveZ(-2.49f, 2f);
        _portal.open = false;
    }

    private void playDoorSound()
    {
        var audioSource = _door.GetComponent<AudioSource>();
        if(audioSource != null)
        {
            audioSource.clip = _doorSound;
            audioSource.Play();
        }
    }
}
