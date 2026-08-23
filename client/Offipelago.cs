using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using FangamerRPG;
using HarmonyLib;
using MelonLoader;
using OFFGame.Battle;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

[assembly: MelonInfo(typeof(Offipelago.Offipelago), "Offipelago", "1.0.0", "WestCraft15")]
[assembly: MelonGame("Fangamer", "OFF")]

namespace Offipelago
{
	public class Offipelago : MelonMod
	{
		public static ArchipelagoSession session;

		public override void OnInitializeMelon()
		{
			new GameObject("Explorer", [typeof(OffExplorer)])
			{
				hideFlags = HideFlags.HideAndDontSave
			};

			session = ArchipelagoSessionFactory.CreateSession("localhost");

			session.ConnectAsync().Wait();

			session.LoginAsync("Off", "West", ItemsHandlingFlags.AllItems).Wait();
		}
	}

	[HarmonyPatch(typeof(BATMain), "LoadEncounter")]
	public class LoadEncounter_Patch
	{
		static void Prefix(ref BATEncounter encounter)
		{
			//encounter.batter.active = false;
			//encounter.judge.active = !encounter.boxxer.active;
		}
	}

	[HarmonyPatch(typeof(FPGOverworldMode), "ApplyMapPatch", MethodType.Enumerator)]
	public class ApplyMapPatch_Patch
	{
		public static readonly MethodInfo PatchInfo = SymbolExtensions.GetMethodInfo(() => Patch());

		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var found = 0;
			foreach (var instruction in instructions)
			{
				yield return instruction;
				if (instruction.opcode == OpCodes.Brfalse)
				{
					found++;
					if (found == 5)
					{
						yield return new CodeInstruction(OpCodes.Call, PatchInfo);
					}
				}
			}
		}

		static void Patch()
		{
			string patch = "Post_" + FPGOverworldMode.instance.gameState.map[..3];

			MethodInfo patchMethod = typeof(Patches).GetMethod(patch);

			if (patchMethod != null)
			{
				MelonLogger.Msg($"Found post patch for room {FPGOverworldMode.instance.gameState.map[..3]}");
				patchMethod.Invoke(null, []);
			}
		}
	}

	[HarmonyPatch(typeof(FPGOverworldMode), "DoMapTravel", MethodType.Enumerator)]
	public class DoMapTravel_Patch
	{
		public static readonly MethodInfo PatchInfo = SymbolExtensions.GetMethodInfo(() => Patch());

		static void Postfix()
		{
			if (FPGOverworldMode.instance.isReady)
			{
				MelonLogger.Msg("New room loaded");
				OffExplorer.instance.NewRoom();
			}
		}

		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var found = 0;
			foreach (var instruction in instructions)
			{
				yield return instruction;
				if (instruction.opcode == OpCodes.Brfalse)
				{
					found++;
					if (found == 3)
					{
						yield return new CodeInstruction(OpCodes.Call, PatchInfo);
					}
				}
			}
		}

		static void Patch()
		{
			string patch = "Pre_" + FPGOverworldMode.instance.gameState.map[..3];

			MethodInfo patchMethod = typeof(Patches).GetMethod(patch);

			if (patchMethod != null)
			{
				MelonLogger.Msg($"Found pre patch for room {FPGOverworldMode.instance.gameState.map[..3]}");
				patchMethod.Invoke(null, []);
			}
		}
	}
}