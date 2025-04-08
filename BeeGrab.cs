using System;
using UnityEngine;

namespace BeePro
{
	internal class BeeGrab : MonoBehaviour
	{
		private void OnTriggerEnter(Collider coll)
		{
			GorillaTriggerColliderHandIndicator componentInParent = coll.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if (componentInParent != null)
			{
				GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 1.5f, GorillaTagger.Instance.tapHapticDuration);
				isLeft = componentInParent.isLeftHand;
				canGrab = true;
			}
		}

		private void OnTriggerExit(Collider coll)
		{
			GorillaTriggerColliderHandIndicator componentInParent = coll.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if (componentInParent != null)
			{
				if (componentInParent.isLeftHand == isLeft)
					canGrab = false;
			}
		}

		public bool canGrab;
		public bool isLeft;
	}
}
