import { Layout, Menu, Button } from 'antd';
import { UserOutlined, FileTextOutlined, HomeOutlined, LogoutOutlined } from '@ant-design/icons';
import React, { useState } from 'react';
import { Routes, Route, useNavigate } from 'react-router-dom';
import LoginForm from './components/LoginForm';
import UserTable from './components/UserTable';
import ProtectedRoute from './components/ProtectedRoute';
import './App.css';
import TasksTable from './components/TasksTable';

const { Header, Sider, Content } = Layout;

const App: React.FC = () => {
    const [collapsed, setCollapsed] = useState(false);
    const navigate = useNavigate();
    const token = localStorage.getItem('token');

    const handleLogout = () => {
        localStorage.removeItem('token');
        navigate('/login');
    };

    return (
        <Layout style={{ minHeight: '100vh', width: '100vw' }}>
            <Sider collapsible collapsed={collapsed} onCollapse={setCollapsed}>
                <div className="logo" />
                <Menu
                    theme="dark"
                    mode="inline"
                    defaultSelectedKeys={['/home']}
                    onClick={({ key }) => navigate(key)}
                    items={[
                        { key: '/home', icon: <HomeOutlined />, label: 'Home' },
                        { key: '/users', icon: <UserOutlined />, label: 'Users' },
                        { key: '/tasks', icon: <FileTextOutlined />, label: 'Tasks' },
                    ]}
                />
            </Sider>
            <Layout>
                <Header className="header">
                    <span className="header-title">TaskSync Admin</span>
                    <Button
                        type="primary"
                        icon={<LogoutOutlined />}
                        onClick={handleLogout}
                        className="logout-button"
                    >
                        Logout
                    </Button>
                </Header>
                <Content className="content">
                    <Routes>
                        <Route path="/login" element={<LoginForm />} />
                        <Route element={<ProtectedRoute />}>
                            <Route
                                path="/home"
                                element={<div className="content-text">Welcome to TaskSync Admin Dashboard</div>}
                            />
                            <Route path="/users" element={<UserTable />} />
                            <Route
                                path="/"
                                element={<div className="content-text">Welcome to TaskSync Admin Dashboard</div>}
                            />
                            <Route
                                path="/tasks"
                                element={<TasksTable />}
                            />
                        </Route>
                    </Routes>
                </Content>
            </Layout>
        </Layout>
    );
};

export default App;