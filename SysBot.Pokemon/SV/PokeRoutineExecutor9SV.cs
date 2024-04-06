using PKHeX.Core;
using RaidCrawler.Core.Structures;
using SysBot.Base;
using SysBot.Pokemon.SV.BotRaid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SysBot.Base.SwitchButton;
using static SysBot.Pokemon.PokeDataOffsetsSV;
using static SysBot.Pokemon.SV.BotRaid.Blocks;
using static System.Buffers.Binary.BinaryPrimitives;

namespace SysBot.Pokemon
{
    public abstract class PokeRoutineExecutor9SV : PokeRoutineExecutor<PK9>
    {
        protected PokeDataOffsetsSV Offsets { get; } = new();
        public ulong returnOfs = 0;

        private ulong KeyBlockAddress = 0;

        public ulong BaseBlockKeyPointer;

        protected PokeRoutineExecutor9SV(PokeBotState cfg) : base(cfg)
        {
        }

        public override async Task<PK9> ReadPokemon(ulong offset, CancellationToken token) => await ReadPokemon(offset, BoxFormatSlotSize, token).ConfigureAwait(false);

        public override async Task<PK9> ReadPokemon(ulong offset, int size, CancellationToken token)
        {
            var data = await SwitchConnection.ReadBytesAbsoluteAsync(offset, size, token).ConfigureAwait(false);
            return new PK9(data);
        }

        public async Task SetCurrentBox(byte box, CancellationToken token)
        {
            await SwitchConnection.PointerPoke(new[] { box }, Offsets.CurrentBoxPointer, token).ConfigureAwait(false);
        }

        public async Task<SAV9SV> IdentifyTrainer(CancellationToken token)
        {
            // Check if botbase is on the correct version or later.
            await VerifyBotbaseVersion(token).ConfigureAwait(false);

            // Check title so we can warn if mode is incorrect.
            string title = await SwitchConnection.GetTitleID(token).ConfigureAwait(false);
            if (title is not (ScarletID or VioletID))
                throw new Exception($"{title} is not a valid SV title. Is your mode correct?");

            // Verify the game version.
            var game_version = await SwitchConnection.GetGameInfo("version", token).ConfigureAwait(false);
            if (!game_version.SequenceEqual(SVGameVersion))
                throw new Exception($"Game version is not supported. Expected version {SVGameVersion}, and current game version is {game_version}.");

            var sav = await GetFakeTrainerSAV(token).ConfigureAwait(false);
            InitSaveData(sav);

            if (!IsValidTrainerData())
            {
                await CheckForRAMShiftingApps(token).ConfigureAwait(false);
                throw new Exception("Refer to the SysBot.NET wiki (https://github.com/kwsch/SysBot.NET/wiki/Troubleshooting) for more information.");
            }

            if (await GetTextSpeed(token).ConfigureAwait(false) < TextSpeedOption.Fast)
                throw new Exception("Text speed should be set to FAST. Fix this for correct operation.");

            return sav;
        }

        public async Task<SAV9SV> GetFakeTrainerSAV(CancellationToken token)
        {
            var sav = new SAV9SV();
            var info = sav.MyStatus;
            var read = await SwitchConnection.PointerPeek(info.Data.Length, Offsets.MyStatusPointer, token).ConfigureAwait(false);
            read.CopyTo(info.Data);
            return sav;
        }

        public async Task<RaidMyStatus> GetTradePartnerMyStatus(IReadOnlyList<long> pointer, CancellationToken token)
        {
            var info = new RaidMyStatus();
            var read = await SwitchConnection.PointerPeek(info.Data.Length, pointer, token).ConfigureAwait(false);
            read.CopyTo(info.Data, 0);
            return info;
        }

        public async Task InitializeHardware(IBotStateSettings settings, CancellationToken token)
        {
            Log("Detaching on startup.");
            await DetachController(token).ConfigureAwait(false);
            if (settings.ScreenOff)
            {
                Log("Turning off screen.");
                await SetScreen(ScreenState.Off, token).ConfigureAwait(false);
            }
        }

        public async Task CleanExit(CancellationToken token)
        {
            await SetScreen(ScreenState.On, token).ConfigureAwait(false);
            Log("Detaching controllers on routine exit.");
            await DetachController(token).ConfigureAwait(false);
        }

        public async Task ReOpenGame(PokeRaidHubConfig config, CancellationToken token)
        {
            await CloseGame(config, token).ConfigureAwait(false);
            await StartGame(config, token).ConfigureAwait(false);
        }

        public async Task GoHome(PokeRaidHubConfig config, CancellationToken token)
        {
            var timing = config.Timings;
            // Close out of the game
            await Click(B, 0_500, token).ConfigureAwait(false);
            await Click(HOME, 2_000 + timing.ExtraTimeReturnHome, token).ConfigureAwait(false);
            Log("Went to Home Screen");
        }

        public async Task CloseGame(PokeRaidHubConfig config, CancellationToken token)
        {
            var timing = config.Timings;
            // Close out of the game
            await Click(B, 0_500, token).ConfigureAwait(false);
            await Click(HOME, 2_000 + timing.ExtraTimeReturnHome, token).ConfigureAwait(false);
            await Click(X, 1_000, token).ConfigureAwait(false);
            await Click(A, 5_000 + timing.RestartGameSettings.ExtraTimeCloseGame, token).ConfigureAwait(false);
            Log("Closed out of the game!");
        }

