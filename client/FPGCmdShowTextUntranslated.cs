using FangamerRPG;

namespace Offipelago;

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
