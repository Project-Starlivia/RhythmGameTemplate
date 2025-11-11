using System.Text;

namespace GameCore
{
    /// <summary>
    /// 楽曲のスコアデータを表現するクラス
    /// 曲名、初期BPM、全体のオフセット、拍データを管理する
    /// </summary>
    public sealed class RuntimeScore
    {
        public readonly string Title;
        public readonly int Bpm;
        public readonly double BeatsOffset;
        private readonly Beat[] _beats;
        public Beat[] Beats => _beats;

        public RuntimeScore(string title, int bpm, double beatsOffset, Beat[] beats)
        {
            Title = title;
            Bpm = bpm;
            BeatsOffset = beatsOffset;
            _beats = beats;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Title: {Title}");
            sb.AppendLine($"BPM: {Bpm}");
            sb.AppendLine($"BeatsOffset: {BeatsOffset}");
            sb.AppendLine($"Measures: {_beats.Length / 9600}");
            sb.AppendLine($"Total Beats: {_beats.Length}");
        
            foreach (var beat in _beats)
            {
                sb.AppendLine(beat.ToString());
            }

            return sb.ToString();
        }
    }


    /// <summary>
    /// 楽曲の1拍分のデータを表すクラス
    /// BPM変更、ストップ、レセプター変更、ノート配置情報を含む
    /// </summary>
    public class Beat
    {
        public readonly double Tick;
        
        public readonly NoteType Type;
        public readonly NoteLane Lane;
        
        public Beat(double tick, NoteType type, NoteLane lane)
        {
            Tick = tick;
            Type = type;
            Lane = lane;
        }
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"BmsTick: {Tick}");
            sb.AppendLine($"Type: {Type}");
            sb.AppendLine($"Lane: {Lane}");
            return sb.ToString();
        }
    }
}
