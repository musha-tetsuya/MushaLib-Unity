using ClosedXML.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MushaLib.MasterData.Editor
{
    /// <summary>
    /// シートデータ
    /// </summary>
    internal class SheetData
    {
        /// <summary>
        /// シート
        /// </summary>
        public IXLWorksheet sheet;

        /// <summary>
        /// 名前
        /// </summary>
        public string name;

        /// <summary>
        /// 概要
        /// </summary>
        public string description;

        /// <summary>
        /// 変数情報
        /// </summary>
        public List<FieldInfo> fields;

        /// <summary>
        /// データ開始位置Y
        /// </summary>
        public int startY;

        /// <summary>
        /// データ終了位置X
        /// </summary>
        public int endX;

        /// <summary>
        /// JArray
        /// </summary>
        public JArray jarray;

        /// <summary>
        /// cs出力文字列を取得する
        /// </summary>
        public string GetCsString(string xlsxName)
        {
            var idType = fields[0].name.Equals("id", StringComparison.OrdinalIgnoreCase) ? fields[0].type : "int";

            var sb = new StringBuilder();
            sb.AppendLine($"// これはMasterDataConverterで自動生成されたファイルです。直接編集しないで下さい。");
            sb.AppendLine($"using System.Collections;");
            sb.AppendLine($"using System.Collections.Generic;");
            sb.AppendLine($"");
            sb.AppendLine($"namespace MasterData.{xlsxName}");
            sb.AppendLine($"{{");
            sb.AppendLine($"    /// <summary>");
            foreach (var description in description.Split('\n'))
            {
                sb.AppendLine($"    /// {description}");
            }
            sb.AppendLine($"    /// </summary>");
            sb.AppendLine($"    public partial class {name} : MushaLib.MasterData.ModelBase<{idType}>");
            sb.AppendLine($"    {{");
            foreach (var fi in fields.Where(_ => !_.name.Equals("id", StringComparison.Ordinal)))
            {
                sb.AppendLine($"        /// <summary>");
                foreach (var summary in fi.summary.Split('\n'))
                {
                    sb.AppendLine($"        /// {summary}");
                }
                sb.AppendLine($"        /// </summary>");
                sb.AppendLine($"        public {fi.type} {fi.name};");
                sb.AppendLine($"");
            }
            sb.AppendLine($"        /// <summary>");
            sb.AppendLine($"        /// {name}テーブル");
            sb.AppendLine($"        /// <summary>");
            sb.AppendLine($"        public partial class Table : MushaLib.MasterData.TableBase<Table, {idType}, {name}>");
            sb.AppendLine($"        {{");
            sb.AppendLine($"        }}");
            sb.AppendLine($"    }}");
            sb.AppendLine($"}}");

            return sb.ToString();
        }

        /// <summary>
        /// JArrayを取得する
        /// </summary>
        public JArray GetJArray()
        {
            if (jarray != null)
            {
                return jarray;
            }

            jarray = new JArray();

            if (fields.Count > 0)
            {
                for (int y = startY + 1; ; y++)
                {
                    var jobj = new JObject();
                    var isSkip = false;
                    var isEnd = true;

                    // 「ID」列が無いかもしれないので先にプロパティ追加しておく
                    jobj["id"] = jarray.Count + 1;

                    for (int i = 0; i < fields.Count; i++)
                    {
                        // 変数情報
                        var fi = fields[i];

                        // データ値
                        var val = sheet.Cell(y, fi.posX).GetString();

                        JToken token = null;

                        // セルの内容が空
                        if (string.IsNullOrEmpty(val))
                        {
                            // 一番左の列が空
                            if (i == 0)
                            {
                                // この行はスキップ
                                isSkip = true;
                            }

                            // 型のデフォルト値取得
                            MasterDataConverter.DefaultValueList.TryGetValue(fi.type, out token);
                        }
                        else
                        {
                            isEnd = false;

                            if (MasterDataConverter.ParserList.ContainsKey(fi.type))
                            {
                                // 型の値に変換
                                token = MasterDataConverter.ParserList[fi.type].Invoke(val);
                            }
                            else
                            {
                                try
                                {
                                    token = JToken.Parse(val);
                                }
                                catch
                                {
                                    // そのまま文字列
                                    token = val;
                                }
                            }
                        }

                        // プロパティ追加
                        jobj[fi.name] = token;
                    }

                    // 全ての列のセルの内容が空だった
                    if (isEnd)
                    {
                        // 終了
                        break;
                    }

                    // 先頭列のセルの内容が空だった
                    if (isSkip)
                    {
                        // 出力しない
                        continue;
                    }

                    // 出力配列に追加
                    jarray.Add(jobj);
                }
            }

            return jarray;
        }

        /// <summary>
        /// csv出力文字列を取得する
        /// </summary>
        public string GetCsvString()
        {
            var jarray = GetJArray();

            // 変数名取得
            var fieldNames = fields.Select(_ => _.name).ToList();

            // 変数名にIDが含まれていなかったらIDを先頭に挿入
            if (!fieldNames.Contains("id"))
            {
                fieldNames.Insert(0, "id");
            }

            var sb = new StringBuilder();

            // 変数名書き込み
            sb.AppendLine(fieldNames.Aggregate((a, b) => $"{a},{b}"));

            // データ書き込み
            foreach (var token in jarray)
            {
                sb.AppendLine(fieldNames
                    .Select(fieldName => token[fieldName])
                    .Select(x =>
                    {
                        if (x.Type == JTokenType.Object
                        || x.Type == JTokenType.Array)
                        {
                            x = JToken.FromObject(x.ToString(Formatting.None));
                        }

                        return x.ToString(Formatting.None);
                    })
                    .Aggregate((a, b) => $"{a},{b}")
                );
            }

            return sb.ToString();
        }
    }
}