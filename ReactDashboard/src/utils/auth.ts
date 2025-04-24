export const isAuthenticated = (): boolean => {
    const token = localStorage.getItem('token');
    if (!token) {
        return false;
    }

    try {
        // Decode JWT payload
        const payload = JSON.parse(atob(token.split('.')[1]));
        const expiry = payload.exp * 1000; // Convert to milliseconds
        return Date.now() < expiry; // Check if token is not expired
    } catch (error) {
        console.error('Invalid token:', error);
        return false;
    }
};