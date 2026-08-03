import { create } from 'zustand'
import { getCookie, setCookie, removeCookie } from '@/lib/cookies'

const ACCESS_TOKEN = 'access_token'
const REFRESH_TOKEN = 'refresh_token'

// Khớp GET /users/me — role backend là employer/admin/moderator (không phải NTD/Ops, xem
// docs/kien-truc/THUAT-NGU.md).
export interface AuthUser {
  id: string
  email: string
  role: string
  status: string
  emailVerified: boolean
}

interface AuthState {
  auth: {
    user: AuthUser | null
    setUser: (user: AuthUser | null) => void
    accessToken: string
    refreshToken: string
    setTokens: (accessToken: string, refreshToken: string) => void
    reset: () => void
  }
}

export const useAuthStore = create<AuthState>()((set) => {
  const initAccessToken = getCookie(ACCESS_TOKEN) ?? ''
  const initRefreshToken = getCookie(REFRESH_TOKEN) ?? ''

  return {
    auth: {
      user: null,
      setUser: (user) =>
        set((state) => ({ ...state, auth: { ...state.auth, user } })),
      accessToken: initAccessToken,
      refreshToken: initRefreshToken,
      setTokens: (accessToken, refreshToken) =>
        set((state) => {
          setCookie(ACCESS_TOKEN, accessToken)
          setCookie(REFRESH_TOKEN, refreshToken)
          return { ...state, auth: { ...state.auth, accessToken, refreshToken } }
        }),
      reset: () =>
        set((state) => {
          removeCookie(ACCESS_TOKEN)
          removeCookie(REFRESH_TOKEN)
          return {
            ...state,
            auth: { ...state.auth, user: null, accessToken: '', refreshToken: '' },
          }
        }),
    },
  }
})