        public async Task StartGame(PokeRaidHubConfig config, CancellationToken token)
        {
            var timing = config.Timings;
            var loadPro = timing.RestartGameSettings.ProfileSelectSettings.ProfileSelectionRequired ? timing.RestartGameSettings.ProfileSelectSettings.ExtraTimeLoadProfile : 0;

            // Menus here can go in the order: System Update Prompt -> Profile -> Checking if Game can be played (Digital Only) -> DLC check -> Unable to use DLC
            await Click(A, 1_000 + loadPro, token).ConfigureAwait(false); // Initial "A" Press to start the Game + a delay if needed for profiles to load

            // Really Shouldn't keep this but we will for now
            if (timing.RestartGameSettings.AvoidSystemUpdate)
            {
                await Task.Delay(0_500, token).ConfigureAwait(false); // Delay bc why not
                await Click(DUP, 0_600, token).ConfigureAwait(false); // Highlight "Start Software"
                await Click(A, 1_000 + loadPro, token).ConfigureAwait(false); // Select "Sttart Software" + delay if Profile selection is needed
            }

            // Only send extra Presses if we need to
            if (timing.RestartGameSettings.ProfileSelectSettings.ProfileSelectionRequired)
            {
                await Click(A, 1_000, token).ConfigureAwait(false); // Now we are on the Profile Screen
                await Click(A, 1_000, token).ConfigureAwait(false); // Select the profile
            }

            // Digital game copies take longer to load
            if (timing.RestartGameSettings.CheckGameDelay)
            {
                await Task.Delay(2_000 + timing.RestartGameSettings.ExtraTimeCheckGame, token).ConfigureAwait(false);
            }

            // If they have DLC on the system and can't use it, requires an UP + A to start the game.
            if (timing.RestartGameSettings.CheckForDLC)
            {
                await Click(DUP, 0_600, token).ConfigureAwait(false);
                await Click(A, 0_600, token).ConfigureAwait(false);
            }

            Log("Restarting the game!");

            // Switch Logo and game load screen
            await Task.Delay(15_000 + timing.RestartGameSettings.ExtraTimeLoadGame, token).ConfigureAwait(false);

            for (int i = 0; i < 8; i++)
                await Click(A, 1_000, token).ConfigureAwait(false);

            var timer = 60_000;
            while (!await IsOnOverworldTitle(token).ConfigureAwait(false))
            {
                await Task.Delay(1_000, token).ConfigureAwait(false);
                timer -= 1_000;
                // We haven't made it back to overworld after a minute, so press A every 6 seconds hoping to restart the game.
                // Don't risk it if hub is set to avoid updates.
                if (timer <= 0 && !timing.RestartGameSettings.AvoidSystemUpdate)
                {
                    Log("Still not in the game, initiating rescue protocol!");
                    while (!await IsOnOverworldTitle(token).ConfigureAwait(false))
                        await Click(A, 6_000, token).ConfigureAwait(false);
                    break;
                }
            }

            await Task.Delay(5_000 + timing.ExtraTimeLoadOverworld, token).ConfigureAwait(false);
            Log("Back in the overworld!");
        }

        public async Task<bool> IsConnectedOnline(ulong offset, CancellationToken token)
        {
            var data = await SwitchConnection.ReadBytesAbsoluteAsync(offset, 1, token).ConfigureAwait(false);
            return data[0] == 1;
        }

        public async Task<bool> IsOnOverworld(ulong offset, CancellationToken token)
        {
            var data = await SwitchConnection.ReadBytesAbsoluteAsync(offset, 1, token).ConfigureAwait(false);
            return data[0] == 0x11;
        }

        // Only used to check if we made it off the title screen; the pointer isn't viable until a few seconds after clicking A.
        public async Task<bool> IsOnOverworldTitle(CancellationToken token)
        {
            var (valid, offset) = await ValidatePointerAll(Offsets.OverworldPointer, token).ConfigureAwait(false);
            if (!valid)
                return false;
            return await IsOnOverworld(offset, token).ConfigureAwait(false);
        }

        public async Task<TextSpeedOption> GetTextSpeed(CancellationToken token)
        {
            var data = await SwitchConnection.PointerPeek(1, Offsets.ConfigPointer, token).ConfigureAwait(false);
            return (TextSpeedOption)(data[0] & 3);
        }

        //Zyro additions
        public async Task SVSaveGameOverworld(CancellationToken token)
        {
            Log("Saving the game...");
            await Click(X, 2_000, token).ConfigureAwait(false);
            await Click(R, 1_800, token).ConfigureAwait(false);
            await Click(A, 1_000, token).ConfigureAwait(false);
            await Task.Delay(6_000, token).ConfigureAwait(false);
            await Click(B, 1_500, token).ConfigureAwait(false);
        }

        private readonly IReadOnlyList<uint> DifficultyFlags = new List<uint>() { 0xEC95D8EF, 0xA9428DFE, 0x9535F471, 0x6E7F8220 };

        public async Task<int> GetStoryProgress(ulong BaseBlockKeyPointer, CancellationToken token)
        {
            for (int i = DifficultyFlags.Count - 1; i >= 0; i--)
            {
                // See https://github.com/Lincoln-LM/sv-live-map/pull/43
                var block = await ReadSaveBlockRaid(BaseBlockKeyPointer, DifficultyFlags[i], 1, token).ConfigureAwait(false);
                if (block[0] == 2)
                    return i + 1;
            }
            return 0;
        }

        public async Task<GameProgress> ReadGameProgress(CancellationToken token)
        {
            var Unlocked6Stars = await ReadEncryptedBlockBool(RaidDataBlocks.KUnlockedRaidDifficulty6, token).ConfigureAwait(false);
            if (Unlocked6Stars)
                return GameProgress.Unlocked6Stars;

            var Unlocked5Stars = await ReadEncryptedBlockBool(RaidDataBlocks.KUnlockedRaidDifficulty5, token).ConfigureAwait(false);
            if (Unlocked5Stars)
                return GameProgress.Unlocked5Stars;

            var Unlocked4Stars = await ReadEncryptedBlockBool(RaidDataBlocks.KUnlockedRaidDifficulty4, token).ConfigureAwait(false);
            if (Unlocked4Stars)
                return GameProgress.Unlocked4Stars;

            var Unlocked3Stars = await ReadEncryptedBlockBool(RaidDataBlocks.KUnlockedRaidDifficulty3, token).ConfigureAwait(false);
            if (Unlocked3Stars)
                return GameProgress.Unlocked3Stars;

            return GameProgress.UnlockedTeraRaids;
        }

