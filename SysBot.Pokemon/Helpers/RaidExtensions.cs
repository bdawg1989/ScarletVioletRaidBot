using PKHeX.Core;
using SysBot.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace SysBot.Pokemon
{
    public class RaidExtensions<T> where T : PKM, new()
    {
        public static string PokeImg(PKM pkm, bool canGmax, bool fullSize)
        {
            bool md = false;
            bool fd = false;
            string[] baseLink;
            if (fullSize)
                baseLink = "https://raw.githubusercontent.com/bdawg1989/HomeImages/master/512x512/poke_capture_0001_000_mf_n_00000000_f_n.png".Split('_');
            else baseLink = "https://raw.githubusercontent.com/bdawg1989/HomeImages/master/128x128/poke_capture_0001_000_mf_n_00000000_f_n.png".Split('_');

            if (Enum.IsDefined(typeof(GenderDependent), pkm.Species) && !canGmax && pkm.Form is 0)
            {
                if (pkm.Gender == 0 && pkm.Species != (int)Species.Torchic)
                    md = true;
                else fd = true;
            }

            int form = pkm.Species switch
            {
                (int)Species.Sinistea or (int)Species.Polteageist or (int)Species.Rockruff or (int)Species.Mothim => 0,
                (int)Species.Alcremie when pkm.IsShiny || canGmax => 0,
                _ => pkm.Form,

            };

            if (pkm.Species is (ushort)Species.Sneasel)
            {
                if (pkm.Gender is 0)
                    md = true;
                else fd = true;
            }

            if (pkm.Species is (ushort)Species.Basculegion)
            {
                if (pkm.Gender is 0)
                {
                    md = true;
                    pkm.Form = 0;
                }
                else
                    pkm.Form = 1;

                string s = pkm.IsShiny ? "r" : "n";
                string g = md && pkm.Gender is not 1 ? "md" : "fd";
                return $"https://raw.githubusercontent.com/bdawg1989/HomeImages/master/128x128/poke_capture_0" + $"{pkm.Species}" + "_00" + $"{pkm.Form}" + "_" + $"{g}" + "_n_00000000_f_" + $"{s}" + ".png";
            }

            baseLink[2] = pkm.Species < 10 ? $"000{pkm.Species}" : pkm.Species < 100 && pkm.Species > 9 ? $"00{pkm.Species}" : pkm.Species >= 1000 ? $"{pkm.Species}" : $"0{pkm.Species}";
            baseLink[3] = pkm.Form < 10 ? $"00{form}" : $"0{form}";
            baseLink[4] = pkm.PersonalInfo.OnlyFemale ? "fo" : pkm.PersonalInfo.OnlyMale ? "mo" : pkm.PersonalInfo.Genderless ? "uk" : fd ? "fd" : md ? "md" : "mf";
            baseLink[5] = canGmax ? "g" : "n";
            baseLink[6] = "0000000" + (pkm.Species == (int)Species.Alcremie && !canGmax ? pkm.Data[0xE4] : 0);
            baseLink[8] = pkm.IsShiny ? "r.png" : "n.png";
            return string.Join("_", baseLink);
        }

        public static string FormOutput(ushort species, byte form, out string[] formString)
        {
            var strings = GameInfo.GetStrings("en");
            formString = FormConverter.GetFormList(species, strings.Types, strings.forms, GameInfo.GenderSymbolASCII, typeof(T) == typeof(PK9) ? EntityContext.Gen9 : EntityContext.Gen4);
            if (formString.Length is 0)
                return string.Empty;

            formString[0] = "";
            if (form >= formString.Length)
                form = (byte)(formString.Length - 1);
            return formString[form].Contains('-') ? formString[form] : formString[form] == "" ? "" : $"-{formString[form]}";
        }

        public static A EnumParse<A>(string input) where A : struct, Enum => !Enum.TryParse(input, true, out A result) ? new() : result;

        public static (int R, int G, int B) GetDominantColor(string imageUrl)
        {
            try
            {
                using var httpClient = new HttpClient();
                using var response = httpClient.GetAsync(imageUrl).Result;
                using var stream = response.Content.ReadAsStreamAsync().Result;
                using var image = new Bitmap(stream);

                var colorCount = new Dictionary<System.Drawing.Color, int>();

                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        var pixelColor = image.GetPixel(x, y);

                        if (pixelColor.A < 128 || pixelColor.GetBrightness() > 0.9) continue;

                        var brightnessFactor = (int)(pixelColor.GetBrightness() * 100);
                        var saturationFactor = (int)(pixelColor.GetSaturation() * 100);
                        var combinedFactor = brightnessFactor + saturationFactor;

                        var quantizedColor = System.Drawing.Color.FromArgb(
                            pixelColor.R / 10 * 10,
                            pixelColor.G / 10 * 10,
                            pixelColor.B / 10 * 10
                        );

                        if (colorCount.ContainsKey(quantizedColor))
                        {
                            colorCount[quantizedColor] += combinedFactor;
                        }
                        else
                        {
                            colorCount[quantizedColor] = combinedFactor;
                        }
                    }
                }

                if (colorCount.Count == 0)
                    return (255, 255, 255);

                var dominantColor = colorCount.Aggregate((a, b) => a.Value > b.Value ? a : b).Key;
                return (dominantColor.R, dominantColor.G, dominantColor.B);
            }
            catch (HttpRequestException ex) when (ex.InnerException is WebException webEx && webEx.Status == WebExceptionStatus.TrustFailure)
            {
                // Handle SSL certificate errors here.
                LogUtil.LogError($"SSL Certificate error when accessing {imageUrl}. Error: {ex.Message}", "GetDominantColor");
            }
            catch (Exception ex)
            {
                // Handle other errors here.
                LogUtil.LogError($"Error processing image from {imageUrl}. Error: {ex.Message}", "GetDominantColor");
            }

            return (255, 255, 255);  // Default to white if an exception occurs.
        }

        public static bool HasMark(IRibbonIndex pk, out RibbonIndex result)
        {
            result = default;
            for (var mark = RibbonIndex.MarkLunchtime; mark <= RibbonIndex.MarkSlump; mark++)
            {
                if (pk.GetRibbon((int)mark))
                {
                    result = mark;
                    return true;
                }
            }
            for (var mark = RibbonIndex.MarkJumbo; mark <= RibbonIndex.MarkMini; mark++)
            {
                if (pk.GetRibbon((int)mark))
                {
                    result = mark;
                    return true;
                }
            }
            return false;
        }

        public static string GetRaidPrintName(PKM pk)
        {
            string markEntryText = "";
            HasMark((IRibbonIndex)pk, out RibbonIndex mark);
            if (mark == RibbonIndex.MarkMightiest)
                markEntryText = " The Unrivaled";
            if (pk is PK9 pkl)
            {
                if (pkl.Scale == 0)
                    markEntryText = " The Teeny";
                if (pkl.Scale == 255)
                    markEntryText = " The Great";
            }
            return markEntryText;
        }
    }
}