using UnityEngine;
 
public class LipSync : MonoBehaviour {
 
    public AudioSource audioSource;
    public Animator therapistAnimator;
    public float updateStep = 0.1f;
    public int sampleDataLength = 1024;
 
    private float currentUpdateTime = 0f;
 
    private float clipLoudness;
    private float prevClipLoudness = 0f;
    private float nextClipLoudness = 0f;
    private float[] clipSampleData;

    // Use this for initialization
    void Awake ()
    {

        audioSource = this.gameObject.GetComponent<AudioSource>();
        therapistAnimator = this.gameObject.GetComponent<Animator>();
     
        if (!audioSource) {
            Debug.LogError(GetType() + ".Awake: there was no audioSource set.");
        }
        clipSampleData = new float[sampleDataLength];
 
    }
     
    // Update is called once per frame
    void Update () {
        if (audioSource.timeSamples < audioSource.clip.samples)
        {
             currentUpdateTime += Time.deltaTime;
                    if (currentUpdateTime >= updateStep)
                    {
                        currentUpdateTime = 0f;
                        audioSource.clip.GetData(clipSampleData, audioSource.timeSamples); //I read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.
                        prevClipLoudness = 0f;
                        foreach (var sample in clipSampleData) {
                            prevClipLoudness += Mathf.Abs(sample);
                        }
                        prevClipLoudness /= sampleDataLength;
                        
                        audioSource.clip.GetData(clipSampleData, audioSource.timeSamples+1024); //I read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.
                        nextClipLoudness = 0f;
                        foreach (var sample in clipSampleData) {
                            nextClipLoudness += Mathf.Abs(sample);
                        }
                        nextClipLoudness /= sampleDataLength;
                    }

                    Mathf.SmoothStep(0.0f, 1.0f, currentUpdateTime * 10);
                    clipLoudness = Mathf.Lerp(prevClipLoudness, nextClipLoudness, currentUpdateTime);
                    therapistAnimator.SetLayerWeight(1, clipLoudness*4);
        }
    }
}