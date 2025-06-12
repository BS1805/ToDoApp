document.addEventListener('DOMContentLoaded', function () {
    const config = window.TasksByStatusConfig;
    let currentPage = 1;
    let pageSize = config.pageSize;
    let statusId = config.statusId;

    function fetchTasks(page = 1) {
        const url = `${config.apiUrl}?statusId=${statusId}&page=${page}&pageSize=${pageSize}`;
        fetch(url, { credentials: 'include' })
            .then(r => r.json())
            .then(renderTasks)
            .catch(() => {
                document.getElementById('tasksContainer').innerHTML = '<div class="alert alert-danger">Failed to load tasks.</div>';
            });
    }

    function renderTasks(data) {
        const container = document.getElementById('tasksContainer');
        container.innerHTML = '';
        if (!data.items || data.items.length === 0) {
            container.innerHTML = '<div class="col-12"><div class="alert alert-info">No tasks found.</div></div>';
            return;
        }
        data.items.forEach(item => {
            container.innerHTML += `
                <div class="col-lg-4 col-md-6 col-sm-12 mb-3">
                    <div class="task-card mx-auto">
                        <div class="card-header">
                            <h5 class="card-title d-flex justify-content-between align-items-center">
                                <span>${item.title}</span>
                                <a class="text-decoration-none text-secondary" href="/ToDo/Edit/${item.id}">
                                    <i class="bi bi-pencil-square"></i> Edit
                                </a>
                            </h5>
                        </div>
                        <div class="card-body">
                            <div class="row">
                                <div class="col-md-3 col-sm-12 mb-md-0 mb-3">
                                    <div class="task-icon">
                                        <i class="bi bi-clipboard-check"></i>
                                    </div>
                                </div>
                                <div class="col-md-9 col-sm-12">
                                    <p class="card-text">${item.description}</p>
                                    <p class="card-text">
                                        <b>Status: </b>
                                        ${renderStatusBadge(item.statusId)}
                                    </p>
                                </div>
                            </div>
                        </div>
                        <div class="card-footer">
                            <div class="d-flex flex-wrap justify-content-between align-items-center gap-2">
                                <a class="btn btn-outline-primary btn-details" href="/ToDo/Details/${item.id}">
                                    <i class="bi bi-eye-fill"></i> Show Details
                                </a>
                                <a class="btn btn-danger text-white" href="/ToDo/Delete/${item.id}">
                                    <i class="bi bi-trash"></i> Delete
                                </a>
                            </div>
                        </div>
                    </div>
                </div>
            `;
        });
        renderPagination(data.pageIndex, data.totalPages);
    }

    function renderStatusBadge(statusId) {
        switch (statusId) {
            case 1: return '<span class="badge bg-primary text-white">PENDING</span>';
            case 2: return '<span class="badge bg-warning text-dark">IN PROGRESS</span>';
            case 3: return '<span class="badge bg-success text-white">COMPLETED</span>';
            default: return '<span class="badge bg-secondary text-white">UNKNOWN</span>';
        }
    }

    function renderPagination(pageIndex, totalPages) {
        const container = document.getElementById('paginationContainer');
        if (totalPages <= 1) {
            container.innerHTML = '';
            return;
        }
        let html = `<nav aria-label="Page navigation"><ul class="pagination justify-content-center">`;
        html += `<li class="page-item${pageIndex == 1 ? ' disabled' : ''}">
                    <a class="page-link" href="#" data-page="${pageIndex - 1}"><i class="bi bi-chevron-left"></i></a>
                 </li>`;
        for (let i = Math.max(1, pageIndex - 1); i <= Math.min(totalPages, pageIndex + 1); i++) {
            html += `<li class="page-item${i == pageIndex ? ' active' : ''}">
                        <a class="page-link" href="#" data-page="${i}">${i}</a>
                     </li>`;
        }
        html += `<li class="page-item${pageIndex == totalPages ? ' disabled' : ''}">
                    <a class="page-link" href="#" data-page="${pageIndex + 1}"><i class="bi bi-chevron-right"></i></a>
                 </li>`;
        html += `</ul></nav>`;
        container.innerHTML = html;

        container.querySelectorAll('a.page-link').forEach(link => {
            link.addEventListener('click', function (e) {
                e.preventDefault();
                const page = parseInt(this.getAttribute('data-page'));
                if (!isNaN(page)) {
                    currentPage = page;
                    fetchTasks(currentPage);
                }
            });
        });
    }

    document.getElementById('filterForm').addEventListener('submit', function (e) {
        e.preventDefault();
        pageSize = document.getElementById('pageSize').value;
        statusId = document.getElementById('statusId').value;
        currentPage = 1;
        fetchTasks(currentPage);
    });

    fetchTasks(currentPage);
});
