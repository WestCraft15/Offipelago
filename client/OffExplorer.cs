using FangamerRPG;
using MelonLoader;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Offipelago
{
	public class OffExplorer : MonoBehaviour
	{
		int current_actor = 1;
		readonly InputAction next_room_action = new(binding: "/Keyboard/p");
		readonly InputAction prev_room_action = new(binding: "/Keyboard/o");
		readonly InputAction up_action = new(binding: "/Keyboard/i");
		readonly InputAction down_action = new(binding: "/Keyboard/k");
		readonly InputAction accept_action = new(binding: "/Keyboard/l");
		readonly InputAction list_action = new(binding: "/Keyboard/j");

		public static OffExplorer instance;

		void Start()
		{
			MelonLogger.Msg("Started OFF Explorer v1.0");

			next_room_action.Enable();
			prev_room_action.Enable();
			up_action.Enable();
			down_action.Enable();
			accept_action.Enable();
			list_action.Enable();

			instance = this;
		}

		void Update()
		{
			if (next_room_action.WasPressedThisFrame())
			{
				MelonLogger.Msg("Loading next scene");
				FPGOverworldMode.instance.PrepareMapTravel(SceneManager.GetSceneByBuildIndex(SceneManager.GetSceneByName(FPGOverworldMode.instance.gameState.map).buildIndex + 1).name, new Vector2Int(0, 0), GridDirection.North);
			}

			if (prev_room_action.WasPressedThisFrame())
			{
				MelonLogger.Msg($"Loading previous scene");
				FPGOverworldMode.instance.PrepareMapTravel(SceneManager.GetSceneByBuildIndex(SceneManager.GetSceneByName(FPGOverworldMode.instance.gameState.map).buildIndex - 1).name, new Vector2Int(0, 0), GridDirection.North);
			}

			if (up_action.WasPressedThisFrame())
			{
				current_actor++;
				var actor = GetActor(current_actor);
				if (actor is null)
					MelonLogger.Warning($"No actor with id {current_actor}");
				else
					MelonLogger.Msg($"Selected actor {actor.name} ({current_actor})");
			}

			if (down_action.WasPressedThisFrame())
			{
				current_actor--;
				var actor = GetActor(current_actor);
				if (actor is null)
					MelonLogger.Warning($"No actor with id {current_actor}");
				else
					MelonLogger.Msg($"Selected actor {actor.name} ({current_actor})");
			}

			if (accept_action.WasPressedThisFrame())
			{
				var actor = GetActor(current_actor);
				if (actor is null)
				{
					MelonLogger.Warning($"No actor with id {current_actor}");
					return;
				}
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
				MelonLogger.Msg($"\nActor info for {actor.name} ({current_actor}):\n  States:\n{states}");
			}

			if (list_action.WasPressedThisFrame())
			{
				NewRoom();
			}
		}

		public void NewRoom()
		{
			var actors = (FPGOverworldMode.instance.GetCurrentMapComponent()?.GetLogicActors()) ?? [];

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
							MelonLogger.Msg($"Actor {actor.name} ({actor.eventID}) contains text!");
							hasText = true;
						}
						if (!isChest && actor.states[i].commands[j].GetType() == typeof(FPGCmdChangeInventory))
						{
							MelonLogger.Msg($"Actor {actor.name} ({actor.eventID}) is likely a chest!");
							isChest = true;
						}
					}
				}
			}
		}

		private static FPGLogicActor GetActor(int eventID)
		{
			return FindObjectsByType<FPGLogicActor>(FindObjectsSortMode.None).FirstOrDefault(actor => actor.eventID == eventID);
		}

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

		private static string Indent(int indent)
		{
			string str = "";
			for (var i = 0; i < indent; i++)
			{
				str += "  ";
			}
			return str;
		}
	}
}
