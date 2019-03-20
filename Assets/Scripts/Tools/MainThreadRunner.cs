
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Aircraft.Tools
{
	public class MainThreadRunner : MonoBehaviour
	{

		private static MainThreadRunner _instance;
		private readonly List<Action> _tasks = new List<Action>();
		private Thread _mainThread;

		public static bool IsBackGround { get; private set; }

		public static bool IsMainThread
		{
			get { return Thread.CurrentThread == _instance._mainThread; }
		}

		private void Awake()
		{
			Application.targetFrameRate = 60;
			if (_instance != null)
				throw new Exception("Duplicating CoroutineStarter");
			_instance = this;
			_instance._mainThread = Thread.CurrentThread;
		}

		public static event Action<bool> ApplicationPaused;

		private static void OnApplicationPaused(bool flag)
		{
			Action<bool> handler = ApplicationPaused;
			if (handler != null) handler(flag);
		}

		private void OnApplicationPause(bool flag)
		{
			IsBackGround = flag;
			OnApplicationPaused(flag);

		}

		public static void AddTask(Action task)
		{
			if (IsMainThread && _instance._tasks.Count == 0)
				task();
			else
				lock (_instance._tasks)
					_instance._tasks.Add(task);
		}

	
		private void Update()
		{
			if (_tasks.Count < 1) return;
			try
			{
				Action action;
				while (_tasks.Count > 0)
				{
					lock (_tasks)
					{
						action = _tasks[0];
						_tasks.RemoveAt(0);
					}
					action();
				}

			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		public static void Clear()
		{
			if (_instance._tasks.Count > 0)
				try
				{
					lock (_instance._tasks)
					{
						for (int i = 0; i < _instance._tasks.Count; i++)
						{
							_instance._tasks.RemoveAt(i);
							i--;
						}
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
		}

	}
}
