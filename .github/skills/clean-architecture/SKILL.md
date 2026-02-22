---
name: clean-architecture
description: >
  クリーンアーキテクチャに基づく設計指針スキル。プロジェクト構成、レイヤー分離、依存関係の方向、テスタビリティ確保の原則を定義する。
  新規プロジェクト作成、機能追加、リファクタリング、テスト作成時に使用すること。
---

# クリーンアーキテクチャ設計指針

**クリーンアーキテクチャに従い、依存関係は常に内側に向かうこと。テスタビリティを最優先に設計せよ。**

---

## レイヤー構成と依存方向

```
Presentation (Blazor / API)
    ↓
Application (ユースケース / CQRS)
    ↓
Domain (エンティティ / 値オブジェクト / ドメインサービス)

Infrastructure (DB / 外部 API / ファイル)
    ↓
Application (インターフェース実装)
```

**依存ルール: 内側のレイヤーは外側のレイヤーを知らない。**

| レイヤー | 責務 | 依存先 |
|---|---|---|
| Domain | エンティティ、値オブジェクト、ドメインイベント、リポジトリインターフェース | なし (外部依存ゼロ) |
| Application | ユースケース、DTO、バリデーション、インターフェース定義 | Domain のみ |
| Infrastructure | DB アクセス、外部 API クライアント、ファイル I/O、インターフェース実装 | Application, Domain |
| Presentation | UI (Blazor)、API コントローラー、DI 構成 | Application, Domain |

---

## プロジェクト構成

```
<SolutionName>/
├── src/
│   ├── <SolutionName>.Domain/           # エンティティ、値オブジェクト、インターフェース
│   ├── <SolutionName>.Application/      # ユースケース、DTO、バリデーション
│   ├── <SolutionName>.Infrastructure/   # EF Core、外部サービス実装
│   └── <SolutionName>.Web/              # Blazor / API (Presentation)
├── tests/
│   ├── <SolutionName>.Domain.Tests/     # ドメインロジックの単体テスト
│   ├── <SolutionName>.Application.Tests/ # ユースケースの単体テスト
│   └── <SolutionName>.Infrastructure.Tests/ # 統合テスト
└── <SolutionName>.slnx
```

プロジェクト作成は `dotnet` CLI で行うこと:

```shell
dotnet new classlib -n <SolutionName>.Domain
dotnet new classlib -n <SolutionName>.Application
dotnet new classlib -n <SolutionName>.Infrastructure
dotnet new blazor -n <SolutionName>.Web
dotnet new mstest -n <SolutionName>.Domain.Tests
dotnet new mstest -n <SolutionName>.Application.Tests
dotnet new mstest -n <SolutionName>.Infrastructure.Tests
dotnet sln add <各プロジェクトパス>
```

---

## 設計原則

### 1. 依存性逆転 (DIP)

- 上位レイヤーがインターフェースを定義し、下位レイヤーが実装する。
- Application 層にインターフェースを定義し、Infrastructure 層が実装する。
- コンストラクター注入で DI すること。`new` による直接生成は禁止。

```csharp
// Application 層: インターフェース定義
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(OrderId id, CancellationToken cancellationToken = default);
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
}

// Infrastructure 層: 実装
public class OrderRepository(AppDbContext db) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(OrderId id, CancellationToken cancellationToken = default)
        => await db.Orders.FindAsync([id], cancellationToken);

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
        => await db.Orders.AddAsync(order, cancellationToken);
}
```

### 2. Domain 層の設計