        public async Task ReadEventRaids(ulong BaseBlockKeyPointer, RaidContainer container, CancellationToken token, bool force = false)
        {
            var priorityFile = Path.Combine(
                Directory.GetCurrentDirectory(),
                "cache",
                "raid_priority_array"
            );
            if (!force && File.Exists(priorityFile))
            {
                var (_, version) = FlatbufferDumper.DumpDeliveryPriorities(
                    await File.ReadAllBytesAsync(priorityFile, token)
                );
                var block = await ReadBlockDefault(BaseBlockKeyPointer, RaidCrawler.Core.Structures.Offsets.BCATRaidPriorityLocation, "raid_priority_array.tmp", true, token).ConfigureAwait(false);
                var (_, v2) = FlatbufferDumper.DumpDeliveryPriorities(block);
                if (version != v2)
                    force = true;

                var tempFile = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "cache",
                    "raid_priority_array.tmp"
                );
                if (File.Exists(tempFile))
                    File.Delete(tempFile);

                if (v2 == 0) // raid reset
                    return;
            }

            var deliveryRaidPriorityFlatbuffer = await ReadBlockDefault(BaseBlockKeyPointer, RaidCrawler.Core.Structures.Offsets.BCATRaidPriorityLocation, "raid_priority_array", force, token).ConfigureAwait(false);
            var (groupID, priority) = FlatbufferDumper.DumpDeliveryPriorities(deliveryRaidPriorityFlatbuffer);
            if (priority == 0)
                return;

            var deliveryRaidFlatbuffer = await ReadBlockDefault(BaseBlockKeyPointer, RaidCrawler.Core.Structures.Offsets.BCATRaidBinaryLocation, "raid_enemy_array", force, token).ConfigureAwait(false);
            var deliveryFixedRewardFlatbuffer = await ReadBlockDefault(BaseBlockKeyPointer, RaidCrawler.Core.Structures.Offsets.BCATRaidFixedRewardLocation, "fixed_reward_item_array", force, token).ConfigureAwait(false);
            var deliveryLotteryRewardFlatbuffer = await ReadBlockDefault(BaseBlockKeyPointer, RaidCrawler.Core.Structures.Offsets.BCATRaidLotteryRewardLocation, "lottery_reward_item_array", force, token).ConfigureAwait(false);

            container.DistTeraRaids = TeraDistribution.GetAllEncounters(deliveryRaidFlatbuffer);
            container.MightTeraRaids = TeraMight.GetAllEncounters(deliveryRaidFlatbuffer);
            container.DeliveryRaidPriority = groupID;
            container.DeliveryRaidFixedRewards = FlatbufferDumper.DumpFixedRewards(
                deliveryFixedRewardFlatbuffer
            );
            container.DeliveryRaidLotteryRewards = FlatbufferDumper.DumpLotteryRewards(
                deliveryLotteryRewardFlatbuffer
            );
        }

        public static (PK9, uint) IsSeedReturned(ITeraRaid enc, Raid raid)
        {
            var param = enc.GetParam();
            var blank = new PK9
            {
                Species = enc.Species,
                Form = enc.Form
            };
            Encounter9RNG.GenerateData(blank, param, EncounterCriteria.Unrestricted, raid.Seed);

            return (blank, raid.Seed);
        }

