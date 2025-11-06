using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BmsCore
{
    /// <summary>
    /// BMSファイル内で使用されるチャンネルの種類を表す列挙型
    /// </summary>
    public enum BmsChannelType
    {
        // Note lanes
        LaneOne = 11,           // xxx11: Lane One
        LaneTwo = 12,           // xxx12: Lane Two
        LaneThree = 13,         // xxx13: Lane Three
        LaneFour = 14,          // xxx14: Lane Four
        LaneFive = 15,          // xxx15: Lane Five
        LaneSix = 18,           // xxx18: Lane Six
    }
    
    /// <summary>
    /// BMSデータを解析し、対応するBMSスコアオブジェクトを生成するクラス
    /// </summary>
    public static class BmsLoader
    {
        private static readonly Regex TitlePattern = new Regex(@"^#TITLE\s+(.+)$", RegexOptions.Compiled);
        private static readonly Regex BpmPattern = new Regex(@"^#BPM([0-9A-Z]{2})\s+(\d+)$", RegexOptions.Compiled);
        private static readonly Regex OffsetPattern = new Regex(@"^#OFFSET\s+([\d.]+)$", RegexOptions.Compiled);
        
        private static readonly Regex MainDataPattern = new Regex(@"^#(\d{3})(\d{2}):(.+)$", RegexOptions.Compiled);
        private static readonly Regex SplitTwoPattern = new Regex(@"(.{2})", RegexOptions.Compiled);


        /// <summary>
        /// BmsデータをMusicScoreデータに変換する
        /// </summary>
        /// <param name="bmsStrings">変換対象のBMSデータ</param>
        /// <returns>変換されたMusicScoreオブジェクト</returns>
        public static BmsScore Load(List<string> bmsStrings)
        {
            // === 基本情報の初期化 ===
            string title = null;                   // 楽曲タイトル
            int bpm = 120;                         // BPM
            double offset = 0;                     // オフセット秒数

            // === チャンネルデータの統合格納構造 ===
            var channelData = new Dictionary<int, Dictionary<BmsChannelType, double[]>>();

            // === BMS行の解析処理 ===
            foreach (var line in bmsStrings)
            {
                string trimmedLine = line.Trim();
                if (!trimmedLine.StartsWith("#")) continue;
                
                // --- タイトル情報の解析 (#TITLE) ---
                var titleMatch = TitlePattern.Match(trimmedLine);
                if (titleMatch.Success)
                {
                    title = titleMatch.Groups[1].Value;
                    continue;
                }
                
                // --- BPM情報の解析 (#BPM) ---
                var bpmMatch = BpmPattern.Match(trimmedLine);
                if (bpmMatch.Success)
                {
                    if (int.TryParse(bpmMatch.Groups[1].Value, out var parsedBpm))
                        bpm = parsedBpm;
                    continue;
                }

                // --- オフセット情報の解析 (#OFFSET) ---
                var offsetMatch = OffsetPattern.Match(trimmedLine);
                if (offsetMatch.Success)
                {
                    if (double.TryParse(offsetMatch.Groups[1].Value, out var parsedOffset))
                        offset = parsedOffset;
                    continue;
                }
                
                // --- チャンネルデータの解析 (#MMMcc:data) ---
                var mainDataMatch = MainDataPattern.Match(trimmedLine);
                if (!(mainDataMatch.Success &&
                    int.TryParse(mainDataMatch.Groups[1].Value, out var measure) && 
                    int.TryParse(mainDataMatch.Groups[2].Value, out var channel))) continue;

                var data = mainDataMatch.Groups[3].Value;
                var channelType = (BmsChannelType)channel;
                
                // 小節用辞書の初期化
                if (!channelData.ContainsKey(measure))
                    channelData[measure] = new Dictionary<BmsChannelType, double[]>();
                
                // --- 2文字区切りでのデータ分割 ---
                var splitMatch = SplitTwoPattern.Matches(data);
                if (splitMatch.Count <= 0) continue;

                // --- 各チャンネル情報を格納 ---
                var splitValues = ParseChannelAsNumbers(splitMatch);
                channelData[measure][channelType] = splitValues;
            }
            
            return new BmsScore(title, bpm, offset, channelData);
        }
        
        /// <summary>
        /// チャンネルデータを数値として解析する共通処理
        /// </summary>
        /// <param name="matches">2文字区切りのマッチ結果</param>
        /// <returns>解析された数値の配列</returns>
        private static double[] ParseChannelAsNumbers(MatchCollection matches)
        {
            var splitValues = new List<double>();
            for (var i = 0; i < matches.Count; i++)
            {
                var matchStr = matches[i].Groups[1].Value;
                if (double.TryParse(matchStr, out var parsedValue))
                    splitValues.Add(parsedValue);
            }
            return splitValues.ToArray();
        }
    }
}