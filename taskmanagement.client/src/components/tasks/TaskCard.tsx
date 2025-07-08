import React, { useState } from "react";
import { format } from "date-fns";
import {
  Edit2,
  Trash2,
  Calendar,
  Clock,
  CheckCircle2,
  Circle,
  PlayCircle,
} from "lucide-react";
import { Card, Button, Badge, Spinner } from "react-bootstrap";
import { Task, TaskStatus } from "../../types/task";
import { useTasks } from "../../contexts/TaskContext";
import TaskForm from "./TaskForm";

interface TaskCardProps {
  task: Task;
}

const TaskCard: React.FC<TaskCardProps> = ({ task }) => {
  const [isEditing, setIsEditing] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);
  const { deleteTask, patchTask } = useTasks();

  const getStatusIcon = (status: TaskStatus) => {
    switch (status) {
      case TaskStatus.PENDING:
        return <Circle size={20} className="text-muted" />;
      case TaskStatus.IN_PROGRESS:
        return <PlayCircle size={20} className="text-primary" />;
      case TaskStatus.COMPLETED:
        return <CheckCircle2 size={20} className="text-success" />;
      default:
        return <Circle size={20} className="text-muted" />;
    }
  };

  const getStatusBadge = (status: TaskStatus) => {
    switch (status) {
      case TaskStatus.PENDING:
        return <Badge bg="secondary">Pending</Badge>;
      case TaskStatus.IN_PROGRESS:
        return (
          <Badge bg="warning" text="dark">
            In Progress
          </Badge>
        );
      case TaskStatus.COMPLETED:
        return <Badge bg="success">Completed</Badge>;
      default:
        return <Badge bg="secondary">Unknown</Badge>;
    }
  };

  const getStatusValue = (status: TaskStatus): number => {
    switch (status) {
      case TaskStatus.PENDING:
        return 0;
      case TaskStatus.IN_PROGRESS:
        return 1;
      case TaskStatus.COMPLETED:
        return 2;
      default:
        return 0;
    }
  };

  const handleStatusChange = async () => {
    const currentStatus = getStatusValue(task.status);
    const nextStatus = currentStatus === 2 ? 0 : currentStatus + 1;

    try {
      await patchTask(task.id, { status: nextStatus });
    } catch (error) {
      // Error is handled by the context
    }
  };

  const handleDelete = async () => {
    if (window.confirm("Are you sure you want to delete this task?")) {
      setIsDeleting(true);
      try {
        await deleteTask(task.id);
      } catch (error) {
        // Error is handled by the context
      } finally {
        setIsDeleting(false);
      }
    }
  };

  const isOverdue =
    new Date(task.dueDate) < new Date() && task.status !== TaskStatus.COMPLETED;

  if (isEditing) {
    return (
      <TaskForm
        task={task}
        onSuccess={() => setIsEditing(false)}
        onCancel={() => setIsEditing(false)}
      />
    );
  }

  return (
    <Card
      className={`h-100 ${
        isOverdue ? "border-danger bg-danger bg-opacity-10" : ""
      }`}
    >
      <Card.Body className="p-3">
        <div className="d-flex align-items-start justify-content-between mb-3">
          <div className="d-flex align-items-center">
            <Button
              variant="link"
              onClick={handleStatusChange}
              className="p-0 me-2 text-decoration-none"
              style={{ lineHeight: 1 }}
              title="Click to change status"
            >
              {getStatusIcon(task.status)}
            </Button>
            <h5
              className={`mb-0 ${
                task.status === TaskStatus.COMPLETED
                  ? "text-decoration-line-through text-muted"
                  : "text-dark"
              }`}
            >
              {task.title}
            </h5>
          </div>
          <div className="d-flex">
            <Button
              variant="outline-secondary"
              size="sm"
              onClick={() => setIsEditing(true)}
              className="me-1 p-1"
              style={{ width: "32px", height: "32px" }}
            >
              <Edit2 size={14} />
            </Button>
            <Button
              variant="outline-danger"
              size="sm"
              onClick={handleDelete}
              disabled={isDeleting}
              className="p-1"
              style={{ width: "32px", height: "32px" }}
            >
              {isDeleting ? (
                <Spinner animation="border" size="sm" />
              ) : (
                <Trash2 size={14} />
              )}
            </Button>
          </div>
        </div>

        {task.description && (
          <p
            className={`text-muted mb-3 ${
              task.status === TaskStatus.COMPLETED
                ? "text-decoration-line-through"
                : ""
            }`}
          >
            {task.description}
          </p>
        )}

        <div className="d-flex align-items-center justify-content-between">
          <div className="d-flex align-items-center">
            {getStatusBadge(task.status)}

            <div className="d-flex align-items-center ms-3 small text-muted">
              <Calendar size={14} className="me-1" />
              <span className={isOverdue ? "text-danger fw-medium" : ""}>
                {format(new Date(task.dueDate), "MMM dd, yyyy")}
              </span>
              {isOverdue && (
                <span className="text-danger fw-medium ms-1">(Overdue)</span>
              )}
            </div>
          </div>

          <div className="d-flex align-items-center small text-muted">
            <Clock size={12} className="me-1" />
            <span>Created {format(new Date(task.createdAt), "MMM dd")}</span>
          </div>
        </div>
      </Card.Body>
    </Card>
  );
};

export default TaskCard;
