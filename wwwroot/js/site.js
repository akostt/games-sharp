// Подтверждение удаления
document.addEventListener('DOMContentLoaded', function () {
    // Подтверждение удаления
    const deleteButtons = document.querySelectorAll('a[href*="/Delete/"], button[formaction*="/Delete"]');
    deleteButtons.forEach(button => {
        button.addEventListener('click', function (e) {
            if (!confirm('Вы уверены, что хотите удалить этот элемент?')) {
                e.preventDefault();
            }
        });
    });

    // Подсветка активного пункта меню
    const currentPath = window.location.pathname;
    const navLinks = document.querySelectorAll('.navbar-nav .nav-link');
    navLinks.forEach(link => {
        if (link.getAttribute('href') === currentPath) {
            link.classList.add('active');
            link.style.fontWeight = 'bold';
        }
    });

    // Анимация для таблиц
    const tableRows = document.querySelectorAll('tbody tr');
    tableRows.forEach((row, index) => {
        setTimeout(() => {
            row.style.opacity = '0';
            row.style.transform = 'translateY(20px)';
            row.style.transition = 'opacity 0.3s ease, transform 0.3s ease';
            setTimeout(() => {
                row.style.opacity = '1';
                row.style.transform = 'translateY(0)';
            }, 50);
        }, index * 50);
    });

    // Tooltip для кнопок
    const buttons = document.querySelectorAll('[title]');
    buttons.forEach(button => {
        button.setAttribute('data-bs-toggle', 'tooltip');
    });

    // Инициализация Bootstrap tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Валидация форм
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function (event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        });
    });

    // Автофокус на первом поле формы
    const firstInput = document.querySelector('form input:not([type="hidden"]):not([readonly])');
    if (firstInput) {
        firstInput.focus();
    }

    // Автоматическое форматирование номера телефона
    const phoneInputs = document.querySelectorAll('input[type="tel"], input[name="Phone"]');
    phoneInputs.forEach(input => {
        input.addEventListener('input', function (e) {
            let value = e.target.value.replace(/\D/g, ''); // Удаляем все нецифровые символы
            
            // Если номер начинается с 8, заменяем на 7
            if (value.startsWith('8')) {
                value = '7' + value.substring(1);
            }
            
            // Если номер не начинается с 7, добавляем 7
            if (value.length > 0 && !value.startsWith('7')) {
                value = '7' + value;
            }
            
            // Форматируем номер
            let formattedValue = '';
            if (value.length > 0) {
                formattedValue = '+7';
                if (value.length > 1) {
                    formattedValue += ' (' + value.substring(1, 4);
                }
                if (value.length >= 5) {
                    formattedValue += ') ' + value.substring(4, 7);
                }
                if (value.length >= 8) {
                    formattedValue += '-' + value.substring(7, 9);
                }
                if (value.length >= 10) {
                    formattedValue += '-' + value.substring(9, 11);
                }
            }
            
            e.target.value = formattedValue;
        });

        // Устанавливаем placeholder
        input.setAttribute('placeholder', '+7 (XXX) XXX-XX-XX');
        
        // Если поле уже содержит значение, форматируем его
        if (input.value) {
            const event = new Event('input', { bubbles: true });
            input.dispatchEvent(event);
        }
    });

    // Плавная прокрутка к якорям
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth'
                });
            }
        });
    });
});

// Функция для отображения уведомлений
function showNotification(message, type = 'info') {
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show position-fixed top-0 end-0 m-3`;
    alertDiv.style.zIndex = '9999';
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    document.body.appendChild(alertDiv);
    
    setTimeout(() => {
        alertDiv.classList.remove('show');
        setTimeout(() => alertDiv.remove(), 150);
    }, 3000);
}

// Функция для форматирования дат
function formatDate(dateString) {
    const options = { year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit' };
    return new Date(dateString).toLocaleDateString('ru-RU', options);
}

// Обработка кликов по строкам таблиц
document.addEventListener('click', function(e) {
    const row = e.target.closest('tbody tr[data-href]');
    if (row) {
        window.location.href = row.dataset.href;
    }
});
