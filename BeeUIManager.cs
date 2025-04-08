using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;

namespace BeePro
{
	internal class BeeUIManager : MonoBehaviour
	{
		private void Start()
		{
			bee = BeeProManager.instance;
			Button[] allButtons = GetAllButtons(transform);
			allButtons[0].gameObject.AddComponent<UIButtonBehaviour>();
			allButtons[0].onClick.AddListener(new UnityAction(ToggleFPV));
			allButtons[1].gameObject.AddComponent<UIButtonBehaviour>();
			allButtons[1].onClick.AddListener(new UnityAction(ToggleSmoothing));
			allButtons[2].gameObject.AddComponent<UIButtonBehaviour>();
			allButtons[2].onClick.AddListener(new UnityAction(TogglePlayerLook));
			allButtons[3].gameObject.AddComponent<UIButtonBehaviour>();
			allButtons[3].onClick.AddListener(new UnityAction(ToggleLocalSpace));
			allButtons[4].gameObject.AddComponent<UIButtonBehaviour>();
			allButtons[4].onClick.AddListener(new UnityAction(ToggleFreeCam));
			allButtons[5].gameObject.AddComponent<UIButtonBehaviour>();
			allButtons[5].onClick.AddListener(new UnityAction(ToggleHideBee));
			allButtons[6].gameObject.AddComponent<UIButtonBehaviour>();
			allButtons[6].onClick.AddListener(new UnityAction(ToggleHideCosmetics));
			lR = EasyAssetLoading.SetupAsset(Assembly.GetExecutingAssembly(), "BeePro.beepro", "pointer", GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget, false).GetComponent<LineRenderer>();
		}

		// Linq doesn't decompile, so screw you Antoca
		private Button[] GetAllButtons(Transform source)
		{
			List<Button> res = new List<Button>();
			for (int i = 0; i < 3; i++)
			{
				List<Button> list = source.GetChild(i).GetComponentsInChildren<Button>().ToList<Button>();

                foreach (Button button in list)
					res.Add(button);
			}
			return res.ToArray();
		}

		private void ToggleFPV()
		{
			bee.fp = !bee.fp;
		}

		private void ToggleSmoothing()
		{
			bee.smoothing = !bee.smoothing;
		}

		private void TogglePlayerLook()
		{
			bee.playerLook = !bee.playerLook;
		}

		private void ToggleLocalSpace()
		{
			bee.localSpace = !bee.localSpace;
		}

		private void ToggleFreeCam()
		{
			bee.freecamEnabled = !bee.freecamEnabled;
		}

		private void ToggleHideBee()
		{
			bee.hideBee = !bee.hideBee;
		}

		private void ToggleHideCosmetics()
		{
			bee.hideCosmetics = !bee.hideCosmetics;
		}

		private void FixedUpdate()
		{
			if (ControllerInputPoller.instance.leftGrab && ControllerInputPoller.instance.rightGrab)
			{
				float num = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, GorillaTagger.Instance.rightHandTransform.position);
				if (num < 0.2f && !canOpen)
				{
					canOpen = true;
					if (GorillaTagger.Instance != null)
					{
						if (GorillaTagger.Instance.offlineVRRig != null)
						{
                            GorillaTagger.Instance.offlineVRRig.tagSound.PlayOneShot(GorillaTagger.Instance.offlineVRRig.clipToPlay[5]);
						}
					}
					GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 1.5f, GorillaTagger.Instance.tapHapticDuration);
					GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 1.5f, GorillaTagger.Instance.tapHapticDuration);
				}

				if (canOpen && (num - lastDistance) > 0.135f)
				{
					transform.position = (GorillaTagger.Instance.leftHandTransform.position + GorillaTagger.Instance.rightHandTransform.position) / 2f;
					transform.LookAt(GorillaTagger.Instance.offlineVRRig.head.rigTarget);
					if (GorillaTagger.Instance != null)
					{
						if (GorillaTagger.Instance.offlineVRRig != null)
						{
                            GorillaTagger.Instance.offlineVRRig.tagSound.PlayOneShot(GorillaTagger.Instance.offlineVRRig.clipToPlay[6]);
						}
					}
					GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 1.5f, GorillaTagger.Instance.tapHapticDuration);
					GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 1.5f, GorillaTagger.Instance.tapHapticDuration);
					canOpen = false;
					isActive = true;
				}

				if (Main.instance.GetLeftJoystickDown() && Main.instance.GetRightJoystickDown() && isActive)
				{
					isActive = false;
					transform.position = Vector3.zero;
				}
				lastDistance = num;
			}
			else
			{
				canOpen = false;
			}
			if (isActive)
			{
				RaycastHit raycastHit;
				if (Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.up, out raycastHit, 30f))
				{
					lR.SetPositions(new Vector3[]
					{
                        GorillaTagger.Instance.rightHandTransform.position,
						raycastHit.point
					});

					if (ControllerInputPoller.TriggerFloat(XRNode.RightHand) > 0.5f)
					{
						if (canClick)
						{
							UIButtonBehaviour uibuttonBehaviour;
							if (raycastHit.collider.TryGetComponent<UIButtonBehaviour>(out uibuttonBehaviour))
							{
								uibuttonBehaviour.Activate();
								if (GorillaTagger.Instance != null)
								{
									if (GorillaTagger.Instance.offlineVRRig != null)
									{
                                        GorillaTagger.Instance.offlineVRRig.tagSound.PlayOneShot(GorillaTagger.Instance.offlineVRRig.clipToPlay[3]);
									}
								}
								GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 1.5f, GorillaTagger.Instance.tapHapticDuration);
								canClick = false;
							}
						}
					}
					else
					{
						canClick = true;
					}
				}
				else
				{
					lR.SetPositions(new Vector3[]
					{
						Vector3.zero,
						Vector3.zero
					});
					canClick = true;
				}
			}
			else
			{
				lR.SetPositions(new Vector3[]
				{
					Vector3.zero,
					Vector3.zero
				});
				canClick = true;
			}
		}

		private BeeProManager bee;
        private LineRenderer lR;

        private bool canOpen = false;
		private bool canClick = false;
		private bool isActive = false;

		private float lastDistance;
	}
}
