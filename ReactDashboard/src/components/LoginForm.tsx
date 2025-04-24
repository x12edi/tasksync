import { Form, Input, Button, message } from 'antd';
import axios from 'axios';
import React from 'react';
import { useNavigate } from 'react-router-dom';

const LoginForm: React.FC = () => {
    const navigate = useNavigate();

    const onFinish = async (values: { username: string; password: string }) => {
        try {
            const response = await axios.post('http://localhost:5000/api/v1/auth/login', values);
            localStorage.setItem('token', response.data.token);
            message.success('Login successful');
            navigate('/home');
        } catch (error) {
            message.error('Login failed: Invalid credentials');
        }
    };

    return (
        <div className="login-container">
            <h2>TaskSync Admin Login</h2>
            <Form onFinish={onFinish} layout="vertical">
                <Form.Item
                    name="username"
                    label="Username"
                    rules={[{ required: true, message: 'Please input your username!' }]}
                >
                    <Input />
                </Form.Item>
                <Form.Item
                    name="password"
                    label="Password"
                    rules={[{ required: true, message: 'Please input your password!' }]}
                >
                    <Input.Password />
                </Form.Item>
                <Form.Item>
                    <Button type="primary" htmlType="submit" block>
                        Login
                    </Button>
                </Form.Item>
            </Form>
        </div>
    );
};

export default LoginForm;