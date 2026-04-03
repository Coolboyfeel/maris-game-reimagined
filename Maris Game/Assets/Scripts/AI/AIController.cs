using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class AIController : MonoBehaviour
{
    public float visionDistance;
    private AudioSource audioSource;
    public AudioSource walkingAudio;
    
    private GameObject player;
    private Kleding kledingScript;
    private NavMeshAgent agent;
    private SphereCollider sc;
    private Patrol patrol;
    

    [Header("AI settings")]
    public bool chase;
    public bool chasing;
    public bool seen;
    public bool inSight;
    public bool playedSound;
    public float yOffset; 
    public Vector3 rayStartPos;
    public Vector3 rayDirection;
    public float fov;
    public bool walking = false;

    [Header("Audio")]
    public string[] normalSounds;
    public string[] kapjeSounds;
    public string[] jasSounds;
    public string[] angrySounds;
    public Subtitles subtitles;

    public Sound[] normalSoundss;
    public Sound[] kapjeSoundss;
    public Sound[] jasSoundss;
    public Sound[] angrySoundss;

    //patrol
    [Header("Patrol")]
    public float walkRange;

    [Header("Turning variables")]
    public float turnSpeed = 1f;
    private Coroutine lookCoroutine;
    private float curTurnTime = 0;


    [Header("Hearing debug")]
    public bool hearingPlayer = false;

    public bool canMove = true;
    
    private GameManager gameM;
    private AudioManager audioM;


    private void Start() {
        audioSource = GetComponent<AudioSource>();
        patrol = GetComponent<Patrol>();
        sc = GetComponent<SphereCollider>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        kledingScript = GameObject.Find("Camera Holder").transform.GetChild(0).GetComponent<Kleding>();

        gameM = GameManager.instance;
        audioM = gameM.audioManager;
    }

    private void Update() {
        if(walking) {

            if(!this.walkingAudio.isPlaying) {
                AudioClip clip = audioM.FindClip("Walking");
                this.walkingAudio.clip = clip;
                this.walkingAudio.Play();
            }
            
        } else if (gameM.paused) {
            this.walkingAudio.Stop(); 
        } else {
            this.walkingAudio.Stop();
        }
        

        if(!canMove) { 
            walking = false;
            return; 
        }
        
        rayStartPos = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
        rayDirection = player.transform.position - this.transform.position;

        RaycastHit hitInfo;
        
        //if the player is in the fov of the AI turn towards the player
        if(Vector3.Angle(rayDirection, transform.forward) < fov / 2) {
            if(Physics.Raycast (transform.position, rayDirection, out hitInfo, visionDistance)) {
                if(hitInfo.transform.tag == "Player") {
                    if(gameM.collectiblesCollected == 0) {
                        chasing = false;
                        inSight = true;
                    } else if(gameM.collectiblesCollected == 1) {
                        if(!kledingScript.mondkapjeOp && chase) {
                            chasing = true;
                            seen = true;
                            inSight = true;
                        } 
                    } else if(gameM.collectiblesCollected == 2) {
                        //if the player doesnt have their face mask on or have their jacket on, chase the player 
                        if(!kledingScript.mondkapjeOp && chase || kledingScript.jasAan && chase) {
                            chasing = true;
                            seen = true;
                            inSight = true;
                        } 
                    } else if(gameM.collectiblesCollected >= 3) {
                        patrol.enabled = false;
                        chasing = true;
                        inSight = true;

                    }
                    
                    Debug.DrawLine(rayStartPos, hitInfo.transform.position, Color.red);
                } else {
                    inSight = false;
                    chasing = false;
                    seen = false;
                    Debug.DrawLine(rayStartPos, hitInfo.transform.position, Color.yellow);
                    patrol.DoPatrol();
                }   
            } else if(GameManager.instance.collectiblesCollected >= 3) {
                patrol.enabled = false;
                chasing = true;
                seen = true;
                inSight = true;

            } else {
                patrol.DoPatrol();
            }        

            if(inSight || seen) {
                RotateToPlayer();
            }

            if(chasing || seen) {
                walking = true;
                Chase();
            }

            
        } else {
            patrol.DoPatrol();
        }
        

        if(gameM.collectiblesCollected == 3) {
            agent.speed = 8f;
        }
    }

    public void RotateToPlayer() {
        //makes the variable target to the player position and sets the y cordinate to 0
        Vector3 target = player.transform.position;
        target.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(target - this.transform.position);

        //if time < 1 then turn smoothly towards the target
        if(curTurnTime < 1) {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, curTurnTime);

            curTurnTime += Time.deltaTime * turnSpeed;
        } else {
            curTurnTime = 0;
        }

    }

    private void Chase() {
        if(!canMove) {
            return;
        }
        
        agent.SetDestination(player.transform.position);
    }

    IEnumerator GameOver() {
        GetComponent<CapsuleCollider>().enabled = false;
        agent.enabled = false;

        float speed = 1f;
        float time = 0f;
        float duration = 1f;
        Quaternion previousRot = transform.rotation;
        Vector3 orgPos = transform.position;
        while (time < duration) {
            time += Time.deltaTime; 
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, new Quaternion(12.85f, previousRot.y, previousRot.z, previousRot.w), speed * Time.deltaTime);
            this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(orgPos.x, 0.92f, orgPos.z), speed * Time.deltaTime);
            yield return null;
        }

        
    }

    public void PlayRandomSound() {
        if(audioSource.isPlaying) {
            return;
        }
        string curName = "";
        Debug.Log("PLay Random Sound");
        int randomNum = Mathf.RoundToInt(Random.Range(0f, 1f));
        int collected = gameM.collectiblesCollected;

        if(collected == 0) {
            int index = Mathf.RoundToInt(Random.Range(0f, 1f));
            curName = normalSounds[index];    
        }
        
        else if(collected == 1) {
            if(!kledingScript.mondkapjeOp && randomNum == 1) {   
                curName = kapjeSounds[0]; 
            } else if(kledingScript.jasAan && randomNum == 1) {
                curName = jasSounds[0];
            } else {
                curName = normalSounds[2];
            }
        } 
        
        else if(collected == 2) {
            if(!kledingScript.mondkapjeOp && randomNum == 1) {
                curName = kapjeSounds[1];
            } else if(kledingScript.jasAan && randomNum == 1) {
                curName = jasSounds[1];
            } else {
                curName = normalSounds[3];
            }
        } 
        
        else if(collected >= 3) {
            int index = Mathf.RoundToInt(Random.Range(0f, 2f));
            curName = angrySounds[index];
        }

        AudioClip clip = audioM.FindClip(curName);
        SubtitleObject subtitle = audioM.FindSubtitle(curName);
        this.audioSource.clip = clip;
        this.audioSource.Play();
        if(subtitle != null) {
            subtitles.SetSubtitle(subtitle.subtitle);
        }
    }
}
