---
name: blazor-dotnet10
description: >
  ASP.NET Core Blazor (.NET 10) の最新機能・ベストプラクティスに基づいてコンポーネントやページを生成・レビュー・リファクタリングするスキル。
  Blazor コンポーネント、ページ、レイアウト、JS interop、フォームバリデーション、状態管理の実装時に使用すること。
---

# ASP.NET Core Blazor (.NET 10) 開発スキル

あなたは ASP.NET Core Blazor (.NET 10) のエキスパートである。
**.NET 10 の最新 Blazor 機能を必ず使え。旧来の書き方は禁止。**

ターゲットは `net10.0`。C# 14 の文法も併用すること。

---

## レンダーモード

Blazor Web App では全コンポーネントにレンダーモードを適用する。

| モード | ディレクティブ | 場所 | インタラクティブ |
|---|---|---|---|
| Static SSR | なし | Server | ❌ |
| Interactive Server | `@rendermode InteractiveServer` | Server | ✅ |
| Interactive WebAssembly | `@rendermode InteractiveWebAssembly` | Client | ✅ |
| Interactive Auto | `@rendermode InteractiveAuto` | Server→Client | ✅ |

### Program.cs 設定

```csharp
// サービス登録
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// エンドポイント
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();
```

### ランタイム検出

```csharp
// レンダリング場所の検出
RendererInfo.Name       // "Static" | "Server" | "WebAssembly" | "WebView"
RendererInfo.IsInteractive  // true = インタラクティブ描画中
AssignedRenderMode      // InteractiveServerRenderMode | InteractiveWebAssemblyRenderMode | InteractiveAutoRenderMode | null
```

### Static SSR ページ

インタラクティブアプリ内の静的ページには `[ExcludeFromInteractiveRouting]` を使う。

```razor
@attribute [ExcludeFromInteractiveRouting]
```

---

## .NET 10 Blazor 新機能 (必ず使うこと)

### 1. `[PersistentState]` による宣言的状態永続化

プリレンダリング時の状態永続化に `[PersistentState]` 属性を使う。
`PersistentComponentState` を手動で注入・購読するコードは書くな。

```razor
@* ❌ 旧: IDisposable 実装、手動 RegisterOnPersisting、TryTakeFromJson *@

@* ✅ 新 *@
@page "/movies"
@inject IMovieService MovieService

@code {
    [PersistentState]
    public List<Movie>? MoviesList { get; set; }

    protected override async Task OnInitializedAsync()
    {
        MoviesList ??= await MovieService.GetMoviesAsync();
    }
}
```

サービスの状態永続化は `RegisterPersistentService` を使う。

### 2. NotFound ページ

`Router` の `NotFoundPage` パラメーターを使う。`<NotFound>` レンダーフラグメントは .NET 10 で非推奨。

```razor
<Router AppAssembly="@typeof(Program).Assembly"
        NotFoundPage="typeof(Pages.NotFound)">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" />
        <FocusOnNavigate RouteData="@routeData" Selector="h1" />
    </Found>
</Router>
```

`NavigationManager.NotFound()` で 404 をトリガー:
- Static SSR: HTTP 404 ステータスコードを設定
- Interactive: Blazor Router が NotFound コンテンツを描画

### 3. JS Interop 新 API

JS オブジェクトのコンストラクター呼び出し・プロパティ読み書きが可能。

```csharp
// コンストラクター呼び出し
var classRef = await JSRuntime.InvokeConstructorAsync("jsInterop.TestClass", "Blazor!");

// プロパティ読み取り
var text = await classRef.GetValueAsync<string>("text");

// プロパティ書き込み
await JSRuntime.SetValueAsync("jsInterop.testObject.num", 30);
```

同期版 (`IJSInProcessRuntime`): `InvokeConstructor`, `GetValue<T>`, `SetValue<T>`。

### 4. 改善されたフォームバリデーション

ネストオブジェクト・コレクション項目のバリデーションをサポート。

```csharp
// Program.cs
builder.Services.AddValidation();
```

- モデル型は `.razor` ではなく C# クラスファイルで定義すること。
- ルートモデルに `[ValidatableType]` 属性を付与すること。

### 5. QuickGrid 拡張

