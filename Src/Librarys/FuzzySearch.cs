﻿/*
 * Arquivo: FuzzySearch.cs
 * Criado em: 10-6-2021
 * https://github.com/ForceFK
 * ForceFK - Force&Kuraiyo Dev
 * Última modificação: 11-6-2021
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Librarys
{
    public class FuzzySearch
    {
        class Word
        {
            public string Text { get; set; } = null!;
            public List<int> Codes { get; set; } = new List<int>();
        }

        class AnalyzedObject
        {
            public string Original { get; set; } = null!;
            public List<Word> Words { get; set; } = new List<Word>();
        }

        public class SearchResult
        {
            public class SearchMatch
            {
                public string SimilarTo { get; set; } = null!;
                public double Difficulty { get; set; }
                public int LanguageType { get; set; }
            }

            public SearchResult(string query, IEnumerable<SearchMatch> matches)
            {
                Query = query;
                Matches = matches.OrderBy(match => match.Difficulty).ToList();
            }

            public List<SearchMatch> Matches { get; }
            public string Query { get; }

            public IEnumerable<SearchMatch> GetBestMatches(int matchesCount)
            {
                return Matches.Take(matchesCount);
            }

            public SearchMatch GetFullMatch() => Matches.FirstOrDefault(match => Math.Abs(match.Difficulty) < 10)!;
        }

        private List<AnalyzedObject> Samples { get; set; } = new List<AnalyzedObject>();

        public void AddData(IEnumerable<string> datas)
        {
            foreach (var data in datas)
            {
                AddData(data);
            }
        }

        public void AddData(string data)
        {
            var codeKeys = CodeKeysRus.Concat(CodeKeysEng).ToList();
            AnalyzedObject languageSet = new()
            {
                Original = data
            };
            if (data.Length > 0)
            {
                languageSet.Words = data.Split(' ').Select(w => new Word
                {
                    Text = w.ToLower(),
                    Codes = GetKeyCodes(codeKeys, w)
                }).ToList();
            }

            Samples.Add(languageSet);
        }

        public SearchResult Search(string targetStr)
        {
            var codeKeys = CodeKeysRus.Concat(CodeKeysEng).ToList();
            var originalSearchObj = new AnalyzedObject();
            if (targetStr.Length > 0)
            {
                originalSearchObj.Words = targetStr.Split(' ').Select(w => new Word
                {
                    Text = w.ToLower(),
                    Codes = GetKeyCodes(codeKeys, w)
                }).ToList();
            }

            var translationSearchObj = new AnalyzedObject();
            if (targetStr.Length > 0)
            {
                translationSearchObj.Words = targetStr.Split(' ').Select(w => {
                    var translateStr = Transliterate(w.ToLower(), Translit_Ru_En);
                    return new Word
                    {
                        Text = translateStr,
                        Codes = GetKeyCodes(codeKeys, translateStr)
                    };
                }).ToList();
            }

            var result = new List<SearchResult.SearchMatch>();
            foreach (var sample in Samples)
            {
                var languageType = 1;
                var cost = GetRangePhrase(sample, originalSearchObj, false);
                // Проверка транслетерационной строки
                var tempCost = GetRangePhrase(sample, translationSearchObj, true);
                if (cost > tempCost)
                {
                    cost = tempCost;
                    languageType = 2;
                }

                result.Add(new SearchResult.SearchMatch { SimilarTo = sample.Original, Difficulty = cost, LanguageType = languageType });
            }

            return new SearchResult(targetStr, result);
        }

        private static double GetRangePhrase(AnalyzedObject source, AnalyzedObject search, bool translation)
        {
            if (!source.Words.Any())
            {
                if (!search.Words.Any())
                    return 0;
                return search.Words.Sum(w => w.Text.Length) * 2 * 100;
            }

            if (!search.Words.Any())
            {
                return source.Words.Sum(w => w.Text.Length) * 2 * 100;
            }

            double result = 0;
            for (var i = 0; i < search.Words.Count; i++)
            {
                var minRangeWord = double.MaxValue;
                var minIndex = 0;
                for (var j = 0; j < source.Words.Count; j++)
                {
                    var currentRangeWord = GetRangeWord(source.Words[j], search.Words[i], translation);
                    if (currentRangeWord < minRangeWord)
                    {
                        minRangeWord = currentRangeWord;
                        minIndex = j;
                    }
                }

                result += minRangeWord * 100 + (Math.Abs(i - minIndex) / 10.0);
            }

            return result;
        }

        private static double GetRangeWord(Word source, Word target, bool translation)
        {
            var minDistance = double.MaxValue;
            var croppedSource = new Word();
            var length = Math.Min(source.Text.Length, target.Text.Length + 1);
            for (var i = 0; i <= source.Text.Length - length; i++)
            {
                croppedSource.Text = source.Text.Substring(i, length);
                croppedSource.Codes = source.Codes.Skip(i).Take(length).ToList();
                minDistance = Math.Min(minDistance,
                    LevenshteinDistance(croppedSource, target, croppedSource.Text.Length == source.Text.Length, translation) + (i * 2 / 10.0));
            }

            return minDistance;
        }

        private static int LevenshteinDistance(Word source, Word target, bool fullWord, bool translation)
        {
            if (String.IsNullOrEmpty(source.Text))
            {
                if (String.IsNullOrEmpty(target.Text))
                    return 0;
                return target.Text.Length * 2;
            }

            if (String.IsNullOrEmpty(target.Text))
                return source.Text.Length * 2;
            var n = source.Text.Length;
            var m = target.Text.Length;
            //TODO Убрать в параметры (для оптимизации)
            var distance = new int[3, m + 1];
            // Initialize the distance 'matrix'
            for (var j = 1; j <= m; j++)
                distance[0, j] = j * 2;
            var currentRow = 0;
            for (var i = 1; i <= n; ++i)
            {
                currentRow = i % 3;
                var previousRow = (i - 1) % 3;
                distance[currentRow, 0] = i * 2;
                for (var j = 1; j <= m; j++)
                {
                    distance[currentRow, j] = Math.Min(Math.Min(
                            distance[previousRow, j] + ((!fullWord && i == n) ? 2 - 1 : 2),
                            distance[currentRow, j - 1] + ((!fullWord && i == n) ? 2 - 1 : 2)),
                        distance[previousRow, j - 1] + CostDistanceSymbol(source, i - 1, target, j - 1, translation));

                    if (i > 1 && j > 1 && source.Text[i - 1] == target.Text[j - 2]
                     && source.Text[i - 2] == target.Text[j - 1])
                    {
                        distance[currentRow, j] = Math.Min(distance[currentRow, j], distance[(i - 2) % 3, j - 2] + 2);
                    }
                }
            }

            return distance[currentRow, m];
        }

        private static int CostDistanceSymbol(Word source, int sourcePosition, Word search, int searchPosition, bool translation)
        {
            if (source.Text[sourcePosition] == search.Text[searchPosition])
                return 0;
            if (translation)
                return 2;
            if (source.Codes[sourcePosition] != 0 && source.Codes[sourcePosition] == search.Codes[searchPosition])
                return 0;
            int resultWeight;
            if (!DistanceCodeKey.TryGetValue(source.Codes[sourcePosition], out List<int>? nearKeys))
                resultWeight = 2;
            else
                resultWeight = nearKeys.Contains(search.Codes[searchPosition]) ? 1 : 2;
            if (PhoneticGroupsRus.TryGetValue(search.Text[searchPosition], out List<char>? phoneticGroups))
                resultWeight = Math.Min(resultWeight, phoneticGroups.Contains(source.Text[sourcePosition]) ? 1 : 2);
            if (PhoneticGroupsEng.TryGetValue(search.Text[searchPosition], out phoneticGroups))
                resultWeight = Math.Min(resultWeight, phoneticGroups.Contains(source.Text[sourcePosition]) ? 1 : 2);
            return resultWeight;
        }

        private static List<int> GetKeyCodes(List<KeyValuePair<char, int>> codeKeys, string word)
        {
            return word.ToLower().Select(ch => codeKeys.FirstOrDefault(ck => ck.Key == ch).Value).ToList();
        }

        private static string Transliterate(string text, Dictionary<char, string> cultureFrom)
        {
            var translateText = text.SelectMany(t => cultureFrom.TryGetValue(t, out string? translateChar) ? translateChar : t.ToString());
            return string.Concat(translateText);
        }

        #region Блок Фонетических групп

        static readonly Dictionary<char, List<char>> PhoneticGroupsRus = new();
        static readonly Dictionary<char, List<char>> PhoneticGroupsEng = new();

        #endregion

        static FuzzySearch()
        {
            SetPhoneticGroups(PhoneticGroupsRus, new List<string> { "ыий", "эе", "ая", "оёе", "ую", "шщ", "оа" });
            SetPhoneticGroups(PhoneticGroupsEng, new List<string> { "aeiouy", "bp", "ckq", "dt", "lr", "mn", "gj", "fpv", "sxz", "csz" });
        }

        private static void SetPhoneticGroups(Dictionary<char, List<char>> resultPhoneticGroups, List<string> phoneticGroups)
        {
            foreach (var group in phoneticGroups)
                foreach (var symbol in group)
                    if (!resultPhoneticGroups.ContainsKey(symbol))
                        resultPhoneticGroups.Add(symbol,
                            phoneticGroups.Where(pg => pg.Contains(symbol)).SelectMany(pg => pg).Distinct().Where(ch => ch != symbol).ToList());
        }

        #region Блок для сопоставления клавиатуры

        /// <summary>
        /// Близость кнопок клавиатуры
        /// </summary>
        private static readonly Dictionary<int, List<int>> DistanceCodeKey = new()
        {
            /* '`' */ {192, new List<int> {49}},
            /* '1' */ {49, new List<int> {50, 87, 81}},
            /* '2' */ {50, new List<int> {49, 81, 87, 69, 51}},
            /* '3' */ {51, new List<int> {50, 87, 69, 82, 52}},
            /* '4' */ {52, new List<int> {51, 69, 82, 84, 53}},
            /* '5' */ {53, new List<int> {52, 82, 84, 89, 54}},
            /* '6' */ {54, new List<int> {53, 84, 89, 85, 55}},
            /* '7' */ {55, new List<int> {54, 89, 85, 73, 56}},
            /* '8' */ {56, new List<int> {55, 85, 73, 79, 57}},
            /* '9' */ {57, new List<int> {56, 73, 79, 80, 48}},
            /* '0' */ {48, new List<int> {57, 79, 80, 219, 189}},
            /* '-' */ {189, new List<int> {48, 80, 219, 221, 187}},
            /* '+' */ {187, new List<int> {189, 219, 221}},
            /* 'q' */ {81, new List<int> {49, 50, 87, 83, 65}},
            /* 'w' */ {87, new List<int> {49, 81, 65, 83, 68, 69, 51, 50}},
            /* 'e' */ {69, new List<int> {50, 87, 83, 68, 70, 82, 52, 51}},
            /* 'r' */ {82, new List<int> {51, 69, 68, 70, 71, 84, 53, 52}},
            /* 't' */ {84, new List<int> {52, 82, 70, 71, 72, 89, 54, 53}},
            /* 'y' */ {89, new List<int> {53, 84, 71, 72, 74, 85, 55, 54}},
            /* 'u' */ {85, new List<int> {54, 89, 72, 74, 75, 73, 56, 55}},
            /* 'i' */ {73, new List<int> {55, 85, 74, 75, 76, 79, 57, 56}},
            /* 'o' */ {79, new List<int> {56, 73, 75, 76, 186, 80, 48, 57}},
            /* 'p' */ {80, new List<int> {57, 79, 76, 186, 222, 219, 189, 48}},
            /* '[' */ {219, new List<int> {48, 186, 222, 221, 187, 189}},
            /* ']' */ {221, new List<int> {189, 219, 187}},
            /* 'a' */ {65, new List<int> {81, 87, 83, 88, 90}},
            /* 's' */ {83, new List<int> {81, 65, 90, 88, 67, 68, 69, 87, 81}},
            /* 'd' */ {68, new List<int> {87, 83, 88, 67, 86, 70, 82, 69}},
            /* 'f' */ {70, new List<int> {69, 68, 67, 86, 66, 71, 84, 82}},
            /* 'g' */ {71, new List<int> {82, 70, 86, 66, 78, 72, 89, 84}},
            /* 'h' */ {72, new List<int> {84, 71, 66, 78, 77, 74, 85, 89}},
            /* 'j' */ {74, new List<int> {89, 72, 78, 77, 188, 75, 73, 85}},
            /* 'k' */ {75, new List<int> {85, 74, 77, 188, 190, 76, 79, 73}},
            /* 'l' */ {76, new List<int> {73, 75, 188, 190, 191, 186, 80, 79}},
            /* ';' */ {186, new List<int> {79, 76, 190, 191, 222, 219, 80}},
            /* '\''*/ {222, new List<int> {80, 186, 191, 221, 219}},
            /* 'z' */ {90, new List<int> {65, 83, 88}},
            /* 'x' */ {88, new List<int> {90, 65, 83, 68, 67}},
            /* 'c' */ {67, new List<int> {88, 83, 68, 70, 86}},
            /* 'v' */ {86, new List<int> {67, 68, 70, 71, 66}},
            /* 'b' */ {66, new List<int> {86, 70, 71, 72, 78}},
            /* 'n' */ {78, new List<int> {66, 71, 72, 74, 77}},
            /* 'm' */ {77, new List<int> {78, 72, 74, 75, 188}},
            /* '<' */ {188, new List<int> {77, 74, 75, 76, 190}},
            /* '>' */ {190, new List<int> {188, 75, 76, 186, 191}},
            /* '?' */ {191, new List<int> {190, 76, 186, 222}},
        };

        /// <summary>
        /// Коды клавиш русскоязычной клавиатуры
        /// </summary>
        private static readonly Dictionary<char, int> CodeKeysRus = new()
        {
            {'ё', 192},
            {'1', 49},
            {'2', 50},
            {'3', 51},
            {'4', 52},
            {'5', 53},
            {'6', 54},
            {'7', 55},
            {'8', 56},
            {'9', 57},
            {'0', 48},
            {'-', 189},
            {'=', 187},
            {'й', 81},
            {'ц', 87},
            {'у', 69},
            {'к', 82},
            {'е', 84},
            {'н', 89},
            {'г', 85},
            {'ш', 73},
            {'щ', 79},
            {'з', 80},
            {'х', 219},
            {'ъ', 221},
            {'ф', 65},
            {'ы', 83},
            {'в', 68},
            {'а', 70},
            {'п', 71},
            {'р', 72},
            {'о', 74},
            {'л', 75},
            {'д', 76},
            {'ж', 186},
            {'э', 222},
            {'я', 90},
            {'ч', 88},
            {'с', 67},
            {'м', 86},
            {'и', 66},
            {'т', 78},
            {'ь', 77},
            {'б', 188},
            {'ю', 190},
            {'.', 191},

            {'!', 49},
            {'"', 50},
            {'№', 51},
            {';', 52},
            {'%', 53},
            {':', 54},
            {'?', 55},
            {'*', 56},
            {'(', 57},
            {')', 48},
            {'_', 189},
            {'+', 187},
            {',', 191},
        };

        /// <summary>
        /// Коды клавиш англиской клавиатуры
        /// </summary>
        private static readonly Dictionary<char, int> CodeKeysEng = new()
        {
            {'`', 192},
            {'1', 49},
            {'2', 50},
            {'3', 51},
            {'4', 52},
            {'5', 53},
            {'6', 54},
            {'7', 55},
            {'8', 56},
            {'9', 57},
            {'0', 48},
            {'-', 189},
            {'=', 187},
            {'q', 81},
            {'w', 87},
            {'e', 69},
            {'r', 82},
            {'t', 84},
            {'y', 89},
            {'u', 85},
            {'i', 73},
            {'o', 79},
            {'p', 80},
            {'[', 219},
            {']', 221},
            {'a', 65},
            {'s', 83},
            {'d', 68},
            {'f', 70},
            {'g', 71},
            {'h', 72},
            {'j', 74},
            {'k', 75},
            {'l', 76},
            {';', 186},
            {'\'', 222},
            {'z', 90},
            {'x', 88},
            {'c', 67},
            {'v', 86},
            {'b', 66},
            {'n', 78},
            {'m', 77},
            {',', 188},
            {'.', 190},
            {'/', 191},

            {'~', 192},
            {'!', 49},
            {'@', 50},
            {'#', 51},
            {'$', 52},
            {'%', 53},
            {'^', 54},
            {'&', 55},
            {'*', 56},
            {'(', 57},
            {')', 48},
            {'_', 189},
            {'+', 187},

            {'{', 219},
            {'}', 221},
            {':', 186},
            {'"', 222},

            {'<', 188},
            {'>', 190},
            {'?', 191},
        };

        #endregion

        #region Блок транслитерации

        /// <summary>
        /// Транслитерация Русский => ASCII (ISO 9-95)
        /// </summary>
        private static readonly Dictionary<char, string> Translit_Ru_En = new()
        {
            {'а', "a"},
            {'б', "b"},
            {'в', "v"},
            {'г', "g"},
            {'д', "d"},
            {'е', "e"},
            {'ё', "yo"},
            {'ж', "zh"},
            {'з', "z"},
            {'и', "i"},
            {'й', "i"},
            {'к', "k"},
            {'л', "l"},
            {'м', "m"},
            {'н', "n"},
            {'о', "o"},
            {'п', "p"},
            {'р', "r"},
            {'с', "s"},
            {'т', "t"},
            {'у', "u"},
            {'ф', "f"},
            {'х', "x"},
            {'ц', "c"},
            {'ч', "ch"},
            {'ш', "sh"},
            {'щ', "shh"},
            {'ъ', ""},
            {'ы', "y"},
            {'ь', "'"},
            {'э', "e"},
            {'ю', "yu"},
            {'я', "ya"},
        };

        #endregion
    }
}
