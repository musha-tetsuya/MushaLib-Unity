using ClosedXML.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace MushaLib.MasterData.Editor
{
    /// <summary>
    /// マスターデータコンバーター
    /// </summary>
    public class MasterDataConverter
#if UNITY_EDITOR
        : EditorWindow
#endif
    {
        /// <summary>
        /// 型のデフォルト値辞書
        /// </summary>
        public static IReadOnlyDictionary<string, JToken> DefaultValueList { get; } = new Dictionary<string, JToken>
        {
            { "byte",   default(byte)   },
            { "sbyte",  default(sbyte)  },
            { "int",    default(int)    },
            { "uint",   default(uint)   },
            { "short",  default(short)  },
            { "ushort", default(ushort) },
            { "long",   default(long)   },
            { "ulong",  default(ulong)  },
            { "float",  default(float)  },
            { "double", default(double) },
            { "char",   default(char)   },
            { "bool",   default(bool)   },
        };

        /// <summary>
        /// 型のJTokenパーサー辞書
        /// </summary>
        public static IReadOnlyDictionary<string, Func<string, JToken>> ParserList { get; } = new Dictionary<string, Func<string,JToken>>
        {
            { "byte",    _ => byte.Parse(_)   },
            { "sbyte",   _ => sbyte.Parse(_)  },
            { "int",     _ => int.Parse(_)    },
            { "uint",    _ => uint.Parse(_)   },
            { "short",   _ => short.Parse(_)  },
            { "ushort",  _ => ushort.Parse(_) },
            { "long",    _ => long.Parse(_)   },
            { "ulong",   _ => ulong.Parse(_)  },
            { "float",   _ => float.Parse(_)  },
            { "double",  _ => double.Parse(_) },
            { "char",    _ => char.Parse(_)   },
            { "bool",    _ => bool.Parse(_)   },
            { "string",  _ => _ },

            { "byte?",   _ => byte.Parse(_)   },
            { "sbyte?",  _ => sbyte.Parse(_)  },
            { "int?",    _ => int.Parse(_)    },
            { "uint?",   _ => uint.Parse(_)   },
            { "short?",  _ => short.Parse(_)  },
            { "ushort?", _ => ushort.Parse(_) },
            { "long?",   _ => long.Parse(_)   },
            { "ulong?",  _ => ulong.Parse(_)  },
            { "float?",  _ => float.Parse(_)  },
            { "double?", _ => double.Parse(_) },
            { "char?",   _ => char.Parse(_)   },
            { "bool?",   _ => bool.Parse(_)   },

            { "byte[]",    _ => new JArray(JsonConvert.DeserializeObject<byte[]>(_))    },
            { "sbyte[]",   _ => new JArray(JsonConvert.DeserializeObject<sbyte[]>(_))   },
            { "int[]",     _ => new JArray(JsonConvert.DeserializeObject<int[]>(_))     },
            { "uint[]",    _ => new JArray(JsonConvert.DeserializeObject<uint[]>(_))    },
            { "short[]",   _ => new JArray(JsonConvert.DeserializeObject<short[]>(_))   },
            { "ushort[]",  _ => new JArray(JsonConvert.DeserializeObject<ushort[]>(_))  },
            { "long[]",    _ => new JArray(JsonConvert.DeserializeObject<long[]>(_))    },
            { "ulong[]",   _ => new JArray(JsonConvert.DeserializeObject<ulong[]>(_))   },
            { "float[]",   _ => new JArray(JsonConvert.DeserializeObject<float[]>(_))   },
            { "double[]",  _ => new JArray(JsonConvert.DeserializeObject<double[]>(_))  },
            { "char[]",    _ => new JArray(JsonConvert.DeserializeObject<char[]>(_))    },
            { "string[]",  _ => new JArray(JsonConvert.DeserializeObject<string[]>(_))  },
            { "bool[]",    _ => new JArray(JsonConvert.DeserializeObject<bool[]>(_))    },

            { "byte?[]",   _ => new JArray(JsonConvert.DeserializeObject<byte?[]>(_))   },
            { "sbyte?[]",  _ => new JArray(JsonConvert.DeserializeObject<sbyte?[]>(_))  },
            { "int?[]",    _ => new JArray(JsonConvert.DeserializeObject<int?[]>(_))    },
            { "uint?[]",   _ => new JArray(JsonConvert.DeserializeObject<uint?[]>(_))   },
            { "short?[]",  _ => new JArray(JsonConvert.DeserializeObject<short?[]>(_))  },
            { "ushort?[]", _ => new JArray(JsonConvert.DeserializeObject<ushort?[]>(_)) },
            { "long?[]",   _ => new JArray(JsonConvert.DeserializeObject<long?[]>(_))   },
            { "ulong?[]",  _ => new JArray(JsonConvert.DeserializeObject<ulong?[]>(_))  },
            { "float?[]",  _ => new JArray(JsonConvert.DeserializeObject<float?[]>(_))  },
            { "double?[]", _ => new JArray(JsonConvert.DeserializeObject<double?[]>(_)) },
            { "char?[]",   _ => new JArray(JsonConvert.DeserializeObject<char?[]>(_))   },
            { "bool?[]",   _ => new JArray(JsonConvert.DeserializeObject<bool?[]>(_))   },
        };

        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args">
        /// "-xlsxPath": xlsxのパス or xlsxが入っているフォルダのパス
        /// "-csOutputDirectory": csファイルの出力先（指定しない場合は出力しない）
        /// "-jsonOutputDirectory": jsonファイルの出力先（指定しない場合は出力しない）
        /// "-csvOutputDirectory": csvファイルの出力先（指定しない場合は出力しない）
        /// </param>
        public static void Main(string[] args)
        {
#if !UNITY_EDITOR
            // カレントディレクトリのセット
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
#endif
            // コンバートするxlsx一覧
            var xlsxList = new List<string>();

            // cs, json, csvの出力先フォルダ
            string csOutputDirectory = null;
            string jsonOutputDirectory = null;
            string csvOutputDirectory = null;

            // 引数解析
            string currentArg = null;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-xlsxPath":
                    case "-csOutputDirectory":
                    case "-jsonOutputDirectory":
                    case "-csvOutputDirectory":
                        currentArg = args[i];
                        continue;
                }

                switch (currentArg)
                {
                    // 変換対象xlsxのパス
                    case "-xlsxPath":
                        {
                            if (args[i].EndsWith(".xlsx"))
                            {
                                // xlsxをリストに追加
                                xlsxList.Add(args[i]);
                            }
                            else if (Directory.Exists(args[i]))
                            {
                                // フォルダ内のxlsxをリストに追加
                                xlsxList.AddRange(Directory.GetFiles(args[i], "*.xlsx"));
                            }
                        }
                        break;

                    // cs出力先
                    case "-csOutputDirectory":
                        {
                            csOutputDirectory = args[i];
                        }
                        break;

                    // json出力先
                    case "-jsonOutputDirectory":
                        {
                            jsonOutputDirectory = args[i];
                        }
                        break;

                    // csv出力先
                    case "-csvOutputDirectory":
                        {
                            csvOutputDirectory = args[i];
                        }
                        break;
                }
            }

            // 存在しないxlsxは除去
            xlsxList.RemoveAll(_ => !File.Exists(_));

            // xlsxが指定されていない
            if (xlsxList.Count == 0)
            {
                LogWarning("Error：コンバートするマスターデータのxlsxを指定して下さい。");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < xlsxList.Count; i++)
            {
                Log(xlsxList[i]);

                // xlsx読み込み
                var workbook = new XLWorkbook(xlsxList[i]);

                // xlsx名
                var xlsxName = Path.GetFileNameWithoutExtension(xlsxList[i]);

                // シートから抽出したシートデータリスト
                var sheets = new List<SheetData>();

                foreach (var worksheet in workbook.Worksheets)
                {
                    SheetData sheetData = null;

                    if (!TryParseToSheetData(worksheet, out sheetData))
                    {
                        continue;
                    }

                    sheets.Add(sheetData);
                }

                // cs出力するなら
                if (!string.IsNullOrEmpty(csOutputDirectory))
                {
                    string outputDirectory = csOutputDirectory;

                    if (!outputDirectory.EndsWith(xlsxName, StringComparison.OrdinalIgnoreCase))
                    {
                        outputDirectory += $"/{xlsxName}";
                    }

                    if (!Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                    }

                    // cs出力
                    foreach (var sheet in sheets)
                    {
                        string outputPath = $"{outputDirectory}/{sheet.name}.cs";
                        Log($"cs出力：{outputPath}");

                        // UTF-8 BOM無 LF
                        File.WriteAllText(outputPath, sheet.GetCsString(xlsxName).Replace("\r", null), Encoding.UTF8);
                    }
                }

                // json出力するなら
                if (!string.IsNullOrEmpty(jsonOutputDirectory))
                {
                    string outputDirectory = jsonOutputDirectory;

                    if (!outputDirectory.EndsWith(xlsxName, StringComparison.OrdinalIgnoreCase))
                    {
                        outputDirectory += $"/{xlsxName}";
                    }

                    if (!Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                    }

                    // json出力
                    foreach (var sheet in sheets)
                    {
                        string outputPath = $"{outputDirectory}/{sheet.name}.json";
                        Log($"json出力：{outputPath}");

                        // UTF-8 BOM無 LF
                        File.WriteAllText(outputPath, sheet.GetJArray().ToString().Replace("\r", null), Encoding.UTF8);
                    }
                }

                // csv出力するなら
                if (!string.IsNullOrEmpty(csvOutputDirectory))
                {
                    string outputDirectory = csvOutputDirectory;

                    if (!outputDirectory.EndsWith(xlsxName, StringComparison.OrdinalIgnoreCase))
                    {
                        outputDirectory += $"/{xlsxName}";
                    }

                    if (!Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                    }

                    // csv出力
                    foreach (var sheet in sheets)
                    {
                        string outputPath = $"{outputDirectory}/{sheet.name}.csv";
                        Log($"csv出力：{outputPath}");

                        // UTF-8 BOM無 LF
                        File.WriteAllText(outputPath, sheet.GetCsvString().Replace("\r", null), Encoding.UTF8);
                    }
                }
            }

            Log("終了");
        }

        /// <summary>
        /// シートからシートデータを取得する
        /// </summary>
        private static bool TryParseToSheetData(IXLWorksheet sheet, out SheetData result)
        {
            // アルファベット文字から始まっていないシートはスキップ
            if (!char.IsLetter(sheet.Name[0]))
            {
                result = null;
                return false;
            }

            // アルファベット、数字、アンダーバー以外の文字が入っているシートはスキップ
            if (!Regex.IsMatch(sheet.Name, @"^[a-zA-Z0-9_]+$"))
            {
                result = null;
                return false;
            }

            result = new SheetData();
            result.sheet = sheet;
            result.name = sheet.Name;
            result.description = sheet.Cell((int)Header.Description, 2).GetString();
            result.fields = new();

            // Start位置の検索
            for (int y = (int)Header.Length, i = 0; ; y++)
            {
                var str = sheet.Cell(y, 1).GetString();

                if (str.Equals("Start", StringComparison.Ordinal))
                {
                    result.startY = y;
                    break;
                }
                else
                {
                    i++;

                    if (i > 100)
                    {
                        // Startが見つからなかった
                        LogWarning($"\"{result.name}\" does not have \"Start\".");
                        return false;
                    }
                }
            }

            // End位置の検索
            for (int x = 2, i = 0; ; x++)
            {
                var str = sheet.Cell(result.startY, x).GetString();

                if (string.IsNullOrEmpty(str))
                {
                    i++;

                    if (i > 100)
                    {
                        // Endが見つからなかった
                        LogWarning($"\"{result.name}\" does not have \"End\".");
                        return false;
                    }
                }
                else
                {
                    i = 0;

                    if (str.Equals("End", StringComparison.Ordinal))
                    {
                        result.endX = x;
                        break;
                    }
                }
            }

            // 変数情報の取得
            for (int x = 2; x < result.endX; x++)
            {
                var fi = new FieldInfo();

                // 変数名取得
                fi.name = sheet.Cell(result.startY, x).GetString();

                // 変数名が空 or アルファベット以外から始まる列はスキップ
                if (string.IsNullOrEmpty(fi.name) || !char.IsLetter(fi.name[0]))
                {
                    continue;
                }

                // 変数型取得
                fi.type = sheet.Cell((int)Header.DataType, x).GetString();

                // 変換可能な変数型なら
                if (ParserList.Keys.Contains(fi.type.ToLower()))
                {
                    // 変数型は小文字に
                    fi.type = fi.type.ToLower();

                    // 変数名を先頭小文字に修正（idは全て小文字に）
                    if (fi.name.Length == 1 || fi.name.Equals("id", StringComparison.OrdinalIgnoreCase))
                    {
                        fi.name = fi.name.ToLower();
                    }
                    else
                    {
                        fi.name = char.ToLower(fi.name[0]) + fi.name.Substring(1);
                    }
                }

                fi.posX = x;
                fi.summary = sheet.Cell((int)Header.Summary, x).GetString();

                result.fields.Add(fi);
            }

            return true;
        }

        /// <summary>
        /// ログ出力
        /// </summary>
        private static void Log(string message)
        {
#if UNITY_EDITOR
            Debug.Log(message);
#else
            Console.WriteLine(message);
#endif
        }

        /// <summary>
        /// 警告ログ出力
        /// </summary>
        private static void LogWarning(string message)
        {
#if UNITY_EDITOR
            Debug.LogWarning(message);
#else
            Console.WriteLine(message);
#endif
        }


