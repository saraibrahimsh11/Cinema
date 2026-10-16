/* Navigation: show one page at a time (all data is static HTML tables) */
const pages = document.querySelectorAll('.page');
const navButtons = document.querySelectorAll('#nav button');

function showPage(key) {
    pages.forEach((page) => page.classList.toggle('active', page.id === `page-${key}`));
    navButtons.forEach((button) => button.setAttribute('aria-current', button.dataset.k === key));
}

/* Add-movie drawer */
const drawer = document.getElementById('drawer');

document.addEventListener('click', (event) => {
    const navButton = event.target.closest('[data-k]');

    if (navButton) showPage(navButton.dataset.k);
    else if (event.target.closest('[data-open]')) drawer.classList.add('open');
    else if (event.target.closest('[data-close]')) drawer.classList.remove('open');
});

showPage('home');
