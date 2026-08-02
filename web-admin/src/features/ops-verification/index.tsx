import { ShieldCheck, ShieldX } from 'lucide-react'
import { MOCK_LICENSE_QUEUE, MOCK_ORG_QUEUE } from '@/lib/mock-data'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { ConfigDrawer } from '@/components/config-drawer'
import { Header } from '@/components/layout/header'
import { Main } from '@/components/layout/main'
import { ProfileDropdown } from '@/components/profile-dropdown'
import { ThemeSwitch } from '@/components/theme-switch'

export function OpsVerification() {
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
        <h1 className='mb-6 text-2xl font-semibold tracking-tight'>
          Duyệt CCHN &amp; tổ chức
        </h1>

        <Tabs defaultValue='license'>
          <TabsList>
            <TabsTrigger value='license'>
              CCHN ({MOCK_LICENSE_QUEUE.length})
            </TabsTrigger>
            <TabsTrigger value='org'>
              Tổ chức ({MOCK_ORG_QUEUE.length})
            </TabsTrigger>
          </TabsList>

          <TabsContent value='license' className='space-y-3'>
            {MOCK_LICENSE_QUEUE.map((item) => (
              <Card key={item.id}>
                <CardHeader className='flex flex-row items-center justify-between'>
                  <div>
                    <CardTitle className='text-base'>
                      {item.candidateName}
                    </CardTitle>
                    <p className='text-sm text-muted-foreground'>
                      {item.specialty} · Số CCHN {item.licenseNumber}
                    </p>
                  </div>
                  <Badge className='bg-amber-pending text-white'>
                    Chờ duyệt
                  </Badge>
                </CardHeader>
                <CardContent className='flex items-center justify-between'>
                  <p className='text-xs text-muted-foreground'>
                    Nộp ngày {item.submittedAt} — xem ảnh CCHN đã tải lên
                  </p>
                  <div className='flex gap-2'>
                    <Button size='sm' variant='outline'>
                      Xem ảnh CCHN
                    </Button>
                    <Button size='sm' variant='destructive'>
                      <ShieldX />
                      Từ chối
                    </Button>
                    <Button size='sm'>
                      <ShieldCheck />
                      Duyệt
                    </Button>
                  </div>
                </CardContent>
              </Card>
            ))}
          </TabsContent>

          <TabsContent value='org' className='space-y-3'>
            {MOCK_ORG_QUEUE.map((item) => (
              <Card key={item.id}>
                <CardHeader className='flex flex-row items-center justify-between'>
                  <div>
                    <CardTitle className='text-base'>{item.name}</CardTitle>
                    <p className='text-sm text-muted-foreground'>
                      {item.type}
                    </p>
                  </div>
                  <Badge className='bg-amber-pending text-white'>
                    Chờ duyệt
                  </Badge>
                </CardHeader>
                <CardContent className='flex items-center justify-between'>
                  <p className='text-xs text-muted-foreground'>
                    Nộp ngày {item.submittedAt} — xem giấy phép hoạt động
                  </p>
                  <div className='flex gap-2'>
                    <Button size='sm' variant='outline'>
                      Xem giấy phép
                    </Button>
                    <Button size='sm' variant='destructive'>
                      <ShieldX />
                      Từ chối
                    </Button>
                    <Button size='sm'>
                      <ShieldCheck />
                      Duyệt
                    </Button>
                  </div>
                </CardContent>
              </Card>
            ))}
          </TabsContent>
        </Tabs>
      </Main>
    </>
  )
}
