using MushaLib.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace MushaLib.UI.DQ.MapEditor
{
    /// <summary>
    /// マップ編集データ
    /// </summary>
    public class MapEditorData : ScriptableObject
    {
        /// <summary>
        /// マップサイズ
        /// </summary>
        [SerializeField]
        private Vector2Int m_Size = new(16, 16);

        /// <summary>
        /// ページ内セルサイズ
        /// </summary>
        [SerializeField]
        private Vector2 m_PageCellSize = new(16, 16);

        /// <summary>
        /// ページ内セル数
        /// </summary>
        [SerializeField]
        private Vector2Int m_PageCellCount = new(16, 16);

        /// <summary>
        /// マップチップデータ群
        /// </summary>
        [SerializeField]
        private MapChipEditorData[] m_ChipDatas;

        /// <summary>
        /// マップサイズ
        /// </summary>
        public Vector2Int Size
        {
            get => m_Size;
            set => m_Size = value;
        }

        /// <summary>
        /// ページ内セルサイズ
        /// </summary>
        public Vector2 PageCellSize
        {
            get => m_PageCellSize;
            set => m_PageCellSize = value;
        }

        /// <summary>
        /// ページ内セル数
        /// </summary>
        public Vector2Int PageCellCount
        {
            get => m_PageCellCount;
            set => m_PageCellCount = value;
        }

        /// <summary>
        /// マップチップデータ群
        /// </summary>
        public MapChipEditorData[] ChipDatas
        {
            get => m_ChipDatas;
            set => m_ChipDatas = value;
        }

#if UNITY_EDITOR
        /// <summary>
        /// カスタムインスペクター
        /// </summary>
        [CustomEditor(typeof(MapEditorData))]
        private class CustomInspector : Editor
        {
            /// <summary>
            /// EditorUserSettingsのキー
            /// </summary>
            private static readonly string EditorUserSettingsKey = typeof(MapEditorData).FullName;

            /// <summary>
            /// json出力先
            /// </summary>
            private string jsonOutputDirectory;

            /// <summary>
            /// アトラスのキャッシュ
            /// </summary>
            private SpriteAtlas[] m_CachedAtlasses;

            /// <summary>
            /// OnEnable
            /// </summary>
            private void OnEnable()
            {
                jsonOutputDirectory = EditorUserSettings.GetConfigValue($"{EditorUserSettingsKey}.jsonOutputDirectory");
            }

            /// <summary>
            /// OnDisable
            /// </summary>
            private void OnDisable()
            {
                EditorUserSettings.SetConfigValue($"{EditorUserSettingsKey}.jsonOutputDirectory", jsonOutputDirectory);
            }

            /// <summary>
            /// OnInspectorGUI
            /// </summary>
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                // Json変換ボタン押下時
                if (GUILayout.Button("Convert to JSON(MapData)"))
                {
                    // 出力先
                    var path = EditorUtility.SaveFilePanelInProject("Save JSON(MapData)", target.name, "json", "", jsonOutputDirectory);

                    if (!string.IsNullOrEmpty(path))
                    {
                        if (m_CachedAtlasses == null)
                        {
                            // アトラスをキャッシュ
                            m_CachedAtlasses = AssetDatabase
                                .FindAssets("t:SpriteAtlas")
                                .Select(AssetDatabase.GUIDToAssetPath)
                                .Select(AssetDatabase.LoadAssetAtPath<SpriteAtlas>)
                                .ToArray();
                        }

                        var obj = target as MapEditorData;
                        var mapData = new MapData();
                        mapData.Size = obj.Size;
                        mapData.PageCellSize = obj.PageCellSize;
                        mapData.PageCellCount = obj.PageCellCount;
                        mapData.AtlasKeys = new List<string>();
                        mapData.ChipDatas = obj.ChipDatas
                            .Select((chip, i) =>
                            {
                                var chipData = new MapChipData();
                                chipData.Index = i;
                                chipData.CollisionNum = chip.CollisionNum;

                                if (chip.Sprite != null && AssetDatabase.TryGetGUIDAndLocalFileIdentifier(chip.Sprite, out string guid, out long localId))
                                {
                                    chipData.SpriteKey = guid;

                                    // アトラスに含まれているかどうかをチェック
                                    var atlas = m_CachedAtlasses.FirstOrDefault(x => x.CanBindTo(chip.Sprite));
                                    if (atlas != null && AssetDatabase.TryGetGUIDAndLocalFileIdentifier(atlas, out guid, out localId))
                                    {
                                        chipData.SpriteKey = chip.Sprite.name;
                                        chipData.AtlasId = mapData.AtlasKeys.IndexOf(guid);
                                        if (chipData.AtlasId < 0)
                                        {
                                            chipData.AtlasId = mapData.AtlasKeys.Count;
                                            mapData.AtlasKeys.Add(guid);
                                        }
                                    }
                                }

                                return chipData;
                            })
                            .ToArray();

                        // Json出力
                        var settings = new JsonSerializerSettings();
                        settings.Converters.Add(new DelegateJsonConverter<Vector2> { OnWriteJson = (writer, value) => new JObject { { "x", value.x }, { "y", value.y } }.WriteTo(writer) });
                        settings.Converters.Add(new DelegateJsonConverter<Vector2Int> { OnWriteJson = (writer, value) => new JObject { { "x", value.x }, { "y", value.y } }.WriteTo(writer) });
                        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        File.WriteAllText(path, JsonConvert.SerializeObject(mapData, Formatting.Indented, settings).Replace("\r", null), Encoding.UTF8);

                        // 出力先保存
                        jsonOutputDirectory = Path.GetDirectoryName(path);

                        AssetDatabase.Refresh();
                    }
                }
            }
        }
#endif
    }
}