        public static string GetSpecialRewards(IReadOnlyList<(int, int, int)> rewards, List<string> rewardsToShow)
        {
            // Initialize reward counters
            int rare = 0, abilitycapsule = 0, bottlecap = 0, abilitypatch = 0, pokeball = 0;
            int expCandyL = 0, expCandyXL = 0, sweetHerba = 0, saltyHerba = 0, sourHerba = 0, bitterHerba = 0, spicyHerba = 0;
            int nugget = 0, tinyMushroom = 0, bigMushroom = 0, pearl = 0, bigPearl = 0, stardust = 0, starPiece = 0, goldBottleCap = 0, ppUp = 0;

            // Initialize Tera Shard counters
            Dictionary<int, int> teraShards = new Dictionary<int, int>();

            // Count rewards
            foreach (var reward in rewards)
            {
                switch (reward.Item1)
                {
                    case 0050: rare += reward.Item2; break;
                    case 0645: abilitycapsule += reward.Item2; break;
                    case 0795: bottlecap += reward.Item2; break;
                    case 1127: expCandyL += reward.Item2; break;
                    case 1128: expCandyXL += reward.Item2; break;
                    case 1606: abilitypatch += reward.Item2; break;
                    case 1904: sweetHerba += reward.Item2; break;
                    case 1905: saltyHerba += reward.Item2; break;
                    case 1906: sourHerba += reward.Item2; break;
                    case 1907: bitterHerba += reward.Item2; break;
                    case 1908: spicyHerba += reward.Item2; break;
                    case 0004: pokeball += reward.Item2; break;
                    case 0092: nugget += reward.Item2; break;
                    case 0086: tinyMushroom += reward.Item2; break;
                    case 0087: bigMushroom += reward.Item2; break;
                    case 0088: pearl += reward.Item2; break;
                    case 0089: bigPearl += reward.Item2; break;
                    case 0090: stardust += reward.Item2; break;
                    case 0091: starPiece += reward.Item2; break;
                    case 0796: goldBottleCap += reward.Item2; break;
                    case 0051: ppUp += reward.Item2; break;
                    case >= 1862 and <= 1879:
                        if (teraShards.ContainsKey(reward.Item1))
                            teraShards[reward.Item1] += reward.Item2;
                        else
                            teraShards[reward.Item1] = reward.Item2;
                        break;
                }
            }
            // Format and filter rewards based on user preferences
            List<string> rewardStrings = new List<string>();
            if (rewardsToShow.Contains("Rare Candy") && rare > 0)
                rewardStrings.Add($"**Rare Candy** x{rare}");
            if (rewardsToShow.Contains("Ability Capsule") && abilitycapsule > 0)
                rewardStrings.Add($"**Ability Capsule** x{abilitycapsule}");
            if (rewardsToShow.Contains("Bottle Cap") && bottlecap > 0)
                rewardStrings.Add($"**Bottle Cap** x{bottlecap}");
            if (rewardsToShow.Contains("Ability Patch") && abilitypatch > 0)
                rewardStrings.Add($"**Ability Patch** x{abilitypatch}");
            if (rewardsToShow.Contains("Exp. Candy L") && expCandyL > 0)
                rewardStrings.Add($"**Exp. Candy L** x{expCandyL}");
            if (rewardsToShow.Contains("Exp. Candy XL") && expCandyXL > 0)
                rewardStrings.Add($"**Exp. Candy XL** x{expCandyXL}");
            if (rewardsToShow.Contains("Sweet Herba Mystica") && sweetHerba > 0)
                rewardStrings.Add($"**Sweet Herba Mystica** x{sweetHerba}");
            if (rewardsToShow.Contains("Salty Herba Mystica") && saltyHerba > 0)
                rewardStrings.Add($"**Salty Herba Mystica** x{saltyHerba}");
            if (rewardsToShow.Contains("Sour Herba Mystica") && sourHerba > 0)
                rewardStrings.Add($"**Sour Herba Mystica** x{sourHerba}");
            if (rewardsToShow.Contains("Bitter Herba Mystica") && bitterHerba > 0)
                rewardStrings.Add($"**Bitter Herba Mystica** x{bitterHerba}");
            if (rewardsToShow.Contains("Spicy Herba Mystica") && spicyHerba > 0)
                rewardStrings.Add($"**Spicy Herba Mystica** x{spicyHerba}");
            if (rewardsToShow.Contains("Pokeball") && pokeball > 0)
                rewardStrings.Add($"**Pokeball** x{pokeball}");
            if (rewardsToShow.Contains("Nugget") && nugget > 0)
                rewardStrings.Add($"**Nugget** x{nugget}");
            if (rewardsToShow.Contains("Tiny Mushroom") && tinyMushroom > 0)
                rewardStrings.Add($"**Tiny Mushroom** x{tinyMushroom}");
            if (rewardsToShow.Contains("Big Mushroom") && bigMushroom > 0)
                rewardStrings.Add($"**Big Mushroom** x{bigMushroom}");
            if (rewardsToShow.Contains("Pearl") && pearl > 0)
                rewardStrings.Add($"**Pearl** x{pearl}");
            if (rewardsToShow.Contains("Big Pearl") && bigPearl > 0)
                rewardStrings.Add($"**Big Pearl** x{bigPearl}");
            if (rewardsToShow.Contains("Stardust") && stardust > 0)
                rewardStrings.Add($"**Stardust** x{stardust}");
            if (rewardsToShow.Contains("Star Piece") && starPiece > 0)
                rewardStrings.Add($"**Star Piece** x{starPiece}");
            if (rewardsToShow.Contains("Gold Bottle Cap") && goldBottleCap > 0)
                rewardStrings.Add($"**Gold Bottle Cap** x{goldBottleCap}");
            if (rewardsToShow.Contains("PP Up") && ppUp > 0)
                rewardStrings.Add($"**PP Up** x{ppUp}");
            if (rewardsToShow.Contains("Shards"))
            {
                foreach (var shard in teraShards)
                {
                    string shardTypeName = GetTeraShardTypeName(shard.Key);
                    rewardStrings.Add($"**{shardTypeName} Tera Shard** x{shard.Value}");
                }
            }

            return string.Join("\n", rewardStrings);
        }

        private static string GetTeraShardTypeName(int shardType)
        {
            return shardType switch
            {
                1862 => "Normal",
                1868 => "Fighting",
                1871 => "Flying",
                1869 => "Poison",
                1870 => "Ground",
                1874 => "Rock",
                1873 => "Bug",
                1875 => "Ghost",
                1878 => "Steel",
                1863 => "Fire",
                1864 => "Water",
                1866 => "Grass",
                1865 => "Electric",
                1872 => "Psychic",
                1867 => "Ice",
                1876 => "Dragon",
                1877 => "Dark",
                1879 => "Fairy",
                _ => "Unknown", // or handle this case as needed
            };
        }

        public static string[] ProcessRaidPlaceholders(string[] description, PKM pk)
        {
            string[] raidDescription = Array.Empty<string>();

            if (description.Length > 0)
                raidDescription = description.ToArray();

            string markEntryText = "";
            string markTitle = "";
            string scaleText = "";
            string scaleNumber = "";
            string shinySymbol = pk.ShinyXor == 0 ? "■" : pk.ShinyXor <= 16 ? "★" : "";
            string shinySymbolText = pk.ShinyXor == 0 ? "Square Shiny" : pk.ShinyXor <= 16 ? "Star Shiny" : "";
            string shiny = pk.ShinyXor <= 16 ? "Shiny" : "";
            string species = SpeciesName.GetSpeciesNameGeneration(pk.Species, 2, 9);
            string IVList = $"{pk.IV_HP}/{pk.IV_ATK}/{pk.IV_DEF}/{pk.IV_SPA}/{pk.IV_SPD}/{pk.IV_SPE}";
            string MaxIV = "";
            string HP = pk.IV_HP.ToString();
            string ATK = pk.IV_ATK.ToString();
            string DEF = pk.IV_DEF.ToString();
            string SPA = pk.IV_SPA.ToString();
            string SPD = pk.IV_SPD.ToString();
            string SPE = pk.IV_SPE.ToString();
            string nature = $"{(Nature)pk.Nature}";
            string genderSymbol = pk.Gender == 0 ? "♂" : pk.Gender == 1 ? "♀" : "⚥";
            string genderText = $"{(Gender)pk.Gender}";
            string ability = $"{GameInfo.GetStrings(1).Ability[pk.Ability]}";

            if (pk.IV_HP == 31 && pk.IV_ATK == 31 && pk.IV_DEF == 31 && pk.IV_SPA == 31 && pk.IV_SPD == 31 && pk.IV_SPE == 31)
                MaxIV = "6IV";

            RaidExtensions<PK9>.HasMark((IRibbonIndex)pk, out RibbonIndex mark);
            if (mark == RibbonIndex.MarkMightiest)
                markEntryText = "the Unrivaled";
            if (pk is PK9 pkl)
            {
                scaleText = $"{PokeSizeDetailedUtil.GetSizeRating(pkl.Scale)}";
                scaleNumber = pkl.Scale.ToString();
                if (pkl.Scale == 0)
                {
                    markEntryText = "The Teeny";
                    markTitle = "Teeny";
                }
                if (pkl.Scale == 255)
                {
                    markEntryText = "The Great";
                    markTitle = "Jumbo";
                }
            }

            for (int i = 0; i < raidDescription.Length; i++)
                raidDescription[i] = raidDescription[i].Replace("{markEntryText}", markEntryText)
                        .Replace("{markTitle}", markTitle).Replace("{scaleText}", scaleText).Replace("{scaleNumber}", scaleNumber).Replace("{shinySymbol}", shinySymbol).Replace("{shinySymbolText}", shinySymbolText)
                        .Replace("{shinyText}", shiny).Replace("{species}", species).Replace("{IVList}", IVList).Replace("{MaxIV}", MaxIV).Replace("{HP}", HP).Replace("{ATK}", ATK).Replace("{DEF}", DEF).Replace("{SPA}", SPA)
                        .Replace("{SPD}", SPD).Replace("{SPE}", SPE).Replace("{nature}", nature).Replace("{ability}", ability).Replace("{genderSymbol}", genderSymbol).Replace("{genderText}", genderText);

            return raidDescription;
        }

