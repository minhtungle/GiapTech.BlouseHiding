import { useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Wallet } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { toast } from 'sonner'
import { organizationsApi, paymentsApi, type PaymentInstructions } from '@/lib/api'
import { useMyOrganization } from '@/hooks/use-my-organization'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { ConfigDrawer } from '@/components/config-drawer'
import { Header } from '@/components/layout/header'
import { Main } from '@/components/layout/main'
import { ProfileDropdown } from '@/components/profile-dropdown'
import { LanguageSwitcher } from '@/components/language-switcher'
import { ThemeSwitch } from '@/components/theme-switch'

const VND_PER_CREDIT = 1_000

function TopupDialog({
  open,
  onOpenChange,
  organizationId,
}: {
  open: boolean
  onOpenChange: (open: boolean) => void
  organizationId: string
}) {
  const { t } = useTranslation('credit')
  const [creditAmount, setCreditAmount] = useState('100')
  const [instructions, setInstructions] = useState<PaymentInstructions | null>(null)

  const mutation = useMutation({
    mutationFn: () => paymentsApi.createCreditTopupPayment(organizationId, Number(creditAmount)),
    onSuccess: (result) => setInstructions(result),
    onError: () => toast.error(t('topupDialog.createFailed')),
  })

  function handleClose(open: boolean) {
    if (!open) {
      setInstructions(null)
      setCreditAmount('100')
    }
    onOpenChange(open)
  }

  return (
    <Dialog open={open} onOpenChange={handleClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{t('topupDialog.title')}</DialogTitle>
        </DialogHeader>

        {!instructions ? (
          <div className='space-y-4'>
            <div className='space-y-1.5'>
              <Label>{t('topupDialog.amountLabel')}</Label>
              <Input
                type='number'
                min={1}
                value={creditAmount}
                onChange={(e) => setCreditAmount(e.target.value)}
              />
              <p className='text-xs text-muted-foreground'>
                {t('topupDialog.conversionHint', {
                  rate: VND_PER_CREDIT.toLocaleString('vi-VN'),
                  amount: (Number(creditAmount || 0) * VND_PER_CREDIT).toLocaleString('vi-VN'),
                })}
              </p>
            </div>
          </div>
        ) : (
          <div className='space-y-3'>
            <p className='text-sm text-muted-foreground'>
              {t('topupDialog.transferHint')}
            </p>
            <div className='rounded-md border p-4 text-sm'>
              <div className='flex justify-between py-1'>
                <span className='text-muted-foreground'>{t('topupDialog.amountToPayLabel')}</span>
                <span className='font-medium'>{instructions.amount.toLocaleString('vi-VN')}đ</span>
              </div>
              <div className='flex justify-between py-1'>
                <span className='text-muted-foreground'>{t('topupDialog.referenceLabel')}</span>
                <span className='font-mono font-medium'>{instructions.referenceCode}</span>
              </div>
            </div>
            <p className='text-xs text-muted-foreground'>
              {t('topupDialog.creditedHint')}
            </p>
          </div>
        )}

        <DialogFooter>
          <Button variant='outline' onClick={() => handleClose(false)}>
            {instructions ? t('topupDialog.close') : t('topupDialog.cancel')}
          </Button>
          {!instructions && (
            <Button
              disabled={!creditAmount || Number(creditAmount) <= 0 || mutation.isPending}
              onClick={() => mutation.mutate()}
            >
              {t('topupDialog.submit')}
            </Button>
          )}
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}

export function Credit() {
  const { t } = useTranslation('credit')
  const { organization } = useMyOrganization()
  const queryClient = useQueryClient()
  const [topupOpen, setTopupOpen] = useState(false)

  const { data: wallet } = useQuery({
    queryKey: ['credit-wallet', organization?.id],
    queryFn: () => organizationsApi.getCreditWallet(organization!.id),
    enabled: !!organization,
  })

  const { data: transactions } = useQuery({
    queryKey: ['credit-transactions', organization?.id],
    queryFn: () => organizationsApi.getCreditTransactions(organization!.id),
    enabled: !!organization,
  })

  function handleTopupOpenChange(open: boolean) {
    setTopupOpen(open)
    if (!open) {
      queryClient.invalidateQueries({ queryKey: ['credit-wallet', organization?.id] })
    }
  }

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
        <h1 className='mb-6 text-2xl font-semibold tracking-tight'>
          {t('pageTitle')}
        </h1>

        <div className='mb-6 grid gap-4 sm:grid-cols-[1fr_auto]'>
          <Card>
            <CardHeader className='flex flex-row items-center justify-between pb-2'>
              <CardTitle className='text-sm font-medium text-muted-foreground'>
                {t('currentBalance')}
              </CardTitle>
              <Wallet className='size-4 text-muted-foreground' />
            </CardHeader>
            <CardContent>
              <div className='text-3xl font-semibold'>
                {t('creditUnit', { count: wallet?.balance ?? 0 })}
              </div>
              <p className='mt-1 text-xs text-muted-foreground'>
                {t('balanceHint')}
              </p>
            </CardContent>
          </Card>
          <div className='flex flex-col items-center justify-center gap-1'>
            <Button size='lg' onClick={() => setTopupOpen(true)} disabled={!organization}>
              {t('topupButton')}
            </Button>
            <p className='text-center text-xs text-muted-foreground'>
              {t('topupHint')}
            </p>
          </div>
        </div>

        <Card>
          <CardHeader>
            <CardTitle className='text-base'>{t('transactionHistory')}</CardTitle>
          </CardHeader>
          <CardContent className='p-0'>
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>{t('colDescription')}</TableHead>
                  <TableHead className='text-right'>{t('colAmount')}</TableHead>
                  <TableHead>{t('colDate')}</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {transactions?.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={3} className='text-center text-muted-foreground'>
                      {t('noTransactions')}
                    </TableCell>
                  </TableRow>
                )}
                {transactions?.map((tx) => (
                  <TableRow key={tx.id}>
                    <TableCell>{t(`reason.${tx.reason}`, tx.reason)}</TableCell>
                    <TableCell className='text-right'>
                      <Badge
                        variant='outline'
                        className={
                          tx.amount > 0
                            ? 'border-accent-jade/40 text-accent-jade'
                            : 'border-accent-seal/40 text-accent-seal'
                        }
                      >
                        {tx.amount > 0 ? '+' : ''}
                        {tx.amount}
                      </Badge>
                    </TableCell>
                    <TableCell className='text-muted-foreground'>
                      {new Date(tx.createdAt).toLocaleDateString('vi-VN')}
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </CardContent>
        </Card>
      </Main>

      {organization && (
        <TopupDialog
          open={topupOpen}
          onOpenChange={handleTopupOpenChange}
          organizationId={organization.id}
        />
      )}
    </>
  )
}
