import React from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { X, Save, Plus } from "lucide-react";
import { Card, Form, Button, Spinner, Row, Col } from "react-bootstrap";
import { Task, TaskStatus } from "../../types/task";
import { useTasks } from "../../contexts/TaskContext";

const taskSchema = z.object({
  title: z
    .string()
    .min(1, "Title is required")
    .max(200, "Title cannot exceed 200 characters"),
  description: z
    .string()
    .max(1000, "Description cannot exceed 1000 characters")
    .optional(),
  status: z.number().min(0).max(2),
  dueDate: z.string().min(1, "Due date is required"),
});

type TaskFormData = z.infer<typeof taskSchema>;

interface TaskFormProps {
  task?: Task;
  onSuccess: () => void;
  onCancel: () => void;
}

const TaskForm: React.FC<TaskFormProps> = ({ task, onSuccess, onCancel }) => {
  const { createTask, updateTask } = useTasks();
  const isEditing = !!task;

  const form = useForm<TaskFormData>({
    resolver: zodResolver(taskSchema),
    defaultValues: {
      title: task?.title || "",
      description: task?.description || "",
      status: task ? getStatusValue(task.status) : 0,
      dueDate: task ? new Date(task.dueDate).toISOString().split("T")[0] : "",
    },
  });

  function getStatusValue(status: TaskStatus): number {
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
  }

  const onSubmit = async (data: TaskFormData) => {
    try {
      const taskData = {
        title: data.title,
        description: data.description || "",
        status: data.status,
        dueDate: new Date(data.dueDate).toISOString(),
      };

      if (isEditing && task) {
        await updateTask(task.id, taskData);
      } else {
        await createTask(taskData);
      }

      onSuccess();
    } catch (error) {
      // Error is handled by the context
    }
  };

  const statusOptions = [
    { value: 0, label: "Pending" },
    { value: 1, label: "In Progress" },
    { value: 2, label: "Completed" },
  ];

  return (
    <Card>
      <Card.Header>
        <div className="d-flex align-items-center justify-content-between">
          <div className="d-flex align-items-center">
            {isEditing ? (
              <Save size={20} className="text-primary me-2" />
            ) : (
              <Plus size={20} className="text-success me-2" />
            )}
            <Card.Title className="mb-0 h5">
              {isEditing ? "Edit Task" : "Create New Task"}
            </Card.Title>
          </div>
          <Button variant="outline-secondary" size="sm" onClick={onCancel}>
            <X size={16} />
          </Button>
        </div>
      </Card.Header>

      <Card.Body>
        <Form onSubmit={form.handleSubmit(onSubmit)}>
          <Row>
            <Col md={6} className="mb-3">
              <Form.Group>
                <Form.Label className="fw-medium">
                  Title <span className="text-danger">*</span>
                </Form.Label>
                <Form.Control
                  type="text"
                  placeholder="Enter task title"
                  {...form.register("title")}
                  isInvalid={!!form.formState.errors.title}
                />
                <Form.Control.Feedback type="invalid">
                  {form.formState.errors.title?.message}
                </Form.Control.Feedback>
              </Form.Group>
            </Col>

            <Col md={6} className="mb-3">
              <Form.Group>
                <Form.Label className="fw-medium">
                  Due Date <span className="text-danger">*</span>
                </Form.Label>
                <Form.Control
                  type="date"
                  {...form.register("dueDate")}
                  isInvalid={!!form.formState.errors.dueDate}
                />
                <Form.Control.Feedback type="invalid">
                  {form.formState.errors.dueDate?.message}
                </Form.Control.Feedback>
              </Form.Group>
            </Col>
          </Row>

          <Row>
            <Col className="mb-3">
              <Form.Group>
                <Form.Label className="fw-medium">Description</Form.Label>
                <Form.Control
                  as="textarea"
                  rows={3}
                  placeholder="Enter task description (optional)"
                  {...form.register("description")}
                  isInvalid={!!form.formState.errors.description}
                  style={{ resize: "none" }}
                />
                <Form.Control.Feedback type="invalid">
                  {form.formState.errors.description?.message}
                </Form.Control.Feedback>
              </Form.Group>
            </Col>
          </Row>

          <Row>
            <Col md={6} className="mb-3">
              <Form.Group>
                <Form.Label className="fw-medium">
                  Status <span className="text-danger">*</span>
                </Form.Label>
                <Form.Select
                  {...form.register("status", { valueAsNumber: true })}
                  isInvalid={!!form.formState.errors.status}
                >
                  {statusOptions.map((option) => (
                    <option key={option.value} value={option.value}>
                      {option.label}
                    </option>
                  ))}
                </Form.Select>
                <Form.Control.Feedback type="invalid">
                  {form.formState.errors.status?.message}
                </Form.Control.Feedback>
              </Form.Group>
            </Col>
          </Row>

          <Row className="mt-4">
            <Col>
              <div className="d-flex gap-2">
                <Button
                  type="submit"
                  variant="primary"
                  disabled={form.formState.isSubmitting}
                  className="flex-fill"
                >
                  {form.formState.isSubmitting ? (
                    <>
                      <Spinner animation="border" size="sm" className="me-2" />
                      {isEditing ? "Updating..." : "Creating..."}
                    </>
                  ) : isEditing ? (
                    "Update Task"
                  ) : (
                    "Create Task"
                  )}
                </Button>
                <Button
                  type="button"
                  variant="secondary"
                  onClick={onCancel}
                  className="flex-fill"
                >
                  Cancel
                </Button>
              </div>
            </Col>
          </Row>
        </Form>
      </Card.Body>
    </Card>
  );
};

export default TaskForm;
