using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class MainScipt : MonoBehaviour
{
    // Start is called before the first frame update


  //  [SerializeField]
    public GameObject LoadingPanel;
    public Slider LoadingSlider;
    public Slider sfxslider, Bgslider;


    public void Start()
    {
      
        Application.targetFrameRate = 60;

        LoadingSlider.value = 0;
        LoadingSlider.maxValue = 2;
        //StartCoroutine(ShowAds());

        

        //sfxslider.value = PlayerPrefs.GetFloat("SFX", 1);
        SoundManager.instance.SfxSource.volume = sfxslider.value;

        //Bgslider.value = PlayerPrefs.GetFloat("BG", 1);
        SoundManager.instance.bg.volume = Bgslider.value;

        //sfxslider.onValueChanged.AddListener(Setsfxsound);
        //Bgslider.onValueChanged.AddListener(Setbgsound);

        

       

    }

    //public void Setbgsound(float volume)
    //{
    //    SoundManager.instance.bg.volume = volume;
    //    PlayerPrefs.SetFloat("BG", volume);
    //}

    //public void Setsfxsound(float volume)
    //{
    //    SoundManager.instance.SfxSource.volume = volume;
    //    PlayerPrefs.SetFloat("SFX", volume);
    //}

    public void ShowAds()
    {
        
        // if (AdManager.Instance.IsLoading())
        // {
        //     AdManager.Instance.ShowInterstitialAd();
        //     AdManager.IntertitialAction += loadScene;

        // }
        // else
        // {
        //     StartCoroutine(AnimateLoading());
        // }


    }



    public void loadScene()
    {
        StartCoroutine(AnimateLoading());
        //AdManager.IntertitialAction -= loadScene;  
    }
          
    public void play()
    {
        SoundManager.instance.PlaySfx("Click");

    }

    public void ClickLevel(int level)
    {
        SoundManager.instance.PlaySfx("Click");
        LevelManager.instance.SetCurrentLevel(level);
        PlayerPrefs.SetInt("Current scene", level);

        StartCoroutine(AnimateLoading());
        ShowAds();

    }

   
    IEnumerator AnimateLoading()
    {
        LoadingPanel.SetActive(true);

        while (LoadingSlider.value != 2)
        {

            LoadingSlider.value += Time.deltaTime;
            yield return new WaitForEndOfFrame();

        }
       

        SceneManager.LoadScene(PlayerPrefs.GetInt("Current scene"));
    }
}
