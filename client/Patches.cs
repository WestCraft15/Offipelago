using FangamerRPG;

namespace Offipelago;

internal static partial class Patches
{
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
