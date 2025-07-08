import React from "react";
import { LogOut, User, Settings, Bell } from "lucide-react";
import { Navbar, Nav, Button, Badge } from "react-bootstrap";
import { useAuth } from "../../contexts/AuthContext";

const Header: React.FC = () => {
  const { state, logout } = useAuth();

  return (
    <Navbar bg="white" className="shadow-sm border-bottom" expand="lg">
      <div className="container-fluid">
        {/* Logo */}
        <Navbar.Brand className="d-flex align-items-center">
          <div
            className="bg-primary rounded d-flex align-items-center justify-content-center me-2"
            style={{ width: "32px", height: "32px" }}
          >
            <span className="text-white fw-bold small">TM</span>
          </div>
          <span className="fw-bold text-dark">TaskManager</span>
        </Navbar.Brand>

        {/* User Menu */}
        <Nav className="ms-auto d-flex align-items-center">
          {/* Notifications */}
          <Button
            variant="link"
            className="text-muted p-2 me-2 position-relative"
          >
            <Bell size={20} />
            {/* Optional notification badge */}
            {/* <Badge bg="danger" pill className="position-absolute top-0 start-100 translate-middle">
              3
            </Badge> */}
          </Button>

          {/* User Info - Hidden on small screens */}
          <div className="d-none d-md-flex align-items-center me-3">
            <div className="text-end me-2">
              <div className="fw-medium small text-dark">
                {state.user?.username}
              </div>
              <div className="text-muted" style={{ fontSize: "0.75rem" }}>
                {state.user?.email}
              </div>
            </div>

            {/* Avatar */}
            <div
              className="bg-primary bg-opacity-10 rounded-circle d-flex align-items-center justify-content-center me-2"
              style={{ width: "32px", height: "32px" }}
            >
              <User size={16} className="text-primary" />
            </div>
          </div>

          {/* Settings */}
          <Button variant="link" className="text-muted p-2 me-2">
            <Settings size={20} />
          </Button>

          {/* Logout */}
          <Button
            variant="outline-danger"
            size="sm"
            onClick={logout}
            className="d-flex align-items-center"
            title="Logout"
          >
            <LogOut size={16} />
            <span className="d-none d-md-inline ms-1">Logout</span>
          </Button>
        </Nav>
      </div>
    </Navbar>
  );
};

export default Header;
