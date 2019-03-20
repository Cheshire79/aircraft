using Aircraft.Controller;
using UnityEngine;

namespace Aircraft
{
	public class StartAppl : MonoBehaviour
	{
		public Camera Camera;
		public MainController MainPrefab;
		private MainController _main;

		private void Start()
		{
			_main = Instantiate(MainPrefab);
			_main.transform.name = "Main";
			_main.GetComponent<Canvas>().worldCamera = Camera;
			_main.Init(Camera);

		}
		private void Quit()
		{
			_main.OnExit();
#if UNITY_EDITOR
			// Application.Quit() does not work in the editor so
			// UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
			Debug.LogWarning(" Quit from EDITOR");
			UnityEditor.EditorApplication.isPlaying = false;
#else
         Debug.LogWarning(" Quit " );			
			Application.Quit();
#endif
		}

		void OnApplicationQuit()
		{
			Quit();
		}
		void Update()
		{
			if (Input.GetKeyUp(KeyCode.Escape))
				Quit();
		}
	}
}

