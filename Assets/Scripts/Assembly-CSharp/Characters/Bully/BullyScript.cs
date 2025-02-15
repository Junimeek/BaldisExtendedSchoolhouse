﻿using UnityEngine;

public class BullyScript : MonoBehaviour
{
	private void Start()
	{
		this.audioDevice = base.GetComponent<AudioSource>(); //Get the Audio Source
		this.waitTime = Random.Range(60f, 120f); //Set the amount of time before the bully appears again
	}

	private void Update()
	{
		if (this.waitTime > 0f) //Decrease the waittime
			this.waitTime -= Time.deltaTime;
		else if (!this.active)
			this.Activate(); //Activate the Bully

		if (this.active) //If the Bully is on the map
		{
			this.activeTime += Time.deltaTime; //Increase active time
			if (this.activeTime >= 180f && (base.transform.position - this.player.position).magnitude >= 120f) //If the bully has been in the map for a long time and the player is far away
			{
				this.audioDevice.PlayOneShot(this.aud_Bored);
				this.Reset(); //Reset the bully
			}
		}
		if (this.guilt > 0f)
			this.guilt -= Time.deltaTime; //Decrease Bully's guilt
	}

	private void FixedUpdate()
	{
		Vector3 direction = this.player.position - base.transform.position;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position + new Vector3(0f, 4f, 0f), direction, out raycastHit, float.PositiveInfinity, 769, QueryTriggerInteraction.Ignore) & raycastHit.transform.tag == "Player" & (base.transform.position - this.player.position).magnitude <= 30f & this.active)
		{
			if (!this.spoken) // If the bully hasn't already spoken
			{
				int num = Mathf.RoundToInt(Random.Range(0f, 1f)); //Get a random number between 0 and 1
				this.audioDevice.PlayOneShot(this.aud_Taunts[num]); //Say a line in an index using num
				this.spoken = true; //Sets spoken to true, preventing the bully from talking again
			}
			this.guilt = 10f; //Makes the bully guilty for "Bullying in the halls"
		}
	}

	private void Activate()
	{
		this.isDetention = false;
		base.transform.position = this.wanderer.NewTarget("Bully") + new Vector3(0f, 5f, 0f); // Go to the wanderTarget + 5 on the Y axis
		while ((base.transform.position - this.player.position).magnitude < 20f) // While the Bully is close to the player
		{
			base.transform.position = this.wanderer.NewTarget("Bully") + new Vector3(0f, 5f, 0f);// Go to the wanderTarget + 5 on the Y axis
        } //This is here to prevent the bully from spawning ontop iof the player
		this.active = true; //Set the bully to active
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Player" && !this.isDetention)
		{
			for (int i = 0; i < this.gc.totalSlotCount; i++)
			{
				if (!(this.gc.item[i] == 0))
				{
					this.TakeItem();
					break;
				}
				else if (this.gc.item[i] == 0 && i == this.gc.totalSlotCount - 1)
					this.audioDevice.PlayOneShot(this.aud_Denied);
			}
		}
		else if (this.isDetention)
			this.audioDevice.PlayOneShot(this.aud_Bored);
	}

	private void TakeItem()
	{
		int num = Mathf.RoundToInt(UnityEngine.Random.Range(0f, this.gc.totalSlotCount - 1)); //Get a random item slot
		while (this.gc.item[num] == 0) //If the selected slot is empty
		{
			num = Mathf.RoundToInt(Random.Range(0f, this.gc.totalSlotCount - 1)); // Choose another slot
		}
		this.gc.LoseItem(num); // Remove the item selected
		int num2 = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 1f));
		this.longAudioDevice.PlayOneShot(this.aud_Thanks[num2]);
		this.Reset();
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.transform.name == "Principal of the Thing" && this.guilt > 0f) //If touching the principal and the bully is guilty
		{
			this.isDetention = true;
			this.Reset(); //Reset the bully
		}
	}

	private void Reset()
	{
		if (this.isDetention)
			base.transform.position = this.detentionPos;
		else
			base.transform.position = base.transform.position + new Vector3(0f, 150f, 0f); // Go to X: 0, Y: 20, Z: 20
		this.waitTime = Random.Range(60f, 120f); //Set the amount of time before the bully appears again
		this.active = false; //Set active to false
		this.activeTime = 0f; //Reset active time
		this.spoken = false; //Reset spoken
	}

	public Transform player;
	public GameControllerScript gc;
	public Renderer bullyRenderer;
	public AILocationSelectorScript wanderer;
	public float waitTime;
	public float activeTime;
	public float guilt;
	public bool active;
	public bool spoken;
	private AudioSource audioDevice;
	[SerializeField] private AudioSource longAudioDevice;
	[SerializeField] private bool isDetention;
	[SerializeField] private Vector3 detentionPos;
	public AudioClip[] aud_Taunts = new AudioClip[2];
	public AudioClip[] aud_Thanks = new AudioClip[2];
	public AudioClip aud_Denied;
	public AudioClip aud_Bored;
}
