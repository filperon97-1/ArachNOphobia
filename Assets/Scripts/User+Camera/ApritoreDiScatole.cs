using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Animations.Rigging;
using static VariabiliGlobali;

public class ApritoreDiScatole : MonoBehaviour
{
     public GameObject spider;
    public Robottino _robottino;
    public Grab _grab;
    public GameObject cam;
    public GameObject table;
    public GameObject ragnoLittle;
    public GameObject ragnoMedium;
    public GameObject ragnoTarantula;
    public int totaleScatole = 4;
    public float maxDistanceOpen = 1.5F;      // a che distanza si puo' aprire la scatola

    // per cinematica inversa delle braccia
    public Transform targetRight;
    public Transform targetLeft;
    public Rig rigRight;
    public Rig rigLeft;

    private int nScatoleAperte = 0;
    private GameObject[] scatoleAperte;
    private float defaultFieldOfView = 60F;

    private float timeToZoom = 0.5F;
    private float maxTimeOnZoom = 1F;
    private float timeOnZoom = 0F;
    private RaycastHit hit;
    private bool firstOpen = true;
    private bool isGrabbing = false;

    [SerializeField] private AudioClip _audioOpenBox;

    public event EventHandler<CustomEventArgs> OnOpen;
    public event EventHandler<CustomEventArgs> OnLastOpen;

    private enum Status
    {
        checkOpen,
        zoomIn,
        wait,
        zoomOut
    }
    private Status currentStatus;

