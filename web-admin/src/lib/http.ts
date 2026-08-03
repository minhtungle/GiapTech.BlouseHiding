import axios from 'axios'
import { useAuthStore } from '@/stores/auth-store'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5100/api/v1'

// Axios instance dùng chung cho mọi gọi API cần auth — app này chỉ tiếng Việt (ADR-0008 mục 5),
// luôn gửi Accept-Language: vi. Gắn token qua interceptor thay vì truyền tay mỗi lần gọi.
export const http = axios.create({
  baseURL: API_BASE_URL,
  headers: { 'Accept-Language': 'vi' },
})

http.interceptors.request.use((config) => {
  const token = useAuthStore.getState().auth.accessToken
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

let refreshPromise: Promise<string | null> | null = null

async function refreshAccessToken(): Promise<string | null> {
  const { auth } = useAuthStore.getState()
  if (!auth.refreshToken) return null

  try {
    const res = await axios.post(`${API_BASE_URL}/auth/refresh`, {
      refreshToken: auth.refreshToken,
    })
    auth.setTokens(res.data.accessToken, res.data.refreshToken)
    return res.data.accessToken as string
  } catch {
    auth.reset()
    return null
  }
}

http.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true

      refreshPromise ??= refreshAccessToken()
      const newToken = await refreshPromise
      refreshPromise = null

      if (newToken) {
        originalRequest.headers.Authorization = `Bearer ${newToken}`
        return http(originalRequest)
      }
    }
    return Promise.reject(error)
  }
)
