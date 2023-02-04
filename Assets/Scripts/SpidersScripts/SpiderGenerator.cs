using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderGenerator : MonoBehaviour
{
    private Mesh[] planesMesh = new Mesh[2];
    private Plane[] planes = new Plane[2];
    public Spider spider;

    private GameObject _user;

    [SerializeField] Robottino _robottino;
    [SerializeField] Therapist _therapist;

    [SerializeField] Transform finalWaypoint;

    private Spider[] eliminationSpiders = new Spider[20];

    private float creatingDistance = 10f;

    private int createdSpiders = 0;   //contatore per ragni creati
    public int spiderInitialLimit;   //limite di creazione iniziale dei ragni
    public int numSecondGenerationSpiders; //numero di ragni da creare se l'utente risponde che va tutto ok
    private bool secondGen = false;
    private bool firstGen = false;


    void Start()
    {
        _user = GameObject.FindObjectOfType<PlayerMovement>().gameObject;

        //iscrivo il generator all'evento di risposta dell'utente
        var robottino = GameObject.FindObjectOfType<Robottino>();
        if (robottino != null)
        {
            _robottino = robottino.GetComponent<Robottino>();
        }
        var therapist = GameObject.FindObjectOfType<Therapist>();
        if (therapist != null)
        {
            _therapist = therapist.GetComponent<Therapist>();
        }

        _robottino.onAnswer += answerReaction;

        int count;
        planes = FindObjectsOfType<Plane>();
        for (count = 0; count < planes.Length; count++)
        {
            planesMesh[count] = planes[count].GetComponent<MeshFilter>().sharedMesh;
        }
        // ciclo per creare i ragni, se la fun di generazione casuale del ragno non trova un punto il contatore non viene aggiornato
        // per cui si istanzia sempre il giusto numero di ragni



    }

    private void Update()
    {
        if (firstGen == false && _therapist != null && _therapist.currentStatus == Therapist.MachineStatus.Corridor)
        {
            do
            {
                if (RandomGeneration()) createdSpiders++;
            } while (createdSpiders < spiderInitialLimit);
            firstGen = true;
        }
    }


    // func per la creazione dei ragni in posizioni casuali
    public bool RandomGeneration()
    {
        int firstRandomSelector = Random.Range(0, planes.Length - 1);
        int secondRandomSelector = Random.Range(0, planesMesh[firstRandomSelector].vertices.Length - 1);

        // trovo un vertice della mesh, scalo in base alla scale del piano che contiene il vertice,
        // e sommo la worldposition dello spider generator (posto al centro del ferro di cavallo del corridoio)

        Vector3 positionOnMesh = planesMesh[firstRandomSelector].vertices[secondRandomSelector];
        Vector3 worldPosition = Vector3.Scale(positionOnMesh, planes[firstRandomSelector].transform.localScale) + transform.position;

        // genero una direzione casuale a partire dal punto trovato sopra
        Vector3 randomPos = Random.insideUnitSphere * creatingDistance + worldPosition;

        NavMeshHit hit;

        bool foundPosition = NavMesh.SamplePosition(randomPos, out hit, creatingDistance, NavMesh.AllAreas);

        if (foundPosition)
        {
            Instantiate(spider, hit.position, planes[firstRandomSelector].transform.rotation);
            return true;
        }
        else
            return false;
    }


    // func per la generazione di punti casuali sulla navMesh come target
    public Vector3 RandomTarget(Vector3 center, float maxDistance)
    {
        Vector3 randomPos = Random.insideUnitSphere * maxDistance + center;

        NavMeshHit hit;

        bool foundPosition = NavMesh.SamplePosition(randomPos, out hit, maxDistance, NavMesh.AllAreas);

        if (!foundPosition)
        {
            //Debug.Log("returned center");
            return center;
        }

        return hit.position;
    }

    public void CreateSpiders()
    {
        while (createdSpiders <= spiderInitialLimit)
        {
            if (RandomGeneration()) createdSpiders++;
        }
    }

    public void secondGeneration()
    {
        for (int i = 0; i <= numSecondGenerationSpiders; i++)
        {
            Instantiate(spider, finalWaypoint.position, Quaternion.identity);
        }
    }

    private void answerReaction(object robottino, CustomEventArgs eventArgs)
    {
        if (eventArgs.texture == "firstTexture" && eventArgs.eventType == "waypoint" && secondGen == false)
        {
            //Debug.Log("risposta positiva");
            secondGeneration();
            secondGen = true;
        }

        if (eventArgs.texture == "secondTexture" && eventArgs.eventType == "waypoint" && secondGen == false)
        {
            //Debug.Log("risposta negativa");
            spiderElimination();
            secondGen = true;
        }
    }

    public void spiderElimination()
    {
        eliminationSpiders = FindObjectsOfType<Spider>();
        for (int i = 0; i <= eliminationSpiders.Length - 10; i++)
        {
            Destroy(eliminationSpiders[i].gameObject);
        }
    }

        public void spiderLastElimination()
    {
        eliminationSpiders = FindObjectsOfType<Spider>();
        for (int i = 0; i < eliminationSpiders.Length; i++)
        {
            if(eliminationSpiders[i].gameObject != null)
                Destroy(eliminationSpiders[i].gameObject);
        }
    }


}
