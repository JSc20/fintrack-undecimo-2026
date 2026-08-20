/**
 * FinTrack — Theme & Mobile Sidebar Manager
 * Applies saved theme immediately (before DOM renders) to avoid flash.
 */
(function () {
    var saved = localStorage.getItem('ft-theme') || 'dark';
    if (saved === 'light') {
        document.documentElement.classList.add('light-theme');
    }
})();

document.addEventListener('DOMContentLoaded', function () {
    var html      = document.documentElement;
    var themeBtn  = document.getElementById('theme-toggle-btn');
    var sidebar   = document.querySelector('.ft-sidebar');
    var hamBtn    = document.getElementById('sidebar-toggle');
    var overlay   = document.getElementById('sidebar-overlay');

    /* ---------- helpers ---------- */
    function isLight() { return html.classList.contains('light-theme'); }

    function updateIcon() {
        if (!themeBtn) return;
        var icon = themeBtn.querySelector('.theme-icon');
        var label = themeBtn.querySelector('.theme-label');
        if (isLight()) {
            icon.className  = 'theme-icon bi bi-moon-fill';
            if (label) label.textContent = 'Modo Oscuro';
        } else {
            icon.className  = 'theme-icon bi bi-sun-fill';
            if (label) label.textContent = 'Modo Claro';
        }
    }

    /* ---------- theme toggle ---------- */
    updateIcon();

    if (themeBtn) {
        themeBtn.addEventListener('click', function (e) {
            e.preventDefault();
            html.classList.toggle('light-theme');
            localStorage.setItem('ft-theme', isLight() ? 'light' : 'dark');
            updateIcon();
        });
    }

    /* ---------- mobile sidebar ---------- */
    function openSidebar() {
        if (!sidebar || !overlay) return;
        sidebar.classList.add('sidebar-open');
        overlay.classList.add('active');
        document.body.style.overflow = 'hidden';
    }

    function closeSidebar() {
        if (!sidebar || !overlay) return;
        sidebar.classList.remove('sidebar-open');
        overlay.classList.remove('active');
        document.body.style.overflow = '';
    }

    if (hamBtn)  hamBtn.addEventListener('click', openSidebar);
    if (overlay) overlay.addEventListener('click', closeSidebar);

    /* Close on nav link click (mobile) */
    document.querySelectorAll('.ft-nav-link, .ft-user-card').forEach(function (el) {
        el.addEventListener('click', function () {
            if (window.innerWidth <= 768) closeSidebar();
        });
    });
});
