using System;
using BmsCore;
using LibUnity;
using UnityEngine;

namespace GameUnity
{
    /// <summary>
    /// RhythmGameManagerクラスはリズムゲームの全体的な管理を行うためのクラスです。
    /// このクラスはUnityの MonoBehaviour を継承しており、ゲーム内でのリズムに関連するロジックや
    /// 設定の管理を行います。
    /// </summary>
    public class RhythmGameManager: MonoBehaviour
    {
        [SerializeField] private TextAsset bmsFile;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private NoteController notePrefab;
        [SerializeField] private LaneInput[] laneInputs;
        [SerializeField] private RhythmGameSettings settings;

        private RuntimeTicker _runtimeTicker;
        private BasicNoteManger _basicNoteManger;

        private void Start()
        {
            var lines = TextAssetLoader.Load(bmsFile);
            var bmsScore = BmsLoader.Load(lines);
            var runtimeScore = BmsToPlayConverter.Convert(bmsScore);
            Debug.Log(bmsScore);
            Debug.Log(runtimeScore);

            _runtimeTicker = new RuntimeTicker(runtimeScore);
            _basicNoteManger = new BasicNoteManger(notePrefab, settings, laneInputs, runtimeScore);

            foreach (var input in laneInputs)
            {
                input.input.action.Enable();
            }
            
        }

        private bool _musicStarted = false;
        
        private void Update()
        {
            if (!_musicStarted && 0 < Time.timeAsDouble)
            {
                audioSource.PlayScheduled(AudioSettings.dspTime + settings.playOffset);
                _runtimeTicker.Play(settings.playOffset);
                _musicStarted = true;
            }
            
            _runtimeTicker.Tick();
            _basicNoteManger.Update(_runtimeTicker.PreviousTick, _runtimeTicker.CurrentTick);
        }
        
        private void OnDestroy()
        {
            foreach (var input in laneInputs)
            {
                input.input.action.Disable();
            }
        }
    }
    
    
    /// <summary>
    /// RhythmGameSettingsクラスはリズムゲームの設定を管理するためのクラスです。
    /// プレイオフセット、ノートの位置比率、ノートが表示・非表示になるタイミング、
    /// そして判定範囲など、リズムゲームの重要なパラメータを保持します。
    /// このクラスは他のクラスから参照され、ゲームの挙動に影響を与える設定値を提供します。
    /// </summary>
    [Serializable]
    public class RhythmGameSettings
    {
        public float playOffset = 3;
        public double notePosRatio = 5;
        public double appearMeasureOffset = -2;
        public double disappearMeasureOffset = 1;
        public float hitRangeMs = 75;
    }
}