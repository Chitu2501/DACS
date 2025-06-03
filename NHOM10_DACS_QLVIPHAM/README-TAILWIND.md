# Tailwind CSS Integration for QLVIPHAM Project

This project uses Tailwind CSS for styling instead of Bootstrap. This README provides instructions on how to work with Tailwind CSS in this project.

## Setup

The project is configured to use Tailwind CSS via CDN. This approach has several advantages:
- No need for Node.js, npm, or build tools
- Instant updates when changing CSS classes
- Works out of the box with no additional setup
- Great for development and prototyping

## How It's Implemented

The Tailwind CSS CDN is included in the `_Layout.cshtml` file:

```html
<script src="https://cdn.tailwindcss.com"></script>
```

We also included Alpine.js for interactive components:
```html
<script defer src="https://cdn.jsdelivr.net/npm/alpinejs@3.x.x/dist/cdn.min.js"></script>
```

## Tailwind Classes

All Bootstrap classes have been replaced with Tailwind CSS equivalents. Here's a quick reference for common UI components:

### Container and Layout
- Use `container mx-auto` instead of `container`
- Use `flex` and `grid` for layouts

### Colors
- Use `text-blue-600` instead of `text-primary`
- Use `bg-red-600` instead of `bg-danger`
- Use `border-gray-200` instead of `border-secondary`

### Components
- Cards: Use `bg-white rounded-lg shadow-md overflow-hidden` instead of `card`
- Buttons: Use `px-4 py-2 bg-blue-600 text-white rounded-md` instead of `btn btn-primary`
- Alerts: Use `p-4 rounded-md bg-yellow-100 text-yellow-800` instead of `alert alert-warning` 

## For Production

When moving to production, consider replacing the CDN version with a self-hosted Tailwind CSS setup for better performance. This would involve:

1. Installing Node.js and npm
2. Setting up Tailwind CSS with postcss
3. Building a production-optimized CSS file

## Resources

- [Tailwind CSS Documentation](https://tailwindcss.com/docs)
- [Tailwind CSS CDN](https://tailwindcss.com/docs/installation/play-cdn)
- [Alpine.js Documentation](https://alpinejs.dev/) 