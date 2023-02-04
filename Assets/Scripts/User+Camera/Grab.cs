using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Animations.Rigging;

public class Grab : MonoBehaviour
{
    public Robottino _robottino;
    public GameObject cam;
    public GameObject table;
    public PlayerMovement _playerMovement;
    public Animator playerAnimator;
    public float maxDistanceGrab = 1F;      // a che distanza si pu� fare il grab di un oggetto
    public float scale = 1.1F;              // quanto ingrandire oggetto che si può afferrare
    public int n_layer = 8;                 // in che layer sono gli oggetti grabbable 
    public float altezzaOggetto = 0.8F;

    // per cinematica inversa delle braccia
    public Transform targetRight;
    public Transform targetLeft;
    public Rig rigRight;
    public Rig rigLeft;

    private Transform lockedObject;
    private bool isLocked = false;          //indica se l'oggetto e' abbastanza vicino per essere preso, se lo e' si applicano delle trasformazioni per farlo intuire meglio all'utente
    private bool isGrabbed = false;         //indica se l'oggetto e' stato preso
    private bool firstGrab = true;

    public event EventHandler<CustomEventArgs> OnGrab;
    public event EventHandler<CustomEventArgs> OnDeGrab;

    [SerializeField] private AudioClip _audioDrop;
    [SerializeField] private AudioClip _audioGrab;

    void Start()
    {
        rigLeft.weight = 0F;
        rigRight.weight = 0F;
        firstGrab = true;
        _robottino = FindObjectOfType<Robottino>();
        table = GameObject.FindObjectOfType<Desk>().gameObject;
    }
    void Update()
    {
        if (!isGrabbed)     //non sta portando nessun oggetto
        {
            RaycastHit hit = isReachable();
            if (hit.distance != -1)         //e' abbastanza vicino per prenderlo?
            {
                if (firstGrab)
                {
                    //_robottino.Parla("VR14Audio/VR14SelezioneScatola");     //istruzioni per aprire scatola
                    _robottino.Parla("VR14Audio/VR14Robot/VR14GrabScatola_Robot");
                    firstGrab = false;
                }
                if (!isLocked)
                {
                    lockedObject = hit.transform;
                    lockObject(lockedObject, scale);
                }
            }
            else if (isLocked)
            {
                unlockObject(lockedObject, scale);
            }
            if (isLocked && Input.GetMouseButtonDown(0))    //grab oggetto
            {
                GrabObject();
            }
        }
        else                //sta portando un oggetto
        {
            UpdateRig();
            if (Input.GetMouseButtonDown(0))
            {
                UnGrabObject();
            }
        }
    }

    // controlla che la distanza tra oggetto e camera (senza considerare l'asse y) sia inferiore a una soglia. 
    // se la distanza è superiore ritorna un RaycastHit con distance=-1
    private RaycastHit isReachable()
    {
        RaycastHit hit;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        int layerMask = 1 << n_layer;         //layer mask 'Grabbable'
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask) && hit.transform.tag == "scatola")
        {
            Vector3 oggetto = hit.point;
            oggetto.y = cam.transform.position.y;
            if(Vector3.Distance(oggetto , cam.transform.position) <= maxDistanceGrab)
            {
                return hit;
            }
        }
        hit = new RaycastHit();
        hit.distance = -1;
        return hit;
    }

    //mostra che l'oggetto è a portata di mano e afferrabile
    private void lockObject(Transform obj, float scale)
    {
        obj.localScale = obj.localScale * scale;
        Outline outline = obj.gameObject.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = true;
        }
        isLocked = true;
    }

    //l'oggetto non è più a portata di mano e afferrabile
    private void unlockObject(Transform obj, float scale)
    {
        obj.localScale = obj.localScale / scale;
        Outline outline = obj.gameObject.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = false;
        }
        if(obj.gameObject.GetComponent<Rigidbody>() != null)
            obj.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        obj.parent = null;
        isGrabbed = false;
        isLocked = false;
    }

    private void GrabObject()
    {
        _playerMovement.isGrabbing = true;
        lockedObject.parent = this.transform;
        if(lockedObject.gameObject.GetComponent<Rigidbody>() != null)
            lockedObject.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        lockedObject.position = this.transform.position + new Vector3(0 , altezzaOggetto, 0) + this.transform.forward*0.5F;
        
        lockedObject.forward = this.transform.forward;
        rigRight.weight = 1;
        rigLeft.weight = 1;
        isGrabbed = true;
        OnGrab(this, new CustomEventArgs("none", "none"));
        PlayBoxSound(_audioGrab);
    }

    private void UnGrabObject()
    {
        _playerMovement.isGrabbing = false;
        rigRight.weight = 0;
        rigLeft.weight = 0;
        unlockObject(lockedObject, scale);
        if(lockedObject.gameObject.GetComponent<Rigidbody>() != null)
            lockedObject.gameObject.GetComponent<Rigidbody>().AddForce(this.transform.forward * 0.5F, ForceMode.Impulse);   //sposto oggetto un po' in avanti
        OnDeGrab(this, new CustomEventArgs("none", "none"));
        PlayBoxSound(_audioDrop);
    }

    private void UpdateRig()
    {
        if (playerAnimator.GetBool("walking"))
        {
            lockedObject.Find("Right").localPosition = new Vector3(0.328F, 2.9F, -0.076F);
            lockedObject.Find("Right").localRotation = Quaternion.Euler(-5.84F, -82F, 240F);
            lockedObject.Find("Left").localPosition = new Vector3(-0.345F, 0, 0.349F);
            lockedObject.Find("Left").localRotation = Quaternion.Euler(11.13F, 75.62F, 122F);
        }
        
        else
        {
            lockedObject.Find("Right").localPosition = new Vector3(0.29F, 5F, -0.15F);
            lockedObject.Find("Right").localRotation = Quaternion.Euler(-3.02F, -82F, 240F);
            lockedObject.Find("Left").localPosition = new Vector3(-0.31F, 0, 0.349F);
            lockedObject.Find("Left").localRotation = Quaternion.Euler(9.46F, 75.62F, 122F);
        }
        targetRight.position = lockedObject.Find("Right").position;
        targetRight.rotation = lockedObject.Find("Right").rotation;
        targetLeft.position = lockedObject.Find("Left").position;
        targetLeft.rotation = lockedObject.Find("Left").rotation;
    }

    public void PlayBoxSound(AudioClip clip)
    {
        if (lockedObject.gameObject.GetComponent<AudioSource>() != null)
        {
            lockedObject.gameObject.GetComponent<AudioSource>().clip = clip;
            lockedObject.gameObject.GetComponent<AudioSource>().Play();
        }
    }
}