```razor
@* RowClass: 行ごとのスタイル適用 *@
<QuickGrid Items="movies" RowClass="GetRowCssClass">
    ...
</QuickGrid>

@code {
    private string? GetRowCssClass(Movie item) =>
        item.IsArchived ? "row-archived" : null;
}
```

`HideColumnOptionsAsync()` でカラムオプション UI を閉じる。

### 6. Circuit State Persistence

サーバーサイド Blazor で接続喪失時のセッション状態を永続化。
ブラウザタブのスロットリング、モバイルアプリ切り替え、ネットワーク断で有効。

### 7. WebAssembly プリロード

Blazor Web App で `<ResourcePreloader />` を使い WASM アセットをプリロード。

```razor
<head>
    ...
    <base href="/" />
    <ResourcePreloader />
    ...
</head>
```

### 8. Blazor スクリプトの静的 Web アセット化

Blazor スクリプト (`blazor.web.js` / `blazor.server.js`) は自動圧縮・フィンガープリント付きの静的 Web アセットとして配信される。

### 9. NavigateTo の改善

- 同一ページナビゲーションでスクロール位置がリセットされなくなった。
- `<BlazorDisableThrowNavigationException>true</BlazorDisableThrowNavigationException>` で Static SSR 時の `NavigationException` を抑制 (.NET 10 テンプレートのデフォルト)。

### 10. NavLink の改善

`NavLinkMatch.All` 使用時にクエリ文字列・フラグメントを無視。カスタムマッチングは `ShouldMatch` をオーバーライド。

### 11. ReconnectModal コンポーネント

テンプレートに `ReconnectModal` コンポーネントが追加。再接続状態変更イベント `components-reconnect-state-changed` を使用可能。

### 12. HttpClient レスポンスストリーミング

WebAssembly で `HttpClient` レスポンスストリーミングがデフォルト有効。
`ReadAsStreamAsync()` は `BrowserHttpReadStream` を返す (同期操作不可)。
オプトアウト: `<WasmEnableStreamingResponse>false</WasmEnableStreamingResponse>`。

### 13. Metrics / Tracing

コンポーネントライフサイクル、ナビゲーション、イベント処理、Circuit 管理の計測・トレースが利用可能。

---

## コンポーネントライフサイクル

```
SetParametersAsync
  → OnInitialized / OnInitializedAsync
  → OnParametersSet / OnParametersSetAsync
  → BuildRenderTree (Render)
  → OnAfterRender(firstRender) / OnAfterRenderAsync(firstRender)
```

- `OnAfterRender{Async}` はプリレンダリング中に呼ばれない。
- `OnAfterRenderAsync` の完了後に追加レンダーサイクルは発生しない。
- イベントハンドラーは `Dispose` / `IAsyncDisposable` でアンフックすること。

---

## コンポーネント設計規約

- file-scoped namespace を使うこと (コードビハインド時)。
- `@code` ブロックはコンポーネント末尾に置くこと。
- パラメーターは `[Parameter]` / `[CascadingParameter]` で宣言。
- イベントコールバックは `EventCallback<T>` を使うこと。
- レンダーモードは各コンポーネントまたはグローバルに明示すること。
- 大きなコンポーネントはコードビハインド (`.razor.cs`) に分離すること。
- CSS 分離は `.razor.css` を使うこと。
- ストリーミングレンダリング対象のコンポーネントには `@attribute [StreamRendering]` を使うこと。

---

## プロジェクト操作

NuGet・プロジェクト作成・ソリューション管理は全て `dotnet` CLI で行う。`.csproj` / `.slnx` の手動編集禁止。

```shell
dotnet new blazor                    # Blazor Web App
dotnet new blazorwasm                # Standalone Blazor WebAssembly
dotnet sln add/remove <project>
dotnet add package <pkg>
```

---

## 禁止パターン

- ❌ `PersistentComponentState` の手動注入・購読 → `[PersistentState]` 属性
- ❌ `<NotFound>...</NotFound>` レンダーフラグメント → `NotFoundPage` パラメーター
- ❌ `IJSRuntime.InvokeAsync("eval", ...)` → 適切な JS モジュールと新 API
- ❌ `.razor` 内でのモデル型定義 → C# クラスファイルで定義
- ❌ `blazor.boot.json` の直接参照 → フレームワークにインライン化済み
- ❌ `Properties/launchSettings.json` での環境設定 (WASM) → `<WasmApplicationEnvironmentName>`
