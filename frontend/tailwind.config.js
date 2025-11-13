module.exports = {
    darkMode: 'class',
    purge: {
        content: [
            './index.html',
            './src/**/*.{vue,js,ts}'
        ],
        options: {
            keyframes: true,
            cssPath: '../css/tailwind.css'
        }
    }
    // ...
}