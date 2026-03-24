document.addEventListener('DOMContentLoaded', () => {
    initDeleteConfirmation();
    highlightActiveNavLink();
    initTooltips();
    initPhoneMask();
    focusFirstFormField();
});

function initDeleteConfirmation() {
    const deleteButtons = document.querySelectorAll('a[href*="/Delete/"], button[formaction*="/Delete"]');
    deleteButtons.forEach((button) => {
        button.addEventListener('click', (event) => {
            if (!window.confirm('Вы уверены, что хотите удалить запись?')) {
                event.preventDefault();
            }
        });
    });
}

function highlightActiveNavLink() {
    const currentPath = window.location.pathname;
    document.querySelectorAll('.navbar-nav .nav-link').forEach((link) => {
        const href = link.getAttribute('href');
        if (!href) {
            return;
        }

        if (href === currentPath || (href !== '/' && currentPath.startsWith(href))) {
            link.classList.add('active');
        }
    });
}

function initTooltips() {
    if (typeof bootstrap === 'undefined' || !bootstrap.Tooltip) {
        return;
    }

    document.querySelectorAll('[title]').forEach((element) => {
        element.setAttribute('data-bs-toggle', 'tooltip');
    });

    document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach((element) => {
        new bootstrap.Tooltip(element);
    });
}

function initPhoneMask() {
    const phoneInputs = document.querySelectorAll('input[type="tel"], input[name="Phone"]');
    phoneInputs.forEach((input) => {
        input.setAttribute('placeholder', '+7 (XXX) XXX-XX-XX');

        input.addEventListener('input', (event) => {
            const target = event.target;
            if (!(target instanceof HTMLInputElement)) {
                return;
            }

            let digits = target.value.replace(/\D/g, '');
            if (digits.startsWith('8')) {
                digits = `7${digits.substring(1)}`;
            }
            if (digits.length > 0 && !digits.startsWith('7')) {
                digits = `7${digits}`;
            }

            target.value = formatPhone(digits.substring(0, 11));
        });

        if (input.value) {
            input.dispatchEvent(new Event('input', { bubbles: true }));
        }
    });
}

function formatPhone(digits) {
    if (!digits) {
        return '';
    }

    let formatted = '+7';
    if (digits.length > 1) {
        formatted += ` (${digits.substring(1, 4)}`;
    }
    if (digits.length >= 5) {
        formatted += `) ${digits.substring(4, 7)}`;
    }
    if (digits.length >= 8) {
        formatted += `-${digits.substring(7, 9)}`;
    }
    if (digits.length >= 10) {
        formatted += `-${digits.substring(9, 11)}`;
    }

    return formatted;
}

function focusFirstFormField() {
    const firstInput = document.querySelector('form input:not([type="hidden"]):not([readonly]), form select:not([readonly]), form textarea:not([readonly])');
    if (firstInput instanceof HTMLElement) {
        firstInput.focus();
    }
}
