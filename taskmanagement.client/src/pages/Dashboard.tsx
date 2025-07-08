// src/pages/Dashboard.tsx
import React, { useState, useEffect } from "react";
import { Navigate } from "react-router-dom";
import {
  Plus,
  BarChart3,
  CheckCircle2,
  Clock,
  AlertCircle,
} from "lucide-react";
import {
  Container,
  Row,
  Col,
  Card,
  Button,
  Alert,
  Spinner,
  ProgressBar,
} from "react-bootstrap";
import { useAuth } from "../contexts/AuthContext";
import { useTasks } from "../contexts/TaskContext";
import { TaskStatus } from "../types/task";
import Layout from "../components/layout/Layout";
import TaskForm from "../components/tasks/TaskForm";
import TaskList from "../components/tasks/TaskList";
import TaskFilters from "../components/tasks/TaskFilters";

const Dashboard: React.FC = () => {
  const [showCreateForm, setShowCreateForm] = useState(false);
  const { state: authState } = useAuth();
  const { state: taskState, fetchTasks, getFilteredTasks } = useTasks();

  // Redirect if not authenticated
  if (!authState.isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  useEffect(() => {
    fetchTasks();
  }, [fetchTasks]);

  const filteredTasks = getFilteredTasks();

  // Calculate statistics
  const totalTasks = taskState.tasks.length;
  const completedTasks = taskState.tasks.filter(
    (task) => task.status === TaskStatus.COMPLETED
  ).length;
  const inProgressTasks = taskState.tasks.filter(
    (task) => task.status === TaskStatus.IN_PROGRESS
  ).length;
  const pendingTasks = taskState.tasks.filter(
    (task) => task.status === TaskStatus.PENDING
  ).length;
  const overdueTasks = taskState.tasks.filter(
    (task) =>
      new Date(task.dueDate) < new Date() &&
      task.status !== TaskStatus.COMPLETED
  ).length;

  const completionRate =
    totalTasks > 0 ? Math.round((completedTasks / totalTasks) * 100) : 0;

  const handleCreateSuccess = () => {
    setShowCreateForm(false);
  };

  const StatCard: React.FC<{
    title: string;
    value: number;
    icon: React.ReactNode;
    color: string;
    description?: string;
  }> = ({ title, value, icon, color, description }) => (
    <Card className="h-100">
      <Card.Body className="p-4">
        <div className="d-flex align-items-center justify-content-between">
          <div>
            <Card.Subtitle className="text-muted mb-2">{title}</Card.Subtitle>
            <Card.Title className="h2 mb-2">{value}</Card.Title>
            {description && <small className="text-muted">{description}</small>}
          </div>
          <div className={`p-3 rounded-circle ${color}`}>{icon}</div>
        </div>
      </Card.Body>
    </Card>
  );

  return (
    <Layout>
      <Container fluid className="py-4">
        {/* Header */}
        <Row className="mb-4">
          <Col>
            <div className="d-flex justify-content-between align-items-start">
              <div>
                <h1 className="h2 fw-bold mb-1">
                  Welcome back, {authState.user?.username}!
                </h1>
                <p className="text-muted">
                  Here's an overview of your tasks and productivity.
                </p>
              </div>
              <Button
                variant="primary"
                onClick={() => setShowCreateForm(true)}
                className="d-flex align-items-center"
              >
                <Plus className="me-2" size={20} />
                New Task
              </Button>
            </div>
          </Col>
        </Row>

        {/* Statistics */}
        <Row className="mb-4">
          <Col lg={3} md={6} className="mb-3">
            <StatCard
              title="Total Tasks"
              value={totalTasks}
              icon={<BarChart3 size={24} className="text-white" />}
              color="bg-primary"
              description="All your tasks"
            />
          </Col>
          <Col lg={3} md={6} className="mb-3">
            <StatCard
              title="Completed"
              value={completedTasks}
              icon={<CheckCircle2 size={24} className="text-white" />}
              color="bg-success"
              description={`${completionRate}% completion rate`}
            />
          </Col>
          <Col lg={3} md={6} className="mb-3">
            <StatCard
              title="In Progress"
              value={inProgressTasks}
              icon={<Clock size={24} className="text-white" />}
              color="bg-warning"
              description="Currently working on"
            />
          </Col>
          <Col lg={3} md={6} className="mb-3">
            <StatCard
              title="Overdue"
              value={overdueTasks}
              icon={<AlertCircle size={24} className="text-white" />}
              color="bg-danger"
              description="Need attention"
            />
          </Col>
        </Row>

        {/* Quick Stats */}
        {totalTasks > 0 && (
          <Row className="mb-4">
            <Col>
              <Card>
                <Card.Header>
                  <Card.Title className="mb-0">Quick Stats</Card.Title>
                </Card.Header>
                <Card.Body>
                  <Row>
                    {/* Completion Progress */}
                    <Col md={4} className="mb-3">
                      <div className="mb-2 d-flex justify-content-between">
                        <span className="fw-medium text-muted">
                          Overall Progress
                        </span>
                        <span className="text-muted small">
                          {completionRate}%
                        </span>
                      </div>
                      <ProgressBar
                        now={completionRate}
                        variant="primary"
                        className="mb-1"
                      />
                    </Col>

                    {/* Status Breakdown */}
                    <Col md={4} className="mb-3">
                      <h6 className="fw-medium text-muted mb-2">
                        Status Breakdown
                      </h6>
                      <div className="d-flex justify-content-between mb-1">
                        <span className="text-muted small">Pending:</span>
                        <span className="fw-medium small">{pendingTasks}</span>
                      </div>
                      <div className="d-flex justify-content-between mb-1">
                        <span className="text-muted small">In Progress:</span>
                        <span className="fw-medium small">
                          {inProgressTasks}
                        </span>
                      </div>
                      <div className="d-flex justify-content-between">
                        <span className="text-muted small">Completed:</span>
                        <span className="fw-medium small">
                          {completedTasks}
                        </span>
                      </div>
                    </Col>

                    {/* Productivity Insights */}
                    <Col md={4} className="mb-3">
                      <h6 className="fw-medium text-muted mb-2">Insights</h6>
                      <div>
                        {overdueTasks > 0 && (
                          <p className="small text-danger mb-1">
                            📅 {overdueTasks} task
                            {overdueTasks !== 1 ? "s" : ""} overdue
                          </p>
                        )}
                        {inProgressTasks > 0 && (
                          <p className="small text-warning mb-1">
                            ⚡ {inProgressTasks} task
                            {inProgressTasks !== 1 ? "s" : ""} in progress
                          </p>
                        )}
                        {completionRate >= 80 && (
                          <p className="small text-success mb-1">
                            🎉 Great job! High completion rate
                          </p>
                        )}
                        {totalTasks === 0 && (
                          <p className="small text-muted mb-1">
                            🚀 Create your first task to get started
                          </p>
                        )}
                      </div>
                    </Col>
                  </Row>
                </Card.Body>
              </Card>
            </Col>
          </Row>
        )}

        {/* Create Task Form */}
        {showCreateForm && (
          <Row className="mb-4">
            <Col>
              <TaskForm
                onSuccess={handleCreateSuccess}
                onCancel={() => setShowCreateForm(false)}
              />
            </Col>
          </Row>
        )}

        {/* Filters */}
        <Row className="mb-4">
          <Col>
            <TaskFilters />
          </Col>
        </Row>

        {/* Task List */}
        <Row>
          <Col>
            <div className="d-flex justify-content-between align-items-center mb-3">
              <h2 className="h4 mb-0">
                Your Tasks
                {filteredTasks.length !== totalTasks && (
                  <span className="text-muted fs-6 fw-normal ms-2">
                    ({filteredTasks.length} of {totalTasks})
                  </span>
                )}
              </h2>
            </div>

            {taskState.error && (
              <Alert variant="danger" className="mb-4">
                <p className="mb-2">{taskState.error}</p>
                <Button variant="outline-danger" size="sm" onClick={fetchTasks}>
                  Try Again
                </Button>
              </Alert>
            )}

            {taskState.isLoading && totalTasks === 0 ? (
              <div className="d-flex justify-content-center align-items-center py-5">
                <Spinner animation="border" role="status">
                  <span className="visually-hidden">Loading...</span>
                </Spinner>
              </div>
            ) : (
              <TaskList tasks={filteredTasks} loading={taskState.isLoading} />
            )}
          </Col>
        </Row>

        {/* Empty State for New Users */}
        {totalTasks === 0 && !taskState.isLoading && !showCreateForm && (
          <Row>
            <Col>
              <Card className="text-center py-5">
                <Card.Body>
                  <div className="mx-auto" style={{ maxWidth: "400px" }}>
                    <div
                      className="bg-primary bg-opacity-10 rounded-circle d-flex align-items-center justify-content-center mx-auto mb-3"
                      style={{ width: "64px", height: "64px" }}
                    >
                      <Plus size={32} className="text-primary" />
                    </div>
                    <Card.Title className="h5 mb-2">
                      Ready to get organized?
                    </Card.Title>
                    <Card.Text className="text-muted mb-4">
                      Create your first task and start managing your
                      productivity like a pro.
                    </Card.Text>
                    <Button
                      variant="primary"
                      onClick={() => setShowCreateForm(true)}
                      className="d-inline-flex align-items-center"
                    >
                      <Plus size={20} className="me-2" />
                      Create Your First Task
                    </Button>
                  </div>
                </Card.Body>
              </Card>
            </Col>
          </Row>
        )}
      </Container>
    </Layout>
  );
};

export default Dashboard;
