# Azure Functions CRUD API サンプル

Azure Functions と Entity Framework Core を使った Todo 管理 API のサンプルです。

## 実行方法

```bash
dotnet restore
dotnet build
cd AzureFunctionSample
func start
```

## API の使い方

### Todo を作成

```powershell
Invoke-RestMethod -Method Post -Uri "http://localhost:7071/api/todos" `
  -ContentType "application/json" `
  -Body '{"title":"買い物に行く","isCompleted":false}'
```

### すべての Todo を取得

```powershell
Invoke-RestMethod -Method Get -Uri "http://localhost:7071/api/todos"
```

### 特定の Todo を取得

```powershell
Invoke-RestMethod -Method Get -Uri "http://localhost:7071/api/todos/1"
```

### Todo を更新

```powershell
Invoke-RestMethod -Method Put -Uri "http://localhost:7071/api/todos/1" `
  -ContentType "application/json" `
  -Body '{"title":"買い物完了","isCompleted":true}'
```

### Todo を削除

```powershell
Invoke-RestMethod -Method Delete -Uri "http://localhost:7071/api/todos/1"
```

## 技術スタック

- .NET 8
- Azure Functions v4
- Entity Framework Core (InMemory Database)
