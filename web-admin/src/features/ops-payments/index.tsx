import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Landmark, X, Check } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { toast } from 'sonner'
import { opsApi } from '@/lib/api'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { ConfigDrawer } from '@/components/config-drawer'
import { Header } from '@/components/layout/header'
import { Main } from '@/components/layout/main'
import { ProfileDropdown } from '@/components/profile-dropdown'
import { LanguageSwitcher } from '@/components/language-switcher'
import { ThemeSwitch } from '@/components/theme-switch'

export function OpsPayments() {
  const { t } = useTranslation('ops')
  const queryClient = useQueryClient()

  const { data: payments } = useQuery({
    queryKey: ['ops', 'payments'],
    queryFn: opsApi.getPendingPayments,
  })

  const confirm = useMutation({
    mutationFn: (paymentId: string) => opsApi.confirmPayment(paymentId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['ops', 'payments'] })
      toast.success(t('payments.confirmSuccess'))
    },
    onError: () => toast.error(t('payments.confirmFailed')),
  })

  const reject = useMutation({
    mutationFn: (paymentId: string) => opsApi.rejectPayment(paymentId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['ops', 'payments'] })
      toast.success(t('payments.rejectSuccess'))
    },
    onError: () => toast.error(t('payments.rejectFailed')),
  })

  return (
    <>
      <Header>
        <div className='ms-auto flex items-center gap-2'>
          <LanguageSwitcher />
          <ThemeSwitch />
          <ConfigDrawer />
          <ProfileDropdown />
        </div>
      </Header>

      <Main>
        <p className='text-xs font-medium text-muted-foreground'>{t('breadcrumb')}</p>
        <h1 className='mb-1 text-2xl font-semibold tracking-tight'>
          {t('payments.pageTitle')}
        </h1>
        <p className='mb-6 text-sm text-muted-foreground'>
          {t('payments.pageDescription')}
        </p>

        {payments?.length === 0 && (
          <p className='text-sm text-muted-foreground'>{t('payments.noPayments')}</p>
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
                      {t(`payments.type.${payment.type}`, payment.type)} ·{' '}
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
                  {t('payments.createdAt', {
                    date: new Date(payment.createdAt).toLocaleString('vi-VN'),
                  })}
                </p>
                <div className='flex gap-2'>
                  <Button
                    size='sm'
                    variant='destructive'
                    disabled={reject.isPending}
                    onClick={() => reject.mutate(payment.id)}
                  >
                    <X />
                    {t('payments.reject')}
                  </Button>
                  <Button size='sm' disabled={confirm.isPending} onClick={() => confirm.mutate(payment.id)}>
                    <Check />
                    {t('payments.confirm')}
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
