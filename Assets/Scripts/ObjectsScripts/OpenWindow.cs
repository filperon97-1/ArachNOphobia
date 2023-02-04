using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class OpenWindow : MonoBehaviour
{
    [SerializeField] private GameObject _window;
    [SerializeField] private GameObject player;

    [SerializeField] private AudioClip _windowSound;

    public int enterCounter;

    private void Start()
    {
        enterCounter = 0;
    }


    private void OnTriggerEnter(Collider other)
    {       
        if (_window!= null && player.GetComponent<FreeSpiderScript>().enabled == true && enterCounter == 0)
        {
            openWindow();
            playWindowSound();
            enterCounter++;
        }
    }

    /*private void OnTriggerExit(Collider other)
    {

        enterCounter--;


        if (_window != null && enterCounter == 0)
        {
            closeWindow();
            playWindowSound();
        }

    }*/

    private void openWindow()
    {
        _window.transform.DOLocalRotate(new Vector3(0, -90, 0), 0.3f).SetRelative();
        playWindowSound();
    }

    public void closeWindow()
    {
        _window.transform.DOLocalRotate(new Vector3(0, 90, 0), 0.3f).SetRelative();
        playWindowSound();
    }

    private void playWindowSound()
    {
        var audioSource = _window.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.clip = _windowSound;
            audioSource.Play();
        }
    }
}
