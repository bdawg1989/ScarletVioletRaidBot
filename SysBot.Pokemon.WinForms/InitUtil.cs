using PKHeX.Core;

namespace SysBot.Pokemon.WinForms
{
    public static class InitUtil
    {
        public static void InitializeStubs(ProgramMode mode)
        {
            SaveFile sav = mode switch
            {
                ProgramMode.SV => new SAV9SV(),
                _ => throw new System.ArgumentOutOfRangeException(nameof(mode)),
            };
        }
    }
}