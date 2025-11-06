using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GameCore;

namespace BmsCore
{
    /// <summary>
    /// BMSデータを解析し、対応するBMSスコアオブジェクトを生成するクラス
    /// </summary>
    public static class BmsToPlayConverter
    {
        private static readonly Regex TitlePattern = new Regex(@"^#TITLE\s+(.+)$", RegexOptions.Compiled);
        private static readonly Regex BpmPattern = new Regex(@"^#BPM([0-9A-Z]{2})\s+(\d+)$", RegexOptions.Compiled);
        private static readonly Regex OffsetPattern = new Regex(@"^#OFFSET\s+([\d.]+)$", RegexOptions.Compiled);
        
        private static readonly Regex MainDataPattern = new Regex(@"^#(\d{3})(\d{2}):(.+)$", RegexOptions.Compiled);
        private static readonly Regex SplitTwoPattern = new Regex(@"(.{2})", RegexOptions.Compiled);

        public static PlayScore Convert(BmsScore bmsScore)
        {
            // BMSチャンネルデータを取得し、最大小節番号を取得
            var bmsChannelData = bmsScore.ChannelData;
            var maxMeasure = bmsChannelData.Keys.Count > 0 ? bmsChannelData.Keys.Max() : -1;

            // BMSのTick値をキーとしてBeatオブジェクトを格納する辞書
            var allBeatsByBmsTime = new Dictionary<int, Beat>();

            // 全小節を順番に処理
            for (var measureIndex = 0; measureIndex <= maxMeasure; measureIndex++)
            {
                // 小節にデータが存在しない場合は空のBeatを作成
                if (!bmsChannelData.TryGetValue(measureIndex, out var measureChannels))
                {
                    var bmsTick = TickCalculate.CalculateBmsTick(measureIndex, 0, 1);
                    allBeatsByBmsTime[bmsTick] = new Beat(bmsTick, null);
                    continue;
                }

                // 小節内の各チャンネル(レーン)を処理
                foreach (var (channelType, values) in measureChannels)
                {
                    // チャンネル内の各位置のデータを処理
                    for (var i = 0; i < values.Length; i++)
                    {
                        // 値が0の場合はノートが存在しないためスキップ
                        if (values[i] == 0) continue;

                        // 現在位置のBMS Tick値を計算
                        var bmsTick = TickCalculate.CalculateBmsTick(measureIndex, i, values.Length);

                        // BMSデータからノート情報を生成
                        var noteType = (NoteType)(int)values[i];
                        var laneType =  (NoteLane)channelType;
                        var note = new Note(noteType,laneType);

                        // 同じTick位置に既存のノートがあれば取得、なければ新規リスト作成
                        var notes = allBeatsByBmsTime[bmsTick].Notes?.ToList() ?? new List<Note>();
                        notes.Add(note);

                        // 更新したノートリストでBeatを上書き
                        allBeatsByBmsTime[bmsTick] = new Beat(bmsTick, notes.ToArray());
                    }
                }
            }

            // Tick値の昇順でソートして配列化
            var beats = allBeatsByBmsTime.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToArray();

            return new PlayScore(bmsScore.Title, bmsScore.Bpm, bmsScore.BeatsOffset, beats);
        }
    }
}