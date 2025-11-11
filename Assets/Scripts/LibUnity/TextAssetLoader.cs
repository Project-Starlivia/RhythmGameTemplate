using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LibUnity
{
    public static class TextAssetLoader
    {
        /// <summary>
        /// 指定されたTextAssetを読み込んで、行ごとに分割した文字列のリストとして返します。
        /// </summary>
        /// <param name="file">読み込むデータが格納されたTextAssetオブジェクト。</param>
        /// <returns>データを行ごとに分割した文字列のリスト。</returns>
        public static List<string> Load(TextAsset file)
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
}