import { useQuery } from '@tanstack/react-query'
import { Wallet } from 'lucide-react'
import { organizationsApi } from '@/lib/api'
import { useMyOrganization } from '@/hooks/use-my-organization'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
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
import { ThemeSwitch } from '@/components/theme-switch'

const REASON_LABEL: Record<string, string> = {
  Purchase: 'Nạp Credit',
  UnlockProfile: 'Mở hồ sơ ứng viên',
  Refund: 'Hoàn Credit (tranh chấp)',
  Bonus: 'Thưởng',
}

export function Credit() {
  const { organization } = useMyOrganization()

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
        <h1 className='mb-6 text-2xl font-semibold tracking-tight'>
          Ví Credit
        </h1>

        <div className='mb-6 grid gap-4 sm:grid-cols-[1fr_auto]'>
          <Card>
            <CardHeader className='flex flex-row items-center justify-between pb-2'>
              <CardTitle className='text-sm font-medium text-muted-foreground'>
                Số dư hiện tại
              </CardTitle>
              <Wallet className='size-4 text-muted-foreground' />
            </CardHeader>
            <CardContent>
              <div className='text-3xl font-semibold'>
                {wallet?.balance ?? 0} Credit
              </div>
              <p className='mt-1 text-xs text-muted-foreground'>
                1 Credit = 1 lượt mở hồ sơ ứng viên
              </p>
            </CardContent>
          </Card>
          <div className='flex flex-col items-center justify-center gap-1'>
            <Button size='lg' disabled title='Đang chờ nối cổng thanh toán'>
              Nạp thêm Credit
            </Button>
            <p className='text-center text-xs text-muted-foreground'>
              Liên hệ Vận hành để nạp Credit trong lúc chờ hoàn thiện
            </p>
          </div>
        </div>

        <Card>
          <CardHeader>
            <CardTitle className='text-base'>Lịch sử giao dịch</CardTitle>
          </CardHeader>
          <CardContent className='p-0'>
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Nội dung</TableHead>
                  <TableHead className='text-right'>Số Credit</TableHead>
                  <TableHead>Ngày</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {transactions?.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={3} className='text-center text-muted-foreground'>
                      Chưa có giao dịch nào.
                    </TableCell>
                  </TableRow>
                )}
                {transactions?.map((tx) => (
                  <TableRow key={tx.id}>
                    <TableCell>{REASON_LABEL[tx.reason] ?? tx.reason}</TableCell>
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
    </>
  )
}
