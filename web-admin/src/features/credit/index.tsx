import { Wallet } from 'lucide-react'
import { MOCK_CREDIT_WALLET } from '@/lib/mock-data'
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
  purchase: 'Nạp Credit',
  unlock_profile: 'Mở hồ sơ ứng viên',
  refund: 'Hoàn Credit (tranh chấp)',
  bonus: 'Thưởng',
}

export function Credit() {
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
                {MOCK_CREDIT_WALLET.balance} Credit
              </div>
              <p className='mt-1 text-xs text-muted-foreground'>
                1 Credit = 1 lượt mở hồ sơ ứng viên
              </p>
            </CardContent>
          </Card>
          <div className='flex items-center'>
            <Button size='lg'>Nạp thêm Credit</Button>
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
                {MOCK_CREDIT_WALLET.transactions.map((tx) => (
                  <TableRow key={tx.id}>
                    <TableCell>{REASON_LABEL[tx.reason]}</TableCell>
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
                      {tx.createdAt}
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
