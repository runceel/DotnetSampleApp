---
name: file-based-apps
description: >
  .NET 10 file-based apps を使ったスクリプト実行スキル。Copilot がデータ処理・ファイル操作・簡易ツールなどの
  スクリプト的な処理を行う際に、C# の file-based app として実行する方法を定義する。
  使い捨ての処理はファイルを作成せず stdin パイプで実行し、再利用可能な処理のみ .cs ファイルとして保存すること。
---

# .NET 10 File-based Apps スクリプト実行スキル

あなたがスクリプト的な処理（データ変換、ファイル操作、API 呼び出し、テキスト処理、計算、コード生成など）を
実行する必要がある場合、**Python や Bash ではなく C# で書き、.NET 10 の file-based app として実行せよ。**

---

## 最重要ルール: 使い捨て処理はファイルを作らない

### stdin パイプ実行 (使い捨て・ワンショット処理)

結果を確認するだけの使い捨て処理では **絶対にファイルを作成するな。**
`dotnet run -` に C# コードを stdin でパイプして実行せよ。

**PowerShell:**

```powershell
'Console.WriteLine("Hello");' | dotnet run -
```

```powershell
@'
var files = Directory.GetFiles(".", "*.cs", SearchOption.AllDirectories);
foreach (var f in files) Console.WriteLine(f);
'@ | dotnet run -
```

**Bash:**

```bash
echo 'Console.WriteLine(DateTime.Now);' | dotnet run -
```

### stdin パイプを使うべきケース

- ワンライナーや数行程度の処理
- 結果を確認するだけで再利用しない処理
- データの一時的な変換・集計・確認
- 環境情報の取得・表示
- 簡単な計算・文字列操作

### 判断基準

| 条件 | 方法 |
|---|---|
| 使い捨て・結果確認だけ | `'code' \| dotnet run -` (ファイル作成禁止) |
| 再利用する・複雑なロジック | `.cs` ファイルとして保存 |
| NuGet パッケージが必要 | `.cs` ファイルとして保存 (`#:package` ディレクティブ使用) |
| コマンドライン引数が必要 | `.cs` ファイルとして保存 |

---

## file-based app (.cs ファイル) として保存する場合

NuGet パッケージの使用、再利用、コマンドライン引数の受け取りなど、ファイルとして保存する理由がある場合のみ `.cs` ファイルを作成する。

### 基本構造

```csharp
#:package Spectre.Console@*

using Spectre.Console;

AnsiConsole.MarkupLine("[green]Hello, World![/]");
```

### ディレクティブ

ファイル先頭に `#:` プレフィックスのディレクティブを記述する。

| ディレクティブ | 用途 | 例 |
|---|---|---|
| `#:package` | NuGet パッケージ追加 | `#:package Newtonsoft.Json@13.0.3` |
| `#:sdk` | SDK 指定 | `#:sdk Microsoft.NET.Sdk.Web` |
| `#:property` | MSBuild プロパティ設定 | `#:property TargetFramework=net10.0` |
| `#:project` | プロジェクト参照 | `#:project ../Lib/Lib.csproj` |

バージョン指定がない場合は `@*` を付けて最新版を使え:

```csharp
#:package CsvHelper@*
#:package Spectre.Console@*
```

### 実行方法

```shell
dotnet run script.cs
dotnet script.cs
dotnet run --file script.cs
```

### 引数の渡し方

```shell
dotnet run script.cs -- arg1 arg2
```

### 配置場所

file-based app を `.csproj` プロジェクトのディレクトリツリー内に置くな。
プロジェクトの `Directory.Build.props` 等が干渉する。

```
✅ 推奨:
📁 repo/
├── src/
│   └── MyProject/
└── scripts/
    └── tool.cs

❌ 非推奨:
📁 repo/
└── src/
    └── MyProject/
        └── scripts/
            └── tool.cs
```

---

## コーディング規約

- file-based app でも C# 14 / .NET 10 の最新文法を使うこと (csharp-dotnet10 スキル参照)。
- top-level statements を使うこと。`Main` メソッドは不要。
- 必要に応じてクラス・レコード等を同一ファイル内に定義してよい。
- `using` はディレクティブの後、コードの前に記述する。

---

## stdin パイプの複数行コード (PowerShell)

PowerShell ではヒア文字列 `@'...'@` を使って複数行コードをパイプする:

```powershell
@'
using System.Text.Json;

var data = new { Name = "Alice", Age = 30 };
var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine(json);
'@ | dotnet run -
```

---

## Office ファイル処理

Excel / Word / PowerPoint などの Office ファイルを処理する場合は **OpenXML SDK** を使うこと。

```csharp
#:package DocumentFormat.OpenXml@*

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using var doc = SpreadsheetDocument.Open("data.xlsx", false);
var sheet = doc.WorkbookPart!.WorksheetParts.First().Worksheet;
var rows = sheet.Descendants<Row>();
foreach (var row in rows)
{
    var cells = row.Elements<Cell>().Select(c => c.CellValue?.Text ?? "");
    Console.WriteLine(string.Join("\t", cells));
}
```

| パッケージ | 用途 |
|---|---|
| `DocumentFormat.OpenXml` | .xlsx / .docx / .pptx の読み書き |
| `DocumentFormat.OpenXml.Framework` | 低レベル API のみ必要な場合 |

- COM Interop (`Microsoft.Office.Interop.*`) は使うな。
- サードパーティ製ライブラリ (EPPlus, ClosedXML 等) よりも OpenXML SDK を優先せよ。

---

## 禁止パターン

- ❌ 使い捨て処理でファイルを作成する → `| dotnet run -` を使え
- ❌ Python / Bash でスクリプトを書く → C# file-based app を使え
- ❌ `.csproj` を手動作成してスクリプトを実行する → file-based app を使え
- ❌ file-based app を `.csproj` プロジェクトディレクトリ内に配置する
- ❌ `#:package` でバージョンを省略する (Central Package Management 未使用時) → `@*` または明示バージョンを付けよ
