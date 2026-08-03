import { useState } from 'react'
import { z } from 'zod'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { AxiosError } from 'axios'
import { useNavigate } from '@tanstack/react-router'
import { Loader2, LogIn } from 'lucide-react'
import { toast } from 'sonner'
import { authApi } from '@/lib/api'
import { useAuthStore } from '@/stores/auth-store'
import { cn } from '@/lib/utils'
import { Button } from '@/components/ui/button'
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form'
import { Input } from '@/components/ui/input'
import { PasswordInput } from '@/components/password-input'

const formSchema = z.object({
  email: z.email({
    error: (iss) => (iss.input === '' ? 'Vui lòng nhập email.' : undefined),
  }),
  password: z.string().min(1, 'Vui lòng nhập mật khẩu.'),
})

interface UserAuthFormProps extends React.HTMLAttributes<HTMLFormElement> {
  redirectTo?: string
}

// Chỉ employer/admin/moderator được dùng web-admin — role candidate bị chặn (ADR-0008).
const ALLOWED_ROLES = ['employer', 'admin', 'moderator']

export function UserAuthForm({
  className,
  redirectTo,
  ...props
}: UserAuthFormProps) {
  const [isLoading, setIsLoading] = useState(false)
  const navigate = useNavigate()
  const { auth } = useAuthStore()

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      email: '',
      password: '',
    },
  })

  async function onSubmit(data: z.infer<typeof formSchema>) {
    setIsLoading(true)

    try {
      const login = await authApi.login(data.email, data.password)
      auth.setTokens(login.accessToken, login.refreshToken)

      const user = await authApi.me()

      if (!ALLOWED_ROLES.includes(user.role)) {
        auth.reset()
        toast.error('Tài khoản này không có quyền truy cập trang quản trị.')
        return
      }

      auth.setUser(user)
      toast.success(`Chào mừng trở lại, ${user.email}!`)
      navigate({ to: redirectTo || '/', replace: true })
    } catch (error) {
      if (error instanceof AxiosError && error.response?.status === 401) {
        toast.error('Email hoặc mật khẩu không đúng.')
      } else {
        toast.error('Đăng nhập không thành công.')
      }
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <Form {...form}>
      <form
        onSubmit={form.handleSubmit(onSubmit)}
        className={cn('grid gap-3', className)}
        {...props}
      >
        <FormField
          control={form.control}
          name='email'
          render={({ field }) => (
            <FormItem>
              <FormLabel>Email</FormLabel>
              <FormControl>
                <Input placeholder='name@example.com' {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name='password'
          render={({ field }) => (
            <FormItem className='relative'>
              <FormLabel>Mật khẩu</FormLabel>
              <FormControl>
                <PasswordInput placeholder='********' {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <Button className='mt-2' disabled={isLoading}>
          {isLoading ? <Loader2 className='animate-spin' /> : <LogIn />}
          Đăng nhập
        </Button>
      </form>
    </Form>
  )
}