- エンティティはビジネスルールをカプセル化すること。貧血ドメインモデル禁止。
- 値オブジェクトには `record` を使うこと。
- ドメインイベントでレイヤー間通信を行うこと。
- Domain 層は NuGet パッケージに依存しないこと (純粋な C#)。

```csharp
// 値オブジェクト
public record OrderId(Guid Value);

// エンティティ
public class Order
{
    public OrderId Id { get; private set; }
    public OrderStatus Status { get; private set; }

    public void Cancel()
    {
        if (Status is OrderStatus.Shipped)
            throw new DomainException("出荷済みの注文はキャンセルできません");
        Status = OrderStatus.Cancelled;
    }
}
```

### 3. Application 層の設計

- ユースケースごとに1クラスを作成すること。
- 入力は DTO / Command / Query、出力は DTO で表現すること。
- `CancellationToken` を必ず受け取ること。
- バリデーションは Application 層で行うこと。

```csharp
public class CancelOrderUseCase(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
{
    public async Task ExecuteAsync(OrderId orderId, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), orderId);

        order.Cancel();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
```

### 4. Infrastructure 層の設計

- Application 層のインターフェースを実装すること。
- EF Core の `DbContext` は Infrastructure 層に配置すること。
- 外部 API クライアントは `HttpClient` + `IHttpClientFactory` を使うこと。
- DI 登録用の拡張メソッド `AddInfrastructure(this IServiceCollection)` を用意すること。

### 5. Presentation 層の設計

- DI 構成はここで行い、各レイヤーの `Add*` 拡張メソッドを呼ぶこと。
- Blazor コンポーネントはユースケースを注入して使うこと。直接リポジトリを注入しないこと。
- コントローラー / Minimal API はシンとし、ロジックを持たないこと。

---

## テスト原則

### テストフレームワーク

**MSTest を使用すること。** xUnit / NUnit は使わない。

```shell
dotnet new mstest -n <ProjectName>.Tests
dotnet add <ProjectName>.Tests package Moq
dotnet add <ProjectName>.Tests package FluentAssertions
```

### テストプロジェクト構成

- テスト対象プロジェクトと 1:1 でテストプロジェクトを作成すること。
- テストクラスはテスト対象と同じ名前空間構造にすること。
- テストクラス名は `<対象クラス名>Tests` とすること。

### テストの書き方

```csharp
[TestClass]
public class CancelOrderUseCaseTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private CancelOrderUseCase _sut = null!;

    [TestInitialize]
    public void Setup()
    {
        _sut = new CancelOrderUseCase(
            _orderRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [TestMethod]
    public async Task ExecuteAsync_注文が存在する場合_キャンセルされること()
    {
        // Arrange
        var orderId = new OrderId(Guid.NewGuid());
        var order = OrderFactory.CreatePending(orderId);
        _orderRepositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        await _sut.ExecuteAsync(orderId);

        // Assert
        order.Status.Should().Be(OrderStatus.Cancelled);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task ExecuteAsync_注文が存在しない場合_NotFoundExceptionをスローすること()
    {
        // Arrange
        var orderId = new OrderId(Guid.NewGuid());
        _orderRepositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        // Act
        var act = () => _sut.ExecuteAsync(orderId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
```

### テスト規約

| 規約 | 内容 |
|---|---|
| 命名 | `メソッド名_条件_期待結果` (日本語可) |
| 構造 | Arrange / Act / Assert (AAA パターン) |
| モック | `Moq` を使用。インターフェースをモック化 |
| アサーション | `FluentAssertions` を推奨 |
| テストデータ | ファクトリメソッド / Builder パターンで生成 |
| 属性 | `[TestClass]`, `[TestMethod]`, `[TestInitialize]`, `[DataTestMethod]`, `[DataRow]` |
| 非同期 | `async Task` を返すこと (`async void` 禁止) |

### テストレベル

| レベル | 対象 | モック範囲 | テストプロジェクト |
|---|---|---|---|
| 単体テスト | Domain エンティティ、値オブジェクト | 不要 | `*.Domain.Tests` |
| 単体テスト | ユースケース | リポジトリ・外部サービス | `*.Application.Tests` |
| 統合テスト | Infrastructure 実装 | In-Memory DB 使用 | `*.Infrastructure.Tests` |

### テスタビリティ確保のルール

- **全てのビジネスロジックはテスト可能であること。**
- 静的メソッド・`DateTime.Now` / `Guid.NewGuid()` の直接使用を避け、抽象化すること。
- `sealed` クラスでもインターフェースを抽出してテスト可能にすること。
- ファイル I/O・DB アクセス・HTTP 呼び出しは必ずインターフェース経由にすること。

```csharp
// ❌ テスト困難
public class OrderService
{
    public Order Create() => new() { Id = new OrderId(Guid.NewGuid()), CreatedAt = DateTime.UtcNow };
}

// ✅ テスト容易
public class OrderService(IDateTimeProvider dateTimeProvider, IGuidProvider guidProvider)
{
    public Order Create() => new() { Id = new OrderId(guidProvider.NewGuid()), CreatedAt = dateTimeProvider.UtcNow };
}
```

---

## 禁止パターン

- ❌ Domain 層から Infrastructure 層への依存
- ❌ Presentation 層でのビジネスロジック実装
- ❌ コンポーネント / コントローラーへのリポジトリ直接注入 (ユースケース経由にすること)
- ❌ `new DbContext()` 等の手動インスタンス生成 (DI を使うこと)
- ❌ テストなしの機能追加
- ❌ xUnit / NUnit の使用 (MSTest を使うこと)
- ❌ `async void` テストメソッド
- ❌ テスト内でのハードコードされたマジックナンバー
