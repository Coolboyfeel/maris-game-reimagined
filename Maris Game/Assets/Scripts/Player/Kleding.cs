using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kleding : MonoBehaviour
{
    public bool mondkapjeOp = false;
    public bool jasAan = false;
    public bool jasMode = false;
    public bool mondkapjeMode = true;
    public GameObject mondkapje;
    public GameObject jas;
    public Animator anim;
    
    private GameManager gameManager;
    private InputManager inputManager;
    


    private void Start() {
        gameManager = GameManager.instance;
        inputManager = GameManager.instance.inputManager;
    }
    private void Update() {
        MondKapje();
        Jas();

        if(Input.GetKeyDown(inputManager.switchClothKey)) {
            if(mondkapjeMode) {
                mondkapjeMode = false;
                jasMode = true;
            } else {
                mondkapjeMode = true;
                jasMode = false;
            }
        }
    }

    private void MondKapje() {
        if(!mondkapjeMode) {
            return;
        }

        if(Input.GetKeyDown(inputManager.clothesOnKey)) {
            GameManager.instance.audioManager.Play("ClothesOn");
            
            if(!mondkapjeOp) {
                mondkapjeOp = true;
                mondkapje.transform.localPosition = new Vector3(0, -0.392f, 0.479f);
                mondkapje.transform.localRotation = new Quaternion(17.506f, 0f, 0f, mondkapje.transform.localRotation.w);
            }
            else { 
                mondkapjeOp = false;
                mondkapje.transform.localPosition = new Vector3(0, -0.697f, 0.485f);
                mondkapje.transform.localRotation = new Quaternion(17.506f, 0f, 0f, mondkapje.transform.localRotation.w);
                
            }
            //StartCoroutine("mondkapjeToggle");
        }
    }

    private void Jas() {
        if(!jasMode) {
            return;
        }

        if(Input.GetKeyDown(inputManager.clothesOnKey)) {
            GameManager.instance.audioManager.Play("ClothesOn");
            if(!jasAan) {
                jasAan = true;
                anim.Play("JasAan", 2);
            }
            else { 
                jasAan = false;
                anim.Play("JasUit", 2);
            }
        }
    }

    IEnumerator mondkapjeToggle() {
        Vector3 onPos = new Vector3(0, -0.392f, 0.479f);
        Quaternion onRot = new Quaternion(0, 0, 0, mondkapje.transform.localRotation.w);
        Vector3 offPos = new Vector3(0, -0.697f, 0.485f);
        Quaternion offRot = new Quaternion(17.506f, 0f, 0f, mondkapje.transform.localRotation.w);

        float time = 0;
        float maxTime = 2f;
        while (time < maxTime) {
            if(!mondkapjeOp) {
                mondkapje.transform.localPosition = Vector3.Lerp(offPos, onPos, time / maxTime);
                mondkapje.transform.localRotation = Quaternion.Lerp(offRot, onRot, time / maxTime);    
            } else {
                mondkapje.transform.localPosition = Vector3.Lerp(onPos, offPos, time / maxTime);
                mondkapje.transform.localRotation = Quaternion.Lerp(onRot, offRot, time / maxTime); 
            }
            time += Time.deltaTime;
        }
        if(!mondkapjeOp) {mondkapjeOp = true;}
        else {mondkapjeOp = false;}

        yield return null; 
    }


}
