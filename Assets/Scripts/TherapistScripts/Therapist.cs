using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TherapistScripts.TherapistStates;

public class Therapist : MonoBehaviour
{

    private FiniteStateMachine<Therapist> finiteStateMachine;

    private Rigidbody therapistRigidbody;
    public Animator therapistAnimator;
    public AudioSource therapistAudioSource;

    public Transform wayRoot;

    public SpiderGenerator spiderGenerator;

    public GameObject user;
    public Robottino _robottino;
    public Grab _grab;
    public ApritoreDiScatole _apritoreDiScatole;
    public MouseLook _mouseLook;
    
    public SelectionManager _selectionManager;
    public Zoom _zoom;

    public event EventHandler<CustomEventArgs> OnAsking;
    public String askingType;

    public bool asking;
    public bool playerTooFar;

    [SerializeField] private AudioClip _footsteps;

    public enum AnimatorWays
    {
        IdleToWalking,
        WalkingToIdle,
        IdleToWalkingLeftTurn,
        WalkingToIdleRightTurn,
        IdleToWalkingTurnBack,
        WalkingToIdleTurnBack,
    }


    public enum MachineStatus
    {
        Begin,
        Puzzle,
        Corridor,
        Boxes,
        FreeSpider
    }

    public MachineStatus currentStatus;

    private Vector3? destination;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float movingSpeed;

    private float _blendTalk = 0f;

    void Start()
    {
        playerTooFar = false;
        asking = false;
        therapistRigidbody = GetComponent<Rigidbody>();
        therapistAnimator = GetComponent<Animator>();
        therapistAudioSource = GetComponent<AudioSource>();


        finiteStateMachine = new FiniteStateMachine<Therapist>(this);

        //STATES
        State beginState = new BeginState("begin", this, user);
        State puzzleState = new PuzzleState("puzzle", this, _zoom, user, _selectionManager , _mouseLook);
        State corridorState = new CorridorState("corridor", this, user);
        State boxesState = new BoxesState("boxes", this , _grab , _apritoreDiScatole, user);
        State freeSpiderState = new FreeSpiderState("freeSpider", this, user);


        //TRANSITIONS
        finiteStateMachine.AddTransition(beginState, puzzleState, () => currentStatus == MachineStatus.Puzzle);
        finiteStateMachine.AddTransition(puzzleState, corridorState, () => currentStatus == MachineStatus.Corridor);
        finiteStateMachine.AddTransition(corridorState, boxesState, () => currentStatus == MachineStatus.Boxes);
        finiteStateMachine.AddTransition(boxesState, freeSpiderState, () => currentStatus == MachineStatus.FreeSpider);

        switch (currentStatus)
        {
            case MachineStatus.Begin:
                finiteStateMachine.SetState(beginState);
                break;
            case MachineStatus.Puzzle:
                finiteStateMachine.SetState(puzzleState);
                break;
            case MachineStatus.Corridor:
                finiteStateMachine.SetState(corridorState);
                break;
            case MachineStatus.Boxes:
                finiteStateMachine.SetState(boxesState);
                break;
            case MachineStatus.FreeSpider:
                finiteStateMachine.SetState(freeSpiderState);
                break;
        }
    }


    void Update(){

        if (asking)
        {
            OnAsking(this, new CustomEventArgs(askingType, "none"));
        }

        _blendTalk += Time.deltaTime/20;
        if (_blendTalk > 1)
            _blendTalk = 0;
        therapistAnimator.SetFloat("BlendTalk", _blendTalk);

        finiteStateMachine.Tik();

    }

    private void FixedUpdate()
    {
        
        if (destination.HasValue && Vector3.Distance(transform.position, destination.Value) > 1f)
        {
            Vector3 targetDirection = (destination.Value - transform.position).normalized;
            targetDirection.y = 0f;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.fixedDeltaTime, 0f);

            therapistRigidbody.MoveRotation(Quaternion.LookRotation(newDirection, transform.up));
            therapistRigidbody.MovePosition(therapistRigidbody.position + transform.forward * movingSpeed * Time.fixedDeltaTime);

        }
        else
            destination = null;
    }

    // funzione per cambiare lo stato del terapeuta, da chiamare quando l'utente termina un task. 
    public void setNextState (MachineStatus nextState)
    {
        currentStatus = nextState;
    }

    //funzione per dare una destinazione al terapeuta

    public void moveToPoint(Vector3 position)
    {
        destination = position;

    }

    public void setAnimator(AnimatorWays way)
    {
        switch (way)
        {
            case AnimatorWays.IdleToWalking:
                therapistAnimator.SetInteger("TherapistController", 1);
               // Debug.Log(therapistAnimator.GetInteger("TherapistController"));
                break;
            case AnimatorWays.WalkingToIdle:
                therapistAnimator.SetInteger("TherapistController", 2);
                //Debug.Log(therapistAnimator.GetInteger("TherapistController"));
                break;
            case AnimatorWays.WalkingToIdleTurnBack:
                therapistAnimator.SetInteger("TherapistController", 3);
                //Debug.Log(therapistAnimator.GetInteger("TherapistController"));
                break;
            case AnimatorWays.IdleToWalkingTurnBack:
                therapistAnimator.SetInteger("TherapistController", 4);
               //Debug.Log(therapistAnimator.GetInteger("TherapistController"));
                break;
            case AnimatorWays.WalkingToIdleRightTurn:
                therapistAnimator.SetInteger("TherapistController", 5);
               // Debug.Log(therapistAnimator.GetInteger("TherapistController"));
                break;
            case AnimatorWays.IdleToWalkingLeftTurn:
                therapistAnimator.SetInteger("TherapistController", 6);
               // Debug.Log(therapistAnimator.GetInteger("TherapistController"));
                break;
        }
    }
}
