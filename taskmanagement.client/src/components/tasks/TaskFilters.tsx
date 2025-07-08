import React from "react";
import { Search, Filter } from "lucide-react";
import {
  Card,
  Form,
  InputGroup,
  Badge,
  Button,
  Row,
  Col,
} from "react-bootstrap";
import { TaskStatus, TaskFilters } from "../../types/task";
import { useTasks } from "../../contexts/TaskContext";

const TaskFiltersComponent: React.FC = () => {
  const { state, setFilters } = useTasks();

  const statusOptions = [
    { value: "All", label: "All Status" },
    { value: TaskStatus.PENDING, label: "Pending" },
    { value: TaskStatus.IN_PROGRESS, label: "In Progress" },
    { value: TaskStatus.COMPLETED, label: "Completed" },
  ];

  const handleStatusChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
    const value = event.target.value as TaskStatus | "All";
    setFilters({ status: value });
  };

  const handleSearchChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setFilters({ search: event.target.value });
  };

  return (
    <Card className="mb-4">
      <Card.Body>
        <div className="d-flex align-items-center mb-3">
          <Filter size={20} className="text-muted me-2" />
          <Card.Title className="mb-0 h5">Filters</Card.Title>
        </div>

        <Row>
          <Col md={6} className="mb-3">
            <Form.Label className="small fw-medium text-muted">
              Search Tasks
            </Form.Label>
            <InputGroup>
              <InputGroup.Text className="bg-light border-end-0">
                <Search size={16} className="text-muted" />
              </InputGroup.Text>
              <Form.Control
                type="text"
                placeholder="Search tasks..."
                value={state.filters.search}
                onChange={handleSearchChange}
                className="border-start-0"
              />
            </InputGroup>
          </Col>

          <Col md={6} className="mb-3">
            <Form.Label className="small fw-medium text-muted">
              Filter by Status
            </Form.Label>
            <Form.Select
              value={state.filters.status}
              onChange={handleStatusChange}
            >
              {statusOptions.map((option) => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </Form.Select>
          </Col>
        </Row>

        {/* Active Filters Display */}
        {(state.filters.search || state.filters.status !== "All") && (
          <div className="mt-3 pt-3 border-top">
            <div className="d-flex align-items-center flex-wrap gap-2">
              <span className="text-muted small">Active filters:</span>
              {state.filters.search && (
                <Badge bg="primary" className="d-flex align-items-center">
                  Search: "{state.filters.search}"
                </Badge>
              )}
              {state.filters.status !== "All" && (
                <Badge bg="success" className="d-flex align-items-center">
                  Status: {state.filters.status}
                </Badge>
              )}
              <Button
                variant="link"
                size="sm"
                onClick={() => setFilters({ search: "", status: "All" })}
                className="p-0 text-decoration-underline small"
              >
                Clear all
              </Button>
            </div>
          </div>
        )}
      </Card.Body>
    </Card>
  );
};

export default TaskFiltersComponent;
