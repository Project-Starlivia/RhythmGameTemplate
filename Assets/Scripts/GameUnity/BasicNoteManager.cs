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
    public class BasicNoteManger
    {
        private readonly NoteController _prefab;
        private readonly RhythmGameSettings _settings;
        private readonly LaneInput[] _laneInputs;
        private readonly int _bpm;
        private readonly Beat[] _beats;

        private Dictionary<Beat, NoteController> _noteInstances = new ();
        
        public BasicNoteManger(NoteController prefab, RhythmGameSettings settings, RuntimeScore score)
        {
            _prefab = prefab;
            _settings = settings;
            _beats = score.Beats;
        }

        public void Update(double previousTick, double currentTick)
        {
            var appearBeats = BeatCalculator.FindOffsetBeats(_beats, previousTick, currentTick, _settings.appearMeasureOffset * TickCalculator.TicksPerMeasure);
            foreach (var beat in appearBeats)
            {
                var instance = Object.Instantiate(_prefab);
                instance.Init(beat.Type);
                _noteInstances[beat] = instance;
            }

            var inputLanes = _laneInputs
                .Where(laneInput => laneInput.input.action.triggered)
                .Select(laneInput => laneInput.lane);
            var hitBeats = BeatCalculator
                .FindHitBeats(_beats, currentTick, _settings.hitRangeMs, _bpm)
                .Where(beat => inputLanes.Contains(beat.Lane));
           
            foreach (var beat in hitBeats)
            {
                if (!_noteInstances.TryGetValue(beat, out var instance)) continue;
                
                Object.Destroy(instance);
                _noteInstances.Remove(beat);
            }
            
            var disappearBeats = BeatCalculator.FindOffsetBeats(_noteInstances.Keys.ToArray(), previousTick, currentTick, _settings.disappearMeasureOffset * TickCalculator.TicksPerMeasure);
            foreach (var beat in disappearBeats)
            {
                if (!_noteInstances.TryGetValue(beat, out var instance)) continue;
                
                Object.Destroy(instance);
                _noteInstances.Remove(beat);
            }

            foreach (var (beat, instance) in _noteInstances)
            {
                var diff = beat.Tick - currentTick;
                var y = diff / TickCalculator.TicksPerMeasure * _settings.notePosRatio;
                var x = (int)beat.Lane;
                instance.transform.position = new Vector2(x, (float)y);
            }
        }
    }

    [Serializable]
    public class LaneInput
    {
        public NoteLane lane;
        public InputActionProperty input;
    }
}