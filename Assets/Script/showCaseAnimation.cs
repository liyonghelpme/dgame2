using UnityEngine;
using System.Collections;

public class showCaseAnimation : MonoBehaviour {
public string[] animations;
private int currentAnimationID;
private string currentAnimationName;
private string previousAnimationName;
public Transform archer;
public Transform horse;
public Transform sword;
public Transform bow;
public Transform spinScene;
bool flag4 = false;
public bool enableHorse;
private bool showSword;
private bool showBow;
public Transform quiver;
public Transform[] arrows;
private Vector3[] arrowsQuiverPosition;
private Vector3[] arrowsQuiverRotation;
public int currentArrow;
private Vector3 cameraArcherPosition;
private Quaternion cameraArcherRotation;
private Vector3 cameraHorsePosition;
private Quaternion cameraHorseRotation;
private Vector3 archerMountedPosition;
private int currentHorseAnimationID;
private string currentHorseAnimationName;
private string previousHorseAnimationName;
public string[] horseAnimations;
private bool isShooting;
private string currentShootAnimation;
public string text;
public Transform rightHand;
private bool arrowGrabbed;
private float lastFootStepTime;
private bool footStepToogle;
private float lastSwooshTime;
private bool swooshToogle;
private float lastArrowAudioTime;
private bool arrowAudioToogle;
public Transform[] footStepAudio;
public Transform[] footStepAudioRun;
public Transform swooshAudio;
public Transform arrowAudio;
//public stickTo other;
//public projectile Projectil;
	// Use this for initialization
	void Start () {
	currentAnimationID = 0;
	currentAnimationName = GetAnimationName(animations[currentAnimationID]);
	previousAnimationName = currentAnimationName;
	archer.animation.Blend(currentAnimationName, 1f);
	arrowsQuiverPosition = new Vector3[arrows.Length];
			arrowsQuiverRotation = new Vector3[arrows.Length];
			for (int i = 0; i < arrows.Length; i++)
			{
				arrowsQuiverPosition[i] = arrows[i].localPosition;
				arrowsQuiverRotation[i] = arrows[i].localRotation.eulerAngles;
			}
			cameraArcherPosition = Camera.main.transform.position;
			cameraArcherRotation = Camera.main.transform.rotation;
			cameraHorsePosition = new Vector3(-1.85012f, 2.527752f, 8.671867f);
			cameraHorseRotation.eulerAngles = new Vector3(4.550819f, 167.2307f, (float)0);
			currentHorseAnimationID = 0;
			currentHorseAnimationName = horseAnimations[currentHorseAnimationID];
			previousHorseAnimationName = currentHorseAnimationName;
			horse.animation.Blend(currentHorseAnimationName, 1f);
	}
	
