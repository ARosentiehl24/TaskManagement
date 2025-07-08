export const API_ENDPOINTS = {
  AUTH: {
    LOGIN: "/auth/login",
    REGISTER: "/auth/register",
    PROFILE: "/auth/profile",
  },
  TASKS: {
    BASE: "/tasks",
    BY_ID: (id: number) => `/tasks/${id}`,
  },
} as const;

export const LOCAL_STORAGE_KEYS = {
  AUTH_TOKEN: "authToken",
  AUTH_USER: "authUser",
  REMEMBER_ME: "rememberMe",
} as const;

export const TASK_STATUS_VALUES = {
  PENDING: 0,
  IN_PROGRESS: 1,
  COMPLETED: 2,
} as const;

export const FORM_VALIDATION = {
  USERNAME: {
    MIN_LENGTH: 3,
    MAX_LENGTH: 50,
    PATTERN: /^[a-zA-Z0-9_]+$/,
  },
  PASSWORD: {
    MIN_LENGTH: 8,
    PATTERN: {
      UPPERCASE: /[A-Z]/,
      LOWERCASE: /[a-z]/,
      NUMBER: /\d/,
    },
  },
  TASK: {
    TITLE_MAX_LENGTH: 200,
    DESCRIPTION_MAX_LENGTH: 1000,
  },
} as const;

export const UI_CONSTANTS = {
  DEBOUNCE_DELAY: 300,
  TOAST_DURATION: 4000,
  MODAL_ANIMATION_DURATION: 200,
} as const;
