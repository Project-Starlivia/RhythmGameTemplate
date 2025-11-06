using System;
using System.Collections.Generic;
using System.IO;
using BmsCore;
using GameCore;
using UnityEngine;
using UnityEngine.Serialization;

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
            var lines = LoadBmsFile(bmsFile);
            var bmsScore = BmsLoader.Load(lines);
            var runtimeScore = BmsToPlayConverter.Convert(bmsScore);

            _runtimeTicker = new RuntimeTicker(runtimeScore);
            _basicNoteManger = new BasicNoteManger(notePrefab, settings, laneInputs, runtimeScore);
            
            audioSource.PlayScheduled(AudioSettings.dspTime + settings.playOffset);
            _runtimeTicker.Play(settings.playOffset);
        }

        private void Update()
        {
            _runtimeTicker.Tick();
            _basicNoteManger.Update(_runtimeTicker.PreviousTick, _runtimeTicker.CurrentTick);
        }

        /// <summary>
        /// 指定されたTextAssetからBMSファイルを読み込んで、行ごとに分割した文字列のリストとして返します。
        /// </summary>
        /// <param name="file">読み込むBMSファイルが格納されたTextAssetオブジェクト。</param>
        /// <returns>BMSデータを行ごとに分割した文字列のリスト。</returns>
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
        public double appearMeasureOffset = 2;
        public double disappearMeasureOffset = 1;
        public float hitRangeMs = 75;
    }
}