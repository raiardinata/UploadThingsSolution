/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './**/*.razor',
    './**/*.html',
    './Pages/**/*.cshtml',
    './Shared/**/*.razor',
    './wwwroot/index.html'
  ],
  theme: {
    extend: {
      fontFamily: {
        helveticaNeue: ['Helvetica', 'Arial', 'sans-serif']
      },
      boxShadow: {
        'custom-double': '0 0 0 0.1rem white, 0 0 0 0.25rem #258cfb',
      },
    },
  },
  plugins: [],
};
