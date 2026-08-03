import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Landmark, X, Check } from 'lucide-react'
import { toast } from 'sonner'
import { opsApi } from '@/lib/api'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { ConfigDrawer } from '@/components/config-drawer'
import { Header } from '@/components/layout/header'
import { Main } from '@/components/layout/main'
import { ProfileDropdown } from '@/components/profile-dropdown'
import { ThemeSwitch } from '@/components/theme-switch'

const TYPE_LABEL: Record<string, string> = {
  JobPackage: 'Mua gói tin',
  CreditTopup: 'Nạp Credit',
}

export function OpsPayments() {
  const queryClient = useQueryClient()

  const { data: payments } = useQuery({
    queryKey: ['ops', 'payments'],
    queryFn: opsApi.getPendingPayments,
  })

  const confirm = useMutation({
    mutationFn: (paymentId: string) => opsApi.confirmPayment(paymentId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['ops', 'payments'] })
      toast.success('Đã xác nhận giao dịch.')
    },
    onError: () => toast.error('Xác nhận giao dịch không thành công.'),
  })

  const reject = useMutation({
    mutationFn: (paymentId: string) => opsApi.rejectPayment(paymentId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['ops', 'payments'] })
      toast.success('Đã đánh dấu không khớp.')
    },
    onError: () => toast.error('Xử lý giao dịch không thành công.'),
  })

  return (
    <>
      <Header>
        <div className='ms-auto flex items-center gap-2'>
          <ThemeSwitch />
          <ConfigDrawer />
          <ProfileDropdown />
        </div>
      </Header>

      <Main>
        <p className='text-xs font-medium text-muted-foreground'>Vận hành</p>
        <h1 className='mb-1 text-2xl font-semibold tracking-tight'>
          Đối soát thanh toán thủ công
        </h1>
        <p className='mb-6 text-sm text-muted-foreground'>
          Đối chiếu với sao kê ngân hàng theo mã tham chiếu trước khi xác nhận.
        </p>

        {payments?.length === 0 && (
          <p className='text-sm text-muted-foreground'>Không có giao dịch nào chờ đối soát.</p>
        )}

        <div className='space-y-3'>
          {payments?.map((payment) => (
            <Card key={payment.id}>
              <CardHeader className='flex flex-row items-center justify-between'>
                <div className='flex items-center gap-3'>
                  <Landmark className='size-5 text-amber-pending' />
                  <div>
                    <CardTitle className='text-base'>
                      {payment.organizationName}
                    </CardTitle>
                    <p className='text-sm text-muted-foreground'>
                      {TYPE_LABEL[payment.type] ?? payment.type} ·{' '}
                      {payment.amount.toLocaleString('vi-VN')}đ
                    </p>
                  </div>
                </div>
                <Badge variant='outline' className='font-mono'>
                  {payment.referenceCode}
                </Badge>
              </CardHeader>
              <CardContent className='flex items-center justify-between'>
                <p className='text-xs text-muted-foreground'>
                  Tạo lúc {new Date(payment.createdAt).toLocaleString('vi-VN')} — kiểm tra sao kê có
                  giao dịch cùng mã tham chiếu và đúng số tiền
                </p>
                <div className='flex gap-2'>
                  <Button
                    size='sm'
                    variant='destructive'
                    disabled={reject.isPending}
                    onClick={() => reject.mutate(payment.id)}
                  >
                    <X />
                    Không khớp
                  </Button>
                  <Button size='sm' disabled={confirm.isPending} onClick={() => confirm.mutate(payment.id)}>
                    <Check />
                    Xác nhận
                  </Button>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      </Main>
    </>
  )
}
