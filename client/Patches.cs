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

		public static void Post_020()
		{
			PatchChest(33, 1000006);
			PatchChest(34, 1000007);
		}

		public static void Post_021()
		{
			PatchChest(5, 1000008);
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
