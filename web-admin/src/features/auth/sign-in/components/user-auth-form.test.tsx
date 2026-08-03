import { beforeEach, describe, expect, it, vi } from 'vitest'
import { render, type RenderResult } from 'vitest-browser-react'
import { type Locator, userEvent } from 'vitest/browser'
import { UserAuthForm } from './user-auth-form'

const navigate = vi.fn()
const setUserMock = vi.fn()
const setTokensMock = vi.fn()
const resetMock = vi.fn()
const loginMock = vi.fn()
const meMock = vi.fn()

vi.mock('@/stores/auth-store', () => ({
  useAuthStore: () => ({
    auth: {
      setUser: setUserMock,
      setTokens: setTokensMock,
      reset: resetMock,
    },
  }),
}))

vi.mock('@/lib/api', () => ({
  authApi: {
    login: (...args: unknown[]) => loginMock(...args),
    me: (...args: unknown[]) => meMock(...args),
  },
}))

vi.mock('@tanstack/react-router', async (importOriginal) => {
  const actual = await importOriginal<typeof import('@tanstack/react-router')>()
  return {
    ...actual,
    useNavigate: () => navigate,
  }
})

describe('UserAuthForm', () => {
  describe('Rendering without redirectTo', () => {
    let screen: RenderResult
    let emailInput: Locator
    let passwordInput: Locator
    let signInButton: Locator

    beforeEach(async () => {
      vi.clearAllMocks()
      loginMock.mockResolvedValue({ accessToken: 'access', refreshToken: 'refresh' })
      meMock.mockResolvedValue({
        id: 'user-1',
        email: 'a@b.com',
        role: 'employer',
        status: 'Active',
        emailVerified: true,
      })

      screen = await render(<UserAuthForm />)
      emailInput = screen.getByRole('textbox', { name: /^Email$/i })
      passwordInput = screen.getByLabelText(/^Mật khẩu$/i)
      signInButton = screen.getByRole('button', { name: /^Đăng nhập$/i })
    })

    it('renders fields and submit button', async () => {
      await expect.element(emailInput).toBeInTheDocument()
      await expect.element(passwordInput).toBeInTheDocument()
      await expect.element(signInButton).toBeInTheDocument()
    })

    it('shows validation message when submitting empty form', async () => {
      await userEvent.click(signInButton)

      await expect
        .element(screen.getByText('Vui lòng nhập email.'))
        .toBeInTheDocument()
    })

    it('authenticates and navigates to default route on success', async () => {
      await userEvent.fill(emailInput, 'a@b.com')
      await userEvent.fill(passwordInput, '1234567')

      await userEvent.click(signInButton)

      await vi.waitFor(() => expect(setTokensMock).toHaveBeenCalledOnce())
      expect(setTokensMock).toHaveBeenCalledWith('access', 'refresh')
      expect(setUserMock).toHaveBeenCalledWith(
        expect.objectContaining({ email: 'a@b.com', role: 'employer' })
      )

      await vi.waitFor(() =>
        expect(navigate).toHaveBeenCalledWith({ to: '/', replace: true })
      )
    })
  })

  it('navigates to redirectTo when provided', async () => {
    vi.clearAllMocks()
    loginMock.mockResolvedValue({ accessToken: 'access', refreshToken: 'refresh' })
    meMock.mockResolvedValue({
      id: 'user-1',
      email: 'a@b.com',
      role: 'employer',
      status: 'Active',
      emailVerified: true,
    })

    const { getByRole, getByLabelText } = await render(
      <UserAuthForm redirectTo='/settings' />
    )

    await userEvent.fill(getByRole('textbox', { name: /Email/i }), 'a@b.com')
    await userEvent.fill(getByLabelText('Mật khẩu'), '1234567')

    await userEvent.click(getByRole('button', { name: /Đăng nhập/i }))

    await vi.waitFor(() => expect(setTokensMock).toHaveBeenCalledOnce())

    await vi.waitFor(() =>
      expect(navigate).toHaveBeenCalledWith({
        to: '/settings',
        replace: true,
      })
    )
  })
})
