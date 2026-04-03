using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using TMPro;

public class GameManager : MonoBehaviour, IDataPersistence
{
    public bool paused;

    public VideoPlayer vp;
    public Slider loadBar;

    [Header("Managers")]
    public static GameManager instance;
    public LocalManager localManager;
    public InputManager inputManager;
    public AudioManager audioManager;
    public DataPersistenceManager dataManager;

    [Header("Collectibles/Game booleans")]
    public string[] collectibleIDs;
    public int collectiblesCollected = 0;
    public bool canMakeBomb = false;
    public bool gameOver = false;
    private bool curCollected = false;

    [Header("Settings")]
    public bool fullScreen;
    public bool vSync;
    public Resolution curRes;
    public bool subtitles;
    public int subtitleSize;
    public string language;
  


    private void OnEnable () {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void Awake() {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(instance.gameObject);
        print(SceneManager.GetActiveScene().name);

        inputManager = GetComponent<InputManager>();
        audioManager = GetComponentInChildren<AudioManager>();
    }
    
    private void Update() {
        if(collectiblesCollected == collectibleIDs.Length) {
            canMakeBomb = true;
        }
    }

    public void LoadNextScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    } 

    public void LoadData(GameData data) {
        this.fullScreen = data.fullScreen;
        this.vSync = data.vSync;
        this.subtitles = data.subtitles;
        this.subtitleSize = data.subtitleSize;
        this.language = data.language;

        collectiblesCollected = 0;
        for(int i = 0; i < collectibleIDs.Length; i++) {
            data.collectiblesCollected.TryGetValue(collectibleIDs[i], out curCollected);
            if(curCollected) {
                collectiblesCollected++;  
            } 
        }
    }

    public void SaveData(ref GameData data) {
        data.fullScreen = this.fullScreen;
        data.vSync = this.vSync;
        data.subtitles = this.subtitles;
        data.subtitleSize = this.subtitleSize;
        data.language = this.language;


        if(gameOver) {
            
            for(int i = 0; i < collectibleIDs.Length; i++) {
                if(data.collectiblesCollected.ContainsKey(collectibleIDs[i])) {
                    data.collectiblesCollected.Remove(collectibleIDs[i]);
                }
                    data.collectiblesCollected.Add(collectibleIDs[i], false);
            }
        }
        
    }

    public void ReturnToMenu() {
        SceneManager.LoadScene("Main Menu");
    }

    public void GameOver() {
        if(collectiblesCollected == 0) {
            return;
        }

        gameOver = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Game Over");
        audioManager.StopAllSounds();
    }

    public void Win() {
        audioManager.StopAllSounds();
        SceneManager.LoadScene("Outro");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        audioManager.OnSceneLoaded();

        string curScene = SceneManager.GetActiveScene().name;
        vp = FindObjectOfType<VideoPlayer>();
        if(vp != null) {
            vp.loopPointReached += VideoFinished;
        }

        if(curScene == "Loading Screen") {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        if(curScene == "Intro") {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            //finds all texts & sets it to the chosen size
            TextMeshProUGUI[] texts = FindObjectsOfType<TextMeshProUGUI>();
            Debug.Log(texts.Length);
            for(int i = 0; i < texts.Length; i++) {
                texts[i].fontSize = subtitleSize;
            }

            GameObject englishSub = GameObject.Find("English Subtitles");
            GameObject dutchSub = GameObject.Find("Dutch Subtitles");
            if(language == "English") {
                englishSub.SetActive(true);
                dutchSub.SetActive(false);
            } else if(language == "Nederlands") {
                englishSub.SetActive(false);
                dutchSub.SetActive(true);
            }
        }

        

        if(curScene == "Game Over") {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if(curScene == "Outro") {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            //finds all texts & sets it to the chosen size
            TextMeshProUGUI[] texts = FindObjectsOfType<TextMeshProUGUI>();
            for(int i = 0; i < texts.Length; i++) {
                texts[i].fontSize = subtitleSize;
            }

            GameObject englishSub = GameObject.Find("English Subtitles");
            GameObject dutchSub = GameObject.Find("Dutch Subtitles");
            if(language == "English") {
                englishSub.SetActive(true);
                dutchSub.SetActive(false);
            } else if(language == "Dutch") {
                englishSub.SetActive(false);
                dutchSub.SetActive(true);
            }
        }
    } 

    private void OnSceneUnloaded(Scene scene) {

    }

    private void VideoFinished(VideoPlayer vp) {
        Debug.Log("Video Ended");
        if(SceneManager.GetActiveScene().name == "Intro") {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);    
        }
    }
}
