import { createRouter, createWebHistory } from 'vue-router';

const routes = [
    {
        path: "",
        name: 'Home',
        meta: { title: "建大导航 - 起始页" },
        component: () => import('./pages/Home.vue'),
    },
    {
        path: '/',
        name: "main",
        component: () => import('./components/Layout.vue'),
        children: [
            {
                path: '/login',
                name: 'Login',
                meta: { title: "登录" },
                component: () => import('./pages/Login.vue'),
            },
            {
                path: '/users',
                name: 'Users',
                meta: { title: "用户管理" },
                component: () => import('./pages/Users.vue'),
            },
            {
                path: '/links',
                name: 'Links',
                meta: { title: "链接管理" },
                component: () => import('./pages/Links.vue'),
            },
            {
                path: '/categories',
                name: 'Categories',
                meta: { title: "分类管理" },
                component: () => import('./pages/Categories.vue'),
            },
            {
                path: '/data',
                name: 'Data',
                meta: { title: "数据管理" },
                component: () => import('./pages/DataImportExport.vue'),
            },
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