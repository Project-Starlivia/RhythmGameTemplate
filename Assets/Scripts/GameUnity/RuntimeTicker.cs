using BmsCore;
using GameCore;
using UnityEngine;

namespace GameUnity
{
    public class RuntimeTicker
    {
        private readonly RuntimeScore _runtimeScore;
        
        private double _startTime;
        private double _currentTick;
        private double _previousTick;

        
        private bool _isPlaying = false;
        
        public double CurrentTick => _currentTick;
        public double PreviousTick => _previousTick;
        
        public RuntimeTicker(RuntimeScore runtimeScore)
        {
            _runtimeScore = runtimeScore;
        }

        public void Play(float offset)
        {
            var offsetTick = -TickCalculator.ElapsedTimeToTick(_runtimeScore.BeatsOffset + offset, _runtimeScore.Bpm);
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
                TickCalculator.ElapsedTimeToTick(elapsedTime, _runtimeScore.Bpm);
            
            _previousTick = _currentTick;
            
            _currentTick = _startTime + elapsedTick;
        }
    }
}