import { Link } from '@tanstack/react-router'
import {
  Briefcase,
  Users,
  Wallet,
  Landmark,
  Plus,
  MessageSquare,
} from 'lucide-react'
import {
  MOCK_ADMIN_JOBS,
  MOCK_APPLICATIONS,
  MOCK_CREDIT_WALLET,
  MOCK_PAYMENT_QUEUE,
  JOB_STATUS_LABEL,
} from '@/lib/mock-data'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { ConfigDrawer } from '@/components/config-drawer'
import { Header } from '@/components/layout/header'
import { Main } from '@/components/layout/main'
import { ProfileDropdown } from '@/components/profile-dropdown'
import { Search } from '@/components/search'
import { ThemeSwitch } from '@/components/theme-switch'

const publishedJobs = MOCK_ADMIN_JOBS.filter((j) => j.status === 'published')
const newApplications = MOCK_APPLICATIONS.filter((a) => a.stage === 'moi')

export function Dashboard() {
  return (
    <>
      <Header>
        <div className='ms-2 font-medium text-muted-foreground'>
          Bệnh viện Đa khoa Tâm Đức
        </div>
        <div className='ms-auto flex items-center gap-2'>
          <Search />
          <ThemeSwitch />
          <ConfigDrawer />
          <ProfileDropdown />
        </div>
      </Header>

      <Main>
        <div className='mb-6 flex items-center justify-between'>
          <div>
            <p className='text-xs font-medium text-muted-foreground'>
              Admin — Nhà tuyển dụng
            </p>
            <h1 className='text-2xl font-semibold tracking-tight'>
              Dashboard
            </h1>
          </div>
          <Button asChild>
            <Link to='/jobs/new'>
              <Plus />
              Đăng tin mới
            </Link>
          </Button>
        </div>

        <div className='mb-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-4'>
          <Card>
            <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
              <CardTitle className='text-sm font-medium text-muted-foreground'>
                Tin đang tuyển
              </CardTitle>
              <Briefcase className='size-4 text-muted-foreground' />
            </CardHeader>
            <CardContent>
              <div className='text-2xl font-semibold'>
                {publishedJobs.length}
              </div>
              <p className='text-xs text-muted-foreground'>
                / {MOCK_ADMIN_JOBS.length} tổng số tin
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
              <CardTitle className='text-sm font-medium text-muted-foreground'>
                Ứng viên mới
              </CardTitle>
              <Users className='size-4 text-muted-foreground' />
            </CardHeader>
            <CardContent>
              <div className='text-2xl font-semibold'>
                {newApplications.length}
              </div>
              <p className='text-xs text-muted-foreground'>chưa xem</p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
              <CardTitle className='text-sm font-medium text-muted-foreground'>
                Số dư Credit
              </CardTitle>
              <Wallet className='size-4 text-muted-foreground' />
            </CardHeader>
            <CardContent>
              <div className='text-2xl font-semibold'>
                {MOCK_CREDIT_WALLET.balance}
              </div>
              <Link
                to='/credit'
                className='text-xs text-accent-jade hover:underline'
              >
                Nạp thêm →
              </Link>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
              <CardTitle className='text-sm font-medium text-muted-foreground'>
                Chờ thanh toán
              </CardTitle>
              <Landmark className='size-4 text-amber-pending' />
            </CardHeader>
            <CardContent>
              <div className='text-2xl font-semibold'>
                {
                  MOCK_ADMIN_JOBS.filter((j) => j.status === 'pending_payment')
                    .length
                }
              </div>
              <p className='text-xs text-muted-foreground'>tin chờ đối soát</p>
            </CardContent>
          </Card>
        </div>

        <div className='grid gap-4 lg:grid-cols-2'>
          <Card>
            <CardHeader>
              <CardTitle className='text-base'>Tin tuyển dụng</CardTitle>
            </CardHeader>
            <CardContent className='divide-y divide-border'>
              {MOCK_ADMIN_JOBS.map((job) => (
                <div
                  key={job.id}
                  className='flex items-center justify-between gap-4 py-3 first:pt-0 last:pb-0'
                >
                  <div>
                    <p className='text-sm font-medium'>{job.title}</p>
                    <p className='text-xs text-muted-foreground'>
                      Gói {job.packageName} · {job.applicantsCount} ứng viên
                    </p>
                  </div>
                  <Badge variant='outline'>
                    {JOB_STATUS_LABEL[job.status]}
                  </Badge>
                </div>
              ))}
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle className='text-base'>Cần xử lý</CardTitle>
            </CardHeader>
            <CardContent className='divide-y divide-border'>
              <div className='flex items-center justify-between gap-4 py-3 first:pt-0'>
                <div className='flex items-center gap-2'>
                  <Users className='size-4 text-muted-foreground' />
                  <span className='text-sm'>Ứng viên mới cần xem</span>
                </div>
                <Badge className='bg-accent-seal text-white'>
                  {newApplications.length}
                </Badge>
              </div>
              <div className='flex items-center justify-between gap-4 py-3'>
                <div className='flex items-center gap-2'>
                  <MessageSquare className='size-4 text-muted-foreground' />
                  <span className='text-sm'>Tin nhắn chưa đọc</span>
                </div>
                <Badge className='bg-accent-seal text-white'>2</Badge>
              </div>
              <div className='flex items-center justify-between gap-4 py-3 last:pb-0'>
                <div className='flex items-center gap-2'>
                  <Landmark className='size-4 text-muted-foreground' />
                  <span className='text-sm'>Chuyển khoản chờ đối soát</span>
                </div>
                <Badge className='bg-amber-pending text-white'>
                  {MOCK_PAYMENT_QUEUE.length}
                </Badge>
              </div>
            </CardContent>
          </Card>
        </div>
      </Main>
    </>
  )
}
