using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyState : State
{
    private Robottino _robottino;

    public FlyState(string name, Robottino robottino) : base(name)
    {
        _robottino = robottino;
    }

    private Transform targetPosition;

    public float velocity=2f;

    private float rays = 23f;
    private float angle = 360f;


     private float maxDistance=2f;

    public override void Enter()
    {
        targetPosition = _robottino.targetPosition;
    }

    public override void Exit()
    {
        
    }

    public override void Tik()
    {
        Vector3 sidePosition = targetPosition.position - targetPosition.right * 1f + targetPosition.forward * 1.5f;
        Vector3 targetDirection = (sidePosition - _robottino.transform.position);
        if (targetDirection.magnitude > 0.5f)
        {
            flyingAI(targetDirection);
        }
    }

    private void flyingAI(Vector3 targetDirection)
    {
        Vector3 aimingDirection = targetDirection.normalized; //destinazione a sinistra dell'utente

        for (int i = 0; i < rays; i++) // creo dei raggi intorno al robottino e se uno di questi fa hit sottraggo alla aimimg direction un vettore che va nella direzione opposta
        {
            float baseShift = angle / rays;

            Quaternion rotation = _robottino.transform.rotation;

            Quaternion mulRotation = Quaternion.AngleAxis(baseShift * i - 90f + baseShift / 2, _robottino.transform.up);

            Vector3 direction = rotation * mulRotation * Vector3.forward;

            Ray ray = new Ray(_robottino.transform.position, direction);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                aimingDirection -= (2.0f / rays) * direction; // sottraggo due volte il vettore per allontanare il robottino dall'ostacolo
            }


        }

        aimingDirection.y = 0f;

        _robottino.transform.Translate(aimingDirection * velocity * Time.deltaTime);
    }

}
