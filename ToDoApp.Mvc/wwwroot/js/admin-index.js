document.addEventListener('DOMContentLoaded', function () {
    // Delete User Confirmation
    document.querySelectorAll('.delete-user-btn').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            var form = btn.closest('form');
            var username = btn.getAttribute('data-username');
            Swal.fire({
                title: 'Are you sure?',
                text: `Delete user "${username}"? This action cannot be undone.`,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Yes, delete!',
                cancelButtonText: 'Cancel'
            }).then((result) => {
                if (result.isConfirmed) {
                    form.submit();
                }
            });
        });
    });

    // Activate User Confirmation
    document.querySelectorAll('.btn-success').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            var form = btn.closest('form');
            Swal.fire({
                title: 'Activate User?',
                text: 'Are you sure you want to activate this user?',
                icon: 'info',
                showCancelButton: true,
                confirmButtonColor: '#28a745',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Yes, activate!',
                cancelButtonText: 'Cancel'
            }).then((result) => {
                if (result.isConfirmed) {
                    form.submit();
                }
            });
        });
    });

    // Deactivate User Confirmation
    document.querySelectorAll('.btn-warning').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            var form = btn.closest('form');
            Swal.fire({
                title: 'Deactivate User?',
                text: 'Are you sure you want to deactivate this user?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#ffc107',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Yes, deactivate!',
                cancelButtonText: 'Cancel'
            }).then((result) => {
                if (result.isConfirmed) {
                    form.submit();
                }
            });
        });
    });
});
