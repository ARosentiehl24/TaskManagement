// src/pages/Login.tsx
import React, { useState, useEffect } from "react";
import { useNavigate, Navigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Eye, EyeOff, LogIn, UserPlus } from "lucide-react";
import {
  Container,
  Card,
  Button,
  Form,
  InputGroup,
  Alert,
  Spinner,
  Row,
  Col,
} from "react-bootstrap";
import { useAuth } from "../contexts/AuthContext";

// Validation schemas
const loginSchema = z.object({
  username: z.string().min(1, "Username is required"),
  password: z.string().min(1, "Password is required"),
  rememberMe: z.boolean().optional(),
});

const registerSchema = z
  .object({
    username: z
      .string()
      .min(3, "Username must be at least 3 characters")
      .max(50, "Username cannot exceed 50 characters")
      .regex(
        /^[a-zA-Z0-9_]+$/,
        "Username can only contain letters, numbers, and underscores"
      ),
    email: z.string().email("Please enter a valid email address"),
    password: z
      .string()
      .min(8, "Password must be at least 8 characters")
      .regex(/[A-Z]/, "Password must contain at least one uppercase letter")
      .regex(/[a-z]/, "Password must contain at least one lowercase letter")
      .regex(/\d/, "Password must contain at least one number"),
    confirmPassword: z.string(),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "Passwords do not match",
    path: ["confirmPassword"],
  });

type LoginFormData = z.infer<typeof loginSchema>;
type RegisterFormData = z.infer<typeof registerSchema>;

