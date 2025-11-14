module.exports = {
    darkMode: 'class',
    content: [
        './index.html',
        './src/**/*.{vue,js,ts}'
    ],
    theme: {
        extend: {
            colors: {
                primary: '#0071e3',
                secondary: '#5856d6',
                success: '#34c759',
                warning: '#ff9500',
                danger: '#ff3b30',
                info: '#5ac8fa',
            },
            fontFamily: {
                sans: ['-apple-system', 'BlinkMacSystemFont', 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Cantarell', 'sans-serif'],
                mono: ['Menlo', 'Monaco', 'Consolas', 'Courier New', 'monospace'],
            },
        },
    },
    plugins: [],
}