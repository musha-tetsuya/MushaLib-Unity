using ClosedXML.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        public static Dictionary<string, JToken> DefaultValueList { get; } = new()
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
        public static Dictionary<string, Func<string, JToken>> ParserList { get; } = new()
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
        /// "-path": xlsxのパス or xlsxが入っているフォルダのパス
        /// "-outputCs": csファイルの出力先（指定しない場合は出力しない）
        /// "-outputJson": jsonファイルの出力先（指定しない場合は出力しない）
        /// "-outputCsv": csvファイルの出力先（指定しない場合は出力しない）
        /// </param>
        private static void Main(string[] args)
        {
#if !UNITY_EDITOR
            // カレントディレクトリのセット
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
#endif
            // コンバートするxlsx一覧
            var xlsxList = new List<string>();

            // cs, json, csvの出力先フォルダ
            string outputCsDir = null;
            string outputJsonDir = null;
            string outputCsvDir = null;

            // 引数解析
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-path":
                        {
                            if (args[i + 1].EndsWith(".xlsx"))
                            {
                                // xlsxをリストに追加
                                xlsxList.Add(args[i + 1]);
                            }
                            else if (Directory.Exists(args[i + 1]))
                            {
                                // フォルダ内のxlsxをリストに追加
                                xlsxList.AddRange(Directory.GetFiles(args[i + 1], "*.xlsx"));
                            }
                        }
                        break;

                    case "-outputCs":
                        {
                            // cs出力先
                            outputCsDir = args[i + 1];
                        }
                        break;

                    case "-outputJson":
                        {
                            // json出力先
                            outputJsonDir = args[i + 1];
                        }
                        break;

                    case "-outputCsv":
                        {
                            // csv出力先
                            outputCsvDir = args[i + 1];
                        }
                        break;
                }
            }

            // 存在しないxlsxは除去
            xlsxList.RemoveAll(_ => !File.Exists(_));

            // xlsxが指定されていない
            if (xlsxList.Count == 0)
            {
                Log("Error：コンバートするマスターデータのxlsxを指定して下さい。");
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

                // シート空抽出したシートデータリスト
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
                if (!string.IsNullOrEmpty(outputCsDir))
                {
                    string csPath = $"{outputCsDir}/{xlsxName}.cs";
                    Log($"cs出力：{csPath}");

                    // cs出力
                    var sb = new StringBuilder();
                    sb.AppendLine($"//これはMasterDataConverterで自動生成されたファイルです。直接編集しないで下さい。");
                    sb.AppendLine($"");
                    sb.AppendLine($"namespace MasterData");
                    sb.AppendLine($"{{");
                    sb.AppendLine($"    namespace {xlsxName}");
                    sb.AppendLine($"    {{");
                    sb.AppendLine(sheets.Select(_ => _.GetCsString(xlsxName)).Aggregate((a, b) => $"{a}\n\n{b}"));
                    sb.AppendLine($"    }}");
                    sb.AppendLine($"}}");

                    // UTF-8 BOM無 LF
                    File.WriteAllText(csPath, sb.ToString().Replace("\r", null), Encoding.UTF8);
                }

                // json出力するなら
                if (!string.IsNullOrEmpty(outputJsonDir))
                {
                    // json出力
                    foreach (var sheet in sheets)
                    {
                        string jsonPath = $"{outputJsonDir}/{xlsxName}_{sheet.name}.json";
                        Log($"json出力：{jsonPath}");

                        // UTF-8 BOM無 LF
                        File.WriteAllText(jsonPath, sheet.GetJArray().ToString().Replace("\r", null), Encoding.UTF8);
                    }
                }

                // csv出力するなら
                if (!string.IsNullOrEmpty(outputCsvDir))
                {
                    // csv出力
                    foreach (var sheet in sheets)
                    {
                        string csvPath = $"{outputCsvDir}/{xlsxName}_{sheet.name}.csv";
                        Log($"csv出力：{csvPath}");

                        // UTF-8 BOM無 LF
                        File.WriteAllText(csvPath, sheet.GetCsvString().Replace("\r", null), Encoding.UTF8);
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
            if (!char.IsLetter(sheet.Name, 0))
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
                        Log($"\"{result.name}\" does not have \"Start\".");
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
                        Log($"\"{result.name}\" does not have \"End\".");
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

                // 変数名が空 or 「-」から始まる列はスキップ
                if (string.IsNullOrEmpty(fi.name) || fi.name.StartsWith("-"))
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
        /// コンバート対象のパス
        /// </summary>
        private string path;

        /// <summary>
        /// cs出力先
        /// </summary>
        private string outputCsDir;

        /// <summary>
        /// json出力先
        /// </summary>
        private string outputJsonDir;

        /// <summary>
        /// csv出力先
        /// </summary>
        private string outputCsvDir;

        /// <summary>
        /// OnGUI
        /// </summary>
        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            {
                path = EditorGUILayout.TextField("コンバート対象", path, GUILayout.Height(38));

                GUILayout.BeginVertical(GUILayout.Width(100));
                {
                    if (GUILayout.Button("Select File"))
                    {
                        path = EditorUtility.OpenFilePanel("Select MasterData File", "", "xlsx");
                    }

                    if (GUILayout.Button("Select Folder"))
                    {
                        path = EditorUtility.OpenFolderPanel("Select MasterData Folder", "", "");
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                outputCsDir = EditorGUILayout.TextField("cs出力先", outputCsDir);

                if (GUILayout.Button("Select", GUILayout.Width(100)))
                {
                    outputCsDir = EditorUtility.OpenFolderPanel("cs出力先の選択", "", "");
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                outputJsonDir = EditorGUILayout.TextField("json出力先", outputJsonDir);

                if (GUILayout.Button("Select", GUILayout.Width(100)))
                {
                    outputJsonDir = EditorUtility.OpenFolderPanel("json出力先の選択", "", "");
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                outputCsvDir = EditorGUILayout.TextField("csv出力先", outputCsvDir);

                if (GUILayout.Button("Select", GUILayout.Width(100)))
                {
                    outputCsvDir = EditorUtility.OpenFolderPanel("csv出力先の選択", "", "");
                }
            }
            GUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(path) || string.IsNullOrEmpty(outputCsDir + outputJsonDir + outputCsvDir));
            {
                if (GUILayout.Button("Convert"))
                {
                    List<string> args = new();
                    args.Add("-path");
                    args.Add(path);

                    if (!string.IsNullOrEmpty(outputCsDir))
                    {
                        args.Add("-outputCs");
                        args.Add(outputCsDir);
                    }
                    if (!string.IsNullOrEmpty(outputJsonDir))
                    {
                        args.Add("-outputJson");
                        args.Add(outputJsonDir);
                    }
                    if (!string.IsNullOrEmpty(outputCsvDir))
                    {
                        args.Add("-outputCsv");
                        args.Add(outputCsvDir);
                    }

                    Main(args.ToArray());

                    AssetDatabase.Refresh();
                }
            }
            EditorGUI.EndDisabledGroup();
        }
#endif
    }
}
