/** @type {import('tailwindcss').Config} */
const { fontFamily } = require("tailwindcss/defaultTheme");

module.exports = {
  content: [
    './src/pages/**/*.{js,ts,jsx,tsx,mdx}',
    './src/components/**/*.{js,ts,jsx,tsx,mdx}',
    './src/app/**/*.{js,ts,jsx,tsx,mdx}',
  ],
  darkMode: "class",
  theme: {
    extend: {
      backgroundImage: {
        'gradient-radial': 'radial-gradient(var(--tw-gradient-stops))',
        'gradient-conic':
          'conic-gradient(from 180deg at 50% 50%, var(--tw-gradient-stops))',
      },
      colors: {
        branding: {
          "primary": "#000000",
          "secondary": "#FFFFFF"
        },
        status: {
          "error": "#FFF000",
          "success": "#218A07",
          "warning": "#C7C10A",
          "neutral": "#F5F5F5"
        }
      },
      fontFamily: {

      }
    },
  },
  plugins: [
    require("@tailwindcss/container-queries")
  ],
}
