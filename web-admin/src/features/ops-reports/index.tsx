import { Flag } from 'lucide-react'
import { MOCK_REPORT_QUEUE } from '@/lib/mock-data'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { ConfigDrawer } from '@/components/config-drawer'
import { Header } from '@/components/layout/header'
import { Main } from '@/components/layout/main'
import { ProfileDropdown } from '@/components/profile-dropdown'
import { LanguageSwitcher } from '@/components/language-switcher'
import { ThemeSwitch } from '@/components/theme-switch'

const TARGET_LABEL: Record<string, string> = {
  job: 'Tin tuyển dụng',
  organization: 'Cơ sở y tế',
  profile: 'Hồ sơ ứng viên',
  message: 'Tin nhắn',
}

export function OpsReports() {
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
        <p className='text-xs font-medium text-muted-foreground'>Vận hành</p>
        <h1 className='mb-6 text-2xl font-semibold tracking-tight'>
          Xử lý báo cáo vi phạm
        </h1>

        <div className='space-y-3'>
          {MOCK_REPORT_QUEUE.map((report) => (
            <Card key={report.id}>
              <CardHeader className='flex flex-row items-center justify-between'>
                <div className='flex items-center gap-3'>
                  <Flag className='size-5 text-accent-seal' />
                  <div>
                    <CardTitle className='text-base'>
                      {report.targetLabel}
                    </CardTitle>
                    <p className='text-sm text-muted-foreground'>
                      {report.reason}
                    </p>
                  </div>
                </div>
                <Badge variant='outline'>
                  {TARGET_LABEL[report.targetType]}
                </Badge>
              </CardHeader>
              <CardContent className='flex items-center justify-between'>
                <p className='text-xs text-muted-foreground'>
                  Báo cáo lúc {report.reportedAt}
                </p>
                <div className='flex gap-2'>
                  <Button size='sm' variant='outline'>
                    Bỏ qua
                  </Button>
                  <Button size='sm' variant='outline'>
                    Nhắc nhở
                  </Button>
                  <Button size='sm' variant='destructive'>
                    Gỡ / Khóa
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
