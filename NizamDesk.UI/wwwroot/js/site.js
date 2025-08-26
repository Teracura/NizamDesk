window.themeHelper = {
    getTheme: function () {
        return localStorage.getItem('theme') || 'light';
    },
    setTheme: function (theme) {
        localStorage.setItem('theme', theme);
        document.body.setAttribute('data-bs-theme', theme);
    }
};

document.addEventListener('DOMContentLoaded', function() {
    const savedTheme = localStorage.getItem('theme') || 'light';
    document.body.setAttribute('data-bs-theme', savedTheme);
});