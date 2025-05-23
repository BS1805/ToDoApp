async function handleApiResponse(response) {
    if (!response.ok) {
        if (response.status === 403) {
            // Redirect to the "No Access" page
            window.location.href = '/Shared/NoAccess';
        } else {
            // Handle other errors (e.g., 404, 500)
            alert('An error occurred: ' + response.statusText);
        }
    }
    return response.json();
}

async function performApiRequest(url, options) {
    try {
        const response = await fetch(url, options);
        return await handleApiResponse(response);
    } catch (error) {
        console.error('Error during API request:', error);
        alert('An unexpected error occurred.');
    }
}
