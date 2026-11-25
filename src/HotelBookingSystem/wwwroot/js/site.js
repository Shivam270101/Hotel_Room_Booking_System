// ============================================================
// Grand Horizon Hotel - Site JavaScript
// ============================================================

document.addEventListener('DOMContentLoaded', function () {

    // ----- Navbar scroll effect -----
    const navbar = document.querySelector('.navbar');
    if (navbar) {
        window.addEventListener('scroll', function () {
            if (window.scrollY > 50) {
                navbar.classList.add('scrolled');
            } else {
                navbar.classList.remove('scrolled');
            }
        });
    }

    // ----- Auto-dismiss toasts -----
    const toasts = document.querySelectorAll('.toast.show');
    toasts.forEach(function (toast) {
        setTimeout(function () {
            toast.classList.remove('show');
            setTimeout(function () {
                toast.remove();
            }, 300);
        }, 5000);
    });

    // ----- Date validation for search forms -----
    const checkInInputs = document.querySelectorAll('input[name="checkInDate"], input[name="checkIn"], input[name="CheckInDate"]');
    const checkOutInputs = document.querySelectorAll('input[name="checkOutDate"], input[name="checkOut"], input[name="CheckOutDate"]');

    checkInInputs.forEach(function (checkIn) {
        checkIn.addEventListener('change', function () {
            // Find the corresponding check-out input
            const form = this.closest('form');
            if (form) {
                const checkOut = form.querySelector('input[name="checkOutDate"], input[name="checkOut"], input[name="CheckOutDate"]');
                if (checkOut) {
                    const nextDay = new Date(this.value);
                    nextDay.setDate(nextDay.getDate() + 1);
                    checkOut.min = nextDay.toISOString().split('T')[0];

                    if (new Date(checkOut.value) <= new Date(this.value)) {
                        checkOut.value = nextDay.toISOString().split('T')[0];
                    }
                }
            }
        });
    });

    // ----- Confirm dialogs -----
    document.querySelectorAll('[data-confirm]').forEach(function (el) {
        el.addEventListener('click', function (e) {
            if (!confirm(this.getAttribute('data-confirm'))) {
                e.preventDefault();
            }
        });
    });

    // ----- Active nav link highlighting -----
    const currentPath = window.location.pathname.toLowerCase();
    document.querySelectorAll('.navbar-nav .nav-link').forEach(function (link) {
        const href = link.getAttribute('href');
        if (href) {
            const linkPath = href.toLowerCase();
            if (linkPath === currentPath ||
                (currentPath.startsWith(linkPath) && linkPath !== '/')) {
                link.classList.add('active');
            }
        }
    });

    // ----- Smooth scroll for anchor links -----
    document.querySelectorAll('a[href^="#"]').forEach(function (anchor) {
        anchor.addEventListener('click', function (e) {
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                e.preventDefault();
                target.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }
        });
    });

    // ----- Format currency inputs -----
    document.querySelectorAll('input[type="number"][step="0.01"]').forEach(function (input) {
        input.addEventListener('blur', function () {
            if (this.value) {
                this.value = parseFloat(this.value).toFixed(2);
            }
        });
    });

    console.log('Grand Horizon Hotel - Site initialized');
});