	// Update is called once per frame
	void Update () {
		guiText.text = "Press left and right arrow keys to switch animations. Currently playing: ";
			if (enableHorse)
			{
				guiText.text = guiText.text + currentHorseAnimationName;
			}
			else
			{
				guiText.text = guiText.text + currentAnimationName;
			}
			bool flag = false;
			bool flag2 = false;
			if (enableHorse)
			{
				if (Input.GetKeyDown(KeyCode.RightArrow))
				{
					flag2 = true;
					currentHorseAnimationID++;
					if (currentHorseAnimationID >= horseAnimations.Length)
					{
						currentHorseAnimationID = 0;
					}
				}
				if (Input.GetKeyDown(KeyCode.LeftArrow))
				{
					flag2 = true;
					currentHorseAnimationID--;
					if (currentHorseAnimationID < 0)
					{
						currentHorseAnimationID = horseAnimations.Length - 1;
					}
				}
				}
				else
			{
				if (Input.GetKeyDown(KeyCode.RightArrow))
				{
					flag = true;
					currentAnimationID++;
					if (currentAnimationID >= this.animations.Length)
					{
						currentAnimationID = 0;
					}
				}
				if (Input.GetKeyDown(KeyCode.LeftArrow))
				{
					flag = true;
					currentAnimationID--;
					if (currentAnimationID < 0)
					{
						currentAnimationID = animations.Length - 1;
					}
				}
			}
			bool flag3 = false;
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				if (showSword)
				{
					showSword = false;
					sword.renderer.enabled = false;
				}
				else
				{
					bow.renderer.enabled = false;
					quiver.renderer.enabled = false;
					showBow = false;
					showSword = true;
					sword.renderer.enabled = true;
				}
					
				
              flag3 = true;
				flag = true;	
           if (enableHorse)
				{
					flag2 = true;
				}				
			} 
		if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				if (showBow)
				{
					showBow = false;
					bow.renderer.enabled = false;
					quiver.renderer.enabled = false;
				}
				else
				{
					showBow = true;
					bow.renderer.enabled = true;
					quiver.renderer.enabled = true;
					sword.renderer.enabled = false;
					showSword = false;
				}
				flag = true;
				if (enableHorse)
				{
					flag2 = true;
				}
				flag3 = true;
			}	
		if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				if (!this.enableHorse)
				{	
				
				horse.Find("horseSaddle").renderer.enabled = true;
				enableHorse = true;
				archer.parent = horse.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1");
				archer.transform.localPosition = new Vector3(-0.4f, 1.45f, (float)0);
				Vector3 eulerAngles = new Vector3((float)180, (float)90, (float)0);
				Quaternion localRotation = archer.transform.localRotation;
				Vector3 vector = localRotation.eulerAngles = eulerAngles;
				Quaternion quaternion = archer.transform.localRotation = localRotation;
					archer.animation.CrossFade ("idleHorse");
				flag2 = true;
					
				
	        }
			else
				{
					horse.Find("horseSaddle").renderer.enabled = false;
					enableHorse = false;
					archer.parent = spinScene;
					archer.transform.position = Vector3.zero;
					Vector3 eulerAngles2 = new Vector3((float)0, (float)0, (float)0);
					Quaternion localRotation2 = archer.transform.localRotation;
					Vector3 vector2 = localRotation2.eulerAngles = eulerAngles2;
					Quaternion quaternion2 = archer.transform.localRotation = localRotation2;
					flag = true;
				}
	}
	  if (enableHorse)
		{
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraHorsePosition, Time.deltaTime * 10f);
			Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, cameraHorseRotation, Time.deltaTime * 10f);
		}
		else
		{
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraArcherPosition, Time.deltaTime * 10f);
			Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, cameraArcherRotation, Time.deltaTime * 10f);
		}
		///////////////////////////////////////
		if (flag2)
		{
			isShooting = false;
			currentHorseAnimationName = horseAnimations[currentHorseAnimationID];
			flag4 = true;
			text = "idleHorse";
			string a = previousHorseAnimationName;
			if (a == "horseIdleShoot")
			{
				this.previousHorseAnimationName = "horseIdle";
			}
			this.horse.animation.Blend(this.previousHorseAnimationName, (float)0);
			string a2 = currentHorseAnimationName;
			if (a2 == "horseIdleShoot")
			{
				this.horse.animation.Blend("horseIdle", 1f);
				flag4 = false;
				text = "idleHorseBowShoot";
				this.archer.animation["idleHorseBowShoot"].time = (float)0;
				this.currentShootAnimation = "idleHorseBowShoot";
				this.isShooting = true;
				if (!this.showBow)
				{
					this.showSword = false;
					this.sword.renderer.enabled = false;
					this.showBow = true;
					this.bow.renderer.enabled = true;
					this.quiver.renderer.enabled = true;
					flag3 = true;
					this.currentShootAnimation = "shoot";
				}
			}
			else
			{
				this.horse.animation.Blend(this.currentHorseAnimationName, 1f);
				this.horse.animation[this.currentHorseAnimationName].normalizedTime = this.horse.animation[this.previousHorseAnimationName].normalizedTime;
			}
			this.archer.animation.Blend(this.previousAnimationName, (float)0);
			string a3 = currentHorseAnimationName;
			if (a3 == "horseIdle" || a3 == "horseIdleShake")
			{
				if (this.showSword)
				{
					text = "idleHorseSword";
				}
				else
				{
					if (this.showBow)
					{
						text = "idleHorseBow";
					}
					else
					{
						text = "idleHorse";
					}
				}
			}
			else
			{
				if (a3 == "horseWalk")
				{
					if (this.showSword)
					{
						text = "walkHorseSword";
					}
					else
					{
						if (this.showBow)
						{
							text = "walkHorseBow";
						}
						else
						{
							text = "walkHorse";
						}
					}
				}
				else
				{
					if (a3 == "horseRun")
					{
						if (this.showSword)
						{
							text = "runHorseSword";
						}
						else
						{
							if (this.showBow)
							{
								text = "runHorseBow";
							}
							else
							{
								text = "runHorse";
							}
						}
					}
					else
					{
						if (a3 == "horseSpinRight")
						{
							text = "spinRightHorse";
						}
						else
						{
							if (a3 == "horseSpinLeft")
							{
								text = "spinLeftHorse";
							}
							else
							{
								if (a3 == "horseJump")
								{
									this.horse.animation[this.currentHorseAnimationName].normalizedTime = (float)0;
									this.horse.animation["horseTwoLegs"].time = (float)0;
									text = "jumpHorse";
								}
								else
								{
									if (a3 == "horseTwoLegs")
									{
										this.horse.animation[this.currentHorseAnimationName].normalizedTime = (float)0;
										text = "archerHorseTwoLegs";
										if (this.showSword && this.currentHorseAnimationName != this.previousHorseAnimationName)
										{
											this.showSword = false;
											this.sword.renderer.enabled = false;
										}
									}
									else
									{
										if (a3 == "horseGetOn")
										{
											this.horse.animation[this.currentHorseAnimationName].normalizedTime = (float)0;
											text = "archerHorseGetOn";
										}
										else
										{
											if (a3 == "horseGetOff")
											{
												this.horse.animation[this.currentHorseAnimationName].normalizedTime = (float)0;
												text = "archerHorseGetOff";
											}
										}
									}
								}
							}
						}
					}
				}
			}
			if (flag4)
			{
				this.archer.animation[text].normalizedTime = this.horse.animation[this.currentHorseAnimationName].normalizedTime;
			}
			this.archer.animation.Blend(text, 1f);
			this.previousAnimationName = text;
			this.previousHorseAnimationName = this.currentHorseAnimationName;
		}
		else
		{
			if (flag)
			{
				this.isShooting = false;
				this.currentAnimationName = this.GetAnimationName(this.animations[this.currentAnimationID]);
			float	normalizedTime = 0f;
				if (this.GetRewind(this.currentAnimationName))
				{
					normalizedTime = (float)0;
				}
				else
				{
					normalizedTime = this.archer.animation[this.previousAnimationName].normalizedTime;
				}
				this.archer.animation.Blend(this.previousAnimationName, (float)0);
				this.archer.animation.Blend(this.currentAnimationName, 1f);
				this.archer.animation[this.currentAnimationName].normalizedTime = normalizedTime;
				if (this.currentAnimationName == "shoot")
				{
					this.isShooting = true;
				}
				if (this.currentAnimationName != this.previousAnimationName)
				{
				string a4 = this.currentAnimationName;
					if (a4 == "bowStance" || a4 == "shoot")
					{
						if (!this.showBow)
						{
							this.showSword = false;
							this.sword.renderer.enabled = false;
							this.showBow = true;
							this.bow.renderer.enabled = true;
							this.quiver.renderer.enabled = true;
							flag3 = true;
							this.currentShootAnimation = "shoot";
						}
					}
					else
					{
						if (a4 == "swordStance" || a4 == "swordStrike1" || a4 == "swordStrike2")
						{
							if (!this.showSword)
							{
								this.showBow = false;
								this.bow.renderer.enabled = false;
								this.quiver.renderer.enabled = false;
								flag3 = true;
								this.showSword = true;
								this.sword.renderer.enabled = true;
							}
						}
						else
						{
							if (a4 == "die1" || a4 == "die2")
							{
								if (this.showBow)
								{
									this.showBow = false;
									this.bow.renderer.enabled = false;
									this.quiver.renderer.enabled = false;
									flag3 = true;
								}
								if (this.showSword)
								{
									this.showSword = false;
									this.sword.renderer.enabled = false;
								}
							}
						}
					}
				}
				this.previousAnimationName = this.currentAnimationName;
			}
		}
		
	//	 stickTo = bow.parent.Find("bowStringHand").GetComponent("stickTo");
	//	stickTo = (stickTo)this.bow.parent.Find("bowStringHand").GetComponent("stickTo");
		stickTo StickTo = bow.parent.Find("bowStringHand").GetComponent<stickTo>();
		if (this.isShooting)
		{
			if (this.archer.animation[this.currentShootAnimation].weight == (float)1)
			{
				if (this.archer.animation[this.currentShootAnimation].normalizedTime > (float)1)
				{
					this.archer.animation[this.currentShootAnimation].normalizedTime = (float)0;
				}
				if (this.archer.animation[this.currentShootAnimation].normalizedTime > 0.4375f && this.archer.animation[this.currentShootAnimation].normalizedTime < 0.74f)
				{
				StickTo.disabled = false;
				}
				else
				{
				StickTo.disabled = true;
				}
			Vector3	from ;
				Vector3 to ;
				Quaternion from2 ;
				Quaternion to2 ;
				float num = 0.12f;
				if (this.archer.animation[this.currentShootAnimation].normalizedTime > num)
				{
					this.arrows[this.currentArrow].parent = this.quiver;
					this.arrows[this.currentArrow].localPosition = this.arrowsQuiverPosition[this.currentArrow];
					Vector3 eulerAngles3 = this.arrowsQuiverRotation[this.currentArrow];
					Quaternion localRotation3 = this.arrows[this.currentArrow].localRotation;
					localRotation3.eulerAngles = eulerAngles3;
					Quaternion quaternion3 = (this.arrows[this.currentArrow].localRotation = localRotation3);
					from = this.arrows[this.currentArrow].position;
					from2 = this.arrows[this.currentArrow].rotation;
					this.arrows[this.currentArrow].parent = this.rightHand;
					this.arrows[this.currentArrow].localPosition = new Vector3(-0.19f, 0.04f, 0.05f);
					Vector3 eulerAngles4 = new Vector3((float)0, 4.8f, 269.3f);
					Quaternion localRotation4 = this.arrows[this.currentArrow].localRotation;
					Vector3 vector4 = (localRotation4.eulerAngles = eulerAngles4);
					Quaternion quaternion4 = (this.arrows[this.currentArrow].localRotation = localRotation4);
					to = this.arrows[this.currentArrow].position;
					to2 = this.arrows[this.currentArrow].rotation;
				float	t = Mathf.Min((this.archer.animation[this.currentShootAnimation].normalizedTime - num) * (float)10, 1f);
					float t2 = Mathf.Min((this.archer.animation[this.currentShootAnimation].normalizedTime - num - 0.09f) * (float)6, 1f);
					checked
					{
						this.arrows[this.currentArrow].position = Vector3.Lerp(from, to, t);
						this.arrows[this.currentArrow].rotation = Quaternion.Slerp(from2, to2, t2);
						if (this.archer.animation[this.currentShootAnimation].normalizedTime < 0.2f)
						{
							this.arrowGrabbed = true;
						}
						if (this.archer.animation[this.currentShootAnimation].normalizedTime > 0.75f)
						{
							if (this.arrowGrabbed)
							{
							//	SpawnScript spawnScript = spawnManager.GetComponent<SpawnScript>();
							//	projectile = (projectile)this.arrows[this.currentArrow].GetComponent("projectile");
							//	projectile Projectile = arrows[currentArrow].GetComponent("projectile");
								projectile Projectile = arrows[currentArrow].GetComponent<projectile>();
								Projectile.Shoot();
								this.currentArrow++;
								if (this.currentArrow >= this.arrows.Length)
								{
									Projectile.GoBackToQuiver();
									this.currentArrow = 0;
									for (int i = 0; i < this.arrows.Length; i++)
									{
										this.arrows[i].parent = this.quiver;
										this.arrows[i].localPosition = this.arrowsQuiverPosition[i];
										Vector3 eulerAngles5 = this.arrowsQuiverRotation[i];
										Quaternion localRotation5 = this.arrows[i].localRotation;
										Vector3 vector5 = (localRotation5.eulerAngles = eulerAngles5);
										Quaternion quaternion5 = (this.arrows[i].localRotation = localRotation5);
									}
								}
								this.arrowGrabbed = false;
							}
							this.arrows[this.currentArrow].parent = this.quiver;
							this.arrows[this.currentArrow].localPosition = this.arrowsQuiverPosition[this.currentArrow];
							Vector3 eulerAngles6 = this.arrowsQuiverRotation[this.currentArrow];
							Quaternion localRotation6 = this.arrows[this.currentArrow].localRotation;
							Vector3 vector6 = (localRotation6.eulerAngles = eulerAngles6);
							Quaternion quaternion6 = (this.arrows[this.currentArrow].localRotation = localRotation6);
						}
					}
				}
			}
		}
		else
		{
			arrows[currentArrow].parent = quiver;
			this.arrows[this.currentArrow].localPosition = this.arrowsQuiverPosition[this.currentArrow];
			Vector3 eulerAngles7 = this.arrowsQuiverRotation[this.currentArrow];
			Quaternion localRotation7 = this.arrows[this.currentArrow].localRotation;
			Vector3 vector7 = (localRotation7.eulerAngles = eulerAngles7);
			Quaternion quaternion7 = (this.arrows[this.currentArrow].localRotation = localRotation7);
			StickTo.disabled = true;
		}
		if (Time.time > this.lastFootStepTime + 0.35f)
		{
			this.footStepToogle = true;
		}
		if (Time.time > this.lastSwooshTime + 0.4f)
		{
			this.swooshToogle = true;
		}
		if (Time.time > this.lastArrowAudioTime + 0.8f)
		{
			this.arrowAudioToogle = true;
		}
		checked
		{
			if (!this.enableHorse)
			{
			 string	a5 = this.currentAnimationName;
				if (a5 == "walk" || a5 == "walkSword" || a5 == "walkBow" || a5 == "run" || a5 == "runSword" || a5 == "runBow" || a5 == "strafeR" || a5 == "strafeRSword" || a5 == "strafeRBow" || a5 == "strafeL" || a5 == "strafeLSword" || a5 == "strafeLBow" || a5 == "crouchWalk" || a5 == "crouchWalkSword" || a5 == "crouchWalkBow" || a5 == "crouchStrafeR" || a5 == "crouchStrafeRSword" || a5 == "crouchStrafeRBow" || a5 == "crouchStrafeL" || a5 == "crouchStrafeLSword" || a5 == "crouchStrafeLBow")
				{
					this.FootStepSound(0.95f);
					this.FootStepSound(0.45f);
				}
				else
				{
					if (a5 == "sprint" || a5 == "sprintSword" || a5 == "sprintBow" || a5 == "crouchRun" || a5 == "crouchRunSword" || a5 == "crouchRunBow")
					{
						this.FootStepRunSound(0.95f);
						this.FootStepRunSound(0.45f);
					}
					else
					{
						if (a5 == "jump")
						{
							this.FootStepSound(0.55f);
						}
						else
						{
							if (a5 == "swordStrike1")
							{
								this.SwooshSound(0.3f);
								this.FootStepRunSound(0.3f);
							}
							else
							{
								if (a5 == "swordStrike2")
								{
									this.SwooshSound(0.4f);
								}
								else
								{
									if (a5 == "die1")
									{
										this.FootStepRunSound(0.64f);
										this.FootStepSound(0.72f);
									}
									else
									{
										if (a5 == "die2")
										{
											this.FootStepRunSound(0.2f);
											this.FootStepSound(0.7f);
										}
										else
										{
											if (a5 == "shoot")
											{
												this.ArrowSound(0.75f);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			if (flag3)
			{
				for (int j = 0; j < this.arrows.Length; j++)
				{
					if (this.showBow)
					{
						this.arrows[j].renderer.enabled = true;
					}
					else
					{
						this.arrows[j].renderer.enabled = false;
					}
				}
			}
		}
	}
		//////////////////////////////////////
		
//}
 public string GetAnimationName (string animationName)
//	void GetAnimationName(string animationName)
	{
		if (this.bow.renderer.enabled)
		{
			string a = animationName;
			if (a == "idle" || a == "walk" || a == "run" || a == "sprint" || a == "strafeR" || a == "strafeL" || a == "crouch" || a == "crouchWalk" || a == "crouchRun" || a == "crouchStrafeR" || a == "crouchStrafeL")
			{
				animationName += "Bow";
			}
		}
		if (this.sword.renderer.enabled)
		{
			string a2 = animationName;
			if (a2 == "idle" || a2 == "walk" || a2 == "run" || a2 == "sprint" || a2 == "strafeR" || a2 == "strafeL" || a2 == "crouch" || a2 == "crouchWalk" || a2 == "crouchRun" || a2 == "crouchStrafeR" || a2 == "crouchStrafeL")
			{
				animationName += "Sword";
			}
		}
		return animationName;
	}
	public  void FootStepSound(float normalizedTime)
	{
	}
	public  bool GetRewind(string animationName)
	{
		return animationName == "jump" || animationName == "swordStance" || animationName == "swordStrike1" || animationName == "swordStrike1Stuck" || animationName == "swordStrike2" || animationName == "swordStrike3" || animationName == "die1" || animationName == "die2" || animationName == "shoot" || animationName == "strike1" || animationName == "strike2" || animationName == "die1" || animationName == "die2";
	}
	public  void FootStepRunSound(float normalizedTime)
	{
		float num = this.archer.animation[this.currentAnimationName].normalizedTime - Mathf.Floor(this.archer.animation[this.currentAnimationName].normalizedTime);
		if (Mathf.Abs(num - normalizedTime) < 0.1f && this.footStepToogle)
		{
			float num2 = Mathf.Floor(UnityEngine.Random.value * (float)this.footStepAudio.Length);
			checked
			{
				this.footStepAudioRun[(int)num2].audio.Play();
				this.footStepAudioRun[(int)num2].audio.pitch = Random.Range(0.75f, 0.85f);
				this.footStepAudioRun[(int)num2].audio.volume = Random.Range(0.65f, 0.75f);
				this.lastFootStepTime = Time.time;
				this.footStepToogle = false;
			}
		}
	}
	public  void SwooshSound(float normalizedTime)
	{
		float num = this.archer.animation[this.currentAnimationName].normalizedTime - Mathf.Floor(this.archer.animation[this.currentAnimationName].normalizedTime);
		if (Mathf.Abs(num - normalizedTime) < 0.1f && this.swooshToogle)
		{
			this.swooshAudio.audio.Play();
			this.swooshAudio.audio.pitch = Random.Range(0.9f, 1.2f);
			this.swooshAudio.audio.volume = Random.Range(0.65f, 0.75f);
			this.lastSwooshTime = Time.time;
			this.swooshToogle = false;
		}
	}
	public  void ArrowSound(float normalizedTime)
	{
		float num = this.archer.animation[this.currentAnimationName].normalizedTime - Mathf.Floor(this.archer.animation[this.currentAnimationName].normalizedTime);
		if (Mathf.Abs(num - normalizedTime) < 0.1f && this.arrowAudioToogle)
		{
			this.arrowAudio.audio.Play();
			this.arrowAudio.audio.pitch = Random.Range(1f, 1.2f);
			this.arrowAudio.audio.volume = Random.Range(0.45f, 0.55f);
			this.lastArrowAudioTime = Time.time;
			this.arrowAudioToogle = false;
		}
	}
}
