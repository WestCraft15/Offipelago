using FangamerRPG;
using HarmonyLib;
using MelonLoader;
using OFFGame.Battle;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Offipelago;

public class OffExplorer : MonoBehaviour
{
	int current_actor = 1;

	readonly InputAction next_room_action = new(binding: "/Keyboard/p");
	readonly InputAction prev_room_action = new(binding: "/Keyboard/o");
	readonly InputAction toggle_alpha_action = new(binding: "/Keyboard/1");
	readonly InputAction toggle_omega_action = new(binding: "/Keyboard/2");
	readonly InputAction toggle_epsilon_action = new(binding: "/Keyboard/3");
	readonly InputAction level_alpha_action = new(binding: "/Keyboard/7");
	readonly InputAction level_omega_action = new(binding: "/Keyboard/8");
	readonly InputAction level_epsilon_action = new(binding: "/Keyboard/9");
	readonly InputAction toggle_noclip_action = new(binding: "/Keyboard/n");
	readonly InputAction up_action = new(binding: "/Keyboard/i");
	readonly InputAction down_action = new(binding: "/Keyboard/k");
	readonly InputAction accept_action = new(binding: "/Keyboard/l");
	readonly InputAction list_action = new(binding: "/Keyboard/j");

	public static OffExplorer instance;

	private GameObject uiPanel;
	private TextMeshProUGUI text;

	public bool noclip = false;

	struct Message(string message)
	{
		public string message = message;
		public double time = 3;
	}

	private static readonly List<Message> messages = [];

	/// <summary>
	/// Adds a message to the on-screen display.
	/// </summary>
	/// <param name="message">The message to display.</param>
	public static void AddMessage(string message)
	{
		messages.Add(new(message));
		if (messages.Count() > 13)
		{
			messages.RemoveAt(0);
		}
	}

	// Runs once when the instance is created.
	void Start()
	{
		MelonLogger.Msg("Started OFF Explorer v1.0");

		next_room_action.Enable();
		prev_room_action.Enable();
		toggle_alpha_action.Enable();
		toggle_omega_action.Enable();
		toggle_epsilon_action.Enable();
		level_alpha_action.Enable();
		level_omega_action.Enable();
		level_epsilon_action.Enable();
		toggle_noclip_action.Enable();
		up_action.Enable();
		down_action.Enable();
		accept_action.Enable();
		list_action.Enable();

		instance = this;
	}

