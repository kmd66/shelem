export default {
    build: {
        outDir: './shelemAssets',
        emptyOutDir: false,
        rollupOptions: {
            treeshake: false,
            input: './controll/main.js',
            output: {
                entryFileNames: 'game.main.js',
                format: 'es'
            },
            onwarn(warning, warn) {
                if (warning.message.includes('/*#__PURE__*/')) {
                    return
                }
                warn(warning)
            }
        }
    }
}