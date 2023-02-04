using DG.Tweening;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private string selectableTag = "Selectable";
    [SerializeField] private Material defaultMaterial;

    [SerializeField] private AudioClip _wrongChoice;
    [SerializeField] private AudioClip _rightChoice;
    [SerializeField] private AudioClip _rotateSpider;

    private AudioSource _therapistAudioSource;
    private AudioSource _robottinoAudioSource;
    private AudioSource _playerAudioSource;

    private Transform _selection;
    
    private float speed = 10;
    private bool grab = false;
    private Vector3 mOffset;
    private float mZCoord;

    private Outline outline;
    public float duration;

    public int puzzleCounter = 0;

    private void Start()
    {
        var therapist = GameObject.FindObjectOfType<Therapist>();
        if(therapist != null)
        {
            _therapistAudioSource = therapist.GetComponent<AudioSource>();
        }
        var robottino = GameObject.FindObjectOfType<Robottino>();
        {
            if(robottino != null)
        {
            _robottinoAudioSource = robottino.GetComponent<AudioSource>();
        }
        }

        _playerAudioSource = this.gameObject.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (_selection != null)
            {
                grab = true;
                
                Vector3 destination = new Vector3(GetMouseAsWorldPoint().x + mOffset.x, _selection.position.y, GetMouseAsWorldPoint().z + mOffset.z);
                _selection.position = Vector3.Lerp(_selection.position, destination, speed * Time.deltaTime);
                outline = _selection.GetComponent<Outline>();
                if (outline != null)
                {
                    outline.enabled = true;
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (_selection != null)
            {
                mZCoord = Camera.main.WorldToScreenPoint(_selection.position).z;
                // Store offset = gameobject world pos - mouse world pos
                mOffset = _selection.position - GetMouseAsWorldPoint();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if(_selection != null)
            {
                grab = false;
                
                var direction = GameObject.FindObjectOfType<SpiderPlatform>();
                if (direction != null)
                {
                    if (_selection.gameObject.GetComponent<PlasticSpider>().chosen == false || 
                        Mathf.Abs(_selection.rotation.z - direction.transform.rotation.z) > 0.01f ||
                        _selection.gameObject.GetComponent<PlasticSpider>().RightPosition == false)
                    {
                        //If the plastic spider is not the chosen one it goes back to original position
                        if (!_robottinoAudioSource.isPlaying)
                        {
                            PlaySound(_therapistAudioSource, _wrongChoice);
                        }
                        _selection.DOMove(_selection.gameObject.GetComponent<PlasticSpider>().StartPosition.position, duration);
                    }
                }
                
                outline = _selection.GetComponent<Outline>();
                if (outline != null)
                {
                    outline.enabled = false;
                }

                if (_selection.gameObject.GetComponent<PlasticSpider>().RightPosition == true)
                {
                    puzzleCounter++;
                    if (puzzleCounter < 4)
                    {
                        if (!_robottinoAudioSource.isPlaying)
                        {
                            PlaySound(_therapistAudioSource, _rightChoice);
                        }
                    }
                }

                _selection = null;
            }
        }

        if (_selection != null)
        {
            var selectionRenderer = _selection.GetComponent<Renderer>();
            if (selectionRenderer != null)
            {
                selectionRenderer.material = defaultMaterial;
            }
            if (grab == false)
            {
                outline = _selection.GetComponent<Outline>();
                if (outline != null)
                {
                    outline.enabled = false;
                }
            }
        }
        
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray);
        foreach (RaycastHit hit in hits)
        {
            var selection = hit.transform;
            if (selection.CompareTag(selectableTag))
            {
                if(grab == false)
                {
                    _selection = selection;
                    outline = _selection.GetComponent<Outline>();
                    if (outline != null)
                    {
                        outline.enabled = true;
                    }
                }
            }
        }

        if (grab)
        {
            if (_selection.GetComponent<PlasticSpider>().Rotate)
            {
                //Rotate to the right
                if (Input.GetKeyUp(KeyCode.E))
                {
                    PlaySound(_playerAudioSource, _rotateSpider);
                    _selection.DOLocalRotate(new Vector3(0, 0, 90), 0.1f).SetRelative();
                }

                //Rotate to the left
                if (Input.GetKeyUp(KeyCode.Q))
                {
                    PlaySound(_playerAudioSource, _rotateSpider);
                    _selection.DOLocalRotate(new Vector3(0, 0, -90), 0.1f).SetRelative();
                }
            }
        }
    }

    private Vector3 GetMouseAsWorldPoint()

    {
        Vector3 mousePoint = Input.mousePosition;
        // z coordinate of game object on screen
        mousePoint.z = mZCoord;
        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void PlaySound(AudioSource source, AudioClip clip)
    {
        if (source != null && !source.isPlaying)
        {
            source.clip = clip;
            source.Play();
        }
    }
}