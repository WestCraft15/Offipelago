using FangamerRPG;
using MelonLoader;
using System.Linq;
using UnityEngine;

namespace Offipelago
{
    internal static class Patches
    {
		private static FPGLogicActor GetUnpatchedActor(int eventID)
		{
			return Object.FindObjectsByType<FPGLogicActor>(FindObjectsSortMode.None).FirstOrDefault(actor => actor.eventID == eventID && !actor.gameObject.scene.name.StartsWith("patch"));
		}

		private static FPGLogicActor GetActor(int eventID)
		{
			return Object.FindObjectsByType<FPGLogicActor>(FindObjectsSortMode.None).FirstOrDefault(actor => actor.eventID == eventID && actor.gameObject.scene.name.StartsWith("patch"));
		}

		private static FPGLogicActor GetAnyActor(int eventID)
		{
			return GetActor(eventID) ?? GetUnpatchedActor(eventID);
		}

		private static void PatchChest(int eventID, long location_id, int textCmd = 4, int inventoryCommand = 5)
		{
			var actor = GetAnyActor(eventID);
			actor.states[0].commands[textCmd] = CreateText("A check has been found.");
			actor.states[0].commands[inventoryCommand] = new FPGCmdSendCheck(location_id);

			MelonLogger.Msg($"Created Check: {location_id} ({((FPGCmdSendCheck)actor.states[0].commands[inventoryCommand]).location_id})");
		}

		private static FPGCmdShowTextUntranslated CreateText(string text, FPGCmdShowText? old = null)
		{
			return new FPGCmdShowTextUntranslated(old)
			{
				text = text
			};
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
	}

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

	internal class FPGCmdSendCheck(long location_id) : FPGCommand
	{
		public long location_id = location_id;

		public override void Activate(FPGLogicInterpreter logic)
		{
			Offipelago.session.Locations.CompleteLocationChecksAsync(location_id);

			MelonLogger.Msg($"Sent Check: {location_id}");
		}
	}
}
