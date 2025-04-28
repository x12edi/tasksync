import React, { useState, useEffect } from 'react';
import { Table, Button, Input, Select, Spin, Modal, Form, DatePicker, message, Popconfirm } from 'antd';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import moment from 'moment';

const { Option } = Select;

interface Task {
    id: number;
    title: string;
    description: string;
    isCompleted: boolean;
    assignedTo: string;
    dueDate: string;
}

interface User {
    username: string;
}

const TasksTable: React.FC = () => {
    const [tasks, setTasks] = useState<Task[]>([]);
    const [totalCount, setTotalCount] = useState<number>(0);
    const [loading, setLoading] = useState<boolean>(false);
    const [modalLoading, setModalLoading] = useState<boolean>(false);
    const [page, setPage] = useState<number>(1);
    const [pageSize, setPageSize] = useState<number>(10);
    const [status, setStatus] = useState<string | undefined>(undefined);
    const [assignedTo, setAssignedTo] = useState<string | undefined>(undefined);
    const [search, setSearch] = useState<string | undefined>(undefined);
    const [sortBy, setSortBy] = useState<string | undefined>(undefined);
    const [sortOrder, setSortOrder] = useState<string | undefined>(undefined);
    const [isModalVisible, setIsModalVisible] = useState<boolean>(false);
    const [isEditMode, setIsEditMode] = useState<boolean>(false);
    const [currentTask, setCurrentTask] = useState<Task | null>(null);
    const [users, setUsers] = useState<User[]>([]);
    const [form] = Form.useForm();
    const navigate = useNavigate();

    // Utility to ensure minimum loading time for visibility
    const withMinimumLoading = async<T>(promise: Promise<T>, minMs: number = 500): Promise<T> => {
    const start = Date.now();
        const result = await promise;
        const elapsed = Date.now() - start;
        if (elapsed < minMs) {
            await new Promise((resolve) => setTimeout(resolve, minMs - elapsed));
    }
        return result;
  };

  const fetchTasks = async () => {
            setLoading(true);
        console.log('fetchTasks: Starting');
        try {
      const token = localStorage.getItem('token');
        if (!token) {
            console.log('fetchTasks: No token, redirecting to login');
        navigate('/login');
        return;
      }
        const params: any = {page, pageSize};
        if (status) params.status = status;
        if (assignedTo) params.assignedTo = assignedTo;
        if (search) params.search = search;
        if (sortBy && sortOrder) {
            params.sortBy = sortBy;
        params.sortOrder = sortOrder;
      }
        const response = await withMinimumLoading(
        axios.get('http://localhost:5000/api/v1/tasks', {
            headers: {Authorization: `Bearer ${token}` },
        params,
        })
        );
        console.log('fetchTasks: Response:', response.data);
        setTasks(response.data.data);
        setTotalCount(response.data.totalCount);
    } catch (error: any) {
            console.error('fetchTasks: Error:', error.response?.data || error.message);
        message.error('Failed to fetch tasks: ' + (error.response?.data?.detail || error.message));
        if (error.response?.status === 401) {
            navigate('/login');
      }
    } finally {
            setLoading(false);
        console.log('fetchTasks: Completed, loading:', false);
    }
  };

  const fetchUsers = async () => {
            console.log('fetchUsers: Starting');
        try {
      const token = localStorage.getItem('token');
        const response = await axios.get('http://localhost:5000/api/v1/users', {
            headers: {Authorization: `Bearer ${token}` },
      });
        console.log('fetchUsers: Response:', response.data.users);
        const userData = Array.isArray(response.data.users)
        ? response.data.users
        : response.data.users && Array.isArray(response.data.users)
        ? response.data.users
        : [];
        if (!userData.length) {
            console.warn('fetchUsers: No users found');
      }
        setUsers(userData);
    } catch (error: any) {
            console.error('fetchUsers: Error:', error.response?.data || error.message);
        message.error('Failed to fetch users: ' + (error.response?.data?.detail || error.message));
        setUsers([]);
    }
  };

  useEffect(() => {
            fetchTasks();
        fetchUsers();
  }, [page, pageSize, status, assignedTo, search, sortBy, sortOrder]);

  const handleAddTask = () => {
            setIsEditMode(false);
        setCurrentTask(null);
        form.resetFields();
        setIsModalVisible(true);
        console.log('handleAddTask: Modal opened');
  };

  const handleEditTask = (task: Task) => {
            setIsEditMode(true);
        setCurrentTask(task);
        form.setFieldsValue({
            title: task.title,
        description: task.description,
        isCompleted: task.isCompleted,
        assignedTo: task.assignedTo,
        dueDate: task.dueDate ? moment(task.dueDate) : null,
    });
        setIsModalVisible(true);
        console.log('handleEditTask: Modal opened for task:', task.id);
  };

  const handleDeleteTask = async (id: number) => {
            setModalLoading(true);
        console.log('handleDeleteTask: Starting for task:', id);
        try {
      const token = localStorage.getItem('token');
        await withMinimumLoading(
        axios.delete(`http://localhost:5000/api/v1/tasks/${id}`, {
            headers: {Authorization: `Bearer ${token}` },
        })
        );
        message.success('Task deleted successfully');
        console.log('handleDeleteTask: Success');
        fetchTasks();
    } catch (error: any) {
            console.error('handleDeleteTask: Error:', error.response?.data || error.message);
        if (error.response?.status === 403) {
            message.error('Only Managers can delete tasks');
      } else {
            message.error('Failed to delete task: ' + (error.response?.data?.detail || error.message));
      }
    } finally {
            setModalLoading(false);
        console.log('handleDeleteTask: Completed, modalLoading:', false);
    }
  };

  const handleModalOk = async () => {
            setModalLoading(true);
        console.log('handleModalOk: Starting');
        try {
      const values = await form.validateFields();
        const token = localStorage.getItem('token');
        const payload = {
            title: values.title,
        description: values.description,
        isCompleted: values.isCompleted,
        assignedTo: values.assignedTo,
        dueDate: values.dueDate ? values.dueDate.toISOString() : null,
      };
        if (isEditMode && currentTask) {
            await withMinimumLoading(
                axios.put(`http://localhost:5000/api/v1/tasks/${currentTask.id}`, payload, {
                    headers: { Authorization: `Bearer ${token}` },
                })
            );
        message.success('Task updated successfully');
        console.log('handleModalOk: Task updated:', currentTask.id);
      } else {
            await withMinimumLoading(
                axios.post('http://localhost:5000/api/v1/tasks', payload, {
                    headers: { Authorization: `Bearer ${token}` },
                })
            );
        message.success('Task created successfully');
        console.log('handleModalOk: Task created');
      }
        setIsModalVisible(false);
        form.resetFields();
        fetchTasks();
    } catch (error: any) {
            console.error('handleModalOk: Error:', error.response?.data || error.message);
        message.error('Failed to save task: ' + (error.response?.data?.detail || error.message));
    } finally {
            setModalLoading(false);
        console.log('handleModalOk: Completed, modalLoading:', false);
    }
  };

  const handleModalCancel = () => {
            setIsModalVisible(false);
        form.resetFields();
        console.log('handleModalCancel: Modal closed');
  };

        const columns = [
        {
            title: 'ID',
        dataIndex: 'id',
        key: 'id',
        sorter: true,
    },
        {
            title: 'Title',
        dataIndex: 'title',
        key: 'title',
        sorter: true,
    },
        {
            title: 'Description',
        dataIndex: 'description',
        key: 'description',
        sorter: true,
    },
        {
            title: 'Status',
        dataIndex: 'isCompleted',
        key: 'isCompleted',
        sorter: true,
      render: (isCompleted: boolean) => (isCompleted ? 'Completed' : 'Incomplete'),
    },
        {
            title: 'Due Date',
        dataIndex: 'dueDate',
        key: 'dueDate',
        sorter: true,
      render: (dueDate: string) => (dueDate ? new Date(dueDate).toLocaleDateString() : ''),
    },
        {
            title: 'Assigned To',
        dataIndex: 'assignedTo',
        key: 'assignedTo',
        sorter: true,
    },
        {
            title: 'Actions',
        key: 'actions',
      render: (_: any, record: Task) => (
        <>
            <Button type="link" onClick={() => handleEditTask(record)}>
                Edit
            </Button>
            <Popconfirm
                title="Are you sure to delete this task?"
                onConfirm={() => handleDeleteTask(record.id)}
                okText="Yes"
                cancelText="No"
            >
                <Button type="link" danger>
                    Delete
                </Button>
            </Popconfirm>
        </>
        ),
    },
        ];

  const handleTableChange = (pagination: any, filters: any, sorter: any) => {
            setPage(pagination.current);
        setPageSize(pagination.pageSize);
        if (sorter.field && sorter.order) {
            setSortBy(sorter.field);
        setSortOrder(sorter.order === 'ascend' ? 'asc' : 'desc');
    } else {
            setSortBy(undefined);
        setSortOrder(undefined);
    }
        console.log('handleTableChange: Pagination:', pagination, 'Sorter:', sorter);
  };

        return (
        <div>
            <div style={{ marginBottom: 16, display: 'flex', gap: 16 }}>
                <Input
                    placeholder="Search by title or description"
                    onChange={(e) => setSearch(e.target.value || undefined)}
                    style={{ width: 200 }}
                />
                <Select
                    placeholder="Filter by status"
                    allowClear
                    style={{ width: 150 }}
                    onChange={(value) => setStatus(value)}
                >
                    <Option value="completed">Completed</Option>
                    <Option value="incomplete">Incomplete</Option>
                </Select>
                <Select
                    placeholder="Filter by assigned user"
                    allowClear
                    style={{ width: 150 }}
                    onChange={(value) => setAssignedTo(value)}
                >
                    {Array.isArray(users) && users.length > 0 ? (
                        users.map((user) => (
                            <Option key={user.username} value={user.username}>
                                {user.username}
                            </Option>
                        ))
                    ) : (
                        <Option value="" disabled>
                            No users available
                        </Option>
                    )}
                </Select>
                <Button type="primary" onClick={handleAddTask}>
                    Add Task
                </Button>
            </div>
            <Spin spinning={loading} tip="Loading tasks...">
                <Table
                    columns={columns}
                    dataSource={tasks}
                    rowKey="id"
                    pagination={{
                        current: page,
                        pageSize,
                        total: totalCount,
                        showSizeChanger: true,
                    }}
                    onChange={handleTableChange}
                />
            </Spin>
            <Modal
                title={isEditMode ? 'Edit Task' : 'Add Task'}
                visible={isModalVisible}
                onOk={handleModalOk}
                onCancel={handleModalCancel}
                confirmLoading={modalLoading}
            >
                <Spin spinning={modalLoading} tip="Saving task...">
                    <Form form={form} layout="vertical">
                        <Form.Item
                            name="title"
                            label="Title"
                            rules={[{ required: true, message: 'Please enter a title' }]}
                        >
                            <Input />
                        </Form.Item>
                        <Form.Item
                            name="description"
                            label="Description"
                            rules={[{ required: true, message: 'Please enter a description' }]}
                        >
                            <Input.TextArea />
                        </Form.Item>
                        <Form.Item
                            name="isCompleted"
                            label="Status"
                            rules={[{ required: true, message: 'Please select a status' }]}
                        >
                            <Select>
                                <Option value={true}>Completed</Option>
                                <Option value={false}>Incomplete</Option>
                            </Select>
                        </Form.Item>
                        <Form.Item
                            name="assignedTo"
                            label="Assigned To"
                            rules={[{ required: true, message: 'Please select an assignee' }]}
                        >
                            <Select>
                                {Array.isArray(users) && users.length > 0 ? (
                                    users.map((user) => (
                                        <Option key={user.username} value={user.username}>
                                            {user.username}
                                        </Option>
                                    ))
                                ) : (
                                    <Option value="" disabled>
                                        No users available
                                    </Option>
                                )}
                            </Select>
                        </Form.Item>
                        <Form.Item
                            name="dueDate"
                            label="Due Date"
                            rules={[{ required: true, message: 'Please select a due date' }]}
                        >
                            <DatePicker style={{ width: '100%' }} />
                        </Form.Item>
                    </Form>
                </Spin>
            </Modal>
        </div>
        );
};

export default TasksTable;