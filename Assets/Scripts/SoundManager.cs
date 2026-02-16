using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public List<AudioClip> SfxClips;
    public AudioSource SfxSource,bg;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySfx(string clipname)
    {

        var clip = SfxClips.Find(a => a.name == clipname);
        if (clip)
        {
            SfxSource.PlayOneShot(clip);
            Debug.Log("clip found :" + clipname);

        }
       
        else
        {
            Debug.LogError("clip not found :" + clipname);
        }
     
    }
}
