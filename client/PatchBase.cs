using FangamerRPG;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Offipelago;

internal static partial class Patches
{
    private static GameObject? _actorPrefab;
    private static GameObject GetActorPrefab()
    {
        if (_actorPrefab is null)
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
    public enum CubePortColor
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

    private static readonly Dictionary<string, Sprite> _sprites = [];
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
            foreach (var sprite in sprites)
            {
                if (!_sprites.TryAdd(sprite.name, sprite))
                    if (!_sprites.TryAdd($"{sprite.name}-{sprite.textureRect.x}-{sprite.textureRect.y}", sprite))
                        _sprites.TryAdd($"{sprite.name}-{sprite.texture.name}", sprite);
            }
        }
        return _sprites.GetValueSafe(name);
    }
}
