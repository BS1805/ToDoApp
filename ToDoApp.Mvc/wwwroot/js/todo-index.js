document.addEventListener('DOMContentLoaded', function () {
    const createTaskBtn = document.getElementById('createTaskBtn');
    const createTaskModal = new bootstrap.Modal(document.getElementById('createTaskModal'));
    const canCreateUrl = createTaskBtn.getAttribute('data-cancreate-url');

    createTaskBtn.addEventListener('click', function () {
        // Check permission before showing modal
        fetch(canCreateUrl, { method: 'GET', credentials: 'include' })
            .then(response => {
                if (!response.ok) {
                    createTaskModal.show();
                } else if (response.status === 403) {
                    Swal.fire('No Access', 'You do not have permission to create tasks.', 'error');
                } else {
                    Swal.fire('Error', 'Could not verify permissions.', 'error');
                }
            })
            .catch(() => {
                Swal.fire('Error', 'Could not verify permissions.', 'error');
            });
    });

    // Delete Task Confirmation (unchanged)
    document.querySelectorAll('.btn-danger').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            const deleteUrl = btn.getAttribute('href');
            Swal.fire({
                title: 'Are you sure?',
                text: 'This action cannot be undone.',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Yes, delete it!',
                cancelButtonText: 'Cancel'
            }).then((result) => {
                if (result.isConfirmed) {
                    fetch(deleteUrl, {
                        method: 'POST',
                        headers: {
                            'X-Requested-With': 'XMLHttpRequest'
                        }
                    })
                        .then(response => {
                            if (response.ok) {
                                Swal.fire(
                                    'Deleted!',
                                    'The task has been deleted.',
                                    'success'
                                ).then(() => {
                                    location.reload();
                                });
                            } else {
                                throw new Error('Failed to delete task');
                            }
                        })
                        .catch(error => {
                            console.error(error);
                            Swal.fire(
                                'Error!',
                                'An error occurred while deleting the task.',
                                'error'
                            );
                        });
                }
            });
        });
    });
});
