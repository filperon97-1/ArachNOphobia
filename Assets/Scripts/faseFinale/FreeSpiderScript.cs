using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeSpiderScript : MonoBehaviour
{
    public Animator playerAnimator;
    public GameObject cam;
     public float maxDistanceGrab = 1F;      // a che distanza si pu� fare il grab di un oggetto
    public bool isTrigger;
    private bool isLocked = false;          //indica se l'oggetto e' abbastanza vicino per essere preso, se lo e' si applicano delle trasformazioni per farlo intuire meglio all'utente
    private int n_layer = 8;                 // in che layer sono gli oggetti grabbable 
    private Transform lockedObject;
    private float scale = 1F;              // quanto ingrandire oggetto che si può afferrare
    public GameObject spider;
    public bool liberato;

    [SerializeField] private Therapist _therapist;
    // Start is called before the first frame update
    void Start()
    {
        liberato = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!liberato)
        {
            RaycastHit hit = isReachable();
            if (hit.distance != -1)         //e' abbastanza vicino per prenderlo?
            {
                if (!isLocked)
                {
                    lockedObject = hit.transform;
                    lockObject(lockedObject, scale);
                }
                if (Input.GetMouseButtonDown(0))
                {
                    playerAnimator.SetBool("Grabbing", false);
                    spider.GetComponent<ByeByeSpider>().enabled = true;
                    spider.GetComponentInChildren<Animator>().SetBool("isGrabbed", false);
                    liberato = true;
                    this.enabled = false;
                }
            }
            else if (isLocked)
            {
                unlockObject(lockedObject, scale);
            }
        }
    }

    private RaycastHit isReachable()
    {
        RaycastHit hit;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        int layerMask = 1 << n_layer;         //layer mask 'Grabbable'
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask) && hit.transform.tag == "window")
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
        isLocked = false;
    }
}
