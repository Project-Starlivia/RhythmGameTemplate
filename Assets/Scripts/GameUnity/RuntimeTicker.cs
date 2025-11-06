using BmsCore;
using GameCore;
using UnityEngine;

namespace GameUnity
{
    /// <summary>
    /// ゲーム内のビートまたはタイミング処理を制御するためのクラス。
    /// 楽曲のスコア、経過時間に基づき現在および前回のTickを管理します。
    /// </summary>
    public class RuntimeTicker
    {
        private readonly RuntimeScore _runtimeScore;
        
        private double _startTime;
        private double _currentTick;
        private double _previousTick;

        
        private bool _isPlaying = false;
        
        public double CurrentTick => _currentTick;
        public double PreviousTick => _previousTick;

        /// <summary>
        /// ゲーム内のビートまたはタイミング処理を制御するためのクラス。
        /// 楽曲のスコア、経過時間に基づき現在および前回のTickを管理します。
        /// </summary>
        public RuntimeTicker(RuntimeScore runtimeScore)
        {
            _runtimeScore = runtimeScore;
        }

        /// <summary>
        /// 再生を開始します。
        /// 指定されたオフセットを考慮し、再生の初期状態を設定します。
        /// </summary>
        /// <param name="offset">再生開始時のオフセット値（秒単位）</param>
        public void Play(float offset)
        {
            var offsetTick = -TickCalculator.TimeToTick(_runtimeScore.BeatsOffset + offset, _runtimeScore.Bpm);
            _startTime = offsetTick;
            _currentTick = offsetTick;
            _previousTick = offsetTick;
            _isPlaying = true;
        }

        
        /// <summary>
        /// 毎フレーム呼び出されるTick処理
        /// 現在の時間に基づいてビートを処理する
        /// </summary>
        public void Tick()
        {
            if (!_isPlaying) return;
            var currentTime = Time.timeAsDouble;
            var elapsedTime = currentTime - _startTime;

            var elapsedTick =
                TickCalculator.TimeToTick(elapsedTime, _runtimeScore.Bpm);
            
            _previousTick = _currentTick;
            
            _currentTick = _startTime + elapsedTick;
        }
    }
}