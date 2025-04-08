using System;
using System.Reflection;
using HarmonyLib;

namespace BeePro
{
	public class HarmonyPatches
	{
		public static bool IsPatched { get; private set; }

		internal static void ApplyHarmonyPatches()
		{
			if (!IsPatched)
			{
				if (instance == null)
					instance = new Harmony(ModInfo._id);

				instance.PatchAll(Assembly.GetExecutingAssembly());
				IsPatched = true;
			}
		}

		internal static void RemoveHarmonyPatches()
		{
			if (instance != null && IsPatched)
			{
				instance.UnpatchSelf();
				IsPatched = false;
			}
		}

		private static Harmony instance;
	}
}
