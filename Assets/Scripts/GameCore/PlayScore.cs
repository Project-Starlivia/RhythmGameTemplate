using System.Text;

namespace GameCore
{
    /// <summary>
    /// 楽曲のスコアデータを表現するクラス
    /// 曲名、初期BPM、全体のオフセット、拍データを管理する
    /// </summary>
    public sealed class PlayScore
    {
        public readonly string Title;
        public readonly int Bpm;
        public readonly double BeatsOffset;
        private readonly Beat[] beats;
        public Beat[] Beats => beats;

        public PlayScore(string title, int bpm, double beatsOffset, Beat[] beats)
        {
            Title = title;
            Bpm = bpm;
            BeatsOffset = beatsOffset;
            this.beats = beats;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Title: {Title}");
            sb.AppendLine($"BeatsOffset: {BeatsOffset}");
            sb.AppendLine($"Measures: {beats.Length / 9600}");
            sb.AppendLine($"Total Beats: {beats.Length}");
        
            foreach (var beat in beats)
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
        public readonly int BmsTick;
        public readonly Note[] Notes;

        public Beat(int bmsTick, Note[] notes)
        {
            BmsTick = bmsTick;
            Notes = notes;
        }
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"BmsTick: {BmsTick}");
            sb.AppendLine($"Notes: {Notes?.Length ?? 0}");

            if (Notes != null)
            {
                foreach (var note in Notes)
                {
                    sb.AppendLine(note.ToString());
                }
            }
            
            return sb.ToString();
        }
    }

    /// <summary>
    /// ノートデータを表現するクラス
    /// 楽曲内のノートの種類と位置情報（レーン）を管理する
    /// </summary>
    public class Note
    {
        public readonly NoteType Type;
        public readonly NoteLane Lane;
        
        public Note(NoteType type, NoteLane lane)
        {
            Type = type;
            Lane = lane;
        }
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Type: {Type}");
            sb.AppendLine($"Lane: {Lane}");
            return sb.ToString();
        }
    }
}
