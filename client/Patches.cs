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

		private static FPGOverworldMode Overworld { get => FPGOverworldMode.instance; }
		private static FPGDatabase Database { get => FPGOverworldMode.instance.globalDatabase; }

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
		/// <param name="x">The x coordinate of the new actor.</param>
		/// <param name="y">The y coordinate of the new actor.</param>
		/// <param name="facing">Optionally specify a facing direction for the actor. Default is South</param>
		/// <returns>The newly created actor. Or null, if an actor with that ID already exists.</returns>
		private static FPGLogicActor CreateActor(int eventID, Vector2Int pos, GridDirection facing = GridDirection.South)
		{
			if (GetActor(eventID) is not null) return null;
			var actor = Overworld.SpawnNewLogicActor(GetActorPrefab(), pos, facing);
			actor.gameObject.hideFlags = HideFlags.None;
			actor.gameObject.transform.SetParent(GameObject.Find("Actors").transform);
			actor.GetComponent<FPGLogicActor>().eventID = eventID;
			actor.name = $"EV{eventID:0000}";
			return actor;
		}

		/// <summary>
		/// Destroys an actor with the specified <paramref name="eventID"/>.
		/// </summary>
		/// <param name="eventID">The id of the actor to destroy.</param>
		private static void DestroyActor(int eventID)
		{
			Object.DestroyImmediate(GetActor(eventID).gameObject);
			Overworld.GetCurrentMapComponent().BuildLogicActorCache();
		}

		/// <summary>
		/// Destroys actors with the specified <paramref name="eventIDs"/>.
		/// </summary>
		/// <param name="eventIDs">The ids of the actors to destroy.</param>
		private static void DestroyActors(params int[] eventIDs)
		{
			foreach (var i in eventIDs)
				Object.DestroyImmediate(GetActor(i).gameObject);
			Overworld.GetCurrentMapComponent().BuildLogicActorCache();
		}

		/// <summary>
		/// Destroys actors with the specified <paramref name="eventIDs"/>.
		/// </summary>
		/// <param name="eventIDs">The ids of the actors to destroy.</param>
		private static void DestroyActors(IEnumerable<int> eventIDs)
		{
			foreach (var i in eventIDs)
				Object.DestroyImmediate(GetActor(i).gameObject);
			Overworld.GetCurrentMapComponent().BuildLogicActorCache();
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

		/// <summary>
		/// Helper function to create a new line of dialog.
		/// </summary>
		/// <param name="text">The text string to display.</param>
		/// <param name="indent">The indent of the command.</param>
		/// <returns>A new FPGCmdShowTextUntranslated that contains your text string.</returns>
		private static FPGCmdShowTextUntranslated CreateText(string text, int indent)
		{
			return new FPGCmdShowTextUntranslated()
			{
				text = text,
				indent = indent
			};
		}

		// The possible colors for a cube port.
		enum CubePortColor
		{
			Yellow,
		}

		/// <summary>
		/// Helper function to create a new cube port.
		/// </summary>
		/// <param name="actorID">The id of the new actor.</param>
		/// <param name="pos">The position of the cube port.</param>
		/// <param name="name">The name of the cube port.</param>
		/// <param name="color">The color of the cube, typically based on the zone.</param>
		/// <param name="activationCommands">A list of FPGCommands to run on successful activation.</param>
		/// <returns>The new cube port actor.</returns>
		private static FPGLogicActor CreateCubePort(int actorID, Vector2Int pos, bool isPuzzle, string name, CubePortColor color, IEnumerable<FPGCommand> activationCommands)
		{
			var cube_port = CreateActor(actorID, pos);
			cube_port.states[0].spriteSheet = GetSpriteSheet("cubeDown_yellow");
			cube_port.states[0].commands = [
				new FPGCmdShowTextPortrait() { portrait = GetSprite("le batteur_0") },
				CreateText("It's a cube port."),
				new FPGCmdIf(),
					new FPGCmdShowChoices() { indent = 1, choices = ["Place cube", "Do nothing"] },
					new FPGCmdIfChoice() { indent = 1, choice = "Place cube" },
						new FPGCmdShowAnimatedEffect() { indent = 2, effectID = 227, targetActor = new LogicActorReference(actorID) },
						new FPGCmdShowTextPortrait() { indent = 2 },
						CreateText("The cube slots into the hole.", 2),
					new FPGCmdEndIf() { indent = 1 },
				new FPGCmdElse(),
					CreateText($"It's labeled “{name}”", 1),
				new FPGCmdEndIf(),
			];
			cube_port.states[0].commands.InsertRange(8, activationCommands.Select(c => { c.indent += 2; return c; }));
			cube_port.Init();
			return cube_port;
		}


		/// <summary>
		/// Helper function to create a new cube port.
		/// </summary>
		/// <param name="actorID">The id of the new actor.</param>
		/// <param name="pos">The position of the cube port.</param>
		/// <param name="name">The name of the cube port.</param>
		/// <param name="color">The color of the cube, typically based on the zone.</param>
		/// <param name="activationCommand">An FPGCommand to run on successful activation.</param>
		/// <returns>The new cube port actor.</returns>
		private static FPGLogicActor CreateCubePort(int actorID, Vector2Int pos, bool isPuzzle, string name, CubePortColor color, FPGCommand activationCommand)
		{
			return CreateCubePort(actorID, pos, isPuzzle, name, color, [activationCommand]);
		}

		private static Dictionary<string, FPGSpriteSheet> _sheets = [];

		/// <summary>
		/// Gets the sprite sheet with the specified name.
		/// </summary>
		/// <param name="name">The name of the sprite sheet to find.</param>
		/// <returns>The sprite sheet, or null if none were found.</returns>
		private static FPGSpriteSheet GetSpriteSheet(string name)
		{
			// Only refresh cache if a missing sheet is requested
			if (!_sheets.ContainsKey(name))
			{
				_sheets = Resources.FindObjectsOfTypeAll<FPGSpriteSheet>().ToDictionary(s => s.name);
			}
			return _sheets.GetValueSafe(name);
		}

		private static Dictionary<string, Sprite> _sprites = [];

		/// <summary>
		/// Gets the sprite with the specified name.
		/// Due to name collisions, this rarely may not return the expected sprite.
		/// Collisions are mitigated first by adding textureRect.x and textureRect.y,
		/// then by adding texture.name instead. If there's still a collision, the sprite is simply excluded.
		/// Please message West if you're having issues.
		/// </summary>
		/// <param name="name">The name of the sprite to find.</param>
		/// <returns>The sprite, or null if none were found.</returns>
		private static Sprite GetSprite(string name)
		{
			// Only refresh cache if a missing sheet is requested
			if (!_sprites.ContainsKey(name))
			{
				var sprites = Resources.FindObjectsOfTypeAll<Sprite>();
				foreach (var sprite in sprites) {
					if (!_sprites.TryAdd(sprite.name, sprite))
						if (!_sprites.TryAdd($"{sprite.name}-{sprite.textureRect.x}-{sprite.textureRect.y}", sprite))
							_sprites.TryAdd($"{sprite.name}-{sprite.texture.name}", sprite);
				}
			}
			return _sprites.GetValueSafe(name);
		}

		public static void Post_003()
		{
			PatchChest(10, 1000000);
			PatchChest(11, 1000001);
		}

		public static void Post_004()
		{
			var pablo = GetActor(14);
			pablo.states[0].commands = [
				pablo.states[0].commands[0],
				pablo.states[0].commands[1],
				CreateText("In this place, puzzles remain dormant until one acquires the necessary “Puzzle Cube.”"),
				CreateText("Once it is found, I implore you to locate the “Cube Port” within the room and place the aforementioned object therein."),
				CreateText("You will then find the puzzle to be activated, and thusly solvable."),
				CreateText("I shall grant you the Puzzle Cube for this room. I believe you will find it quite useful for this upcoming obstacle."),
				new FPGCmdShowTextPortrait(),
				CreateText("A Puzzle Cube (Zone 0 Top Floor) has been found."),
			];

			CreateCubePort(15, new(8, 8), true, "Zone 0 Top Floor", CubePortColor.Yellow, []);
		}

		public static void Post_006()
		{
			var pablo = GetActor(22);
			pablo.states[0].commands = [
				pablo.states[0].commands[0],
				pablo.states[0].commands[1],
				CreateText("Ah, a similar concept. How delightful."),
				CreateText("This room requires an “Access Cube” to proceed."),
				CreateText("It functions in much the same way as a Puzzle Cube, but will simply remove the obstruction without the need to activate any additional floating geometric objects."),
				CreateText("Here, I shall once again grant you the required artifact posthaste."),
				new FPGCmdShowTextPortrait(),
				CreateText("A Puzzle Cube (Zone 0 Middle Floor) has been found."),
			];

			CreateCubePort(23, new(11, 8), false, "Zone 0 Middle Floor", CubePortColor.Yellow, GetActor(4).states[1].commands.GetRange(9, 5));

			DestroyActors(1, 2, 3, 4, 8, 9, 12, 13, 14, 15, 16, 17, 18, 19);
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
		public FPGCmdShowTextUntranslated(FPGCmdShowText? old = null)
		{
			if (old != null)
			{
				text = old.text;
				autoOff = old.autoOff;
				indent = old.indent;
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