	// Runs on every frame.
	// Handles keybinds, as well as updating the message display.
	void Update()
	{
		if (FPGOverworldMode.instance is null)
			return;

		if (uiPanel is null || text.IsDestroyed())
		{
			MelonLogger.Msg("Creating Exporer Panel");
			uiPanel = Instantiate(FPGOverworldMode.instance.GetComponentInChildren<FPGHintPanel>(true).gameObject);
			uiPanel.name = "ExplorerPanel";
			uiPanel.GetComponent<FPGHintPanel>().enabled = false;
			uiPanel.GetComponent<VerticalLayoutGroup>().enabled = false;
			uiPanel.transform.SetParent(FPGOverworldMode.instance.transform.Find("UI"), false);
			text = uiPanel.GetComponentInChildren<TextMeshProUGUI>();
			text.font = FPGAppMaster.instance.romanFontWithShadow;
			text.fontSize = 16f;
			text.margin = Vector4.zero;
		}

		if (toggle_noclip_action.WasPressedThisFrame())
		{
			AddMessage(noclip ? "Noclip deactivated" : "Noclip activated");
			noclip = !noclip;
		}

		if (next_room_action.WasPressedThisFrame())
		{
			AddMessage("Loading next scene");
			FPGOverworldMode.instance.PrepareMapTravel(System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1)), new Vector2Int(0, 0), GridDirection.North);
		}

		if (prev_room_action.WasPressedThisFrame())
		{
			AddMessage($"Loading previous scene");
			FPGOverworldMode.instance.PrepareMapTravel(System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(SceneManager.GetActiveScene().buildIndex - 1)), new Vector2Int(0, 0), GridDirection.North);
		}

		if (toggle_alpha_action.WasPressedThisFrame())
		{
			var enabled = FPGOverworldMode.instance.gameState.GetActivePartyMembers().Any((m) => m.characterID == 2);
			if (enabled)
				FPGOverworldMode.instance.RemoveFromParty(2);
			else
				FPGOverworldMode.instance.AddToParty(2);
			AddMessage(enabled ? $"Alpha disabled" : "Alpha enabled");
		}

		if (toggle_omega_action.WasPressedThisFrame())
		{
			var enabled = FPGOverworldMode.instance.gameState.GetActivePartyMembers().Any((m) => m.characterID == 3);
			if (enabled)
				FPGOverworldMode.instance.RemoveFromParty(3);
			else
				FPGOverworldMode.instance.AddToParty(3);
			AddMessage(enabled ? $"Omega disabled" : "Omega enabled");
		}

		if (toggle_epsilon_action.WasPressedThisFrame())
		{
			var enabled = FPGOverworldMode.instance.gameState.GetActivePartyMembers().Any((m) => m.characterID == 4);
			if (enabled)
				FPGOverworldMode.instance.RemoveFromParty(4);
			else
				FPGOverworldMode.instance.AddToParty(4);
			AddMessage(enabled ? $"Epsilon disabled" : "Epsilon enabled");
		}

		if (level_alpha_action.WasPressedThisFrame())
		{
			FPGOverworldMode.instance.gameState.partyMemberStates[3].LevelUp(1);
			AddMessage("Alpha leveled up");
		}

		if (level_omega_action.WasPressedThisFrame())
		{
			FPGOverworldMode.instance.gameState.partyMemberStates[4].LevelUp(1);
			AddMessage("Omega leveled up");
		}

		if (level_epsilon_action.WasPressedThisFrame())
		{
			FPGOverworldMode.instance.gameState.partyMemberStates[5].LevelUp(1);
			AddMessage("Epsilon leveled up");
		}

		if (up_action.WasPressedThisFrame())
		{
			current_actor++;
		}

		if (down_action.WasPressedThisFrame())
		{
			current_actor--;
		}

		if (accept_action.WasPressedThisFrame())
		{
			var actor = GetActor(current_actor);
			if (actor is null)
				return;
			string states = "";
			for (var i = 0; i < actor.states.Count; i++)
			{
				states += $"    {i}:\n";
				var indent = 0;
				for (var j = 0; j < actor.states[i].commands.Count; j++)
				{
					var subIndent = 0;
					var desc = CommandDescription(actor.states[i].commands[j], ref indent, ref subIndent);
					states += $"{Indent(indent - subIndent)}      {j}: {desc}\n";
				}
			}
			MelonLogger.Msg($"\nInfo for actor {current_actor} ({actor.name}):\n  Position: {actor.GetTruePosition()}\n  States:\n{states}");
			AddMessage($"Actor info printed to console");
		}

		if (list_action.WasPressedThisFrame())
		{
			ScanRoom();
		}

		if (text is not null)
		{
			uiPanel.SetActive(true);

			for (var i = 0; i < messages.Count(); i++)
			{
				Message m = messages[i];
				m.time -= Time.deltaTime;
				messages[i] = m;
			}

			messages.RemoveAll(m => m.time <= 0);
			var actor = GetActor(current_actor);
			if (actor is null)
				text.text = $"No actor with id {current_actor}\n";
			else
				text.text = $"Selected actor: {current_actor} ({actor.name})\n";
			text.text += messages.Join((m) => m.message, "\n");
		}
	}

	// Run when a new room is loaded.
	public void NewRoom()
	{
		//ScanRoom(); // not super useful right now
		current_actor = 1;
	}

	// Scans all actors in a room and logs any that are of note.
	public void ScanRoom()
	{
		AddMessage($"Important actors for room {FPGOverworldMode.instance.GetCurrentMapComponent().GetMapName()[..3]} printed to console");

		var actors = (FPGOverworldMode.instance.GetCurrentMapComponent()?.GetLogicActors()) ?? [];

		MelonLogger.Msg($"Important actors for room {FPGOverworldMode.instance.GetCurrentMapComponent().GetMapName()[..3]}:");

		foreach (var actor in actors)
		{
			bool hasText = false;
			bool isChest = false;
			for (var i = 0; i < actor.states.Count; i++)
			{
				for (var j = 0; j < actor.states[i].commands.Count; j++)
				{
					if (!hasText && actor.states[i].commands[j].GetType().Name.Contains("Text"))
					{
						MelonLogger.Msg($"Actor {actor.eventID} ({actor.name}) contains text");
						hasText = true;
					}
					if (!isChest && actor.states[i].commands[j].GetType() == typeof(FPGCmdChangeInventory))
					{
						MelonLogger.Msg($"Actor {actor.eventID} ({actor.name}) is possibly a check");
						isChest = true;
					}
				}
			}
		}
	}

	/// <summary>
	/// Gets an actor by it's <paramref name="eventID"/>.
	/// </summary>
	/// <param name="eventID">The id of the actor to find.</param>
	/// <returns>The actor, if one was found. Null otherwise.</returns>
	private static FPGLogicActor GetActor(int eventID)
	{
		return FindObjectsByType<FPGLogicActor>(FindObjectsSortMode.None).FirstOrDefault(actor => actor.eventID == eventID);
	}

	// Gets a string representation of an FPGCommand.
	private static string CommandDescription(FPGCommand command, ref int indent, ref int subIndent)
	{
		switch (command.GetType().Name)
		{
			case "FPGCmdShowChoices":
				indent += 2;
				subIndent = 2;
				return "ShowChoices";

			case "FPGCmdIfChoice":
				subIndent = 1;
				return $"If (Choice == \"{((FPGCmdIfChoice)command).choice}\")";

			case "FPGCmdIf":
				indent++;
				subIndent = 1;
				return $"If ({FormatIfConditions(((FPGCmdIf)command).conditions)})";

			case "FPGCmdElse":
				subIndent = 1;
				return "Else";

			case "FPGCmdEndIf":
				indent--;
				return "EndIf";

			case "FPGCmdEndChoices":
				indent -= 2;
				return "EndChoices";

			case "FPGCmdGoto":
				return $"Goto {((FPGCmdGoto)command).label}";

			case "FPGCmdLabel":
				return $"Label {((FPGCmdLabel)command).name}:";

			case "FPGCmdShowTextUntranslated":
			case "FPGCmdShowText":
				return $"ShowText {((FPGCmdShowText)command).text.Replace("\n", "\\n")}";

			case "FPGCmdChangeInventory":
				return $"{(((FPGCmdChangeInventory)command).operation == FPGCmdChangeInventory.OpType.Increase ? "Increase" : "Decrease")} {((FPGCmdChangeInventory)command).item.itemInfo.GetLocalizedName()} by {(((FPGCmdChangeInventory)command).valueType == FPGCmdChangeInventory.ValueType.Constant ? ((FPGCmdChangeInventory)command).valueConstant : $"variable {((FPGCmdChangeInventory)command).valueVariable}")}";

			default:
				return command.GetType().Name;
		}
	}

	// Separate function specifically to format the conditions in an FPGCmdIf.
	private static string FormatIfConditions(List<FPGCondition> conditions)
	{
		string cond = "";

		for (var i = 0; i < conditions.Count; i++)
		{
			if (i > 0) { cond += " && "; }
			cond += "";
			cond += conditions[i].GetType().Name switch
			{
				"FPGCondSwitch" => $"{(((FPGCondSwitch)conditions[i]).value ? "" : "!")}Switch{((FPGCondSwitch)conditions[i]).variable}",
				"FPGCondVariable" => $"Variable{((FPGCondVariable)conditions[i]).variable} {FormatOperandType(((FPGCondVariable)conditions[i]).operation)} {(((FPGCondVariable)conditions[i]).valueType == FPGCondVariable.ValueType.Constant ? $"{((FPGCondVariable)conditions[i]).valueConstant}" : $"Variable{((FPGCondVariable)conditions[i]).valueVariable}")}",
				_ => conditions[i].GetType().Name,
			};
		}

		return cond;
	}

	// Converts FPGCondVariable.OpType into a more standard notation.
	private static string FormatOperandType(FPGCondVariable.OpType opType)
	{
		return opType switch
		{
			FPGCondVariable.OpType.Equals => "==",
			FPGCondVariable.OpType.NotEquals => "!=",
			FPGCondVariable.OpType.EqOrMore => ">=",
			FPGCondVariable.OpType.EqOrLess => "<=",
			FPGCondVariable.OpType.More => ">",
			FPGCondVariable.OpType.Less => "<",
			_ => "?",
		};
	}

	// Indents FPGCommand based on their depth inside of FPGCmdIf/FPGCmdIfChoice.
	// Could probably use the built in FPGCommand.indent, but I didn't trust it to be accurate.
	private static string Indent(int indent)
	{
		string str = "";
		for (var i = 0; i < indent; i++)
		{
			str += "  ";
		}
		return str;
	}

	// Leftover from an earlier version.
	// I kept it just in case I needed to patch encounters at some point.
	[HarmonyPatch(typeof(BATMain), "LoadEncounter")]
	public class LoadEncounter_Patch
	{
		static void Prefix(ref BATEncounter encounter) { }
	}

	// Activates noclip by simply telling the game that all movement is valid.
	[HarmonyPatch(typeof(FPGLogicActor), "ValidateMove")]
	public class NoClipPatch
	{
		static void Postfix(ref FPGLogicActor __instance, ref GridMoveInfo __result)
		{
			if (__instance is FPGOverworldPawn && instance.noclip)
			{
				__result.actorMoveValid = true;
				__result.boatMoveValid = true;
			}
		}
	}
}
