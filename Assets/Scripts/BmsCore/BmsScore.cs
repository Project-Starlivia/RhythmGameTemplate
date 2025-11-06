using System.Collections.Generic;
using System.Text;

namespace BmsCore
{
    /// <summary>
    /// BMS譜面データを表現するクラス
    /// </summary>
    public class BmsScore
    {
        public readonly string Title;
        public readonly int Bpm;
        public readonly double BeatsOffset;
        public readonly Dictionary<int, Dictionary<BmsChannelType, double[]>> ChannelData;
        
        public BmsScore(string title, int bpm, double beatsOffset, Dictionary<int, Dictionary<BmsChannelType, double[]>> channelData)
        {
            Title = title;
            Bpm = bpm;
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
            sb.AppendLine($"BPM: {Bpm}");
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