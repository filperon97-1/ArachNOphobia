using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avoidance : MonoBehaviour
{

    public Transform targetPosition;


    public float velocity;

    private float rays = 23f;
    private float angle = 360f;


    [SerializeField] private float maxDistance;

    [SerializeField] private float scoutDistance;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 sidePosition = targetPosition.position -targetPosition.right * 1f +targetPosition.forward *5f;
        Vector3 targetDirection = (sidePosition - transform.position);

        if (targetDirection.magnitude > 0.5f)
        {
            flyingAI(targetDirection);
        }



    }

    private void OnDrawGizmos()
    {
        for (int i=0; i<rays; i++)
        {
            float baseShift = angle /rays;

            Quaternion rotation = transform.rotation;

            Quaternion mulRotation=Quaternion.AngleAxis(baseShift*i -90f + baseShift/2, transform.up);

            Vector3 direction = rotation * mulRotation * Vector3.forward;

            Gizmos.color = Color.red;

            Gizmos.DrawRay(this.transform.position, direction); 

        }
    }


    private void flyingAI(Vector3 targetDirection)
    {
        Vector3 aimingDirection = targetDirection.normalized; //destinazione a sinistra dell'utente
        
        for (int i = 0; i < rays; i++) // creo dei raggi intorno al robottino e se uno di questi fa hit sottraggo alla aimimg direction un vettore che va nella direzione opposta
        {
            float baseShift = angle / rays;

            Quaternion rotation = transform.rotation;

            Quaternion mulRotation = Quaternion.AngleAxis(baseShift * i - 90f + baseShift / 2, transform.up);

            Vector3 direction = rotation * mulRotation * Vector3.forward;

            Ray ray = new Ray(transform.position, direction);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                aimingDirection -= (2.0f / rays) * direction; // sottraggo due volte il vettore per allontanare il robottino dall'ostacolo
            }

            //AGGIUNTA, AGGIUNGO RAGGI ANCHE IN VERTICALE
            
            mulRotation = Quaternion.AngleAxis(baseShift * i - 90f + baseShift / 2, transform.up);

            direction = rotation * mulRotation * Vector3.forward;

            ray = new Ray(transform.position, direction);

            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                aimingDirection -= (2.0f / rays) * direction; // sottraggo due volte il vettore per allontanare il robottino dall'ostacolo
            }
            
            //FINE AGGIUNTA


        }

        //aimingDirection.y = 0f;       // non si muove in verticale così

        transform.Translate(aimingDirection * velocity * Time.deltaTime);
    }


}