        // Save Block Additions from TeraFinder/RaidCrawler/sv-livemap
        public async Task<object?> ReadBlock(DataBlock block, CancellationToken token)
        {
            if (block.IsEncrypted)
                return await ReadEncryptedBlock(block, token).ConfigureAwait(false);
            else
                return await ReadDecryptedBlock(block, token).ConfigureAwait(false);
        }

        public async Task<bool> WriteBlock(object data, DataBlock block, CancellationToken token, object? toExpect = default)
        {
            if (block.IsEncrypted)
                return await WriteEncryptedBlockSafe(block, toExpect, data, token).ConfigureAwait(false);
            else
                return await WriteDecryptedBlock((byte[])data!, block, token).ConfigureAwait(false);
        }

        public async Task<bool> WriteEncryptedBlockSafe(DataBlock block, object? toExpect, object toWrite, CancellationToken token)
        {
            if (toExpect == default || toWrite == default)
                return false;

            return block.Type switch
            {
                SCTypeCode.Object => await WriteEncryptedBlockObject(block, (byte[])toExpect, (byte[])toWrite, token),
                SCTypeCode.Array => await WriteEncryptedBlockArray(block, (byte[])toExpect, (byte[])toWrite, token).ConfigureAwait(false),
                SCTypeCode.Bool1 or SCTypeCode.Bool2 or SCTypeCode.Bool3 => await WriteEncryptedBlockBool(block, (bool)toExpect, (bool)toWrite, token).ConfigureAwait(false),
                SCTypeCode.Byte or SCTypeCode.SByte => await WriteEncryptedBlockByte(block, (byte)toExpect, (byte)toWrite, token).ConfigureAwait(false),
                SCTypeCode.UInt32 => await WriteEncryptedBlockUint(block, (uint)toExpect, (uint)toWrite, token).ConfigureAwait(false),
                SCTypeCode.Int32 => await WriteEncryptedBlockInt32(block, (int)toExpect, (int)toWrite, token).ConfigureAwait(false),
                _ => throw new NotSupportedException($"Block {block.Name} (Type {block.Type}) is currently not supported.")
            };
        }

        private async Task<bool> WriteEncryptedBlockInt32(DataBlock block, int valueToExpect, int valueToInject, CancellationToken token)
        {
            if (Config.Connection.Protocol is SwitchProtocol.WiFi && !Connection.Connected)
                throw new InvalidOperationException("No remote connection");

            //Always read and decrypt first to validate address and data
            ulong address;
            try { address = await GetBlockAddress(block, token).ConfigureAwait(false); }
            catch (Exception) { return false; }
            //If we get there without exceptions, the block address is valid
            var header = await SwitchConnection.ReadBytesAbsoluteAsync(address, 5, token).ConfigureAwait(false);
            header = BlockUtil.DecryptBlock(block.Key, header);
            //Validate ram data
            var ram = ReadInt32LittleEndian(header.AsSpan()[1..]);
            if (ram != valueToExpect) return false;
            //If we get there then both block address and block data are valid, we can safely inject
            WriteInt32LittleEndian(header.AsSpan()[1..], valueToInject);
            header = BlockUtil.EncryptBlock(block.Key, header);
            await SwitchConnection.WriteBytesAbsoluteAsync(header, address, token).ConfigureAwait(false);
            return true;
        }

        private async Task<byte[]> ReadDecryptedBlock(DataBlock block, CancellationToken token)
        {
            if (Config.Connection.Protocol is SwitchProtocol.WiFi && !Connection.Connected)
                throw new InvalidOperationException("No remote connection");

            var data = await SwitchConnection.PointerPeek(block.Size, block.Pointer!, token).ConfigureAwait(false);
            return data;
        }

