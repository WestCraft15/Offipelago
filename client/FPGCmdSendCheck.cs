using FangamerRPG;
using MelonLoader;

namespace Offipelago;

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
