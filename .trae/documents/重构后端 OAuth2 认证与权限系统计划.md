我已详细分析了现有的后端代码结构，特别是 `Program.cs`、`AuthController.cs` 和 `AuthModel.cs`。

根据您的需求，我制定了以下重构计划：

### 1. 修改 OAuth2 配置 (`Program.cs`)
- **调整 Scope**：将原有的 `profile`, `email`, `openid`, `read` 替换为单一的 `full` scope。
- **确认映射规则**：保留 `ClaimActions.MapJsonKey(ClaimTypes.Role, "role")`，确保从 OAuth2 响应的 `role` 字段中正确提取权限信息。
- **清理代码**：移除不再需要的 Scope 添加语句。

### 2. 验证认证流程 (`AuthController.cs` & `JwtGenerate.cs`)
- **独立 JWT 颁发**：现有的 `JwtGenerate` 类已经实现了独立的 JWT 颁发逻辑（使用本地 `JWT_SECRET` 签名）。`AuthController.LoginSuccess` 方法在 OAuth2 认证成功后，会调用此服务生成新的 JWT 返回给前端。这符合您"JWT 为独立颁发"的要求。
- **权限处理**：`AuthController` 目前已包含根据 `role` 字段判断用户身份（Admin/User）的逻辑，这将与新的 `full` scope 配合工作。

### 3. 数据模型确认 (`AuthModel.cs`)
- `OAuthUserInfo` 类中已经定义了 `[JsonPropertyName("role")]`，这与您的"rule 字段为 role"（即权限字段为 role）的要求一致。

**执行步骤：**
1.  修改 `backend/UniversityLink.WebApi/UniversityLink.WebApi/Program.cs`，更新 OAuth2 的 Scope 配置。
2.  检查并微调 `AuthController.cs` 中的回调逻辑（如有必要），确保流程顺畅。

请确认是否开始执行此计划？