using Aircraft.Tools.GUI;
using System;
using UnityEngine;

namespace Aircraft.View
{
	public class AirDefense : MonoBehaviour
	{
		public Action Fire;
		public ButtonAdv FireButton;
		public void Init()
		{
			FireButton.onClick.AddListener(OnCalculateButtonClicked);
		}

		private void OnCalculateButtonClicked()
		{
			if (Fire != null)
				Fire();
		}
	}
}
