using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    

    [Header("Movement variables")]
    public float speed = 12f;
    public float sprintMultiplier;

    public float gravity = -9.81f;
    public float groundDistance = 0.4f;

    public Transform groundCheck;
    
    public LayerMask groundMask;
    

    private Vector3 velocity;
    private bool grounded;
    public bool walking = false;
    public bool sprinting = false;
    public bool canMove = true;
    
    private InputManager inputM;
    private GameManager gameM;

    [Header("Audio")]
    public AudioSource audioSource;
    private AudioManager audioM;

    private CapsuleCollider cc;
    public Kleding kledingScript;
    public Camera mainCam;
    public Camera sceneCam;

    [Header("Win Animation")]
    public Vector3 winPosition;
    public Quaternion winRotation;
    public float zoomSpeed;
    public float rotateSpeed;
    public float moveSpeed;
    public float duration;
    public bool won = false;
    public bool gameOver = false;

    private void Start() {
        cc = GetComponent<CapsuleCollider>();
        audioSource = GetComponent<AudioSource>();

        //Managers
        gameM = GameManager.instance;
        inputM = GameManager.instance.inputManager;
        audioM = GameManager.instance.audioManager;
    }

    private void Update() {
        if(won) {
            if(transform.position == winPosition 
                && transform.rotation == winRotation 
                && sceneCam.fieldOfView == 39.6f) {
                GameManager.instance.Win();
            }
        }
        
        if(!canMove) {
            return;
        }

        
        float x = 0f;
        float z = 0f;
        //Vertical Inputs
        if(Input.GetKey(inputM.forwardKey)) {
            z = 1f;
            
        } else if(Input.GetKey(inputM.backwardKey)) {
            z = -1f;

        }
        //Horizontal Inputs
        if(Input.GetKey(inputM.rightKey)) {
            x = 1f;
        }
        else if(Input.GetKey(inputM.leftKey)) {
            x = -1f;
            
        } 

        Vector3 move = transform.right * x + transform.forward * z;

        if(Input.GetKey(inputM.sprintKey)) {
            if(kledingScript.mondkapjeOp) {
                return;
            }

            sprinting = true;
            move *= sprintMultiplier;
        } else {
            sprinting = false;
        }
        controller.Move(move * speed * Time.deltaTime);


        if((x != 0 || z != 0) && !sprinting) {
            audioM.Stop("Sprinting", audioSource);

            if(!audioSource.isPlaying) {
               audioM.Play("Walking", audioSource);
            }
            
        } else if((x != 0 || z != 0) && sprinting) {
            audioM.Stop("Walking", audioSource);

            if(!audioSource.isPlaying) {
               audioM.Play("Sprinting", audioSource);
            }  
        } else {
            audioM.Stop(audioSource);
        }

        if(walking && sprinting) {
            audioM.Stop("Sprinting", audioSource);

            if(!audioSource.isPlaying) {
               audioM.Play("Walking", audioSource);
            }
        }        
    }

    IEnumerator Win() {
        cc.enabled = false;
        controller.enabled = false;
        won = true;
        canMove = false;
        Vector3 orgPos = transform.position;
        Quaternion orgRot = transform.rotation;
        float dur = Vector3.Distance(transform.position, winPosition) / duration; 
        float time = 0;
        
        while (time < duration) {
            time += Time.deltaTime;
            this.transform.position = Vector3.MoveTowards(transform.position, winPosition, moveSpeed * Time.deltaTime);
            this.transform.rotation = Quaternion.RotateTowards(transform.rotation, winRotation, rotateSpeed * Time.deltaTime);
            sceneCam.fieldOfView = Mathf.MoveTowards(sceneCam.fieldOfView, 39.6f, zoomSpeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator GameOver() {
        Vector3 orgPos = transform.position;
        mainCam.GetComponent<MouseLook>().GameOver();
        cc.enabled = false;
        controller.enabled = false;
        gameOver = true;
        canMove = false;
        float time = 0f;
        float duration = 10f;

        while (time < duration) {
            time += Time.deltaTime;
            this.transform.position = orgPos;
            yield return null;
        }

    }

    private void FixedUpdate() {
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(grounded && velocity.y < 0) {
            velocity.y = 0f;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity);
    }
}
