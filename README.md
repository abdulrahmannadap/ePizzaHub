# ePizzaHub



//Theme     

document.addEventListener('DOMContentLoaded', function () {
    const toggleBtn = document.getElementById('themeToggleBtn');
    const themeIcon = document.getElementById('themeIcon');
    const themeDropdown = document.getElementById('themeDropdown');
    const themeStylesheet = document.getElementById('themeStylesheet');
    const htmlElement = document.documentElement;

    const themes = [
        'Brite', 'Cerulean', 'Cosmo', 'Cyborg', 'Darkly', 'Flatly', 'Journal', 'Litera', 'Lumen',
        'Lux', 'Materia', 'Minty', 'Morph', 'Pulse', 'Quartz', 'Sandstone', 'Simplex', 'Sketchy',
        'Slate', 'Solar', 'Spacelab', 'Superhero', 'United', 'Vapor', 'Yeti', 'Zephyr'
    ];

    // Default theme (Bootstrap CDN link)
    const defaultTheme = 'default';
    const themeLinks = {
        default: 'https://cdn.jsdelivr.net/npm/bootstrap@5.3.0-alpha1/dist/css/bootstrap.min.css'
    };

    themes.forEach(theme => {

        themeLinks[theme] = `https://cdn.jsdelivr.net/npm/bootswatch@latest/dist/${theme.toLowerCase()}/bootstrap.min.css`;
    });

    // Load saved theme and color mode
    const savedTheme = localStorage.getItem('bootwatchTheme') || defaultTheme;
    const savedMode = localStorage.getItem('bootstrapColorMode') || 'light';

    // Apply the saved theme
    if (savedTheme === 'default') {
        themeStylesheet.href = themeLinks['default'];
    } else if (themeLinks[savedTheme]) {
        themeStylesheet.href = themeLinks[savedTheme];
    }

    htmlElement.setAttribute('data-bs-theme', savedMode);

    // Append mode options
    const darkLi = document.createElement('li');
    darkLi.innerHTML = `<a class="dropdown-item" href="#" data-mode="dark"><i class="bi bi-moon-stars"></i> Dark Mode</a>`;
    themeDropdown.appendChild(darkLi);

    const lightLi = document.createElement('li');
    lightLi.innerHTML = `<a class="dropdown-item" href="#" data-mode="light"><i class="bi bi-brightness-high"></i> Light Mode</a>`;
    themeDropdown.appendChild(lightLi);

    // Divider
    themeDropdown.appendChild(document.createElement('li')).innerHTML = `<hr class="dropdown-divider">`;

    // Add "Default Theme" option
    const defaultLi = document.createElement('li');
    defaultLi.innerHTML = `<a class="dropdown-item text-danger fw-bold" href="#" data-theme="default">
        <i class="bi bi-arrow-counterclockwise"></i> HIND TERMINALS 
    </a>`;
    themeDropdown.appendChild(defaultLi);

    themeDropdown.appendChild(document.createElement('li')).innerHTML = `<hr class="dropdown-divider">`;

    // Append other theme options
    themes.forEach(theme => {
        const li = document.createElement('li');
        li.innerHTML = `<a class="dropdown-item" href="#" data-theme="${theme}">
            <i class="bi bi-circle"></i> ${theme}
        </a>`;
        themeDropdown.appendChild(li);
    });

    // Handle theme and mode change
    themeDropdown.addEventListener('click', function (e) {
        const target = e.target.closest('a');
        if (!target) return;

        const newTheme = target.getAttribute('data-theme');
        const newMode = target.getAttribute('data-mode');

        if (newMode) {
            htmlElement.setAttribute('data-bs-theme', newMode);
            localStorage.setItem('bootstrapColorMode', newMode);
        }

        if (newTheme === 'default') {
            themeStylesheet.href = themeLinks['default'];
            localStorage.setItem('bootwatchTheme', 'default');
            updateButton('default');
        } else if (themeLinks[newTheme]) {
            themeStylesheet.href = themeLinks[newTheme];
            localStorage.setItem('bootwatchTheme', newTheme);
            updateButton(newTheme);
        }
    });

    // Update the theme toggle button to reflect the selected theme
    function updateButton(theme) {
        // Save selected theme again to localStorage (to make sure it's consistent)
        localStorage.setItem('bootwatchTheme', theme);

        // Update button text and icon
        toggleBtn.innerHTML = `<i id="themeIcon" class="bi bi-record-fill text-primary"></i> ${theme} Theme`;

        // Reset all theme icons to empty circle
        const themeItems = themeDropdown.querySelectorAll('a[data-theme]');
        themeItems.forEach(item => {
            const icon = item.querySelector('i');
            if (icon) {
                icon.className = 'bi bi-circle';
            }
        });

        // Highlight the selected theme with filled circle
        const selectedItem = themeDropdown.querySelector(`a[data-theme="${theme}"]`);
        if (selectedItem) {
            const icon = selectedItem.querySelector('i');
            if (icon) {
                icon.className = 'bi bi-record-fill text-primary';
            }
        }
    }

    // Highlight selected theme on page load
    updateButton(savedTheme);

});




  <div class="dropdown" data-bs-toggle="tooltip" data-bs-placement="left" data-bs-title="🎨 Try our new theme feature!" >
      <button  class="btn btn-primary dropdown-toggle me-2" type="button" id="themeToggleBtn" data-bs-toggle="dropdown" aria-expanded="false">
Change Theme
      </button>
      <ul class="dropdown-menu theme-scroll-dropdown" aria-labelledby="themeToggleBtn" id="themeDropdown"></ul>
  </div>


<!-- Default Bootstrap theme -->
<link id="themeStylesheet" rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css">
