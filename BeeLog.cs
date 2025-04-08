using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace BeePro
{
	public class BeeLog
	{
		public static void Log(string toLog)
		{
			UnityEngine.Debug.Log("<BeePro> " + toLog);
		}
	}
}
