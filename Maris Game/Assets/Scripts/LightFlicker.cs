using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class LightFlicker : MonoBehaviour
{
	public AudioSource audioSource;
	public Light curLight;
	public float minWaitTime = 0.1f;
	public float maxWaitTime = 0.5f;
	public Material[] curMats;
	public float[] minWaitTimes = {2f, 1f, 0.5f, 0.1f};
	public float[] maxWaitTimes = {3f, 2f, 1f, 0.5f};
	public bool flash;
	public bool playingSound;
	public Material lightOnMat;
	public Material lightOfMat;
	public Renderer renderer;
	private int collected;

	private GameManager gameM;
	private AudioManager audioM;
	
	
	void Start () {
		renderer = GetComponentInParent<Renderer>();
		audioSource = GetComponent<AudioSource>();
		curLight = GetComponent<Light>();
		StartCoroutine(Flashing());
		
		gameM = GameManager.instance;
		audioM = gameM.audioManager;
	}
	
	void Update() {
		int collected = gameM.collectiblesCollected;
		
		if(collected <= 3) {
			minWaitTime = minWaitTimes[GameManager.instance.collectiblesCollected];
			maxWaitTime = maxWaitTimes[GameManager.instance.collectiblesCollected];
		} else {
			minWaitTime = minWaitTimes[3];
			maxWaitTime = maxWaitTimes[3];
		}

		
		switch (collected) {
			case 0:
				audioM.Play("Lights1", audioSource);
				break;
			case 1: 
				audioM.Play("Lights2", audioSource);
				break;
			case 2: 
				audioM.Play("Lights3", audioSource);
				break;
			case 3: 
				audioM.Play("Lights4", audioSource);
				break;
			}	
		if(collected != gameM.collectiblesCollected) {
			audioSource.Stop();
		}

		curMats = renderer.materials;
		if(curLight.enabled) {
			if(renderer.materials[0] == lightOfMat) {
				//curMats[0] = lightOnMat;
				//renderer.materials = curMats;
			}
		} else if(!curLight.enabled) {
			if(renderer.materials[0] == lightOnMat) {
				//curMats[0] = lightOfMat;
				//renderer.materials = curMats;
			}
		}
		
	}
	IEnumerator Flashing ()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(minWaitTime,maxWaitTime));
			curLight.enabled = ! curLight.enabled;
		}
	}
}
