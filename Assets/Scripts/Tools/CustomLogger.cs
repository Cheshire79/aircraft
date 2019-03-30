using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Logger with customizable output. Use Logger.Log(component, message);
/// For output customization use <see cref="EnabledComponents"/>
/// </summary>
namespace Aircraft
{
	public static class CustomLogger
	{
		public static string LogFilename = "AndroidProblemReport.txt";

		private static int _maxLogSize = 200048;
		private static List<String> _log = new List<string>();
		private static StringBuilder _logBuilder = new StringBuilder();
		public static string DevicePin { get; set; }


#if DEVELOPMENT_BUILD || UNITY_EDITOR || DEBUG
		private const bool IsDebugMode = true;
#else
    private const bool IsDebugMode = false;
#endif

		//Customize for your needs!
		public const LogComponents
			EnabledComponents = ImportantComponents |// LogComponents.ProgressBarManager | LogComponents.Audio;
		//|LogComponents.Branch;//
		LogComponents.All; //<== Customize for your needs!
		//Customize for your needs!

		public static bool Enabled = true;

		//Used when component is not specified
		private const LogComponents ImportantComponents = LogComponents.Exceptions;

		// | LogComponents.Errors|                                                         LogComponents.Warnings;

		[Flags]
		public enum LogComponents : ushort
		{
			//None = 0x0000,
			//Map = 0x0001,
			//Mobs = 0x0002,
			//Network = 0x0004,
			//Blocks = 0x0008,
			//GUI = 0x0010,
			//Light = 0x0020,
			//Player = 0x0040,
			//Environment = 0x0080,
			//IO = 0x0100,
			NeedToImplement = 0x0001,
			Messages = 0x1000,
			Warnings = 0x2000,
			Errors = 0x4000,
			Exceptions = 0x8000,
			Test = 0x0010,
			Branch = 0x0020,
			ProgressBarManager = 0x0040,
			Audio = 0x0080,
			BrunchController = 0x0100,
			All = 0xFFFF,

		}

		private static string FormatString(params string[] text)
		{
			if (text.Length == 1) return text[0];
			var sb = new StringBuilder();
			for (int i = 0; i < text.Length; i++)
				sb.Append(text[i]);
			return sb.ToString();
		}

		public static void Log(LogComponents component, params string[] text)
		{
			if (IsDebugMode && Enabled && (component & EnabledComponents) != 0)
			{
#if UNITY_EDITOR || DEBUG
				Debug.Log(FormatString(text));
#else

#endif
			}

			DoLog(FormatString(text), component.ToString());
		}

		public static void Log(params string[] text)
		{
			Log(LogComponents.Messages, text);
		}

		public static void Log(object obj)
		{
			Log(LogComponents.Messages, obj.ToString());
		}

		public static void LogForcibly(params string[] text)
		{
			Debug.Log(FormatString(text));
		}

		public static void LogWarning(LogComponents component, params string[] text)
		{
			if (IsDebugMode && Enabled && (component & EnabledComponents) != 0)
				Debug.LogWarning(FormatString(text));

			DoLog(FormatString(text), component.ToString());
		}

		public static void LogWarning(params string[] text)
		{
			LogWarning(LogComponents.Warnings, text);
		}

		public static void LogError(LogComponents component, params string[] text)
		{
			if (IsDebugMode && Enabled && (component & EnabledComponents) != 0)
				Debug.LogError(FormatString(text));

			DoLog(FormatString(text), component.ToString());
		}

		public static void LogError(params string[] text)
		{
			LogError(LogComponents.Errors, text);
		}

		public static void LogException(LogComponents component, Exception exception)
		{
			if (IsDebugMode && ((component & EnabledComponents) != 0 ||
								(LogComponents.Exceptions & EnabledComponents) != 0))
				Debug.LogException(exception);

			DoLog(FormatString(exception.ToString()), component.ToString());
		}

		public static void LogException(Exception exception)
		{
			LogException(LogComponents.Exceptions, exception);
		}

		public static string Dump(bool includeDeviceInfo)
		{
			TrimLog(_logBuilder.Length - _maxLogSize);

			return _logBuilder.ToString();
		}

		private static void DoLog(string msg, string component)
		{
			string separator = "║";

			_logBuilder.Append(DateTime.Now.TimeOfDay)
				.Append(separator)
				.Append(component)
				.Append(separator)
				.Append(msg)
				.AppendLine();

			//TrimLog(_maxLogSize / 4);
		}

