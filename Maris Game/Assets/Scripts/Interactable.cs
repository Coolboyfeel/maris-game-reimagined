using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour, IDataPersistence
{
    public string[] interactSort = new string[] {"Collectible", "Bomb", "Door"};
    public Transform[] spawnPoints;
    public int arrayIndex = 0;
    public string collectibleName;

    [Header("Debug")]
    public bool collected = false;
    public bool canOpen = false;
    
    protected  string collectibleID {get; private set; }

    [ContextMenu("Generate ID for collectible")]
    private void GenerateGuid() {
        collectibleID = System.Guid.NewGuid().ToString();
    }

    private void Awake() {
        if(this.interactSort[arrayIndex] == "Collectible" && collectibleName != "secret") {
            int index = Mathf.RoundToInt(Random.Range(0f, spawnPoints.Length - 1));
            this.transform.position = spawnPoints[index].position;
            this.transform.rotation = spawnPoints[index].rotation;
        }
    }

    public void Update() {
    }


    public void LoadData(GameData data) {
        Debug.Log("Interactable Data Loaded)");
        data.collectiblesCollected.TryGetValue(this.collectibleID, out collected);
        if(collected) {
            this.gameObject.SetActive(false);
        }
    }


     
    public void SaveData(ref GameData data) {
        if(data.collectiblesCollected.ContainsKey(collectibleID)) {
            data.collectiblesCollected.Remove(collectibleID);
        }

        if(this.interactSort[arrayIndex] == "Collectible") {
            data.collectiblesCollected.Add(collectibleID, this.collected);
        }
    }

    public void Interacted() {
        if(this.interactSort[arrayIndex] == "Collectible") {
            GameManager.instance.audioManager.PlaySound("Item Collected");
            this.collected = true;
            this.gameObject.SetActive(false);
            if(this.collectibleName != "secret") {
                GameManager.instance.collectiblesCollected++;
                if(GameManager.instance.collectiblesCollected == 3) {
                    GameManager.instance.audioManager.PlaySound("Laugh");
                }
            }
            
        } else if(this.interactSort[arrayIndex] == "Door") {
            this.GetComponent<Door>().Interacted();
        }
    }

}
