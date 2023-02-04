using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class secondDoorTrigger : MonoBehaviour
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
        if (therapist.currentStatus == Therapist.MachineStatus.Corridor)
        {
            enterCounter++;


            if (_door != null && enterCounter == 1)
            {
                openDoor();
                playDoorSound();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (therapist.currentStatus == Therapist.MachineStatus.Corridor)
        {
            enterCounter--;


            if (_door != null && enterCounter == 0)
            {
                closeDoor();
                playDoorSound();
            }

        }
    }

    private void openDoor()
    {
        _door.transform.DOMoveZ(3.46f, 0.5f);
        _portal.open = true;
    }

    private void closeDoor()
    {
        _door.transform.DOMoveZ(2.5f, 2f);
        _portal.open = false;
    }

    private void playDoorSound()
    {
        var audioSource = _door.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.clip = _doorSound;
            audioSource.Play();
        }
    }
}