    // Start is called before the first frame update
    void Start()
    {
        _robottino = FindObjectOfType<Robottino>();
        scatoleAperte = new GameObject[totaleScatole];
        currentStatus = Status.checkOpen;
        timeOnZoom = 0F;
        rigLeft.weight = 0F;
        rigRight.weight = 0F;
        firstOpen = true;
        isGrabbing = false;
        _grab.OnGrab += (object obj, CustomEventArgs eventArgs) =>isGrabbing=true;
        _grab.OnDeGrab += (object obj, CustomEventArgs eventArgs) => isGrabbing = false;
        VariabiliGlobali.ragnoScelto = VariabiliGlobali.Ragno.Tarantula;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentStatus)
        {
            case Status.checkOpen:
                CheckOpen();
                break;
            case Status.zoomIn:
                ZoomInAndOpen();
                break;
            case Status.wait:
                Wait();
                break;
            case Status.zoomOut:
                ZoomOut();
                break;
        }
    }

    void CheckOpen()
    {
        // audio robottino istruzioni per aprire scatola
        if (firstOpen && !isGrabbing)
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            if (Physics.Raycast(ray, out hit, maxDistanceOpen))
            {
                if (hit.collider.gameObject.tag == "scatola" && isOnTable(hit.collider.gameObject)) 
                {
                    //_robottino.Parla("VR14Audio/Vr14AprireScatola");
                    _robottino.Parla("VR14Audio/VR14Robot/Vr14AprireScatola_Robot");
                    firstOpen = false;
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && !isGrabbing)
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            if (Physics.Raycast(ray, out hit, maxDistanceOpen))
            {
                if (hit.collider.gameObject.tag == "scatola" && !isOpen(hit.collider.gameObject) && isOnTable(hit.collider.gameObject))     //� una scatola e non � gi� stata aperta e si trova sul tavolo (NECESSARIO??)
                {
                    _grab.PlayBoxSound(_audioOpenBox);
                    scatoleAperte[nScatoleAperte] = hit.collider.gameObject;
                    nScatoleAperte++;
                    currentStatus = Status.zoomIn;
                    OnOpen(this, new CustomEventArgs("none", "none"));
                }
            }
        }
    }

    void ZoomInAndOpen()
    {
        if (Camera.main.fieldOfView <= defaultFieldOfView / 2)
        {
            hit.collider.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            UpdateRig(hit.transform, 0);
            currentStatus = Status.wait;
        }
        else
        {
            if (nScatoleAperte == totaleScatole)
            {
                // METTERE IL RAGNO NELLA SCATOLA
                spider.transform.parent = hit.transform;
                spider.transform.position = hit.transform.position + new Vector3(0, 0.03f, 0);
                spider.transform.localScale = new Vector3(1, 1, 1);
                VariabiliGlobali.scatola = hit.collider.gameObject;
                hit.transform.tag = "BoxWithSpider";
                InstantiateRagno();
            }
            //ANIMAZIONE UTENTE CHE APRE SCATOLA
            float speed = 0.5F;
            Camera.main.fieldOfView -= (Time.deltaTime * defaultFieldOfView) / (2 * timeToZoom);
            hit.transform.GetChild(0).position = hit.transform.GetChild(0).position + new Vector3(0, 1, 0) * speed * Time.deltaTime;
            UpdateRig(hit.transform , 1);
        }
    }

    void Wait()
    {
        if(timeOnZoom >= maxTimeOnZoom)
        {
            currentStatus = Status.zoomOut;
            timeOnZoom = 0;
        }
        else
        {
            timeOnZoom += Time.deltaTime;
        }
    }

    void ZoomOut()
    {
        if (Camera.main.fieldOfView >= defaultFieldOfView)
        {
            currentStatus = Status.checkOpen;
            if (nScatoleAperte >= totaleScatole)
            {
                OnLastOpen(this, new CustomEventArgs("none", "none"));
            }
        }
        else
        {
            Camera.main.fieldOfView += (Time.deltaTime * defaultFieldOfView) / (2 * timeToZoom);
        }
    }

    bool isOnTable(GameObject oggetto)
    {
        RaycastHit hit;
        Ray ray = new Ray(oggetto.transform.position, new Vector3(0, -1, 0));
        if (Physics.Raycast(ray, out hit, 0.5F))
        {
            if (hit.collider.gameObject == table)
            {
                return true;
            }
        }
        return false;
    }

    bool isOpen(GameObject oggetto)
    {
        for (int i = 0; i < nScatoleAperte; i++)
        {
            if (oggetto == scatoleAperte[i])
            {
                return true;
            }
        }
        return false;
    }

    void UpdateRig(Transform obj , float weight)
    {
        rigLeft.weight = weight;
        rigRight.weight = weight;
        if (weight != 0)
        {
            for (int i = 0; i < obj.childCount; i++)
            {
                if (obj.GetChild(i).name == "coperchio2")
                {
                    for (int j = 0; j < obj.GetChild(i).childCount; j++)
                    {
                        if (obj.GetChild(i).GetChild(j).name == "Right")
                        {
                            targetRight.position = obj.GetChild(i).GetChild(j).position;
                            targetRight.rotation = obj.GetChild(i).GetChild(j).rotation;
                        }
                        if (obj.GetChild(i).GetChild(j).name == "Left")
                        {
                            targetLeft.position = obj.GetChild(i).GetChild(j).position;
                            targetLeft.rotation = obj.GetChild(i).GetChild(j).rotation;
                        }
                    }
                }
            }
        }
    }

    void InstantiateRagno()
    {
        GameObject ragno = null;
        switch (VariabiliGlobali.ragnoScelto)
        {
            case VariabiliGlobali.Ragno.Little:
                ragno = Instantiate(ragnoLittle);
                ragno.transform.parent = hit.transform.Find("Spider/Spider_little");
                ragno.transform.position = hit.transform.Find("Spider/Spider_little").position;
                ragno.transform.localScale = new Vector3(1,1,1);

                break;
            case VariabiliGlobali.Ragno.Medium:
                ragno = Instantiate(ragnoMedium);
                ragno.transform.parent = hit.transform.Find("Spider/Spider_medium");
                ragno.transform.position = hit.transform.Find("Spider/Spider_medium").position;
                ragno.transform.localScale = new Vector3(1,1,1);
                break;
            case VariabiliGlobali.Ragno.Tarantula:
                ragno = Instantiate(ragnoTarantula);
                ragno.transform.parent = hit.transform.Find("Spider/Tarantula");
                ragno.transform.position = hit.transform.Find("Spider/Tarantula").position;
                ragno.transform.localScale = new Vector3(1,1,1);
                break;
        }
        VariabiliGlobali.ragno = ragno;

        hit.transform.Find("Spider").GetComponent<BoxWalkScript>().enabled = true;
        nScatoleAperte++;
    }
}