﻿using System;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
	private void Start()
	{
		this.myAudio = base.GetComponent<AudioSource>();
	}

	private void Update()
	{
		if (this.lockTime > 0f) // If the lock time is greater then 0, decrease lockTime
			this.lockTime -= 1f * Time.deltaTime;

		else if (this.bDoorLocked) //If the door is locked, unlock it
			this.UnlockDoor();

		if (this.openTime > 0f) // If the open time is greater then 0, decrease lockTime Decrease open time
			this.openTime -= 1f * Time.deltaTime;

		if (this.openTime <= 0f & this.bDoorOpen)
		{
			this.barrier.enabled = true; // Turn on collision
			this.invisibleBarrier.enabled = true; //Enable the invisible barrier
			this.bDoorOpen = false; //Set the door open status to false
			this.inside.material = this.closed; // Change one side of the door to the closed material
			
			this.outside.material = this.closed; // Change the other side of the door to the closed material
            if (this.silentOpens <= 0) //If the door isn't silent
				this.myAudio.PlayOneShot(this.doorClose, 1f); //Play the door close sound
		}
		if (Input.GetMouseButtonDown(0) && Time.timeScale != 0f) //If the door is left clicked and the game isn't paused
		{
			Ray ray = Camera.main.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit) && (raycastHit.collider == this.trigger & Vector3.Distance(this.player.position, base.transform.position) < this.openingDistance & !this.bDoorLocked))
			{
				if (this.baldi.isActiveAndEnabled & this.silentOpens <= 0)
					this.baldi.AddNewSound(base.transform.position, 1);
				
				if (this.sweepDoor)
					this.sweepScript.EarlyActivate();

				this.OpenDoor(3f);

				if (this.silentOpens > 0) //If the door is silent
					this.silentOpens--; //Decrease the amount of opens the door will stay quite for.
			}
		}
	}

	public void OpenDoor(float time)
	{
		if (this.silentOpens <= 0 && !this.bDoorOpen) //Play the door sound if the door isn't silent
		{
			this.myAudio.PlayOneShot(this.doorOpen, 1f);
		}
		this.barrier.enabled = false; //Disable the Barrier
		this.invisibleBarrier.enabled = false;//Disable the invisible Barrier
		this.bDoorOpen = true; //Set the door open status to false
		this.inside.material = this.open; //Change one side of the door to the open material
		this.outside.material = this.open; //Change the other side of the door to the open material
        this.openTime = time; //Set the open time to 3 seconds
	}

	private void OnTriggerStay(Collider other)
	{
		if (!this.bDoorLocked && other.gameObject.name == "NullDoorCollider")
			this.OpenDoor(0.5f);
		else if (!this.bDoorLocked && other.CompareTag("NPC"))
			this.OpenDoor(3f);
	}

	public void LockDoor(float time) //Lock the door for a specified amount of time
	{
		this.bDoorLocked = true;
		this.lockTime = time;
	}

	public void UnlockDoor() //Unlock the door
	{
		this.bDoorLocked = false;
	}

	public bool DoorLocked
	{
		get
		{
			return this.bDoorLocked;
		}
	}

	public void SilenceDoor() //Set the amount of times the door can be open silently
	{
		this.silentOpens += 4;
	}

	public float openingDistance;
	public Transform player;
	public BaldiScript baldi;
	[SerializeField] private SweepScript sweepScript;
	public MeshCollider barrier;
	public MeshCollider trigger;
	public MeshCollider invisibleBarrier;
	public MeshRenderer inside;
	public MeshRenderer outside;
	public AudioClip doorOpen;
	public AudioClip doorClose;
	public Material closed;
	public Material open;
	private bool bDoorOpen;
	private bool bDoorLocked;
	public int silentOpens;
	private float openTime;
	public float lockTime;
	private AudioSource myAudio;
	public bool sweepDoor;
}
