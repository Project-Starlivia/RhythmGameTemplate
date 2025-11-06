using System;
using System.Collections.Generic;
using System.Linq;
using GameCore;
using Unity.IntegerTime;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace GameUnity
{
    /// <summary>
    /// リズムゲーム内でのノート管理を行うクラス。
    /// ノートの出現、ヒット、消失などの管理を担当する。
    /// </summary>
    public class BasicNoteManger
    {
        private readonly NoteController _prefab;
        private readonly RhythmGameSettings _settings;
        private readonly LaneInput[] _laneInputs;
        private readonly int _bpm;
        private readonly Beat[] _beats;

        private Dictionary<Beat, NoteController> _noteInstances = new ();
        
        public BasicNoteManger(NoteController prefab, RhythmGameSettings settings, LaneInput[] laneInputs, RuntimeScore score)
        {
            _prefab = prefab;
            _settings = settings;
            _laneInputs = laneInputs;
            _beats = score.Beats;
        }

        public void Update(double previousTick, double currentTick)
        {
            // ===== ノートの出現処理 =====
            // 前フレームから現フレームの間に出現タイミングを迎えるノートを検索
            var appearBeats = BeatCalculator.FindOffsetBeats(_beats, previousTick, currentTick, _settings.appearMeasureOffset * TickCalculator.TicksPerMeasure);
            foreach (var beat in appearBeats)
            {
                // ノートオブジェクトを生成して初期化
                var instance = Object.Instantiate(_prefab);
                instance.Init(beat.Type);
                // 管理用ディクショナリに登録
                _noteInstances[beat] = instance;
            }

            // ===== 入力判定とノートのヒット処理 =====
            // 今フレームでトリガーされた入力のレーン情報を取得
            var inputLanes = _laneInputs
                .Where(laneInput => laneInput.input.action.triggered)
                .Select(laneInput => laneInput.lane);
            // ヒット可能範囲内のノートを検索し、入力されたレーンと一致するものを抽出
            var hitBeats = BeatCalculator
                .FindHitBeats(_beats, currentTick, _settings.hitRangeMs, _bpm)
                .Where(beat => inputLanes.Contains(beat.Lane));
           
            // ヒットしたノートを削除
            foreach (var beat in hitBeats)
            {
                if (!_noteInstances.TryGetValue(beat, out var instance)) continue;
                
                // ノートオブジェクトを破棄して管理リストから削除
                Object.Destroy(instance);
                _noteInstances.Remove(beat);
            }
            
            // ===== ノートの消失処理 =====
            // 判定ラインを通り過ぎて消失タイミングを迎えたノートを検索
            var disappearBeats = BeatCalculator.FindOffsetBeats(_noteInstances.Keys.ToArray(), previousTick, currentTick, _settings.disappearMeasureOffset * TickCalculator.TicksPerMeasure);
            foreach (var beat in disappearBeats)
            {
                if (!_noteInstances.TryGetValue(beat, out var instance)) continue;
                
                // ノートオブジェクトを破棄して管理リストから削除
                Object.Destroy(instance);
                _noteInstances.Remove(beat);
            }

            // ===== ノート位置の更新 =====
            // 現在存在する全てのノートの位置を更新
            foreach (var (beat, instance) in _noteInstances)
            {
                // ノートのタイミングと現在時刻の差分を計算
                var diff = beat.Tick - currentTick;
                // Y座標: 差分を小節単位に変換し、設定された比率を掛ける
                var y = diff / TickCalculator.TicksPerMeasure * _settings.notePosRatio;
                // X座標: レーン番号をそのまま使用
                var x = (int)beat.Lane;
                // 計算された位置を適用
                instance.transform.position = new Vector2(x, (float)y);
            }
        }
    }

    /// <summary>
    /// LaneInputクラスは、特定のノートレーンとその入力アクションを関連付けて管理するクラスです。
    /// リズムゲームでのレーンごとの入力処理をサポートします。
    /// </summary>
    [Serializable]
    public class LaneInput
    {
        public NoteLane lane;
        public InputActionProperty input;
    }
}