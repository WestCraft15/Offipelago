using FangamerRPG;
using HarmonyLib;
using MelonLoader;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Offipelago
{
    internal static class Patches
	{
		private static GameObject? _actorPrefab = null;

		public static GameObject GetActorPrefab()
		{
			if (_actorPrefab == null)
			{
				_actorPrefab = new() { name = "OffipelagoActorPrefab", hideFlags = HideFlags.HideAndDontSave };
				_actorPrefab.SetActive(true);
				GameObject sprite = new() { name = "Sprite" };
				sprite.transform.parent = _actorPrefab.transform;
				var renderer = sprite.AddComponent<SpriteRenderer>();
				renderer.sortingLayerName = "Foreground";
				FPGLogicActor actor = _actorPrefab.AddComponent<FPGLogicActor>();
				actor.states = [new FPGLogicActorState(actor)];
				actor.states[0].collision = true;
				actor.enabled = true;
			}
			return _actorPrefab;
		}

		/// <summary>
		/// Gets an unpatched actor by it's <paramref name="eventID"/>. You will rarely need to use this instead of <c>GetActor()</c>.
		/// </summary>
		/// <param name="eventID">The id of the actor to find.</param>
		/// <returns>The actor, if one was found. Null otherwise.</returns>
		private static FPGLogicActor GetUnpatchedActor(int eventID)
		{
			return Object.FindObjectsByType<FPGLogicActor>(FindObjectsSortMode.None).FirstOrDefault(actor => actor.eventID == eventID && !actor.gameObject.scene.name.StartsWith("patch"));
		}

		/// <summary>
		/// Gets a patched actor by it's <paramref name="eventID"/>. You will rarely need to use this instead of <c>GetActor()</c>.
		/// </summary>
		/// <param name="eventID">The id of the actor to find.</param>
		/// <returns>The actor, if one was found. Null otherwise.</returns>
		private static FPGLogicActor GetPatchedActor(int eventID)
		{
			return Object.FindObjectsByType<FPGLogicActor>(FindObjectsSortMode.None).FirstOrDefault(actor => actor.eventID == eventID && actor.gameObject.scene.name.StartsWith("patch"));
		}

		/// <summary>
		/// Gets an actor by it's <paramref name="eventID"/>.
		/// </summary>
		/// <param name="eventID">The id of the actor to find.</param>
		/// <returns>The actor, if one was found. Null otherwise.</returns>
		private static FPGLogicActor GetActor(int eventID)
		{
			return GetPatchedActor(eventID) ?? GetUnpatchedActor(eventID);
		}

		[HarmonyPatch(typeof(FPGLogicActor), "Awake")]
		public class FixNullReferenceException
		{
			// Fixes a NullReferenceException in FPGLogicActor.Awake()
			static bool Prefix(ref FPGLogicActor __instance, ref SpriteRenderer ____actorRenderer)
			{
				____actorRenderer = __instance.GetComponentInChildren<SpriteRenderer>(includeInactive: true);
				__instance.transform.position = Vector3Int.FloorToInt(__instance.transform.position);
				if (__instance.states is not null)
				{
					foreach (FPGLogicActorState state in __instance.states)
					{
						state.SetOwner(__instance);
					}
				}
				return false;
			}
		}

		/// <summary>
		/// Create a new actor in the room with the specified <paramref name="eventID"/>.
		/// Don't forget to call actor.Init() once you're done setting it up.
		/// </summary>
		/// <param name="eventID">The id of the actor to create.</param>
		/// <returns>The newly created actor. Or null, if an actor with that ID already exists.</returns>
		private static FPGLogicActor CreateActor(int eventID, int x, int y, GridDirection facing = GridDirection.South)
		{
			var actor = FPGOverworldMode.instance.SpawnNewLogicActor(GetActorPrefab(), new Vector2Int(x, y), facing);
			actor.gameObject.hideFlags = HideFlags.None;
			actor.gameObject.transform.SetParent(GameObject.Find("Actors").transform);
			actor.GetComponent<FPGLogicActor>().eventID = eventID;
			actor.name = $"EV{eventID:0000}";
			return actor;
		}

		/// <summary>
		/// Automatically patches a chest actor to instead send a check.
		/// </summary>
		/// <param name="eventID">The eventId of the chest.</param>
		/// <param name="locationID">The locationID of the check.</param>
		/// <param name="textCmd">Optionally specify the index of the FPGCmdShowText command. Default 4.</param>
		/// <param name="inventoryCommand">Optionally specify the index of the FPGCmdChangeInventory command. Default 5.</param>
		private static void PatchChest(int eventID, long locationID, int textCmd = 4, int inventoryCommand = 5)
		{
			var actor = GetActor(eventID);
			actor.states[0].commands[textCmd] = CreateText("A check has been found.");
			actor.states[0].commands[inventoryCommand] = new FPGCmdSendCheck(locationID);
		}

		/// <summary>
		/// Helper function to create a new line of dialog.
		/// </summary>
		/// <param name="text">The text string to display.</param>
		/// <param name="old">Optionally copy the parameters of the FPGCmdShowText you will replace.</param>
		/// <returns>A new FPGCmdShowTextUntranslated that contains your text string.</returns>
		private static FPGCmdShowTextUntranslated CreateText(string text, FPGCmdShowText? old = null)
		{
			return new FPGCmdShowTextUntranslated(old)
			{
				text = text
			};
		}

		private static FPGSpriteSheet[]? _sheets = null;

		private static FPGSpriteSheet GetSpriteSheet(string name)
		{
			_sheets = Resources.FindObjectsOfTypeAll<FPGSpriteSheet>();
			return _sheets.FirstOrDefault(o => o.name == name);
		}

		public static void Post_003()
		{
			PatchChest(10, 1000000);
			PatchChest(11, 1000001);
		}

		public static void Post_015()
		{
			PatchChest(21, 1000002, 3, 4);
		}

		public static void Post_018()
		{
			PatchChest(13, 1000003, 3, 4);
		}

		public static void Post_020()
		{
			PatchChest(33, 1000006);
			PatchChest(34, 1000007);
		}

		public static void Post_021()
		{
			PatchChest(5, 1000008);
		}

		public static void Post_030()
		{
			PatchChest(12, 1000015);
			PatchChest(24, 1000016);
			PatchChest(26, 1000017);
		}

		public static void Post_032()
		{
			PatchChest(36, 1000010);
			PatchChest(37, 1000011);
			PatchChest(38, 1000012);
			PatchChest(39, 1000013);
			PatchChest(40, 1000014);
			PatchChest(41, 1000015);
		}

		public static void Post_039()
		{
			PatchChest(1, 1000022);
			PatchChest(2, 1000023, 5, 6);
		}

		public static void Post_040()
		{
			PatchChest(6, 1000024);
		}

		public static void Post_054()
		{
			PatchChest(3, 1000030);
		}

		public static void Post_067()
		{
			PatchChest(6, 1000031);
		}

		public static void Post_112()
		{
			PatchChest(75, 1000031);
		}

		public static void Post_114()
		{
			PatchChest(129, 1000033);
		}

		public static void Post_121()
		{
			PatchChest(1, 1000034);
			PatchChest(2, 1000035);
		}

		public static void Post_122()
		{
			PatchChest(1, 1000041);
		}

		public static void Post_124()
		{
			PatchChest(2, 1000036);
			PatchChest(3, 1000037);
			PatchChest(4, 1000038);
		}

		public static void Post_125()
		{
			PatchChest(1, 1000039);
			PatchChest(4, 1000040);
		}

		public static void Post_134()
		{
			PatchChest(5, 1000042, 3, 0);
		}

		public static void Post_139()
		{
			PatchChest(3, 1000049);
			PatchChest(4, 1000050);
			PatchChest(5, 1000051);
			PatchChest(6, 1000052);
			PatchChest(7, 1000053);
			PatchChest(8, 1000054);
		}

		public static void Post_140()
		{
			PatchChest(2, 1000044, 3, 1);
			PatchChest(3, 1000045, 3, 1);
			PatchChest(4, 1000046, 3, 1);
			PatchChest(5, 1000047, 3, 1);
			PatchChest(69, 1000048, 3, 1);
		}

		public static void Post_144()
		{
			PatchChest(13, 1000043, 3, 1);
			GetActor(14).states[1].commands[49] = CreateText("...\\!The grand prize was a \ncheck?");
		}

		public static void Post_210()
		{
			PatchChest(6, 1000055);
		}

		public static void Post_217()
		{
			PatchChest(3, 1000056, 10, 9);
		}

		public static void Post_221()
		{
			PatchChest(15, 1000057);
		}

		public static void Post_222()
		{
			PatchChest(3, 1000058);
		}
	}

	// A version of FPGCmdShowText that doesn't try to get a translated string.
	internal class FPGCmdShowTextUntranslated : FPGCmdShowText
	{
		public FPGCmdShowTextUntranslated(FPGCmdShowText? old)
		{
			if (old != null)
			{
				text = old.text;
				autoOff = old.autoOff;
			}
		}

		public override void ApplyLocalization() { }
	}

	// An FPGCommand that sends an Archipelago check when activated.
	internal class FPGCmdSendCheck(long locationID) : FPGCommand
	{
		public long locationID = locationID;

		public override void Activate(FPGLogicInterpreter logic)
		{
			Offipelago.session.Locations.CompleteLocationChecksAsync(locationID);

			MelonLogger.Msg($"Sent Check: {locationID}");
		}
	}
}
