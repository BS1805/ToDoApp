let userPermissions = 0;

async function fetchPermissions() {
    try {
        const response = await fetch('/api/account/permissions');
        if (response.ok) {
            const data = await response.json();
            userPermissions = data.Permissions;
        }
    } catch (error) {
        console.error('Failed to fetch permissions:', error);
    }
}

function updateActionVisibility() {
    const CREATE = 1;
    const DETAILS = 2;
    const EDIT = 4;
    const DELETE = 8;

    document.querySelectorAll('[id^="details-"]').forEach(el => {
        if ((userPermissions & DETAILS) === DETAILS) {
            el.style.display = 'inline-block';
        }
    });

    document.querySelectorAll('[id^="edit-"]').forEach(el => {
        if ((userPermissions & EDIT) === EDIT) {
            el.style.display = 'inline-block';
        }
    });

    document.querySelectorAll('[id^="delete-"]').forEach(el => {
        if ((userPermissions & DELETE) === DELETE) {
            el.style.display = 'inline-block';
        }
    });
}

// Fetch permissions and update visibility on page load
document.addEventListener('DOMContentLoaded', async () => {
    await fetchPermissions();
    updateActionVisibility();
});
