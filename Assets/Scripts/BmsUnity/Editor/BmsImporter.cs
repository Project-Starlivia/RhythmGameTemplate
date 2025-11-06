using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace BmsUnity.Editor
{
    [ScriptedImporter(1, "bms")]
    public class BmsImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            TextAsset subAsset = new(File.ReadAllText(ctx.assetPath));
            ctx.AddObjectToAsset("text", subAsset);
            ctx.SetMainObject(subAsset);
        }
    }
}
