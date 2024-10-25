/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}", // Adjust based on your Angular project file structure
  ],
  theme: {
    extend: {
      colors: {
        primary: '#3498db', // Example primary color (blue)
      },
    },
  },
  plugins: [],
};
