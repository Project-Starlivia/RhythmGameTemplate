using System;
using System.Collections.Generic;
using System.IO;
using BmsCore;
using GameCore;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameUnity
{
    public class RhythmGameManager: MonoBehaviour
    {
        [SerializeField] private TextAsset bmsFile;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private NoteController notePrefab;
        [SerializeField] private RhythmGameSettings settings;

        private RuntimeTicker _runtimeTicker;
        private BasicNoteManger _basicNoteManger;

        private void Start()
        {
            var lines = LoadBmsFile(bmsFile);
            var bmsScore = BmsLoader.Load(lines);
            var runtimeScore = BmsToPlayConverter.Convert(bmsScore);

            _runtimeTicker = new RuntimeTicker(runtimeScore);
            _basicNoteManger = new BasicNoteManger(notePrefab, settings, runtimeScore);
            
            audioSource.PlayScheduled(AudioSettings.dspTime + settings.playOffset);
            _runtimeTicker.Play(settings.playOffset);
        }

        private void Update()
        {
            _runtimeTicker.Tick();
            _basicNoteManger.Update(_runtimeTicker.PreviousTick, _runtimeTicker.CurrentTick);
        }

        private List<string> LoadBmsFile(TextAsset file)
        {
            List<string> lines = new();
            using StringReader reader = new(file.text);
            while (reader.ReadLine() is { } line)
            {
                lines.Add(line);
            }

            return lines;
        }
    }

    [Serializable]
    public class RhythmGameSettings
    {
        public float playOffset = 3;
        public double notePosRatio = 5;
        public double appearMeasureOffset = 2;
        public double disappearMeasureOffset = 1;
        public float hitRangeMs = 75;
    }
}