namespace LibBms
{
    /// <summary>
    /// Tick関連の計算を行うためのユーティリティを提供します。
    /// </summary>
    public static class TickCalculate
    {
        /// <summary>
        /// 1小節あたりのティック数、分解能を表します。
        /// 固定値として9600が設定されています。
        /// </summary>
        public const int TicksPerMeasure = 9600;

        /// <summary>
        /// 指定された小節インデックス、拍のインデックス、1小節内の拍数、および小節の長さからBMSのティック値を計算します。
        /// </summary>
        /// <param name="measureIndex">小節のインデックス（0から始まる）。</param>
        /// <param name="beatIndex">小節内の拍のインデックス（0から始まる）。</param>
        /// <param name="beatCountInMeasure">1小節内の総拍数。</param>
        /// <param name="measureLength">小節の長さを表す係数。</param>
        /// <returns>計算されたBMSのティック値。</returns>
        public static int CalculateBmsTick(int measureIndex, int beatIndex, int beatCountInMeasure, int measureLength)
        {
            return (int)(measureIndex * TicksPerMeasure + (beatIndex * TicksPerMeasure / beatCountInMeasure) / measureLength);
        }

        /// TicksPerBeat は 1 拍（beat）あたりの時間単位（tick）数を定義します。
        /// 音楽シーケンサーやリズムベースの処理において、タイムベースとして利用されます。
        /// この値は、1 小節（measure）あたりの時間単位（tick）数 TicksPerMeasure を基準に計算され、
        /// 通常は 4 分の 1 拍（クォーターノート）に相当します。
        private const int TicksPerBeat = TicksPerMeasure / 4;

        /// <summary>
        /// 指定されたBPMと小節の長さから1秒間に生成されるBMS Ticksの数を計算します。
        /// </summary>
        /// <param name="bpm">1分間あたりのビート数 (BPM: Beats Per Minute)。</param>
        /// <param name="measureLength">小節の長さを表す係数。</param>
        /// <returns>1秒間に生成されるBMS Ticksの数。</returns>
        private static double TicksPerSecond(int bpm, float measureLength)
        {
            return TicksPerBeat * (bpm / 60.0) * measureLength;
        }

        /// <summary>
        /// 秒単位の経過時間をBMSのTickに変換します。
        /// </summary>
        /// <param name="elapsedTimeSeconds">経過時間（秒単位）</param>
        /// <param name="bpm">楽曲のテンポ（BPM）</param>
        /// <param name="measureLength">小節の長さ（標準小節からの割合）</param>
        /// <returns>経過時間に対応するBMSのTick値</returns>
        public static double ElapsedTimeToTick(double elapsedTimeSeconds, int bpm, float measureLength)
        {
            var bmsTicksPerSecond = TicksPerSecond(bpm, measureLength);
            return elapsedTimeSeconds * bmsTicksPerSecond;
        }

        /// <summary>
        /// 指定されたBMSのティック数を経過時間（秒）に変換します。
        /// </summary>
        /// <param name="bpm">BMSのテンポを表すビート毎分（BPM）。</param>
        /// <param name="measureLength">1小節の相対的な長さ（標準的な長さは1.0）。</param>
        /// <param name="bmsTick">変換対象となるBMSのティック数。</param>
        /// <returns>指定されたティック数に相当する経過時間（秒）。</returns>
        public static double TickToElapsedTime(int bpm, float measureLength, double bmsTick)
        {
            var bmsTicksPerSecond = TicksPerSecond(bpm, measureLength);
            return bmsTick / bmsTicksPerSecond;
        }
    }
}