#if UNITY_EDITOR
        /// <summary>
        /// EditorWindowを開く
        /// </summary>
        [MenuItem("MushaLib/MasterData/MasterDataConverter")]
        private static void OpenWindow()
        {
            GetWindow<MasterDataConverter>();
        }

        /// <summary>
        /// EditorUserSettingsのキー
        /// </summary>
        private static readonly string EditorUserSettingsKey = typeof(MasterDataConverter).FullName;

        /// <summary>
        /// 変換対象xlsxのパス
        /// </summary>
        private string xlsxPath;

        /// <summary>
        /// cs出力先
        /// </summary>
        private string csOutputDirectory;

        /// <summary>
        /// json出力先
        /// </summary>
        private string jsonOutputDirectory;

        /// <summary>
        /// csv出力先
        /// </summary>
        private string csvOutputDirectory;

        /// <summary>
        /// OnEnable
        /// </summary>
        private void OnEnable()
        {
            xlsxPath = EditorUserSettings.GetConfigValue($"{EditorUserSettingsKey}.xlsxPath");
            csOutputDirectory = EditorUserSettings.GetConfigValue($"{EditorUserSettingsKey}.csOutputDirectory");
            jsonOutputDirectory = EditorUserSettings.GetConfigValue($"{EditorUserSettingsKey}.jsonOutputDirectory");
            csvOutputDirectory = EditorUserSettings.GetConfigValue($"{EditorUserSettingsKey}.csvOutputDirectory");
        }

        /// <summary>
        /// OnDisable
        /// </summary>
        private void OnDisable()
        {
            EditorUserSettings.SetConfigValue($"{EditorUserSettingsKey}.xlsxPath", xlsxPath);
            EditorUserSettings.SetConfigValue($"{EditorUserSettingsKey}.csOutputDirectory", csOutputDirectory);
            EditorUserSettings.SetConfigValue($"{EditorUserSettingsKey}.jsonOutputDirectory", jsonOutputDirectory);
            EditorUserSettings.SetConfigValue($"{EditorUserSettingsKey}.csvOutputDirectory", csvOutputDirectory);
        }

        /// <summary>
        /// OnGUI
        /// </summary>
        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            {
                xlsxPath = EditorGUILayout.TextField("変換対象xlsx", xlsxPath, GUILayout.Height(38));

                GUILayout.BeginVertical(GUILayout.Width(100));
                {
                    if (GUILayout.Button("Select File"))
                    {
                        var xlsxDirectory = string.IsNullOrEmpty(xlsxPath) ? "" : Path.GetDirectoryName(xlsxPath);
                        xlsxPath = EditorUtility.OpenFilePanel("Select MasterData File", xlsxDirectory, "xlsx");
                    }

                    if (GUILayout.Button("Select Folder"))
                    {
                        var xlsxDirectory = string.IsNullOrEmpty(xlsxPath) ? "" : Path.GetDirectoryName(xlsxPath);
                        xlsxPath = EditorUtility.OpenFolderPanel("Select MasterData Folder", xlsxDirectory, "");
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                csOutputDirectory = EditorGUILayout.TextField("cs出力先", csOutputDirectory);

                if (GUILayout.Button("Select", GUILayout.Width(100)))
                {
                    csOutputDirectory = EditorUtility.SaveFolderPanel("cs出力先の選択", csOutputDirectory, "");
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                jsonOutputDirectory = EditorGUILayout.TextField("json出力先", jsonOutputDirectory);

                if (GUILayout.Button("Select", GUILayout.Width(100)))
                {
                    jsonOutputDirectory = EditorUtility.SaveFolderPanel("json出力先の選択", jsonOutputDirectory, "");
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                csvOutputDirectory = EditorGUILayout.TextField("csv出力先", csvOutputDirectory);

                if (GUILayout.Button("Select", GUILayout.Width(100)))
                {
                    csvOutputDirectory = EditorUtility.SaveFolderPanel("csv出力先の選択", csvOutputDirectory, "");
                }
            }
            GUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(xlsxPath) || string.IsNullOrEmpty(csOutputDirectory + jsonOutputDirectory + csvOutputDirectory));
            {
                if (GUILayout.Button("Convert"))
                {
                    List<string> args = new();
                    args.Add("-xlsxPath");
                    args.Add(xlsxPath);

                    if (!string.IsNullOrEmpty(csOutputDirectory))
                    {
                        args.Add("-csOutputDirectory");
                        args.Add(csOutputDirectory);
                    }
                    if (!string.IsNullOrEmpty(jsonOutputDirectory))
                    {
                        args.Add("-jsonOutputDirectory");
                        args.Add(jsonOutputDirectory);
                    }
                    if (!string.IsNullOrEmpty(csvOutputDirectory))
                    {
                        args.Add("-csvOutputDirectory");
                        args.Add(csvOutputDirectory);
                    }

                    EditorUtility.DisplayProgressBar("Convert MasterData", xlsxPath, 0.5f);

                    Main(args.ToArray());

                    EditorUtility.ClearProgressBar();

                    AssetDatabase.Refresh();
                }
            }
            EditorGUI.EndDisabledGroup();
        }
#endif
    }
}
