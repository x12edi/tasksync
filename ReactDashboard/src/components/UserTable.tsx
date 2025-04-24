import { Table, message } from 'antd';
import axios from 'axios';
import React, { useEffect, useState, useCallback } from 'react';

interface User {
    id: number;
    username: string;
    role: string;
}

const UserTable: React.FC = () => {
    const [users, setUsers] = useState<User[]>([]);
    const [loading, setLoading] = useState(false);
    const [pagination, setPagination] = useState({
        current: 1,
        pageSize: 10,
        total: 0,
    });

    const fetchUsers = useCallback(async (page: number, pageSize: number, sortBy?: string, sortOrder?: string) => {
        setLoading(true);
        try {
            const token = localStorage.getItem('token');
            const response = await axios.get('http://localhost:5000/api/v1/users', {
                headers: { Authorization: `Bearer ${token}` },
                params: { page, pageSize, sortBy, sortOrder },
            });
            console.log('Raw API Response:', response); // Log full response
            const { users, total } = response.data;
            if (!users || !Array.isArray(users)) {
                console.error('Invalid users data:', users);
                setUsers([]);
                setPagination({ current: page, pageSize, total: 0 });
                message.error('No users found');
                return;
            }
            setUsers(users);
            setPagination({
                current: page,
                pageSize,
                total: total || 0,
            });
        } catch (error: any) {
            console.error('Fetch Users Error:', error.response || error);
            message.error('Failed to fetch users');
        }
        setLoading(false);
    }, []);

    useEffect(() => {
        fetchUsers(pagination.current, pagination.pageSize);
    }, [fetchUsers]);

    const handleTableChange = (pagination: any, filters: any, sorter: any) => {
        const sortBy = sorter.field;
        const sortOrder = sorter.order === 'descend' ? 'desc' : sorter.order === 'ascend' ? 'asc' : undefined;
        fetchUsers(pagination.current, pagination.pageSize, sortBy, sortOrder);
    };

    const columns = [
        {
            title: 'ID',
            dataIndex: 'id',
            key: 'id',
        },
        {
            title: 'Username',
            dataIndex: 'username',
            key: 'username',
            sorter: true,
        },
        {
            title: 'Role',
            dataIndex: 'role',
            key: 'role',
            sorter: true,
        },
    ];

    return (
        <Table
            columns={columns}
            dataSource={users}
            rowKey="id"
            loading={loading}
            pagination={pagination}
            onChange={handleTableChange}
            scroll={{ x: 'max-content' }}
        />
    );
};

export default UserTable;