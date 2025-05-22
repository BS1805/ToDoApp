// Show a "not allowed" message (customize as needed)
function showNotAllowed() {
    // You can use a modal, toast, or redirect to a static page
    alert('You are not allowed to perform this action.');
    // Or redirect: window.location.href = '/not-allowed.html';
}

// Intercept delete form submissions (for forms that use POST to delete)
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.delete-form').forEach(function (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            const action = form.getAttribute('action');
            const formData = new FormData(form);

            fetch(action, {
                method: 'POST',
                body: formData,
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            })
                .then(response => {
                    if (response.status === 403) {
                        showNotAllowed();
                    } else if (response.ok) {
                        window.location.reload();
                    } else {
                        alert('An error occurred.');
                    }
                });
        });
    });

    // Optionally, intercept AJAX for Edit/Create/Details if you use AJAX for those.
    // For normal navigation, the backend should return a "not allowed" view or error page.
});
