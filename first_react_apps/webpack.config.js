const path = require('path');

module.exports = {
    entry: './src/index.tsx', // Entry point for the React app
    output: {
        path: path.resolve(__dirname, 'dist'),
        filename: 'bundle.js', // Output bundle
    },
    module: {
        rules: [
            {
                test: /\.tsx?$/, // Apply ts-loader for TypeScript files
                exclude: /node_modules/,
                use: {
                    loader: 'ts-loader',
                },
            },
            {
                test: /\.(js|jsx)$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                },
            },
        ],
    },
    resolve: {
        extensions: ['.tsx', '.ts', '.js', '.jsx'], // Resolve .tsx, .ts, .js, and .jsx files
    },
};