        private async Task<ulong> GetBlockAddress(DataBlock block, CancellationToken token, bool prepareAddress = true)
        {
            KeyBlockAddress = 0;

            if (block.Pointer == null)
            {
                Log("Block pointer is null. Aborting operation.");
                throw new ArgumentNullException(nameof(block.Pointer), "Block pointer cannot be null.");
            }

            if (KeyBlockAddress == 0)
                KeyBlockAddress = await SwitchConnection.PointerAll(block.Pointer, token).ConfigureAwait(false);

            var keyblock = await SwitchConnection.ReadBytesAbsoluteAsync(KeyBlockAddress, 16, token).ConfigureAwait(false);
            if (keyblock == null || keyblock.Length < 16)
            {
                Log("Failed to read keyblock or keyblock is too short.");
                throw new InvalidOperationException("Failed to read keyblock.");
            }

            var start = BitConverter.ToUInt64(keyblock.AsSpan()[..8]);
            var end = BitConverter.ToUInt64(keyblock.AsSpan()[8..]);
            var ct = (ulong)48;

            while (start < end)
            {
                var block_ct = (end - start) / ct;
                var mid = start + (block_ct >> 1) * ct;

                var data = await SwitchConnection.ReadBytesAbsoluteAsync(mid, 4, token).ConfigureAwait(false);
                if (data == null || data.Length < 4)
                {
                    Log("Failed to read data or data is too short.");
                    continue; // or break, depending on your error handling strategy
                }

                var found = BitConverter.ToUInt32(data);
                if (found == block.Key)
                {
                    if (prepareAddress)
                        mid = await PrepareAddress(mid, token).ConfigureAwait(false);
                    return mid;
                }

                if (found >= block.Key)
                    end = mid;
                else start = mid + ct;
            }

            Log("Block key not found within the specified range.");
            throw new ArgumentOutOfRangeException(nameof(block), "Block key not found.");
        }

        private async Task<ulong> PrepareAddress(ulong address, CancellationToken token) =>
            BitConverter.ToUInt64(await SwitchConnection.ReadBytesAbsoluteAsync(address + 8, 8, token).ConfigureAwait(false));

        private async Task<bool> WriteEncryptedBlockUint(DataBlock block, uint valueToExpect, uint valueToInject, CancellationToken token)
        {
            if (Config.Connection.Protocol is SwitchProtocol.WiFi && !Connection.Connected)
                throw new InvalidOperationException("No remote connection");

            //Always read and decrypt first to validate address and data
            ulong address;
            try { address = await GetBlockAddress(block, token).ConfigureAwait(false); }
            catch (Exception) { return false; }
            //If we get there without exceptions, the block address is valid
            var header = await SwitchConnection.ReadBytesAbsoluteAsync(address, 5, token).ConfigureAwait(false);
            header = BlockUtil.DecryptBlock(block.Key, header);
            //Validate ram data
            var ram = ReadUInt32LittleEndian(header.AsSpan()[1..]);
            if (ram != valueToExpect) return false;
            //If we get there then both block address and block data are valid, we can safely inject
            WriteUInt32LittleEndian(header.AsSpan()[1..], valueToInject);
            header = BlockUtil.EncryptBlock(block.Key, header);
            await SwitchConnection.WriteBytesAbsoluteAsync(header, address, token).ConfigureAwait(false);
            return true;
        }

        private async Task<byte[]> ReadEncryptedBlockHeader(DataBlock block, CancellationToken token)
        {
            if (Config.Connection.Protocol is SwitchProtocol.WiFi && !Connection.Connected)
                throw new InvalidOperationException("No remote connection");

            var address = await GetBlockAddress(block, token).ConfigureAwait(false);
            var header = await SwitchConnection.ReadBytesAbsoluteAsync(address, 5, token).ConfigureAwait(false);
            header = BlockUtil.DecryptBlock(block.Key, header);
            return header;
        }

        private async Task<int> ReadEncryptedBlockInt32(DataBlock block, CancellationToken token)
        {
            var header = await ReadEncryptedBlockHeader(block, token).ConfigureAwait(false);
            return ReadInt32LittleEndian(header.AsSpan()[1..]);
        }

        public async Task<bool> WriteEncryptedBlockSByte(DataBlock block, sbyte valueToExpect, sbyte valueToInject, CancellationToken token)
        {
            Log("Starting WriteEncryptedBlockSByte method.");

            if (Config.Connection.Protocol is SwitchProtocol.WiFi && !Connection.Connected)
            {
                Log("No remote connection. Aborting write operation.");
                throw new InvalidOperationException("No remote connection");
            }

            ulong address;
            try
            {
                address = await GetBlockAddress(block, token).ConfigureAwait(false);
                Log($"Block address obtained: {address}");
            }
            catch (Exception ex)
            {
                Log($"Exception in getting block address: {ex.Message}");
                return false;
            }

            var header = await SwitchConnection.ReadBytesAbsoluteAsync(address, 5, token).ConfigureAwait(false);
            header = BlockUtil.DecryptBlock(block.Key, header);
            Log("Header decrypted.");

            // Directly inject new value without checking current RAM value
            header[1] = (byte)valueToInject; // Convert sbyte to byte for writing
            header = BlockUtil.EncryptBlock(block.Key, header);
            Log("Header encrypted with new value.");

            try
            {
                await SwitchConnection.WriteBytesAbsoluteAsync(header, address, token).ConfigureAwait(false);
                Log("Write operation successful.");
            }
            catch (Exception ex)
            {
                Log($"Exception in write operation: {ex.Message}");
                return false;
            }

            return true;
        }

        public async Task<bool> WriteEncryptedBlockByte(DataBlock block, byte valueToExpect, byte valueToInject, CancellationToken token)
        {
            if (Config.Connection.Protocol is SwitchProtocol.WiFi && !Connection.Connected)
                throw new InvalidOperationException("No remote connection");

            //Always read and decrypt first to validate address and data
            ulong address;
            try { address = await GetBlockAddress(block, token).ConfigureAwait(false); }
            catch (Exception) { return false; }
            //If we get there without exceptions, the block address is valid
            var header = await SwitchConnection.ReadBytesAbsoluteAsync(address, 5, token).ConfigureAwait(false);
            header = BlockUtil.DecryptBlock(block.Key, header);
            //Validate ram data
            var ram = header[1];
            if (ram != valueToExpect) return false;
            //If we get there then both block address and block data are valid, we can safely inject
            header[1] = valueToInject;
            header = BlockUtil.EncryptBlock(block.Key, header);
            await SwitchConnection.WriteBytesAbsoluteAsync(header, address, token).ConfigureAwait(false);
            return true;
        }

