using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyState2 : State
{
    private Robottino _robottino;
    private GameObject personaggio;
    private float distanza = 1F;             //distanza minima tra robottino e personaggio 
    private float normalSpeed = 2F;
    private float assaiSpeed = 3F;
    private float minHeight = 1F;      //altezza minima dal terreno

    private float actualSpeed;
    private Vector3 targetPosition;
    private Vector3 direzione;
    private bool visible;
    private Renderer rend;

    public FlyState2(string name, Robottino robottino) : base(name)
    {
        _robottino = robottino;
    }

    public override void Enter()
    {
        personaggio = _robottino.personaggio;
        actualSpeed = normalSpeed;
        rend = _robottino.GetComponent<MeshRenderer>();
        visible = true;
    }

    public override void Exit()
    {

    }

    public override void Tik()
    {
        visible = true;
        NotTooClose();
        FollowUser();
        rend.enabled = visible;
    }

    void NotTooClose()
    {
        if ((personaggio.transform.position - _robottino.transform.position).sqrMagnitude < Mathf.Pow(distanza, 2))
        {
            actualSpeed = assaiSpeed;
        }
        else
        {
            actualSpeed = normalSpeed;
        }
    }

    void FollowUser()
    {
        _robottino.transform.forward = personaggio.transform.position - _robottino.transform.position;                         // guarda sempre l'utente
        targetPosition = personaggio.transform.position + personaggio.transform.forward * distanza + personaggio.transform.right * 0.5f;      // posizione in cui deve spostarsi
        targetPosition.y = Mathf.Max(targetPosition.y, minHeight);                                       // deve stare sollevato da terra 
        direzione = targetPosition - _robottino.transform.position;
        bool cannotMove = false;
        if (!NearObstacle(targetPosition))  //controlla se c'� un ostacolo vicino
        {
            _robottino.transform.position += direzione * actualSpeed * Time.deltaTime;
        }
        else if (!TryDifferentPositions(targetPosition, 0.2F)) //se c'� ostacolo vicino prova altre posizioni (non faccio stessa cosa anche col muro perch� altrimenti vicino al muro ha movimenti strani)
        {
            cannotMove = true;
        }
        // se robottino non riesce a muoversi ed � molto vicino a utente lo rendo invisibile (risulterebbe troppo vicino alla camera)
        if (cannotMove && (personaggio.transform.position - _robottino.transform.position).sqrMagnitude < Mathf.Pow(distanza / 2, 2))
        {
            visible = false;
        }
    }

    // destinazione � troppo vicina a un ostacolo?
    private bool NearObstacle(Vector3 dest)
    {
        float radius = 0.2F;
        Collider[] hitColliders = Physics.OverlapSphere(dest, radius);
        if (hitColliders.Length >= 1)
        {
            foreach (Collider hit in hitColliders)
            {
                if (hit.gameObject.name != "Screen")
                {
                    return true;
                }
            }
        }
        return false;
    }

    // prova destinazioni alternative per il robottino (destinazione pi� in alto, pi� in basso, pi� a destra e pi� a sinistra rispetto alla camera)
    private bool TryDifferentPositions(Vector3 targetPosition, float dist)
    {
        Vector3 up = personaggio.transform.up;
        Vector3 destra = Vector3.Cross(personaggio.transform.forward, up);
        Vector3[] possibleDestinations = new Vector3[] { targetPosition + up * dist, targetPosition - up * dist, targetPosition + destra * dist, targetPosition - destra * dist };
        for (int i = 0; i < 4; i++)
        {
            Vector3 dest = possibleDestinations[i];
            dest.y = Mathf.Max(dest.y, minHeight);
            if (!NearObstacle(dest))
            {
                direzione = dest - _robottino.transform.position;
                _robottino.transform.position += direzione * actualSpeed * Time.deltaTime;
                return true;
            }
        }
        return false;
    }

}