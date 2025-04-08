using System;
using Cinemachine;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BeePro
{
	internal class BeeProManager : MonoBehaviour
	{
		private void Start()
		{
			BeeProManager.instance = this;
			grab = transform.GetChild(0).gameObject.AddComponent<BeeGrab>();
			bee = transform.GetChild(0);
			rightWing = bee.GetChild(0).GetComponent<Renderer>();
			leftWing = bee.GetChild(1).GetComponent<Renderer>();
			cam = bee.GetChild(2);
			bee.gameObject.layer = 18;
			gTF = gameObject.AddComponent<TransformFollow>();
			gTF.enabled = false;
			cinemachineCam = GameObject.Find("Shoulder Camera");
			cinemachineCam.transform.SetParent(null, false);
			tF = cinemachineCam.AddComponent<TransformFollow>();
			tF.transformToFollow = cam;
			cinemachineCam.transform.GetChild(0).gameObject.SetActive(false);
			cinemachineCam.GetComponent<CinemachineBrain>().enabled = false;
			cinemachineCam.GetComponent<Camera>().fieldOfView = 90f;
		}

		private void OnGUI()
		{
			if (GUIEnabled)
			{
				fp = GUI.Toggle(new Rect(0f, 0f, 100f, 20f), fp, "FPV");
				smoothing = GUI.Toggle(new Rect(0f, 20f, 100f, 20f), smoothing, "Smoothing");
				smooth = GUI.HorizontalSlider(new Rect(0f, 40f, 100f, 20f), smooth, 0.0001f, 5f);
				smoothPos = GUI.HorizontalSlider(new Rect(0f, 60f, 100f, 20f), smoothPos, 1E-06f, 0.01f);
				GUI.Label(new Rect(100f, 60f, 100f, 20f), "Position");
				smoothRot = GUI.HorizontalSlider(new Rect(0f, 80f, 100f, 20f), smoothRot, 0.22f, 0.0012f);
				GUI.Label(new Rect(100f, 80f, 100f, 20f), "Rotation");
				playerLook = GUI.Toggle(new Rect(0f, 100f, 100f, 20f), playerLook, "Player Look");
				localSpace = GUI.Toggle(new Rect(0f, 120f, 100f, 20f), localSpace, "Local Space");
				freecamEnabled = GUI.Toggle(new Rect(0f, 140f, 100f, 20f), freecamEnabled, "Free Cam");
				camSpeed = GUI.HorizontalSlider(new Rect(0f, 160f, 100f, 20f), camSpeed, 0.01f, 3.2f);
				hideBee = GUI.Toggle(new Rect(0f, 180f, 100f, 20f), hideBee, "Hide Bee");
				hideCosmetics = GUI.Toggle(new Rect(0f, 200f, 100f, 20f), hideCosmetics, "Hide Cosmetics");
			}
		}

		private void Update()
		{
			if (smoothing)
			{
				tF.enabled = false;
				cinemachineCam.transform.position = Vector3.SmoothDamp(cinemachineCam.transform.position, tF.transformToFollow.position, ref moveSpeed, smoothPos * smooth);
				cinemachineCam.transform.rotation = Quaternion.Slerp(cinemachineCam.transform.rotation, tF.transformToFollow.rotation, smoothRot * smooth);
			}
			else
			{
				tF.enabled = true;
			}
		}

		private void FixedUpdate()
		{
			if (Keyboard.current.rightShiftKey.isPressed)
			{
				if (canToggle)
				{
					GUIEnabled = !GUIEnabled;
				}
				canToggle = false;
			}
			else
			{
				if (!canToggle)
				{
					canToggle = true;
				}
			}
			tF.transformToFollow = (fp ? GorillaTagger.Instance.offlineVRRig.mainCamera.transform : cam);

			if (playerLook)
			{
				transform.LookAt(GorillaTagger.Instance.offlineVRRig.transform.position);
				bee.localRotation = Quaternion.Euler(270f, 90f, 0f);
			}
			if (grab.canGrab && (GorillaTagger.Instance.offlineVRRig.leftIndex.calcT > 0.6f) | (GorillaTagger.Instance.offlineVRRig.rightIndex.calcT > 0.6f))
			{
				if (canFireGrab)
				{
					FireGrab();
				}
			}
			else
			{
				if (!canFireGrab)
				{
					canFireGrab = true;
				}
			}

			if (localSpace != lastLocalSpace && !freecamEnabled)
			{
				if (canFireLocalSpace)
				{
					FireLocalSpace();
				}
			}
			else
			{
				if (!canFireLocalSpace)
				{
					canFireLocalSpace = true;
				}
			}

			if (freecamEnabled && canFireLocalSpace)
			{
				float num = camSpeed / 50f * (float)(Keyboard.current.shiftKey.isPressed ? 6 : 1);

				if (Keyboard.current.wKey.isPressed)
				{
					transform.position += transform.forward * num;
				}

				if (Keyboard.current.aKey.isPressed)
				{
					transform.position += -transform.right * num;
				}

				if (Keyboard.current.sKey.isPressed)
				{
					transform.position += -transform.forward * num;
				}

				if (Keyboard.current.dKey.isPressed)
				{
					transform.position += transform.right * num;
				}

				if (Keyboard.current.spaceKey.isPressed)
				{
					transform.position += transform.up * num;
				}

				if (Keyboard.current.ctrlKey.isPressed)
				{
					transform.position += -transform.up * num;
				}

				if (Mouse.current.rightButton.isPressed)
				{
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;
					mousePos += Mouse.current.delta.ReadValue() / 5f;
					transform.rotation = Quaternion.Euler(-mousePos.y * 0.5f, mousePos.x * 0.5f, 0f);
				}
				else
				{
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
				}
			}

			if (hideBee == lastHideBee)
			{
				if (canFireHideBee)
				{
					FireHideBee();
				}
			}
			else
			{
				if (!canFireHideBee)
				{
					canFireHideBee = true;
				}
			}

			if (hideCosmetics == lastHideCosmetics)
			{
				if (canFireHideCosmetics)
				{
					FireHideCosmetics();
				}
			}
			else
			{
				if (!canFireHideCosmetics)
				{
					canFireHideCosmetics = true;
				}
			}
		}

		private void FireGrab()
		{
			if (!grab.isLeft)
			{
				if (GorillaTagger.Instance.offlineVRRig.rightIndex.calcT > 0.6f && !grabSetOnce)
				{
					transform.SetParent(GTPlayer.Instance.rightControllerTransform, false);
					transform.localRotation = Quaternion.Euler(270f, 180f, 0f);
					transform.localPosition = new Vector3(0f, 0.035f, 0.035f);
					grabSetOnce = true;
				}

				if (GorillaTagger.Instance.offlineVRRig.rightIndex.calcT < 0.6f && grabSetOnce)
				{
					transform.SetParent(null);
					grab.canGrab = false;
					grabSetOnce = false;
				}
			}
			else
			{
				if (GorillaTagger.Instance.offlineVRRig.leftIndex.calcT > 0.6f && !grabSetOnce)
				{
					transform.SetParent(GTPlayer.Instance.leftControllerTransform, false);
					transform.localRotation = Quaternion.Euler(270f, 180f, 0f);
					transform.localPosition = new Vector3(0f, 0.035f, 0.035f);
					grabSetOnce = true;
				}

				if (GorillaTagger.Instance.offlineVRRig.leftIndex.calcT < 0.6f && grabSetOnce)
				{
					transform.SetParent(null);
					grab.canGrab = false;
					grabSetOnce = false;
				}
			}
			canFireGrab = false;
		}

		private void FireLocalSpace()
		{
			if (transform.parent != GorillaTagger.Instance.offlineVRRig.transform)
			{
				transform.SetParent(GorillaTagger.Instance.offlineVRRig.transform);
				lastLocalSpace = true;
			}
			else
			{
				if (transform.parent == GorillaTagger.Instance.offlineVRRig.transform)
				{
					transform.SetParent(null);
					lastLocalSpace = false;
				}
			}
			canFireLocalSpace = false;
		}

		private void FireHideBee()
		{
			Renderer renderer;

			if (bee.TryGetComponent<Renderer>(out renderer))
			{
				renderer.enabled = !renderer.enabled;
				rightWing.enabled = !rightWing.enabled;
				leftWing.enabled = !leftWing.enabled;
				lastHideBee = !lastHideBee;
			}
			canFireHideBee = false;
		}

		private void FireHideCosmetics()
		{
			Transform transform = GorillaTagger.Instance.offlineVRRig.mainCamera.transform.Find("HeadCosmetics");
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				int j = 0;
				while (j < child.childCount)
				{
					Transform transform2;
					if (child.GetChild(j).gameObject.TryGetComponent<Transform>(out transform2))
					{
						transform2.gameObject.layer = ((transform2.gameObject.layer == 3) ? 0 : 3);
					}
					else
					{
						if (child.GetChild(j).GetComponentsInChildren<Transform>() != null)
						{
							foreach (Transform transform3 in child.GetChild(j).GetComponentsInChildren<Transform>())
							{
								transform3.gameObject.layer = ((transform3.gameObject.layer == 3) ? 0 : 3);
							}
						}
					}

					j++;
					continue;
				}
			}
			lastHideCosmetics = !lastHideCosmetics;
			canFireHideCosmetics = false;
		}

		public static BeeProManager instance;
		private GameObject cinemachineCam;
		private Transform bee;
		private Transform cam;
		private TransformFollow gTF;
		private TransformFollow tF;
		private Renderer leftWing;
		private Renderer rightWing;
		private BeeGrab grab;
		private Vector3 moveSpeed = new Vector3(1f, 1f, 1f);

		private Vector2 mousePos;
		private bool GUIEnabled = false;
		private bool canToggle = true;
		private bool grabSetOnce = false;
		private bool canFireLocalSpace;
		private bool lastLocalSpace;
		private bool canFireHideBee;
		private bool lastHideBee = true;
		private bool canFireHideCosmetics;
		private bool lastHideCosmetics = true;
		private bool canFireGrab;
		public bool fp = true;

		public bool smoothing = true;
		private float smooth = 1f;
		private float smoothPos = 0.0005f;
		private float smoothRot = 0.072f;

		public bool playerLook;
		public bool localSpace;
		public bool freecamEnabled;
		private float camSpeed = 1f;
		public bool hideBee;
		public bool hideCosmetics;
	}
}
