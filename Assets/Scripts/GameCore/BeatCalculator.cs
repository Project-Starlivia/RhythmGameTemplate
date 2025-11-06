using System.Collections.Generic;
using System.Linq;

namespace GameCore
{
    public class BeatCalculator
    {
        /// <summary>
        /// 指定された範囲内の拍（Beat）を取得します。
        /// </summary>
        /// <param name="beats">評価対象の拍の配列</param>
        /// <param name="from">範囲の開始（この値を超えるTickが含まれる）</param>
        /// <param name="to">範囲の終了（この値以下のTickが含まれる）</param>
        /// <returns>指定された範囲内に含まれる拍の列挙</returns>
        public static IEnumerable<Beat> FindRangeBeats(Beat[] beats, double from, double to)
        {
            return beats
                .Where(beat => beat.Tick > from && beat.Tick <= to);
        }

        /// <summary>
        /// オフセットを考慮して、指定されたTick範囲内に含まれる拍（Beat）を取得します。
        /// </summary>
        /// <param name="beats">評価対象の拍の配列</param>
        /// <param name="previousTick">範囲の開始となる前回のTick</param>
        /// <param name="currentTick">範囲の終了となる現在のTick</param>
        /// <param name="offsetTick">開始と終了のTickに追加されるオフセット値</param>
        /// <returns>指定されたオフセットを考慮した範囲内に含まれる拍の列挙</returns>
        public static IEnumerable<Beat> FindOffsetBeats(Beat[] beats, double previousTick, double currentTick, double offsetTick)
        {
            var previousAppearThreshold = previousTick + offsetTick;
            var currentAppearThreshold = currentTick + offsetTick;

            return FindRangeBeats(beats, previousAppearThreshold, currentAppearThreshold);
        }

        /// <summary>
        /// 指定された範囲内でヒット可能な拍（Beat）を取得します。
        /// </summary>
        /// <param name="beats">評価対象の拍の配列</param>
        /// <param name="currentTick">現在のTick値</param>
        /// <param name="rangeMs">ヒット範囲を表すミリ秒</param>
        /// <param name="bpm">楽曲のBPM（1分間の拍数）</param>
        /// <returns>ヒット可能な拍の列挙</returns>
        public static IEnumerable<Beat> FindHitBeats(Beat[] beats, double currentTick, double rangeMs, int bpm)
        {
            var rangeTick = TickCalculator.TimeToTick(rangeMs / 1000, bpm);

            return FindRangeBeats(beats, currentTick - rangeTick, currentTick + rangeTick);
        }
    }
}