const Login: React.FC = () => {
  const [isLogin, setIsLogin] = useState(true);
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const navigate = useNavigate();
  const { state, login, register, clearError } = useAuth();

  const loginForm = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      username: "",
      password: "",
      rememberMe: false,
    },
  });

  const registerForm = useForm<RegisterFormData>({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      username: "",
      email: "",
      password: "",
      confirmPassword: "",
    },
  });

  useEffect(() => {
    if (state.error) {
      const timeout = setTimeout(() => {
        clearError();
      }, 3000);

      return () => clearTimeout(timeout);
    }
  }, [state.error]);

  const onLoginSubmit = async (data: LoginFormData) => {
    try {
      await login({
        username: data.username,
        password: data.password,
      });
      navigate("/dashboard");
    } catch (error) {
      // Error is handled by the context
    }
  };

  const onRegisterSubmit = async (data: RegisterFormData) => {
    try {
      await register({
        username: data.username,
        email: data.email,
        password: data.password,
      });
      navigate("/dashboard");
    } catch (error) {
      // Error is handled by the context
    }
  };

  const switchMode = () => {
    setIsLogin(!isLogin);
    loginForm.reset();
    registerForm.reset();
    clearError();
  };

  // Demo credentials helper
  const fillDemoCredentials = () => {
    loginForm.setValue("username", "demo_user");
    loginForm.setValue("password", "Demo123!");
  };

  if (state.isLoading) {
    return (
      <Container
        fluid
        className="d-flex justify-content-center align-items-center min-vh-100 bg-light"
      >
        <div className="text-center">
          <Spinner
            animation="border"
            role="status"
            size="sm"
            className="me-2"
          />
          <span>Please wait...</span>
        </div>
      </Container>
    );
  }

  if (state.isAuthenticated) {
    return <Navigate to="/dashboard" replace />;
  }

  return (
    <Container
      fluid
      className="min-vh-100 d-flex align-items-center justify-content-center bg-light py-5"
    >
      <Row className="w-100 justify-content-center">
        <Col xs={12} sm={8} md={6} lg={4} xl={3}>
          {/* Header */}
          <div className="text-center mb-4">
            <h2 className="h3 fw-bold text-dark mb-2">
              {isLogin ? "Sign in to your account" : "Create your account"}
            </h2>
            <p className="text-muted small">
              {isLogin
                ? "Don't have an account? "
                : "Already have an account? "}
              <Button
                variant="link"
                size="sm"
                onClick={switchMode}
                className="p-0 text-decoration-none"
              >
                {isLogin ? "Sign up" : "Sign in"}
              </Button>
            </p>
          </div>

          <Card className="shadow-sm">
            <Card.Header className="bg-white text-center py-3">
              <div className="d-flex align-items-center justify-content-center">
                {isLogin ? (
                  <LogIn className="me-2" size={24} color="#0d6efd" />
                ) : (
                  <UserPlus className="me-2" size={24} color="#0d6efd" />
                )}
                <h4 className="mb-0 text-dark">
                  {isLogin ? "Login" : "Register"}
                </h4>
              </div>
            </Card.Header>

            <Card.Body className="p-4">
              {/* Error Display */}
              {state.error && (
                <Alert variant="danger" className="mb-3">
                  {state.error}
                </Alert>
              )}

              {/* Demo Credentials Banner */}
              {isLogin && (
                <Alert variant="info" className="mb-3">
                  <p className="mb-2 fw-semibold">Demo Credentials:</p>
                  <div className="d-flex justify-content-between align-items-center">
                    <small className="text-muted">demo_user / Demo123!</small>
                    <Button
                      variant="outline-info"
                      size="sm"
                      onClick={fillDemoCredentials}
                    >
                      Use Demo
                    </Button>
                  </div>
                </Alert>
              )}

              {/* Login Form */}
              {isLogin ? (
                <Form onSubmit={loginForm.handleSubmit(onLoginSubmit)}>
                  <Form.Group className="mb-3">
                    <Form.Label>Username</Form.Label>
                    <Form.Control
                      type="text"
                      placeholder="Enter your username"
                      {...loginForm.register("username")}
                      isInvalid={!!loginForm.formState.errors.username}
                    />
                    <Form.Control.Feedback type="invalid">
                      {loginForm.formState.errors.username?.message}
                    </Form.Control.Feedback>
                  </Form.Group>

                  <Form.Group className="mb-3 position-relative">
                    <Form.Label>Password</Form.Label>
                    <InputGroup>
                      <Form.Control
                        type={showPassword ? "text" : "password"}
                        placeholder="Enter your password"
                        {...loginForm.register("password")}
                        isInvalid={!!loginForm.formState.errors.password}
                      />
                      <Button
                        variant="outline-secondary"
                        onClick={() => setShowPassword(!showPassword)}
                        style={{ zIndex: 1 }}
                      >
                        {showPassword ? (
                          <EyeOff size={16} />
                        ) : (
                          <Eye size={16} />
                        )}
                      </Button>
                    </InputGroup>
                    {loginForm.formState.errors.password && (
                      <div className="invalid-feedback d-block">
                        {loginForm.formState.errors.password.message}
                      </div>
                    )}
                  </Form.Group>

                  <Form.Group className="mb-3">
                    <Form.Check
                      type="checkbox"
                      id="rememberMe"
                      label="Remember me"
                      {...loginForm.register("rememberMe")}
                    />
                  </Form.Group>

                  <Button
                    type="submit"
                    variant="primary"
                    className="w-100"
                    disabled={state.isLoading}
                  >
                    {state.isLoading ? (
                      <>
                        <Spinner
                          animation="border"
                          size="sm"
                          className="me-2"
                        />
                        Signing in...
                      </>
                    ) : (
                      "Sign in"
                    )}
                  </Button>
                </Form>
              ) : (
                /* Register Form */
                <Form onSubmit={registerForm.handleSubmit(onRegisterSubmit)}>
                  <Form.Group className="mb-3">
                    <Form.Label>Username</Form.Label>
                    <Form.Control
                      type="text"
                      placeholder="Choose a username"
                      {...registerForm.register("username")}
                      isInvalid={!!registerForm.formState.errors.username}
                    />
                    <Form.Control.Feedback type="invalid">
                      {registerForm.formState.errors.username?.message}
                    </Form.Control.Feedback>
                  </Form.Group>

                  <Form.Group className="mb-3">
                    <Form.Label>Email</Form.Label>
                    <Form.Control
                      type="email"
                      placeholder="Enter your email"
                      {...registerForm.register("email")}
                      isInvalid={!!registerForm.formState.errors.email}
                    />
                    <Form.Control.Feedback type="invalid">
                      {registerForm.formState.errors.email?.message}
                    </Form.Control.Feedback>
                  </Form.Group>

                  <Form.Group className="mb-3">
                    <Form.Label>Password</Form.Label>
                    <InputGroup>
                      <Form.Control
                        type={showPassword ? "text" : "password"}
                        placeholder="Create a password"
                        {...registerForm.register("password")}
                        isInvalid={!!registerForm.formState.errors.password}
                      />
                      <Button
                        variant="outline-secondary"
                        onClick={() => setShowPassword(!showPassword)}
                        style={{ zIndex: 1 }}
                      >
                        {showPassword ? (
                          <EyeOff size={16} />
                        ) : (
                          <Eye size={16} />
                        )}
                      </Button>
                    </InputGroup>
                    {registerForm.formState.errors.password && (
                      <div className="invalid-feedback d-block">
                        {registerForm.formState.errors.password.message}
                      </div>
                    )}
                  </Form.Group>

                  <Form.Group className="mb-3">
                    <Form.Label>Confirm Password</Form.Label>
                    <InputGroup>
                      <Form.Control
                        type={showConfirmPassword ? "text" : "password"}
                        placeholder="Confirm your password"
                        {...registerForm.register("confirmPassword")}
                        isInvalid={
                          !!registerForm.formState.errors.confirmPassword
                        }
                      />
                      <Button
                        variant="outline-secondary"
                        onClick={() =>
                          setShowConfirmPassword(!showConfirmPassword)
                        }
                        style={{ zIndex: 1 }}
                      >
                        {showConfirmPassword ? (
                          <EyeOff size={16} />
                        ) : (
                          <Eye size={16} />
                        )}
                      </Button>
                    </InputGroup>
                    {registerForm.formState.errors.confirmPassword && (
                      <div className="invalid-feedback d-block">
                        {registerForm.formState.errors.confirmPassword.message}
                      </div>
                    )}
                  </Form.Group>

                  <Button
                    type="submit"
                    variant="primary"
                    className="w-100"
                    disabled={state.isLoading}
                  >
                    {state.isLoading ? (
                      <>
                        <Spinner
                          animation="border"
                          size="sm"
                          className="me-2"
                        />
                        Creating account...
                      </>
                    ) : (
                      "Create account"
                    )}
                  </Button>
                </Form>
              )}
            </Card.Body>
          </Card>

          {/* Footer */}
          <div className="text-center mt-4">
            <p className="small text-muted">
              By signing {isLogin ? "in" : "up"}, you agree to our{" "}
              <a href="#" className="text-decoration-none">
                Terms of Service
              </a>{" "}
              and{" "}
              <a href="#" className="text-decoration-none">
                Privacy Policy
              </a>
            </p>
          </div>
        </Col>
      </Row>
    </Container>
  );
};

export default Login;
