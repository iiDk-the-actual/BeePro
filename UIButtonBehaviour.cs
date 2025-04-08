using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BeePro
{
	internal class UIButtonBehaviour : MonoBehaviour
	{
		private void Start()
		{
			button = GetComponent<Button>();
			gameObject.layer = 18;
		}

		public void Activate()
		{
			button.OnPointerClick(new PointerEventData(EventSystem.current));
		}

		private Button button;
	}
}
