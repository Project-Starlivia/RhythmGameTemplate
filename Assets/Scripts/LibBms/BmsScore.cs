using System.Collections.Generic;
using System.Text;

namespace LibBms
{
    /// <summary>
    /// BMSファイル内で使用されるチャンネルの種類を表す列挙型
    /// </summary>
    public enum BmsChannelType
    {
        // Note lanes
        LaneOne = 11,           // 011: Lane One
        LaneTwo = 12,           // 012: Lane Two
        LaneThree = 13,         // 013: Lane Three
        LaneFour = 14,          // 014: Lane Four
        LaneFive = 15,          // 015: Lane Five
        LaneSix = 18,           // 018: Lane Six
        
        // BMS commands
        MeasureLength = 2,      // xxx02: Measure length change
        BpmChange = 3,     // xxx03: Normal BPM change
        ExtBpmChange = 8,   // xxx08: Extended BPM change
        ExtStop = 9         // xxx09: Extended STOP command
    }

    /// <summary>
    /// BMS譜面データを表現するクラス
    /// </summary>
    public class BmsScore
    {
        public readonly string Title;
        public readonly double BeatsOffset;
        public readonly Dictionary<int, Dictionary<BmsChannelType, double[]>> ChannelData;
        
        public BmsScore(string title, double beatsOffset, Dictionary<int, Dictionary<BmsChannelType, double[]>> channelData)
        {
            Title = title;
            BeatsOffset = beatsOffset;
            ChannelData = channelData;
        }

        /// <summary>
        /// BmsScoreオブジェクトの内容を文字列として返す
        /// </summary>
        /// <returns>BmsScoreオブジェクトの情報を含む文字列表現</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Title: {Title}");
            sb.AppendLine($"BeatsOffset: {BeatsOffset}");
            sb.AppendLine($"Measures: {ChannelData.Count}");

            foreach (var (key, value) in ChannelData)
            {
                sb.AppendLine($"Measure {key}:");
                foreach (var (channelType, values) in value)
                {
                    sb.AppendLine($"  {channelType}: {string.Join(", ", values)}");
                }
            }
            
            return sb.ToString();
        }
    }
}