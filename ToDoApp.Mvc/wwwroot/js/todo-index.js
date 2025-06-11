document.addEventListener('DOMContentLoaded', function () {
    const createTaskBtn = document.getElementById('createTaskBtn');
    const createTaskModal = new bootstrap.Modal(document.getElementById('createTaskModal'));
    createTaskBtn.addEventListener('click', function () {
        createTaskModal.show();
    });
});