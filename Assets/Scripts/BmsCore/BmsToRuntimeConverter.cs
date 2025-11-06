using System.Collections.Generic;
using System.Linq;
using GameCore;

namespace BmsCore
{
    /// <summary>
    /// BMS譜面データをPlay用スコアデータに変換する静的クラス
    /// </summary>
    public static class BmsToPlayConverter
    {
        /// <summary>
        /// BMS譜面データをPlay用スコアデータに変換するメソッド
        /// </summary>
        /// <param name="bmsScore">変換する対象のBMS譜面データ</param>
        /// <returns>生成されたPlay用スコアデータ</returns>
        public static RuntimeScore Convert(BmsScore bmsScore)
        {
            // BMSチャンネルデータを取得し、最大小節番号を取得
            var bmsChannelData = bmsScore.ChannelData;
            var maxMeasure = bmsChannelData.Keys.Count > 0 ? bmsChannelData.Keys.Max() : -1;

            // Beatオブジェクトを格納する配列
            var beatsList = new List<Beat>();

            // 全小節を順番に処理
            for (var measureIndex = 0; measureIndex <= maxMeasure; measureIndex++)
            {
                // 小節にデータが存在しない場合はスキップ
                if (!bmsChannelData.TryGetValue(measureIndex, out var measureChannels))
                    continue;

                // 小節内の各チャンネル(レーン)を処理
                foreach (var (channelType, values) in measureChannels)
                {
                    // チャンネル内の各位置のデータを処理
                    for (var i = 0; i < values.Length; i++)
                    {
                        // 値が0の場合はノートが存在しないためスキップ
                        if (values[i] == 0) continue;

                        // 現在位置のBMS Tick値を計算
                        var bmsTick = TickCalculator.CalculateCurrentTick(measureIndex, i, values.Length);

                        // BMSデータからビート情報を生成
                        var noteType = (NoteType)(int)values[i];
                        var laneType =  (NoteLane)channelType;
                        var beat = new Beat(bmsTick, noteType,laneType);
                        
                        // 作成したBeatを追加
                        beatsList.Add(beat);
                    }
                }
            }

            // Tick値の昇順でソートして配列化
            var beatsArray = beatsList.OrderBy(beat => beat.Tick).ToArray();

            return new RuntimeScore(bmsScore.Title, bmsScore.Bpm, bmsScore.BeatsOffset, beatsArray);
        }
    }
}