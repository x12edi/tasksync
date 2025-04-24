import { message } from 'antd';
import { useEffect, useState } from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { isAuthenticated } from '../utils/auth';

const ProtectedRoute: React.FC = () => {
    const isAuth = isAuthenticated();
    const [countdown, setCountdown] = useState(5);
    const [shouldRedirect, setShouldRedirect] = useState(false);

    useEffect(() => {
        if (!isAuth && !shouldRedirect) {
            console.log('Unauthorized detected, starting countdown:', countdown);

            // Try Ant Design message
            message.error({
                content: (
                    <div>
                        Unauthorized access. Redirecting to login in {countdown} second{countdown !== 1 ? 's' : ''}...
                    </div>
                ),
                duration: 0,
                key: 'unauthorized',
            });

            // Start countdown
            const timer = setInterval(() => {
                setCountdown((prev) => {
                    const next = prev - 1;
                    console.log('Countdown:', next);
                    if (next <= 0) {
                        clearInterval(timer);
                        message.destroy('unauthorized');
                        setShouldRedirect(true);
                    }
                    return next;
                });
            }, 1000);

            // Cleanup
            return () => {
                console.log('Cleaning up ProtectedRoute');
                clearInterval(timer);
                message.destroy('unauthorized');
            };
        }
    }, [isAuth, countdown, shouldRedirect]);

    if (shouldRedirect) {
        console.log('Redirecting to /login');
        return <Navigate to="/login" replace />;
    }

    if (!isAuth) {
        // Fallback UI if Ant Design message doesn't show
        return (
            <div
                style={{
                    position: 'fixed',
                    top: '20px',
                    left: '50%',
                    transform: 'translateX(-50%)',
                    background: '#fff1f0',
                    color: '#ff4d4f',
                    padding: '10px 20px',
                    borderRadius: '4px',
                    zIndex: 1000,
                }}
            >
                Unauthorized access. Redirecting to login in {countdown} second{countdown !== 1 ? 's' : ''}...
            </div>
        );
    }

    return <Outlet />;
};

export default ProtectedRoute;