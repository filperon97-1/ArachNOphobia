using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public class BeginState : State
{
    private AudioClip clip_intro = Resources.Load("TherapistAudio/Intro") as AudioClip;

    private Therapist _therapist;
    private GameObject _player;
    private Robottino _robottino;

    private bool _endAudioIntro;
    private bool _endWalking;
    private bool _loading;


    public BeginState(string name, Therapist therapist, GameObject player) : base(name)
    {
        _therapist = therapist;
        _player = player;
    }

    public override void Enter()
    {
        SceneManager.LoadSceneAsync("FaseIniziale", LoadSceneMode.Additive);
        _endAudioIntro = false;
        _endWalking = true;
        _loading = false;
     
        _therapist.therapistAudioSource.clip = clip_intro;
        _therapist.therapistAudioSource.Play();
        _robottino = GameObject.FindObjectOfType<Robottino>();
    }

    public override void Exit()
    {
        SceneManager.LoadSceneAsync("FasePuzzle", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("FaseIniziale");
    }

    public override void Tik()
    {
        LookAtPlayer();

        if (!_loading)
        {
            var waypoints = GameObject.FindObjectOfType<WaypointsInizio>();
            if (waypoints != null)
            {
                _therapist.wayRoot = waypoints.transform;
                _loading = true;
            }
        }

        _therapist.therapistAnimator.SetBool("talking", _therapist.therapistAudioSource.isPlaying);
        if (_therapist.therapistAudioSource.isPlaying == false && _endAudioIntro == false)
        {
            _endAudioIntro = true;
            _endWalking = false;
            CreatePath();
            _robottino.Parla("VR14Audio/VR14Robot/VR14Intro_Robot");
        }

        if(_endAudioIntro == true && _endWalking == true)
        {
            float distance = Vector3.Distance(_therapist.transform.position, _player.transform.position);
            if (distance <= 1.5f)
            {
                _therapist.currentStatus = Therapist.MachineStatus.Puzzle;
            }
        }
    }

    private void CreatePath()
    {
        float pathDuration = 6f;
        _therapist.setAnimator(Therapist.AnimatorWays.IdleToWalking);

        // generazione del path e della sequence 
        if (_therapist.wayRoot != null && _therapist.wayRoot.childCount > 0)
        {
            Vector3[] pathPositions = new Vector3[_therapist.wayRoot.childCount];
            for (int i = 0; i < _therapist.wayRoot.childCount; i++)
            {
                pathPositions[i] = _therapist.wayRoot.GetChild(i).position;
                pathPositions[i].y = _therapist.transform.position.y;
            }
            Sequence walkingSequence = DOTween.Sequence();
            walkingSequence.Append(_therapist.transform.DOPath(pathPositions, pathDuration, PathType.CatmullRom, PathMode.Full3D, resolution: 10).SetEase(Ease.Linear)
                .SetLookAt(0.01f)).SetId("walking")
                .OnComplete(() => { 
                    _therapist.setAnimator(Therapist.AnimatorWays.WalkingToIdle);
                    _endWalking = true;
                });
        }
    }

    private void LookAtPlayer()
    {
        if (_endWalking)
        {
            float speed = 3.0f;
            Vector3 targetDirection = _therapist.user.transform.position - _therapist.transform.position;
            targetDirection.y = _therapist.transform.forward.y;
            float singleStep = speed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(_therapist.transform.forward, targetDirection, singleStep, 0.0f);
            _therapist.transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }
}