        private async Task<bool> WriteDecryptedBlock(byte[] data, DataBlock block, CancellationToken token)
        {
            if (Config.Connection.Protocol is SwitchProtocol.WiFi && !Connection.Connected)
                throw new InvalidOperationException("No remote connection");

            var pointer = await SwitchConnection.PointerAll(block.Pointer!, token).ConfigureAwait(false);
            await SwitchConnection.WriteBytesAbsoluteAsync(data, pointer, token).ConfigureAwait(false);

            return true;
        }

        private async Task<bool> WriteEncryptedBlockBool(DataBlock block, bool valueToExpect, bool valueToInject, CancellationToken token)
        {
            if (Config.Connection.Protocol is SwitchProtocol.WiFi && !Connection.Connected)
                throw new InvalidOperationException("No remote connection");

            //Always read and decrypt first to validate address and data
            ulong address;
            try { address = await GetBlockAddress(block, token).ConfigureAwait(false); }
            catch (Exception) { return false; }
            //If we get there without exceptions, the block address is valid
            var data = await SwitchConnection.ReadBytesAbsoluteAsync(address, block.Size, token).ConfigureAwait(false);
            data = BlockUtil.DecryptBlock(block.Key, data);
            //Validate ram data
            var ram = data[0] == 2;
            if (ram != valueToExpect) return false;
            //If we get there then both block address and block data are valid, we can safely inject
            data[0] = valueToInject ? (byte)2 : (byte)1;
            data = BlockUtil.EncryptBlock(block.Key, data);
            await SwitchConnection.WriteBytesAbsoluteAsync(data, address, token).ConfigureAwait(false);
            return true;
        }

        private async Task<bool> WriteEncryptedBlockArray(DataBlock block, byte[] arrayToExpect, byte[] arrayToInject, CancellationToken token)
        {
            if (Config.Connection.Protocol is SwitchProtocol.WiFi && !Connection.Connected)
                throw new InvalidOperationException("No remote connection");

            //Always read and decrypt first to validate address and data
            ulong address;
            try { address = await GetBlockAddress(block, token).ConfigureAwait(false); }
            catch (Exception) { return false; }
            //If we get there without exceptions, the block address is valid
            var data = await SwitchConnection.ReadBytesAbsoluteAsync(address, 6 + block.Size, token).ConfigureAwait(false);
            data = BlockUtil.DecryptBlock(block.Key, data);
            //Validate ram data
            var ram = data[6..];
            if (!ram.SequenceEqual(arrayToExpect)) return false;
            //If we get there then both block address and block data are valid, we can safely inject
            Array.ConstrainedCopy(arrayToInject, 0, data, 6, block.Size);
            data = BlockUtil.EncryptBlock(block.Key, data);
            await SwitchConnection.WriteBytesAbsoluteAsync(data, address, token).ConfigureAwait(false);
            return true;
        }

        public async Task<bool> WriteEncryptedBlockObject(DataBlock block, byte[] valueToExpect, byte[] valueToInject, CancellationToken token)
        {
            if (Config.Connection.Protocol is SwitchProtocol.WiFi && !Connection.Connected)
                throw new InvalidOperationException("No remote connection");

            //Always read and decrypt first to validate address and data
            ulong address;
            try { address = await GetBlockAddress(block, token).ConfigureAwait(false); }
            catch (Exception) { return false; }
            //If we get there without exceptions, the block address is valid
            var header = await SwitchConnection.ReadBytesAbsoluteAsync(address, 5, token).ConfigureAwait(false);
            header = BlockUtil.DecryptBlock(block.Key, header);
            var size = ReadUInt32LittleEndian(header.AsSpan()[1..]);
            var data = await SwitchConnection.ReadBytesAbsoluteAsync(address, 5 + (int)size, token);
            var ram = BlockUtil.DecryptBlock(block.Key, data)[5..];
            if (!ram.SequenceEqual(valueToExpect)) { return false; }
            //If we get there then both block address and block data are valid, we can safely inject
            Array.ConstrainedCopy(valueToInject.ToArray(), 0, data, 5, block.Size);
            data = BlockUtil.EncryptBlock(block.Key, data);
            await SwitchConnection.WriteBytesAbsoluteAsync(data, address, token).ConfigureAwait(false);
            return true;
        }

        private async Task<object?> ReadEncryptedBlock(DataBlock block, CancellationToken token)
        {
            return block.Type switch
            {
                SCTypeCode.Object => await ReadEncryptedBlockObject(block, token).ConfigureAwait(false),
                SCTypeCode.Array => await ReadEncryptedBlockArray(block, token).ConfigureAwait(false),
                SCTypeCode.Bool1 or SCTypeCode.Bool2 or SCTypeCode.Bool3 => await ReadEncryptedBlockBool(block, token).ConfigureAwait(false),
                SCTypeCode.Byte or SCTypeCode.SByte => await ReadEncryptedBlockByte(block, token).ConfigureAwait(false),
                SCTypeCode.UInt32 => await ReadEncryptedBlockUint(block, token).ConfigureAwait(false),
                SCTypeCode.Int32 => await ReadEncryptedBlockInt32(block, token).ConfigureAwait(false),
                _ => throw new NotSupportedException($"Block {block.Name} (Type {block.Type}) is currently not supported.")
            };
        }

        private async Task<byte[]?> ReadEncryptedBlockArray(DataBlock block, CancellationToken token)
        {
            if (Config.Connection.Protocol is SwitchProtocol.WiFi && !Connection.Connected)
                throw new InvalidOperationException("No remote connection");

            var address = await GetBlockAddress(block, token).ConfigureAwait(false);
            var data = await SwitchConnection.ReadBytesAbsoluteAsync(address, 6 + block.Size, token).ConfigureAwait(false);
            data = BlockUtil.DecryptBlock(block.Key, data);
            return data[6..];
        }

        public async Task<bool> ReadEncryptedBlockBool(DataBlock block, CancellationToken token)
        {
            if (Config.Connection.Protocol is SwitchProtocol.WiFi && !Connection.Connected)
                throw new InvalidOperationException("No remote connection");
            var address = await GetBlockAddress(block, token).ConfigureAwait(false);
            var data = await SwitchConnection.ReadBytesAbsoluteAsync(address, block.Size, token).ConfigureAwait(false);
            var res = BlockUtil.DecryptBlock(block.Key, data);
            return res[0] == 2;
        }

