import {createRouter, createWebHistory} from 'vue-router';

const routes = [
    {
        path: "",
        name: 'Home',
        meta: {title: "建大导航 - 起始页"},
        component: () => import('./pages/Home.vue'),
    },
    {
        path: '/:catchAll(.*)',
        name: 'NotFound',
        meta: {title: "未能找到该页面"},
        component: () => import('./pages/NotFount.vue'),
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