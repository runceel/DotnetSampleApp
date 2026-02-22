# Copilot Instructions

## Build & Run

```shell
dotnet build
dotnet run --project <ProjectName>
```

## Testing

```shell
# Full suite
dotnet test

# Single test by fully-qualified name
dotnet test --filter "FullyQualifiedName~Namespace.ClassName.MethodName"

# Tests matching a pattern
dotnet test --filter "DisplayName~some_keyword"
```

## Architecture

- Solution uses the `.slnx` format (`DotnetSampleApp.slnx`).
- Add new projects via `dotnet sln DotnetSampleApp.slnx add <path-to-csproj>`.

## dotnet CLI 操作

- NuGet パッケージの追加・削除は `dotnet add package` / `dotnet remove package` コマンドで行うこと。
- プロジェクトの新規作成は `dotnet new` コマンドで行うこと。
- ソリューションへのプロジェクトの追加・削除は `dotnet sln add` / `dotnet sln remove` コマンドで行うこと。
- プロジェクト参照の追加・削除は `dotnet add reference` / `dotnet remove reference` コマンドで行うこと。
- csproj や slnx ファイルを直接手動編集しないこと。

## Conventions

- Target the latest .NET LTS release unless otherwise specified.
- Use file-scoped namespaces and nullable reference types (`<Nullable>enable</Nullable>`).
- Prefer `record` types for DTOs and immutable data.
