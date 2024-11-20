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
    },
  },
  plugins: [],
};
