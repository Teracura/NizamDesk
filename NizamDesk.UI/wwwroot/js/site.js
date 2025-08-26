document.addEventListener("DOMContentLoaded", () => {
    initializeThemeToggle();
});

function initializeThemeToggle() {
    const themeToggle = document.getElementById("themeToggle");
    if (!themeToggle) return; // Exit if button not found
    
    const htmlEl = document.documentElement;

    // Load saved theme
    const savedTheme = localStorage.getItem("theme") || "light";
    htmlEl.setAttribute("data-bs-theme", savedTheme);
    updateToggleIcon(savedTheme);

    themeToggle.addEventListener("click", () => {
        const currentTheme = htmlEl.getAttribute("data-bs-theme");
        const newTheme = currentTheme === "light" ? "dark" : "light";

        htmlEl.setAttribute("data-bs-theme", newTheme);
        localStorage.setItem("theme", newTheme); // save the choice
        updateToggleIcon(newTheme);
    });

    function updateToggleIcon(theme) {
        const icon = themeToggle.querySelector(".theme-toggle-icon");
        if (icon) {
            if (theme === "light") {
                icon.src = "/images/moon-icon.svg"; // moon for switching to dark
            } else {
                icon.src = "/images/sun-warm-icon.svg"; // sun for switching to light
            }
        }
    }
}