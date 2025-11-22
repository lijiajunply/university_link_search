import { createRouter, createWebHistory } from 'vue-router';

const routes = [
    {
        path: "",
        name: 'Home',
        meta: { title: "建大导航 - 起始页" },
        component: () => import('./pages/Home.vue'),
    },
    {
        path: '/admin',
        name: "main",
        component: () => import('./layouts/CentreLayout.vue'),
        children: [
            {
                path: '/users',
                name: 'Users',
                meta: { title: "用户管理" },
                component: () => import('./adminPages/Users.vue'),
            },
            {
                path: '/categories',
                name: 'Categories',
                meta: { title: "分类管理" },
                component: () => import('./adminPages/Categories.vue'),
            },
            {
                path: '/data',
                name: 'Data',
                meta: { title: "数据管理" },
                component: () => import('./adminPages/DataImportExport.vue'),
            },
            {
                path: '/category/:id',
                name: 'Category',
                meta: { title: "链接分类管理" },
                component: () => import('./adminPages/Category.vue'),
            }
        ]
    },
    {
        path: '/:catchAll(.*)',
        name: 'NotFound',
        meta: { title: "未能找到该页面" },
        component: () => import('./pages/NotFound.vue'),
    },
];

const router = createRouter({
    history: createWebHistory(""),
    routes,
});

router.beforeEach((to, _, next) => {
    if (to.meta.title && typeof to.meta.title === 'string') {//判断是否有标题
        document.title = to.meta.title
    }
    next()
})


export default router;