        public async Task<sbyte> ReadEncryptedBlockByte(DataBlock block, CancellationToken token)
        {
            BaseBlockKeyPointer = await SwitchConnection.PointerAll(Offsets.BlockKeyPointer, token).ConfigureAwait(false);
            var addr = await SearchSaveKey(BaseBlockKeyPointer, block.Key, token).ConfigureAwait(false);
            addr = BitConverter.ToUInt64(await SwitchConnection.ReadBytesAbsoluteAsync(addr + 8, 0x8, token).ConfigureAwait(false), 0);
            var header = await SwitchConnection.ReadBytesAbsoluteAsync(addr, 5, token).ConfigureAwait(false);
            header = DecryptBlock(block.Key, header);
            return (sbyte)header[1];
        }

        private async Task<uint> ReadEncryptedBlockUint(DataBlock block, CancellationToken token)
        {
            var header = await ReadEncryptedBlockHeader(block, token).ConfigureAwait(false);
            return ReadUInt32LittleEndian(header.AsSpan()[1..]);
        }

        private async Task<byte[]?> ReadEncryptedBlockObject(DataBlock block, CancellationToken token)
        {
            if (Config.Connection.Protocol is SwitchProtocol.WiFi && !Connection.Connected)
                throw new InvalidOperationException("No remote connection");

            var address = await GetBlockAddress(block, token).ConfigureAwait(false);
            var header = await SwitchConnection.ReadBytesAbsoluteAsync(address, 5, token).ConfigureAwait(false);
            header = BlockUtil.DecryptBlock(block.Key, header);
            var size = ReadUInt32LittleEndian(header.AsSpan()[1..]);
            var data = await SwitchConnection.ReadBytesAbsoluteAsync(address, 5 + (int)size, token);
            var res = BlockUtil.DecryptBlock(block.Key, data)[5..];
            return res;
        }

        public async Task<ulong> SearchSaveKey(ulong baseBlock, uint key, CancellationToken token)
        {
            var data = await SwitchConnection.ReadBytesAbsoluteAsync(baseBlock + 8, 16, token).ConfigureAwait(false);
            var start = BitConverter.ToUInt64(data.AsSpan()[..8]);
            var end = BitConverter.ToUInt64(data.AsSpan()[8..]);

            while (start < end)
            {
                var block_ct = (end - start) / 48;
                var mid = start + (block_ct >> 1) * 48;

                data = await SwitchConnection.ReadBytesAbsoluteAsync(mid, 4, token).ConfigureAwait(false);
                var found = BitConverter.ToUInt32(data);
                if (found == key)
                    return mid;

                if (found >= key)
                    end = mid;
                else start = mid + 48;
            }
            return start;
        }

        private static byte[] DecryptBlock(uint key, byte[] block)
        {
            var rng = new SCXorShift32(key);
            for (int i = 0; i < block.Length; i++)
                block[i] = (byte)(block[i] ^ rng.Next());
            return block;
        }

        public async Task<ulong> SearchSaveKeyRaid(ulong BaseBlockKeyPointer, uint key, CancellationToken token)
        {
            var data = await SwitchConnection.ReadBytesAbsoluteAsync(BaseBlockKeyPointer + 8, 16, token).ConfigureAwait(false);
            var start = BitConverter.ToUInt64(data.AsSpan()[..8]);
            var end = BitConverter.ToUInt64(data.AsSpan()[8..]);

            while (start < end)
            {
                var block_ct = (end - start) / 48;
                var mid = start + (block_ct >> 1) * 48;

                data = await SwitchConnection.ReadBytesAbsoluteAsync(mid, 4, token).ConfigureAwait(false);
                var found = BitConverter.ToUInt32(data);
                if (found == key)
                    return mid;

                if (found >= key)
                    end = mid;
                else start = mid + 48;
            }
            return start;
        }

        public async Task<byte[]> ReadSaveBlockRaid(ulong BaseBlockKeyPointer, uint key, int size, CancellationToken token)
        {
            var block_ofs = await SearchSaveKeyRaid(BaseBlockKeyPointer, key, token).ConfigureAwait(false);
            var data = await SwitchConnection.ReadBytesAbsoluteAsync(block_ofs + 8, 0x8, token).ConfigureAwait(false);
            block_ofs = BitConverter.ToUInt64(data, 0);

            var block = await SwitchConnection.ReadBytesAbsoluteAsync(block_ofs, size, token).ConfigureAwait(false);
            return DecryptBlock(key, block);
        }

        public async Task<byte[]> ReadSaveBlockObject(ulong BaseBlockKeyPointer, uint key, CancellationToken token)
        {
            var header_ofs = await SearchSaveKeyRaid(BaseBlockKeyPointer, key, token).ConfigureAwait(false);
            var data = await SwitchConnection.ReadBytesAbsoluteAsync(header_ofs + 8, 8, token).ConfigureAwait(false);
            header_ofs = BitConverter.ToUInt64(data);

            var header = await SwitchConnection.ReadBytesAbsoluteAsync(header_ofs, 5, token).ConfigureAwait(false);
            header = DecryptBlock(key, header);

            var size = BitConverter.ToUInt32(header.AsSpan()[1..]);
            var obj = await SwitchConnection.ReadBytesAbsoluteAsync(header_ofs, (int)size + 5, token).ConfigureAwait(false);
            return DecryptBlock(key, obj)[5..];
        }

        public async Task<byte[]> ReadBlockDefault(ulong BaseBlockKeyPointer, uint key, string? cache, bool force, CancellationToken token)
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "cache");
            Directory.CreateDirectory(folder);

            var path = Path.Combine(folder, cache ?? "");
            if (force is false && cache is not null && File.Exists(path))
                return File.ReadAllBytes(path);

            var bin = await ReadSaveBlockObject(BaseBlockKeyPointer, key, token).ConfigureAwait(false);
            File.WriteAllBytes(path, bin);
            return bin;
        }
    }
}