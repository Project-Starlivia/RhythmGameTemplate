namespace GameCore
{
    /// <summary>
    /// Tick関連の計算を行うためのユーティリティを提供します。
    /// </summary>
    public static class TickCalculator
    {
        /// <summary>
        /// 1小節あたりのティック数、分解能を表します。
        /// 固定値として9600が設定されています。
        /// </summary>
        public const int TicksPerMeasure = 9600;

        /// <summary>
        /// 指定された小節インデックス、拍のインデックス、1小節内の拍数、および小節の長さからティック値を計算します。
        /// </summary>
        /// <param name="measureIndex">小節のインデックス（0から始まる）。</param>
        /// <param name="beatIndex">小節内の拍のインデックス（0から始まる）。</param>
        /// <param name="beatCountInMeasure">1小節内の総拍数。</param>
        /// <returns>計算されたティック値。</returns>
        public static int CalculateCurrentTick(int measureIndex, int beatIndex, int beatCountInMeasure)
        {
            return (measureIndex + 1 / beatCountInMeasure * beatIndex) * TicksPerMeasure;
        }

        /// TicksPerBeat は 1 拍（beat）あたりの時間単位（tick）数を定義します。
        /// 音楽シーケンサーやリズムベースの処理において、タイムベースとして利用されます。
        /// この値は、1 小節（measure）あたりの時間単位（tick）数 TicksPerMeasure を基準に計算され、
        /// 通常は 4 分の 1 拍（クォーターノート）に相当します。
        private const int TicksPerBeat = TicksPerMeasure / 4;

        /// <summary>
        /// 指定されたBPMと小節の長さから1秒間に生成されるTicksの数を計算します。
        /// </summary>
        /// <param name="bpm">1分間あたりのビート数 (BPM: Beats Per Minute)。</param>
        /// <returns>1秒間に生成されるTicksの数。</returns>
        private static double TicksPerSecond(int bpm)
        {
            return TicksPerBeat * (bpm / 60.0);
        }

        /// <summary>
        /// 秒単位の時間をTickに変換します。
        /// </summary>
        /// <param name="timeSeconds">時間（秒単位）</param>
        /// <param name="bpm">楽曲のテンポ（BPM）</param>
        /// <returns>時間に対応するTick値</returns>
        public static double TimeToTick(double timeSeconds, int bpm)
        {
            var ticksPerSecond = TicksPerSecond(bpm);
            return timeSeconds * ticksPerSecond;
        }

        /// <summary>
        /// 指定されたティック数を時間（秒）に変換します。
        /// </summary>
        /// <param name="bpm">テンポを表すビート毎分（BPM）。</param>
        /// <param name="tick">変換対象となるティック数。</param>
        /// <returns>指定されたティック数に相当する時間（秒）。</returns>
        public static double TickToTime(int bpm, double tick)
        {
            var ticksPerSecond = TicksPerSecond(bpm);
            return tick / ticksPerSecond;
        }
    }
}