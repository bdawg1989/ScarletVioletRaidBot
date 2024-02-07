using System.ComponentModel;

namespace SysBot.Pokemon
{
    public enum DTFormat
    {
        MMDDYY,
        DDMMYY,
        YYMMDD,
    }

    public enum TeraCrystalType : int
    {
        Base = 0,
        Black = 1,
        Distribution = 2,
        Might = 3,
    }

    public enum LobbyMethodOptions
    {
        SkipRaid,
        OpenLobby,
        ContinueRaid
    }

    public enum RaidAction
    {
        AFK,
        MashA
    }

    public enum GameProgress : byte
    {
        Beginning = 0,
        UnlockedTeraRaids = 1,
        Unlocked3Stars = 2,
        Unlocked4Stars = 3,
        Unlocked5Stars = 4,
        Unlocked6Stars = 5,
        None = 6,
    }

    public enum EmbedColorOption
    {
        Blue,
        Green,
        Red,
        Gold,
        Purple,
        Teal,
        Orange,
        Magenta,
        LightGrey,
        DarkGrey
    }

    public enum ThumbnailOption
    {
        Gengar,
        Pikachu,
        Umbreon,
        Sylveon,
        Charmander,
        Jigglypuff,
        Flareon,
        Custom
    }

    public enum TeraIconType
    {
        Icon1, // Use special set
        Icon2 // Use boring set
    }

    public enum Action1Type
    {
        GoAllOut,
        HangTough,
        HealUp,
        Move1,
        Move2,
        Move3,
        Move4
    }

    public enum ScreenshotTimingOptions
    {
        [Description("1500 milliseconds")]
        _1500 = 1500, // Team SS
        [Description("9000 milliseconds")]
        _9000 = 9000 // Everything SS
    }

    public enum GenderDependent : ushort
    {
        Venusaur = 3,
        Butterfree = 12,
        Rattata = 19,
        Raticate = 20,
        Pikachu = 25,
        Raichu = 26,
        Zubat = 41,
        Golbat = 42,
        Gloom = 44,
        Vileplume = 45,
        Kadabra = 64,
        Alakazam = 65,
        Doduo = 84,
        Dodrio = 85,
        Hypno = 97,
        Rhyhorn = 111,
        Rhydon = 112,
        Goldeen = 118,
        Seaking = 119,
        Scyther = 123,
        Magikarp = 129,
        Gyarados = 130,
        Eevee = 133,
        Meganium = 154,
        Ledyba = 165,
        Ledian = 166,
        Xatu = 178,
        Sudowoodo = 185,
        Politoed = 186,
        Aipom = 190,
        Wooper = 194,
        Quagsire = 195,
        Murkrow = 198,
        Wobbuffet = 202,
        Girafarig = 203,
        Gligar = 207,
        Steelix = 208,
        Scizor = 212,
        Heracross = 214,
        Sneasel = 215,
        Ursaring = 217,
        Piloswine = 221,
        Octillery = 224,
        Houndoom = 229,
        Donphan = 232,
        Torchic = 255,
        Combusken = 256,
        Blaziken = 257,
        Beautifly = 267,
        Dustox = 269,
        Ludicolo = 272,
        Nuzleaf = 274,
        Shiftry = 275,
        Meditite = 307,
        Medicham = 308,
        Roselia = 315,
        Gulpin = 316,
        Swalot = 317,
        Numel = 322,
        Camerupt = 323,
        Cacturne = 332,
        Milotic = 350,
        Relicanth = 369,
        Starly = 396,
        Staravia = 397,
        Staraptor = 398,
        Bidoof = 399,
        Bibarel = 400,
        Kricketot = 401,
        Kricketune = 402,
        Shinx = 403,
        Luxio = 404,
        Luxray = 405,
        Roserade = 407,
        Combee = 415,
        Pachirisu = 417,
        Floatzel = 418,
        Buizel = 419,
        Ambipom = 424,
        Gible = 443,
        Gabite = 444,
        Garchomp = 445,
        Hippopotas = 449,
        Hippowdon = 450,
        Croagunk = 453,
        Toxicroak = 454,
        Finneon = 456,
        Lumineon = 457,
        Snover = 459,
        Abomasnow = 460,
        Weavile = 461,
        Rhyperior = 464,
        Tangrowth = 465,
        Mamoswine = 473,
        Unfezant = 521,
        Frillish = 592,
        Jellicent = 593,
        Pyroar = 668,
    }
}