using Archipelago.MultiClient.Net;
using FangamerRPG;
using HarmonyLib;
using MelonLoader;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

[assembly: MelonInfo(typeof(Offipelago.Offipelago), "Offipelago", "1.0.0", "WestCraft15")]
[assembly: MelonGame("Fangamer", "OFF")]

namespace Offipelago;

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

		//session.ConnectAsync().Wait();

		//session.LoginAsync("Off", "West", ItemsHandlingFlags.AllItems).Wait();
	}
}

[HarmonyPatch(typeof(FPGOverworldMode), "ApplyMapPatch", MethodType.Enumerator)]
public class ApplyPostPatch
{
	public static readonly MethodInfo PatchInfo = SymbolExtensions.GetMethodInfo(() => Patch());

	// Places a call to the Patch() function inside of the FPGOverworldMode.ApplyMapPatch() function.
	// It has to be done using the transpiled code because the function is async.
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

	// Applies a post patch to the room, if the corresponding function is found in the Patches class.
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
public class ApplyPrePatch
{
	public static readonly MethodInfo PatchInfo = SymbolExtensions.GetMethodInfo(() => Patch());

	// Calls OffExplorer.NewRoom() when a new room is loaded.
	static void Postfix()
	{
		if (FPGOverworldMode.instance.isReady)
		{
			OffExplorer.instance.NewRoom();
		}
	}

	// Places a call to the Patch() function inside of the FPGOverworldMode.DoMapTravel() function.
	// It has to be done using the transpiled code because the function is async.
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

	// Applies a pre patch to the room, if the corresponding function is found in the Patches class.
	// You'll likely never need to make a pre patch, unless the room you're patching isn't already patched by the game.
	// Confusing, huh?
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
