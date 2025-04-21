function Add(x, y) {
    return x + y;
}

$(document).ready(function () {
    $('.dataTableMain').DataTable();
});


    document.addEventListener('DOMContentLoaded', function () {
        const toggleBtn = document.getElementById('themeToggleBtn');
    const themeIcon = document.getElementById('themeIcon');
    const htmlElement = document.documentElement;

    // Apply stored theme
    const savedTheme = localStorage.getItem('theme') || 'light';
    htmlElement.setAttribute('data-bs-theme', savedTheme);
    updateIcon(savedTheme);

    toggleBtn.addEventListener('click', function () {
            const currentTheme = htmlElement.getAttribute('data-bs-theme');
    const newTheme = currentTheme === 'light' ? 'dark' : 'light';
    htmlElement.setAttribute('data-bs-theme', newTheme);
    localStorage.setItem('theme', newTheme);
    updateIcon(newTheme);
        });

    function updateIcon(theme) {
            if (theme === 'dark') {
        themeIcon.classList.remove('bi-moon-fill');
    themeIcon.classList.add('bi-sun-fill');
            } else {
        themeIcon.classList.remove('bi-sun-fill');
    themeIcon.classList.add('bi-moon-fill');
            }
        }
    });
