using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.SceneManagement;


public class CorridorState : State
{
    private Therapist _therapist;
    private GameObject _player;
    private bool _loading;
    private bool _spiderGen;
    private bool _endPath;
    private bool _endCorridor;
    private bool _endClip;
    private bool _startEndClip;
    private bool _asking;
    private bool _isWalking;
    private bool _askingAfterTime;
    private SpiderGenerator spiderGenerator;
    private GameObject _firstDoor;
    private GameObject _secondDoor;

    private float pathDuration = 40f;

    public Sequence walkingSequence;

    public Sequence askingSequence;

    private int distanceCounter = 0;

    private float turnBackTime;
    private float turnLeftfTime;

    private AudioClip clip_end = Resources.Load("TherapistAudio/Parte2_fine") as AudioClip;

    public CorridorState(string name, Therapist therapist, GameObject player) : base(name)
    {
        _therapist = therapist;
        _player = player;
    }

    public override void Enter()
    {
        _isWalking = true;
        _loading = false;
        _spiderGen = false;
        _endPath = false;
        _endCorridor = false;
        _endClip = false;
        _asking = true;
        _startEndClip = false;
        _askingAfterTime = true;
        spiderGenerator = GameObject.FindObjectOfType<SpiderGenerator>();

        if (_therapist != null)
        {        
            //iscrizone ad evento per definire reazione del terapista quando l'utente dice come sta andando
            _therapist._robottino.onAnswer += answerReaction;

            //faccio camminare il terapeuta
            _therapist.setAnimator(Therapist.AnimatorWays.IdleToWalking);
        }

        //definizione lunghezza clip animator
        AnimationClip[] clips = _therapist.therapistAnimator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            switch (clip.name)
            {
                case "Armature_turnBack_Step":
                    turnBackTime = clip.length;
                    break;
                case "Armature_turnRight90_Step":
                    turnLeftfTime = clip.length;
                    break;
            }
        }

