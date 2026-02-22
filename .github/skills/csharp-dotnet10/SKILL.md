---
name: csharp-dotnet10
description: >
  C# 14 / .NET 10 の最新文法に基づいてコードを生成・レビュー・リファクタリングするスキル。
  C# のコード生成、レビュー、リファクタリング、新規ファイル作成時に使用すること。
---

# C# 14 / .NET 10 開発スキル

あなたは C# 14 (.NET 10) のエキスパートである。
**C# 14 の最新文法を必ず使え。旧来の書き方は禁止。**

ターゲットは `net10.0`。file-scoped namespace、Nullable enable、ImplicitUsings enable を前提とする。

---

## C# 14 新機能 (必ず使うこと)

### 1. Extension Members

`extension` ブロックで拡張プロパティ・拡張メソッド・静的拡張メンバー・演算子を定義する。
従来の `this` パラメーター拡張メソッドは使うな。

```csharp
public static class Enumerable
{
    extension<TSource>(IEnumerable<TSource> source)
    {
        // 拡張プロパティ
        public bool IsEmpty => !source.Any();
        // 拡張メソッド
        public IEnumerable<TSource> Where(Func<TSource, bool> predicate) { ... }
    }

    extension<TSource>(IEnumerable<TSource>)
    {
        // 静的拡張メソッド
        public static IEnumerable<TSource> Combine(IEnumerable<TSource> first, IEnumerable<TSource> second) { ... }
        // 静的拡張プロパティ
        public static IEnumerable<TSource> Identity => Enumerable.Empty<TSource>();
        // ユーザー定義演算子
        public static IEnumerable<TSource> operator +(IEnumerable<TSource> left, IEnumerable<TSource> right) => left.Concat(right);
    }
}
```

レシーバー名ありブロック = インスタンス拡張、レシーバー名なしブロック = 静的拡張。

### 2. `field` キーワード (Field-backed Properties)

プロパティのアクセサー内で `field` トークンを使い、コンパイラ生成バッキングフィールドを参照する。
手動バッキングフィールド (`private string _msg;`) は不要。

```csharp
// ❌ 旧
private string _msg;
public string Message { get => _msg; set => _msg = value ?? throw new ArgumentNullException(nameof(value)); }

// ✅ 新
public string Message
{
    get;
    set => field = value ?? throw new ArgumentNullException(nameof(value));
}
```

既存コードに `field` という名前のシンボルがある場合は `@field` または `this.field` で区別する。

### 3. Null 条件付き代入

`?.` / `?[]` を代入の左辺に使える。右辺は左辺が null でない場合のみ評価される。

```csharp
// ❌ 旧
if (customer is not null) { customer.Order = GetCurrentOrder(); }

// ✅ 新
customer?.Order = GetCurrentOrder();
```

複合代入 (`+=`, `-=` 等) も可。ただし `++` / `--` は不可。

### 4. Simple Lambda Parameter Modifiers

ラムダパラメーターに型を指定せず `scoped`, `ref`, `in`, `out`, `ref readonly` 修飾子を付けられる。

```csharp
// ❌ 旧 (型の指定が必要だった)
TryParse<int> parse = (string text, out int result) => int.TryParse(text, out result);

// ✅ 新
TryParse<int> parse = (text, out result) => int.TryParse(text, out result);
```

`params` 修飾子のみ明示的な型付きパラメーターリストが必要。

### 5. Implicit Span Conversions

`Span<T>`, `ReadOnlySpan<T>`, `T[]` 間の暗黙変換をサポート。
拡張メソッドのレシーバーにも使え、ジェネリック型推論にも対応。
パフォーマンス重視のコードでは `Span<T>` を積極的に使え。

### 6. `nameof` と Unbound Generic Types

```csharp
nameof(List<>)   // → "List" (C# 14 で許可)
```

### 7. Partial Constructors / Partial Events

インスタンスコンストラクターとイベントを `partial` として宣言可能。
定義宣言と実装宣言の2つが必要。コンストラクター初期化子 (`this()` / `base()`) は実装宣言のみ。
primary constructor 構文は1つの partial 型宣言のみ。

### 8. User-defined Compound Assignment Operators

型に `+=`, `-=` などの複合代入演算子と `++` / `--` をユーザー定義できる。

---

## C# 12–13 機能 (引き続き使うこと)

| 機能 | 使い方 |
|---|---|
| Primary constructors | `public class Svc(IDep dep);` — コンストラクター DI に使う |
| Collection expressions | `int[] a = [1, 2, 3];` — `new List<T>{...}` は禁止 |
| Raw string literals | `"""..."""` — 複数行・エスケープ多用時に使う |
| Required members | `required` で初期化を強制 |
| Record types | DTO・不変データに `record` / `record struct` |
| Pattern matching | `switch` 式、リスト・プロパティパターンを活用 |
| Target-typed new | 型が明確なら `new()` |
| Global using | 共通 using は `GlobalUsings.cs` にまとめる |
| `params` コレクション | `params ReadOnlySpan<T>` 等を活用 |

---

## コーディング規約

- 非同期: `async`/`await` 必須。`Async` サフィックス。`Task.Result`/`Task.Wait()` 禁止。
- LINQ: メソッド構文基本。
- 文字列: 補間 `$"..."` 必須。`string.Format` / `+` 連結禁止。
- 型推論: `var` 積極使用。
- null チェック: `is null` / `is not null` パターン。`== null` / `!= null` 禁止。
- 引数検証: `ArgumentNullException.ThrowIfNull()`, `ArgumentException.ThrowIfNullOrEmpty()` を使う。
- ストリーミング: `IAsyncEnumerable<T>` を使う。
- 例外フィルター: `catch ... when (...)` を活用。

---

## プロジェクト操作

NuGet・プロジェクト作成・ソリューション管理・参照追加は全て `dotnet` CLI で行う。`.csproj` / `.slnx` の手動編集禁止。

```shell
dotnet new <template>
dotnet sln add/remove <project>
dotnet add package <pkg>
dotnet add reference <project>
```

---

## 禁止パターン一覧

- ❌ `namespace X { }` → `namespace X;`
- ❌ `public static T Method(this T x)` → `extension(T x) { }` ブロック
- ❌ `private string _field;` → `field` キーワード
- ❌ `new List<T>()` → `[]`
- ❌ `string.Format()` → `$"..."`
- ❌ `Task.Result` / `.Wait()` → `await`
- ❌ `== null` / `!= null` → `is null` / `is not null`
- ❌ `if (x != null) x.Prop = v;` → `x?.Prop = v;`
