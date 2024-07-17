using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Player player;

    public bool isSafe;
    public bool alarm;
    public Transform alarmPosition; //알람을 울린 경비, cctv의 위치 저장용

    public bool gameOver;
    public GameObject overText;

    public bool gameClear;
    public GameObject clearText;

    public bool hasBkey;
    public bool hasGkey;
    public bool hasRkey;

    public GameObject BKey;
    public GameObject GKey;
    public GameObject RKey;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            Time.timeScale = 0;
            overText.SetActive(true);
            if(Input.GetKey(KeyCode.R))
            {
                Time.timeScale = 1;
                overText.SetActive(false);
                gameOver = false;
                SceneManager.LoadScene(0);
            }
        }
        if (gameClear)
        {
            Time.timeScale = 0;
            clearText.SetActive(true);
            if (Input.GetKey(KeyCode.R))
            {
                Time.timeScale = 1;
                clearText.SetActive(false);
                gameClear= false;
                SceneManager.LoadScene(0);
            }
        }

        if (hasBkey)
        {
            BKey.SetActive(true);
        }
        if (hasGkey)
        {
            GKey.SetActive(true);
        }
        if (hasRkey)
        {
            RKey.SetActive(true);
        }
    }
}