        SetDoorEnabled(1, true);
        SetDoorEnabled(2, false);
        _player.GetComponent<Grab>().enabled = false;
    }

    public override void Exit()
    {
        spiderGenerator.spiderLastElimination();
        SetDoorEnabled(2, false);

        SceneManager.UnloadSceneAsync("FaseCorridoio");
    }

    public override void Tik()
    {
        LookAtPlayer();

        if(!_loading)
            {
            var waypoints = GameObject.FindObjectOfType<Waypoints>();
            if (waypoints != null)
            {
                _therapist.wayRoot = waypoints.transform;
            }
            var vr14 = GameObject.FindObjectOfType<Robottino>();
            if (vr14 != null)
            {
                vr14.GetComponent<Robottino>().currentStatus = Robottino.RbMachineStatus.Flying;
                _loading = true;
            }
        }

        if (!_spiderGen)
        {
            var spiderGenerator = GameObject.FindObjectOfType<SpiderGenerator>();
            if (spiderGenerator != null)
            {
                //generazione ragni
                if (_therapist.spiderGenerator != null)
                {
                    _therapist.spiderGenerator.CreateSpiders();
                }
                _therapist.spiderGenerator = spiderGenerator.GetComponent<SpiderGenerator>();
                
                // generazione del path e della sequece con callback al cambio di waypoint
                if (_therapist.wayRoot != null && _therapist.wayRoot.childCount > 0)
                {
                    Vector3[] pathPositions = new Vector3[_therapist.wayRoot.childCount];
                    for (int i = 0; i < _therapist.wayRoot.childCount; i++)
                    {
                        pathPositions[i] = _therapist.wayRoot.GetChild(i).position;
                        pathPositions[i].y = _therapist.transform.position.y;
                    }
                    walkingSequence = DOTween.Sequence();

                    walkingSequence.Append(_therapist.transform.DOPath(pathPositions, pathDuration, PathType.CatmullRom, PathMode.Full3D, resolution: 10).SetEase(Ease.Linear)
                        .SetLookAt(0.01f).OnWaypointChange(counterCallback)).SetId("walking")
                        .OnComplete(() => {
                            _therapist.setAnimator(Therapist.AnimatorWays.WalkingToIdle);
                            _endPath = true;
                            _isWalking = false;
                        });
                }
                _spiderGen = true;
            }
            
        }
        if (_endPath)
        {
            float dist = Vector3.Distance(_therapist.transform.position, _player.transform.position);
            if (dist <= 2f)
            {
                _therapist.currentStatus = Therapist.MachineStatus.Boxes;
            }
        }

        if (_asking && _askingAfterTime)
        {
            float distance = Vector3.Distance(_therapist.transform.position, _therapist.user.transform.position);

            if (distance > 8f && _therapist.asking == false && _therapist.playerTooFar == false)
            {
                _isWalking = false;
                highDistance();
            }

            //se il player si riavvicina il terapeuta ricomincia a camminare
            if (_therapist.playerTooFar == true && distance < 4f)
            {
                rightDistance();
                _isWalking = true;
            }

            if (_therapist.playerTooFar == true)
            {
                distanceCounter++;
            }

            _therapist.therapistAnimator.SetBool("talking", _therapist.therapistAudioSource.isPlaying);

            if (distanceCounter == 1200 && _therapist.playerTooFar == true)
            {
                _therapist.askingType = "timeout";
                _therapist.asking = true;
                _therapist.playerTooFar = false;
                _therapist.setAnimator(Therapist.AnimatorWays.WalkingToIdleTurnBack);

                var clip = Resources.Load("TherapistAudio/Domanda1") as AudioClip;
                _therapist.therapistAudioSource.clip = clip;
                _therapist.therapistAudioSource.Play();
                _isWalking = false;
            }
        }

        if (_endCorridor)
        {

            float distance = Vector3.Distance(_therapist.transform.position, _therapist.user.transform.position);

            if (distance <= 1.5f)
            {
                _therapist.therapistAnimator.SetBool("talking", true);

                if (!_startEndClip)
                {
                    _therapist.therapistAudioSource.clip = clip_end;
                    _therapist.therapistAudioSource.Play();
                    _endCorridor = false;
                    _asking = false;
                    SceneManager.LoadSceneAsync("FaseScatole", LoadSceneMode.Additive);
                    _startEndClip = true;
                    SetDoorEnabled(1, false);
                }
            }
        }

        if(_therapist.therapistAudioSource.clip == clip_end && !_endClip)
        {
            if (!_therapist.therapistAudioSource.isPlaying)
            {
                _therapist.therapistAnimator.SetBool("talking", false);
                _therapist.setAnimator(Therapist.AnimatorWays.IdleToWalkingTurnBack);
                _therapist.StartCoroutine(delaySequenceCoroutine(turnBackTime));
                _endClip = true;
                _isWalking = true;
            }
        }
    }

    // funzione per la gestione delle fasi di asking del terapeuta, chiamata in corrispondenza degli waypoint
    private void counterCallback(int waypointIndex)
    {
         if (waypointIndex==5)
        {
            distanceCounter = 0;
            _therapist.askingType = "waypoint";
            DOTween.TogglePause("walking");
            _therapist.setAnimator(Therapist.AnimatorWays.WalkingToIdleRightTurn);

            var clip = Resources.Load("TherapistAudio/Domanda1") as AudioClip;
            _therapist.therapistAudioSource.clip = clip;
            _therapist.therapistAudioSource.Play();
            _therapist.asking=true;
            _isWalking = false;
            _askingAfterTime = true;
        }

        if(waypointIndex == 8)
        {
            _therapist.askingType = "waypoint";
            DOTween.TogglePause("walking");
            _therapist.setAnimator(Therapist.AnimatorWays.WalkingToIdleRightTurn);
            _endCorridor = true;
            _isWalking = false;
            SetDoorEnabled(2, true);
        }

        if(waypointIndex == 4 || waypointIndex == 7)
        {
            _askingAfterTime = false;
        }
    }

    //funzione che gestisce la situazione in cui il player è troppo lontano
    private void highDistance()
    {
        DOTween.Pause("walking");
        _therapist.setAnimator(Therapist.AnimatorWays.WalkingToIdleTurnBack);
        _therapist.playerTooFar = true;
    }

    //funzione che gestisce la situazione in cui il player si riavvicina
    private void rightDistance()
    {
        distanceCounter = 0;
        _therapist.setAnimator(Therapist.AnimatorWays.IdleToWalkingTurnBack);
        _therapist.StartCoroutine(delaySequenceCoroutine(turnBackTime));

        _therapist.playerTooFar = false;
    }

    //funzione per definire la reazione alla risposta dell'utente in asking
    private void answerReaction(object robottino, CustomEventArgs eventArgs)
    {
        distanceCounter = 0;
        _therapist.asking = false;

        switch (eventArgs.texture)
        {
            case "firstTexture"://l'utente dice che va tutto bene
                if (eventArgs.eventType == "waypoint") //si creano nuovi ragni
                {
                    var clip = Resources.Load("TherapistAudio/Risposta2") as AudioClip;
                    _therapist.therapistAudioSource.clip = clip;
                   _therapist.StartCoroutine(delayAnimatonCoroutine(clip.length));
                    _therapist.therapistAudioSource.Play();
                }
                else // si incoraggia l'utente ma non si creano nuovi ragni
                {
                    //serve clip audio differente
                    var clip = Resources.Load("TherapistAudio/SceltaGiusta") as AudioClip;
                    _therapist.therapistAudioSource.clip = clip;
                    _therapist.therapistAudioSource.Play();
                    _therapist.playerTooFar = true;

                }
                break;


            case "secondTexture":// l'utente dice che non va bene
                if (eventArgs.eventType == "waypoint") 
                {
                    var clip = Resources.Load("TherapistAudio/Risposta1") as AudioClip;
                    _therapist.therapistAudioSource.clip = clip;
                    _therapist.StartCoroutine(delayAnimatonCoroutine(clip.length));
                    _therapist.therapistAudioSource.Play();
                }
                else
                {
                    var clip = Resources.Load("TherapistAudio/AudioIncoraggiamento") as AudioClip;
                    _therapist.therapistAudioSource.clip = clip;
                    _therapist.therapistAudioSource.Play();
                    _therapist.playerTooFar = true;

                }
                break;
        }
    }


    private IEnumerator delayAnimatonCoroutine(float clipLength) //coroutine per aspettare chre il therapist finisca di parlare prima di farlo girare
    {
        yield return new WaitForSeconds(clipLength);
        //Debug.Log("first Coroutine started");
        _therapist.setAnimator(Therapist.AnimatorWays.IdleToWalkingLeftTurn);
        _therapist.StartCoroutine(delaySequenceCoroutine(turnLeftfTime));

    }

    private IEnumerator delaySequenceCoroutine(float clipLenght) //finisco l'animazione prima di farlo camminare
    {
        //Debug.Log("second Coroutine started");
        yield return new WaitForSeconds(clipLenght);
        DOTween.TogglePause("walking");
    }

    private void LookAtPlayer()
    {
        if (!_isWalking)
        {
            float speed = 3.0f;
            Vector3 targetDirection = _therapist.user.transform.position - _therapist.transform.position;
            targetDirection.y = _therapist.transform.forward.y;
            float singleStep = speed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(_therapist.transform.forward, targetDirection, singleStep, 0.0f);
            _therapist.transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    private void SetDoorEnabled(int id, bool enabled)
    {
        if (id == 1)
        {
            _firstDoor = GameObject.FindObjectOfType<firstDoorTrigger>().gameObject;
            if(_firstDoor != null)
            {
                _firstDoor.GetComponentInChildren<BoxCollider>().enabled = !enabled; //Forse non necessario
                _firstDoor.GetComponent<BoxCollider>().isTrigger = enabled;
                _firstDoor.GetComponent<BoxCollider>().enabled = enabled;
                _firstDoor.GetComponent<firstDoorTrigger>().enabled = enabled;
            }

        }
        else
        {
            _secondDoor = GameObject.FindObjectOfType<secondDoorTrigger>().gameObject;
            if (_secondDoor != null)
            {
                _secondDoor.GetComponentInChildren<BoxCollider>().enabled = !enabled; //Forse non necessario
                _secondDoor.GetComponent<BoxCollider>().isTrigger = enabled;
                _secondDoor.GetComponent<BoxCollider>().enabled = enabled;
                _secondDoor.GetComponent<secondDoorTrigger>().enabled = enabled;
            }
        }
    }

}
