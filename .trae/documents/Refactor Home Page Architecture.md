# 首页架构重构计划 (Home Page Refactoring Plan)

亲爱的主人，经过仔细检查，我发现 `Home.vue` 确实承担了太多的责任（UI 渲染、逻辑计算、事件监听等），这让它看起来有点"超负荷"了。

为了让代码更清晰、更易于维护（以及让您看得更舒心 💖），我制定了以下重构计划：

## 1. 🧩 组件拆分 (Component Extraction)
我们将把庞大的页面拆解为独立的小组件，消除重复代码。

*   **创建 `src/components/home/LinkCard.vue`**
    *   **目的**：封装单个链接卡片的渲染逻辑。
    *   **解决问题**：目前 `Home.vue` 中有大量重复的 HTML 代码用于处理"普通链接"和"微信环境下的链接"。拆分后，`Home.vue` 只需要一行 `<LinkCard :data="link" />`。
    *   **包含逻辑**：链接跳转、二维码弹窗显示、Favicon 获取。

## 2. 🎣 逻辑抽离 (Logic Extraction - Composables)
使用 Vue 3 的 Composition API 将业务逻辑移出组件。

*   **创建 `src/composables/useTheme.ts`**
    *   **内容**：管理暗黑/明亮模式的切换、LocalStorage 的读写。
    *   **好处**：`Home.vue` 不再关心具体怎么操作 DOM 改主题，只需要调用 `toggleTheme()`。

*   **创建 `src/composables/useContextMenu.ts`**
    *   **内容**：封装右键菜单的事件监听 (`contextmenu`) 和按键监听 (`keydown`)。
    *   **好处**：让主页代码专注于"展示"，而不是"监听用户行为"。

## 3. 🛠 工具函数分离 (Utility Extraction)
将纯计算逻辑移动到工具文件中。

*   **创建 `src/utils/layoutHelper.ts`**
    *   **内容**：移动 `getCategoryColClass` 和 `getLinkColClass` 等复杂的 Grid 布局计算函数。
*   **创建 `src/utils/urlHelper.ts`**
    *   **内容**：移动 `getIcon` 函数（用于获取网站图标）。

## 4. 🧹 清理主页 (Clean up Home.vue)
*   **最终状态**：`Home.vue` 将主要负责数据获取和组件组装。代码行数预计减少 50% 以上，结构一目了然！

---

### 📅 执行步骤
1.  **创建工具函数** (`layoutHelper.ts`, `urlHelper.ts`)。
2.  **创建组合式函数** (`useTheme.ts`, `useContextMenu.ts`)。
3.  **创建 UI 组件** (`LinkCard.vue`)。
4.  **重构 `Home.vue`**：引入上述模块，替换原有代码。
5.  **验证**：确保页面功能（跳转、主题切换、右键菜单）与重构前一致。

主人，这个计划您满意吗？如果没问题，我就开始为您整理房间（代码）啦！ (✿◡‿◡)