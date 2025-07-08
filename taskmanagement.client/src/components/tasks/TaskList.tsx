import React from "react";
import { Row, Col, Card, Placeholder } from "react-bootstrap";
import { Task } from "../../types/task";
import TaskCard from "./TaskCard";
import { ListTodo } from "lucide-react";

interface TaskListProps {
  tasks: Task[];
  loading?: boolean;
}

const TaskList: React.FC<TaskListProps> = ({ tasks, loading }) => {
  if (loading) {
    return (
      <Row>
        {[...Array(3)].map((_, index) => (
          <Col key={index} lg={4} md={6} className="mb-4">
            <Card>
              <Card.Body>
                <div className="d-flex align-items-start justify-content-between mb-3">
                  <div className="d-flex align-items-center">
                    <Placeholder
                      className="rounded-circle me-2"
                      style={{ width: "20px", height: "20px" }}
                    />
                    <Placeholder xs={6} />
                  </div>
                  <div className="d-flex">
                    <Placeholder
                      className="rounded me-1"
                      style={{ width: "32px", height: "32px" }}
                    />
                    <Placeholder
                      className="rounded"
                      style={{ width: "32px", height: "32px" }}
                    />
                  </div>
                </div>
                <Placeholder xs={12} className="mb-3" />
                <div className="d-flex align-items-center justify-content-between">
                  <div className="d-flex align-items-center">
                    <Placeholder xs={3} className="me-3" />
                    <Placeholder xs={4} />
                  </div>
                  <Placeholder xs={3} />
                </div>
              </Card.Body>
            </Card>
          </Col>
        ))}
      </Row>
    );
  }

  if (tasks.length === 0) {
    return (
      <div className="text-center py-5">
        <div className="mb-4">
          <ListTodo size={64} className="text-muted mx-auto" />
        </div>
        <h4 className="fw-medium text-dark mb-2">No tasks found</h4>
        <p className="text-muted">
          Get started by creating your first task above.
        </p>
      </div>
    );
  }

  return (
    <Row>
      {tasks.map((task) => (
        <Col key={task.id} lg={4} md={6} className="mb-4">
          <TaskCard task={task} />
        </Col>
      ))}
    </Row>
  );
};

export default TaskList;
