import React, {
  createContext,
  useContext,
  useReducer,
  ReactNode,
  useCallback,
} from "react";
import {
  TaskState,
  TaskAction,
  Task,
  CreateTaskRequest,
  UpdateTaskRequest,
  PatchTaskRequest,
  TaskFilters,
} from "../types/task";
import { taskService } from "../services/taskService";
import toast from "react-hot-toast";

const initialState: TaskState = {
  tasks: [],
  isLoading: false,
  error: null,
  filters: {
    status: "All",
    search: "",
  },
};

const taskReducer = (state: TaskState, action: TaskAction): TaskState => {
  switch (action.type) {
    case "TASKS_LOADING":
      return {
        ...state,
        isLoading: true,
        error: null,
      };
    case "TASKS_SUCCESS":
      return {
        ...state,
        tasks: action.payload,
        isLoading: false,
        error: null,
      };
    case "TASKS_FAILURE":
      return {
        ...state,
        isLoading: false,
        error: action.payload,
      };
    case "TASK_ADD":
      return {
        ...state,
        tasks: [...state.tasks, action.payload],
      };
    case "TASK_UPDATE":
      return {
        ...state,
        tasks: state.tasks.map((task) =>
          task.id === action.payload.id ? action.payload : task
        ),
      };
    case "TASK_DELETE":
      return {
        ...state,
        tasks: state.tasks.filter((task) => task.id !== action.payload),
      };
    case "TASKS_SET_FILTER":
      return {
        ...state,
        filters: { ...state.filters, ...action.payload },
      };
    case "TASKS_CLEAR_ERROR":
      return {
        ...state,
        error: null,
      };
    default:
      return state;
  }
};

interface TaskContextType {
  state: TaskState;
  fetchTasks: () => Promise<void>;
  createTask: (taskData: CreateTaskRequest) => Promise<void>;
  updateTask: (id: number, taskData: UpdateTaskRequest) => Promise<void>;
  patchTask: (id: number, taskData: PatchTaskRequest) => Promise<void>;
  deleteTask: (id: number) => Promise<void>;
  setFilters: (filters: Partial<TaskFilters>) => void;
  clearError: () => void;
  getFilteredTasks: () => Task[];
}

const TaskContext = createContext<TaskContextType | undefined>(undefined);

interface TaskProviderProps {
  children: ReactNode;
}

export const TaskProvider: React.FC<TaskProviderProps> = ({ children }) => {
  const [state, dispatch] = useReducer(taskReducer, initialState);

  const fetchTasks = useCallback(async (): Promise<void> => {
    try {
      dispatch({ type: "TASKS_LOADING" });
      const tasks = await taskService.getTasks();
      dispatch({ type: "TASKS_SUCCESS", payload: tasks });
    } catch (error) {
      const errorMessage =
        error instanceof Error ? error.message : "Failed to fetch tasks";
      dispatch({ type: "TASKS_FAILURE", payload: errorMessage });
      toast.error(errorMessage);
    }
  }, []);

  const createTask = async (taskData: CreateTaskRequest): Promise<void> => {
    try {
      const newTask = await taskService.createTask(taskData);
      dispatch({ type: "TASK_ADD", payload: newTask });
      toast.success("Task created successfully!");
    } catch (error) {
      const errorMessage =
        error instanceof Error ? error.message : "Failed to create task";
      toast.error(errorMessage);
      throw error;
    }
  };

  const updateTask = async (
    id: number,
    taskData: UpdateTaskRequest
  ): Promise<void> => {
    try {
      const updatedTask = await taskService.updateTask(id, taskData);
      dispatch({ type: "TASK_UPDATE", payload: updatedTask });
      toast.success("Task updated successfully!");
    } catch (error) {
      const errorMessage =
        error instanceof Error ? error.message : "Failed to update task";
      toast.error(errorMessage);
      throw error;
    }
  };

  const patchTask = async (
    id: number,
    taskData: PatchTaskRequest
  ): Promise<void> => {
    try {
      const updatedTask = await taskService.patchTask(id, taskData);
      dispatch({ type: "TASK_UPDATE", payload: updatedTask });
      toast.success("Task updated successfully!");
    } catch (error) {
      const errorMessage =
        error instanceof Error ? error.message : "Failed to update task";
      toast.error(errorMessage);
      throw error;
    }
  };

  const deleteTask = async (id: number): Promise<void> => {
    try {
      await taskService.deleteTask(id);
      dispatch({ type: "TASK_DELETE", payload: id });
      toast.success("Task deleted successfully!");
    } catch (error) {
      const errorMessage =
        error instanceof Error ? error.message : "Failed to delete task";
      toast.error(errorMessage);
      throw error;
    }
  };

  const setFilters = (filters: Partial<TaskFilters>): void => {
    dispatch({ type: "TASKS_SET_FILTER", payload: filters });
  };

  const clearError = (): void => {
    dispatch({ type: "TASKS_CLEAR_ERROR" });
  };

  const getFilteredTasks = (): Task[] => {
    let filtered = state.tasks;

    // Filter by status
    if (state.filters.status !== "All") {
      filtered = filtered.filter(
        (task) => task.status === state.filters.status
      );
    }

    // Filter by search
    if (state.filters.search) {
      const searchLower = state.filters.search.toLowerCase();
      filtered = filtered.filter(
        (task) =>
          task.title.toLowerCase().includes(searchLower) ||
          task.description.toLowerCase().includes(searchLower)
      );
    }

    return filtered;
  };

  const value: TaskContextType = {
    state,
    fetchTasks,
    createTask,
    updateTask,
    patchTask,
    deleteTask,
    setFilters,
    clearError,
    getFilteredTasks,
  };

  return <TaskContext.Provider value={value}>{children}</TaskContext.Provider>;
};

export const useTasks = (): TaskContextType => {
  const context = useContext(TaskContext);
  if (context === undefined) {
    throw new Error("useTasks must be used within a TaskProvider");
  }
  return context;
};
