using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

//using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public LevelConfigurationSO CurrentLevel;
    public static GameManager a;
    public List<Ring> rings = new List<Ring>(); 
    public Tower t1,from,to,WinTower;
    public bool Isanimate,IsGameStart;
    public Tween animations;
    public TextMeshProUGUI ErrorText,MoveText,TimeText,Score;
    public string Error;
    public float ErrorTime;
    public int Moves,Gametime=60;
    public GameObject  Gameover,Gamewin;
    public GameObject LoadingPanel;
    public Slider LoadingSlider;
    public GameObject Adsbutton,towersprefab,ringprefab;




    private void Start()
    {
        CurrentLevel = LevelManager.instance.GetCurrentLevelConfig();
        Application.targetFrameRate = 60;

        InitLevel();
        LoadingSlider.value = 0;
        LoadingSlider.maxValue = 2;

       

        IsGameStart = true;
       
        StartCoroutine(Timeing());
        MoveText.text = Moves.ToString();


        // AdManager.Instance.LoadBannerAd();
        // AdsManager.instance.LoadRewardedAd();
    
        
    }

    
    public void InitLevel()
    {
        Vector2 Startx = Vector2.zero;
        
        float rightpos =0,leftpos=0;
        List<Transform> Spownedtowers = new List<Transform>();
        for (int i = 0; i < CurrentLevel.towerCount; i++)
        {
            var tower = Instantiate(towersprefab);
            tower.transform.position = new Vector3(Startx.x, 0.7f, 0);
            tower.gameObject.name = "Tower " + (i + 1);

            if(i % 2==0)
            {
                rightpos += 3;
                Startx.x = rightpos;

            }
            else
            {
                leftpos -= 3;
                Startx.x = leftpos;
            }

            Spownedtowers.Add(tower.transform);
            
        }


       //Initilize the rings and towers based on the level configuration
        float starty = -1.15f;
        float startscale = 2.9f;
        List<GameObject> spawnedrings = new List<GameObject>();



        for (int i = 0; i < CurrentLevel.ringCount; i++)
        {
            var ring = Instantiate(ringprefab);
            Debug.LogError(Spownedtowers[CurrentLevel.Ringparentindex[i]].name);
            ring.transform.parent = Spownedtowers[CurrentLevel.Ringparentindex[i]].transform;
            ring.GetComponent<SpriteRenderer>().color = CurrentLevel.ringColors[i];
            ring.transform.localScale = new Vector3(startscale, 0.45f, 0.3f);
            ring.transform.localPosition = new Vector3(0,starty,-0.01f);
            ring.GetComponent<Ring>().size = CurrentLevel.ringCount - i;
            starty += 0.5f;
            startscale -= 0.6f;


            ring.gameObject.name = "Ring " + (i + 1);

            spawnedrings.Add(ring);

        }
        

        rings.Clear();
        for (int i = 0; i < CurrentLevel.ringCount; i++)
        {
            rings.Add(spawnedrings[i].GetComponent<Ring>());
        }

        
        for (int i = 0; i < rings.Count; i++)
        {
            Spownedtowers[0].GetComponent<Tower>().Ringstack.Push(rings[i]);

        }



        
        Moves = CurrentLevel.maxMoves;
        Gametime = CurrentLevel.timeLimit;

    }

    
    IEnumerator Timeing()
    {
        while (Gametime >= 0 &&IsGameStart)
        {
            Gametime--;
            TimeText.text = Gametime.ToString();

            if(Moves == 0 || Gametime<=0)
            {
                ///Adsbutton.SetActive(AdsManager.instance.IsRewardAdLoading());
               
                if (Moves == 0)
                {
                    Score.text = "Moves are over";
                }
                else
                {
                    Score.text = "Time is over";
                }
                    IsGameStart = false;
                Debug.Log("Gameover");
                Gameover.SetActive(true);
                break;
                
            }
            yield return new WaitForSecondsRealtime(1);
        }
    }

    public void Update()
    {
        //if (Gametime <=0 )
        //{
        //    IsGameStart = false;
        //    Debug.Log("Lose");
        //}

        if(Input.GetKeyDown(KeyCode.Escape) )
        {
            SceneManager.LoadScene(0);
        }

    }
   

    IEnumerator AnimationDelay(Ring a)
    {

        Isanimate = true;
       
        while (animations.IsPlaying())
        {
            Debug.Log(animations.IsPlaying());
            yield return new WaitForEndOfFrame();
        }

        a.transform.DOMove(new Vector3( to.transform.position.x,to.transform.position.y+3,to.transform.position.z ), 0.5f);
        yield return new WaitForSeconds(0.5f);


        a.transform.DOMoveY(to.position[to.Ringstack.Count].transform.position.y,0.5f);
        yield return new WaitForSeconds(0.5f);
        
        Moves--;
        MoveText.text = Moves.ToString();
        to.Ringstack.Push(a);


        //Handheld.Vibrate();

        if (WinTower.Ringstack.Count == rings.Count)
        {
            IsGameStart = false;
            Gamewin.SetActive(true);
            Debug.Log("Win");
            SoundManager.instance.PlaySfx("Win");
        }




            a.transform.parent = to.transform;  
        from = null;
        to = null;
        Isanimate = false;
    }


    IEnumerator Clickanimation(float time)
    {
        Isanimate = true;
        yield return new WaitForSeconds(time);
        Isanimate = false;
    }


    IEnumerator ShowError(string Error,float time)
    {
        ErrorText.text = Error;
        ErrorText.transform.GetComponent<RectTransform>().DOLocalJump(Vector3.down, 10f, 10, time);
        yield return new WaitForSeconds(time);
        ErrorText.text = "";
    }
    // public void Towerclick(Tower t)
    // {
    //     SoundManager.instance.PlaySfx("Click");
    //     if (IsGameStart)
    //     {
           

    //         if (Isanimate == true)
    //         {
    //             return;
    //         }

           
    //         if (from == null && t.Ringstack.Count > 0)
    //         {

    //             from = t;
    //             var ring = from.Ringstack.Peek();
    //             ring.Ringposition = ring.transform.position;
    //             animations = ring.transform.DOMoveY(from.transform.position.y + 3, 0.5f);

    //         }

    //         else if (from != null && from != t)
    //         {
    //             to = t;
    //             if (to.Ringstack.Count == 0)
    //             {
    //                 var a = from.Ringstack.Pop();
    //                 StartCoroutine(AnimationDelay(a));

    //                 Debug.Log("Swaped");


    //             }

    //             else if (from.Ringstack.Peek().size < to.Ringstack.Peek().size)
    //             {
    //                 var a = from.Ringstack.Pop();
    //                 StartCoroutine(AnimationDelay(a));


    //             }
    //             else
    //             {
    //                 SoundManager.instance.PlaySfx("Error");

    //                 var Ring = from.Ringstack.Peek();
    //                 Ring.transform.DOMoveY(Ring.Ringposition.y, 0.5f);
    //                 StartCoroutine(Clickanimation(0.5f));

    //                 from = null;
    //                 to = null;
    //                 Error = "Invalid Move! Cannot place a larger ring on top of a smaller ring";
    //                 ErrorTime = 2f;
                    
    //                 StartCoroutine(ShowError(Error, ErrorTime));
    //             }

    //         }
    //         else if (t.Ringstack.Count <= 0)
    //         {
    //             Debug.Log("Error");
    //             SoundManager.instance.PlaySfx("Error");


    //             Error = "Ring is not selected";
    //             ErrorTime = 1f;
    //             StartCoroutine(ShowError(Error, ErrorTime));
    //         }
    //         else
    //         {
    //             var Ring = from.Ringstack.Peek();
    //             Ring.transform.DOMoveY(Ring.Ringposition.y, 0.5f);
    //             StartCoroutine(Clickanimation(0.5f));

    //             from = null;
    //             to = null;
    //             Debug.Log("Wrong");
    //         }

            

    //     }
    // }

    public void Restart()
    {
        SoundManager.instance.PlaySfx("Click");
        SceneManager.LoadScene(PlayerPrefs.GetInt("Current scene"));
    }

    public void Home()
    {
        SoundManager.instance.PlaySfx("Click");

        LoadingPanel.SetActive(true);
        StartCoroutine(AnimateLoading(0));
    }

    public void NextLevel()
    {
        SoundManager.instance.PlaySfx("Click");

        LoadingPanel.SetActive(true);
        StartCoroutine(AnimateLoading(PlayerPrefs.GetInt("Current scene")+1));
    }


    IEnumerator AnimateLoading(int Scene)
    {

        while (LoadingSlider.value != 2)
        {

            LoadingSlider.value += Time.deltaTime;
            yield return new WaitForEndOfFrame();

        }
        SceneManager.LoadScene(Scene);

    }

    public void Continue()
    {
        //AdsManager.instance.ShowRewardedAd();
        //AdsManager.RewardAdAction += RewardOfAds;
        //RewardOfAds();
    }

    public void RewardOfAds()
    {

        Debug.LogError("Rewarded ad full screen content closed.");
        if (Moves==0 && Gametime==0)
        {
            Moves = 5;
            MoveText.text = Moves.ToString();
            Gametime = 30;
            TimeText.text = Gametime.ToString();
        }
        if(Moves==0)
        {
            Moves = 5;
            MoveText.text = Moves.ToString();

        }
         if(Gametime==-1)
        {
            Gametime = 30;
            TimeText.text = Gametime.ToString();

        }
        IsGameStart = true;
        StartCoroutine(Timeing());
     
        Gameover.SetActive(false);
        
    }
}
