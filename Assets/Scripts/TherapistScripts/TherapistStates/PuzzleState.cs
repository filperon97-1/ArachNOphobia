using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using DG.Tweening.Core;

namespace TherapistScripts.TherapistStates
{
    public class PuzzleState : State
    {
        private AudioClip clip_begin = Resources.Load("TherapistAudio/Parte1_inizio") as AudioClip;
        private AudioClip clip_end = Resources.Load("TherapistAudio/Parte1_fine") as AudioClip;

        private SelectionManager _selectionManager;
        private Zoom _zoom;

        private Therapist _therapist;
        private GameObject _player;
        private Robottino _robottino;
        private MouseLook _mouseLook;

        private bool complete;
        private bool _endAudioPuzzle;
        private bool _endWalking;
        private bool _loading;
        private bool _robottinoTalking;


        public PuzzleState(string name, Therapist therapist, Zoom zoom, GameObject player, SelectionManager selectionManager , MouseLook mouseLook) : base(name)
        {
            _therapist = therapist;
            _selectionManager = selectionManager;
            _zoom = zoom;
            _player = player;
            _mouseLook = mouseLook;
        }

        public override void Enter()
        {
            
            if (_selectionManager != null)
            {
                _selectionManager.enabled = false;
            }

            if (_therapist != null)
            {
                _therapist.therapistAudioSource.clip = clip_begin;
                _therapist.therapistAudioSource.Play();
            }

            if (_mouseLook != null)
                _mouseLook.disableAnimation = true;
            _robottino = GameObject.FindObjectOfType<Robottino>();
            //_zoom.enabled = true;
            _endAudioPuzzle = false;
            _endWalking = true;
            complete = false;
            _loading = false;
            _robottinoTalking = false;

            GameObject.FindObjectOfType<MouseCursor>().gameObject.transform.DOScale(0.001f, 0.2f).SetRelative();
        }

        public override void Exit()
        {
            SceneManager.UnloadSceneAsync("FasePuzzle");
        }

        public override void Tik()
        {
            LookAtPlayer();

            if (!_loading)
            {
                var waypoints = GameObject.FindObjectOfType<WaypointsPuzzle>();
                if (waypoints != null)
                {
                    _therapist.wayRoot = waypoints.transform;
                    _loading = true;
                }
                var vr14 = GameObject.FindObjectOfType<Robottino>();
                if(vr14 != null)
                {
                    vr14.GetComponent<Robottino>().currentStatus = Robottino.RbMachineStatus.Asking;
                }
            }

            //BEGIN FASE1
            if (_therapist.therapistAudioSource.clip == clip_begin) 
            {
                if (_therapist.therapistAudioSource.isPlaying)
                {
                    //loop animation
                    _selectionManager.enabled = false;
                    _therapist.therapistAnimator.SetBool("talking", true);
                }
                else
                {
                    //once audio is ended selection is enabled
                    _selectionManager.enabled = true;
                    _therapist.therapistAnimator.SetBool("talking", false);
                    if (!_robottinoTalking)
                    {
                        //_robottino.Parla("VR14Audio/VR14Rotazione");
                        _robottino.Parla("VR14Audio/VR14Robot/VR14Rotazione_Robot");
                        _robottinoTalking = true;
                    }
                }
            }

            if(_therapist.therapistAudioSource.clip == clip_end)
            {
                _robottino.currentStatus = Robottino.RbMachineStatus.Flying;
                _therapist.therapistAnimator.SetBool("talking", _therapist.therapistAudioSource.isPlaying);
                if (_therapist.therapistAudioSource.isPlaying == false && _endAudioPuzzle == false)
                {
                    _endWalking = false;
                    _endAudioPuzzle = true;
                    CreatePath();
                }
            }

            if (_endAudioPuzzle == true && _endWalking == true)
            {
                float distance = Vector3.Distance(_therapist.transform.position, _player.transform.position);
                if (distance <= 1.5f)
                {
                    _therapist.currentStatus = Therapist.MachineStatus.Corridor;
                }
            }

            if (IsPuzzleComplete() && complete == false)
            {
                _therapist.therapistAudioSource.clip = clip_end;
                _therapist.therapistAudioSource.Play();
                if(_selectionManager!=null)
                    _selectionManager.enabled = false;
                //_zoom.stopZoom = true;
                complete = true;
                GameObject.FindObjectOfType<MouseCursor>().gameObject.transform.DOScale(0, 0.2f);
                SceneManager.LoadSceneAsync("FaseCorridoio", LoadSceneMode.Additive);
                if (_mouseLook != null)
                    _mouseLook.disableAnimation = false;
            }
        }

        private bool IsPuzzleComplete()
        {
            if(_selectionManager != null)
            {
                if(_selectionManager.puzzleCounter == 4)
                {
                    return true;
                }
            }
            return false;
        }

        private void CreatePath()
        {
            float pathDuration = 5f;
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
                walkingSequence.Append(_therapist.transform.DOPath(pathPositions, pathDuration, PathType.CatmullRom, PathMode.Full3D, resolution: 10).SetLookAt(0.01f).SetEase(Ease.Linear)
                    ).SetId("walking")
                    .OnComplete(() => {
                        _therapist.setAnimator(Therapist.AnimatorWays.WalkingToIdle);
                        //_zoom.enabled = false;
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
}