		private static void TrimLog(int trimSize)
		{
			int length = _logBuilder.Length;

			if (length > _maxLogSize)
			{
				if (trimSize > length)
				{
					_logBuilder.Length = 0;
				}
				else
					_logBuilder.Remove(0, trimSize);
			}
		}

		public static string LogDeviceInfo(bool ignoreExceptions)
		{
			var logBuilder = new StringBuilder();

			logBuilder = WriteToStringBuilder(logBuilder, LogComponents.Messages,
				"===============DEVICE INFO==========================");
			//logBuilder = WriteToStringBuilder(logBuilder, LogComponents.Messages, "App Version: ");
			logBuilder = WriteToStringBuilder(logBuilder, LogComponents.Messages,
				"TimeZone = " + TimeZone.CurrentTimeZone);
			logBuilder = WriteToStringBuilder(logBuilder, LogComponents.Messages, "Device = " + Application.platform);
			logBuilder = WriteToStringBuilder(logBuilder, LogComponents.Messages, "Model = " + SystemInfo.deviceModel);
			logBuilder = WriteToStringBuilder(logBuilder, LogComponents.Messages, "OS = " + SystemInfo.operatingSystem);

			try
			{
				logBuilder = WriteToStringBuilder(logBuilder, LogComponents.Messages, "DEVICE PIN = " + DevicePin);
			}
			catch (Exception e)
			{
				string notificationText = "Fatal Error. Failed to obtain device PIN.";
				LogException(e);
				if (!ignoreExceptions)
				{
					//ProblemReportSender.SendReportAProblem("", notificationText);
				}
			}

			logBuilder = WriteToStringBuilder(logBuilder, LogComponents.Messages,
				"=========================================");

			return logBuilder.ToString();
		}

		private static StringBuilder WriteToStringBuilder(StringBuilder stringBuilder, LogComponents component,
			string text)
		{
			string separator = "║";

			stringBuilder.Append(DateTime.Now.TimeOfDay)
				.Append(separator)
				.Append(component)
				.Append(separator)
				.Append(text)
				.AppendLine();
			return stringBuilder;
		}

		public static string DumpToFile()
		{
			try
			{
				string dump = Dump(true);
				string infoDevice = LogDeviceInfo(true);

				byte[] byteData = Encoding.UTF8.GetBytes(dump);
				char[] charData = Encoding.UTF8.GetChars(byteData);
				byte[] info = Encoding.UTF8.GetBytes(infoDevice);

				string fileName = Application.persistentDataPath +
								  "/" + LogFilename;

				if (!File.Exists(fileName))
				{
					StreamWriter fileWriter = File.CreateText(fileName);

					fileWriter.Write(charData, 0, charData.Length);
					fileWriter.Close();
				}
				else
				{
					StreamWriter appendWriter = File.AppendText(fileName);
					appendWriter.Write(charData);
					appendWriter.Close();
				}


				return infoDevice + dump;
			}
			catch (FileNotFoundException e)
			{
			}
			catch (IOException e)
			{
				LogError("Logger.DumpToFile " + e);
			}

			return null;
		}

		public static void RestoreLogFromFile()
		{
			try
			{
				string fileName = Application.persistentDataPath +
								  "/" + LogFilename;

				if (!File.Exists(fileName))
					return;

				StreamReader reader = File.OpenText(fileName);
				string text;

				while ((text = reader.ReadLine()) != null)
				{
					if (text.Contains(LogComponents.Messages.ToString()))
					{
						text = text.Substring(text.IndexOf(LogComponents.Messages.ToString()) +
											  LogComponents.Messages.ToString().Length);
						Log(text);
					}
					else if (text.Contains(LogComponents.Errors.ToString()))
					{
						text = text.Substring(text.IndexOf(LogComponents.Errors.ToString()) +
											  LogComponents.Errors.ToString().Length);
						LogError(text);
					}
					else if (text.Contains(LogComponents.Warnings.ToString()))
					{
						text = text.Substring(text.IndexOf(LogComponents.Warnings.ToString()) +
											  LogComponents.Warnings.ToString().Length);
						LogWarning(text);
					}
				}
			}
			catch (FileNotFoundException e)
			{
			}
			catch (IOException e)
			{
				LogError("Logger.RestoreLogFromFile " + e);
			}
		}
	}
}