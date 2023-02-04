using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static VariabiliGlobali;

public class GrabSpider : MonoBehaviour
{

    public Animator VR14;
    public Animator playerAnimator;
    public GameObject cam;
    //[SerializeField] private GameObject player;
    public float maxDistanceGrab = 1F;      // a che distanza si pu� fare il grab di un oggetto
    public bool isTrigger;
    private bool isLocked = false;          //indica se l'oggetto e' abbastanza vicino per essere preso, se lo e' si applicano delle trasformazioni per farlo intuire meglio all'utente
    private int n_layer = 8;                 // in che layer sono gli oggetti grabbable 
    private Transform lockedObject;
    private float scale = 1F;              // quanto ingrandire oggetto che si può afferrare
    public GameObject rightHand;
    public GameObject spider;
    public float altezzaOggetto = 7F;
    private bool isGrabbed;
    public GameObject follow;
    private bool startTimer = false;
    private float tempoAnimazione = 2F;
    private float timer = 0F;
    private Vector3 localPosition;

    // Start is called before the first frame update
    void Start()
    {
        isGrabbed=false;
        timer = 0F;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit = isReachable();
        if (hit.distance != -1)         //e' abbastanza vicino per prenderlo?
        {
            if (!isLocked)
            {
                lockedObject = hit.transform;
                lockObject(lockedObject, scale);
            }
            if(Input.GetMouseButtonDown(0))
            {
                playerAnimator.SetBool("Grabbing",true);
                spider.GetComponent<BoxWalkScript>().enabled = false;
                Grab();
                ChangeCam();
            }
        }
        else if (isLocked)
        {
            unlockObject(lockedObject, scale);
        }
        if(isGrabbed)
        {
            spider.transform.localRotation = new Quaternion(0,0,0,1);
            spider.transform.LookAt(follow.transform);
            spider.GetComponent<BoxWalkScript>().enabled = false;
            this.GetComponent<FreeSpiderScript>().enabled = true;
        }
        if(isGrabbed && spider.GetComponent<ByeByeSpider>().jumped == false)
            spider.GetComponentInChildren<Animator>().SetBool("isGrabbed",true);
        else if (spider.GetComponent<ByeByeSpider>().jumped == true)
        {
            isGrabbed = false;
            spider.GetComponentInChildren<Animator>().SetBool("isGrabbed",false);
        }

        if (startTimer)
        {
            timer += Time.deltaTime;
            if (timer >= tempoAnimazione)
            {
                cam.transform.parent = this.transform;
                cam.transform.localPosition = localPosition;
                startTimer = false;
            }
        }   
    }

    private RaycastHit isReachable()
    {
        RaycastHit hit;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        int layerMask = 1 << n_layer;         //layer mask 'Grabbable'
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask) && hit.transform.tag == "BoxWithSpider")
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

    private void unlockObject(Transform obj, float scale)
    {
        obj.localScale = obj.localScale / scale;
        Outline outline = obj.gameObject.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = false;
        }
        obj.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        obj.parent = null;
        isLocked = false;
    }

    private void Grab()
    {
        isGrabbed=true;
        
        spider.transform.parent = rightHand.transform;
        spider.transform.position = rightHand.transform.position;
        spider.transform.rotation = rightHand.transform.rotation;
        spider.transform.forward = rightHand.transform.forward;
    }

    private void ChangeCam()
    {
        localPosition = cam.transform.localPosition;
        cam.transform.position = cam.transform.position + cam.transform.forward*0.18F + cam.transform.up*0.1F;
        startTimer = true;
    }

    public void EnterTabletMode()
    {
        VR14.SetBool("tablet", true);
        isTrigger = true;
    }

    public void ExitTabletMode()
    {
        VR14.SetBool("tablet", false);
        isTrigger = false;
    }
}
