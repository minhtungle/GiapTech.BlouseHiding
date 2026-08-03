import { Link } from '@tanstack/react-router'
import { useQuery } from '@tanstack/react-query'
import { Briefcase, Wallet, Landmark, Plus } from 'lucide-react'
import { jobsApi, organizationsApi } from '@/lib/api'
import { useMyOrganization } from '@/hooks/use-my-organization'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { ConfigDrawer } from '@/components/config-drawer'
import { Header } from '@/components/layout/header'
import { Main } from '@/components/layout/main'
import { ProfileDropdown } from '@/components/profile-dropdown'
import { Search } from '@/components/search'
import { LanguageSwitcher } from '@/components/language-switcher'
import { ThemeSwitch } from '@/components/theme-switch'

const STATUS_LABEL: Record<string, string> = {
  Draft: 'Nháp',
  PendingPayment: 'Chờ thanh toán',
  Pending: 'Chờ duyệt',
  Published: 'Đang tuyển',
  Rejected: 'Bị từ chối',
  Expired: 'Hết hạn',
  Closed: 'Đã đóng',
  Suspended: 'Bị ẩn',
}

export function Dashboard() {
  const { organization } = useMyOrganization()

  const { data: jobs } = useQuery({
    queryKey: ['jobs', 'organization', organization?.id],
    queryFn: () => jobsApi.getOrganizationJobs(organization!.id),
    enabled: !!organization,
  })

  const { data: wallet } = useQuery({
    queryKey: ['credit-wallet', organization?.id],
    queryFn: () => organizationsApi.getCreditWallet(organization!.id),
    enabled: !!organization,
  })

  const publishedJobs = jobs?.filter((j) => j.status === 'Published') ?? []
  const pendingPaymentJobs = jobs?.filter((j) => j.status === 'PendingPayment') ?? []

  return (
    <>
      <Header>
        <div className='ms-2 font-medium text-muted-foreground'>
          {organization?.name ?? ''}
        </div>
        <div className='ms-auto flex items-center gap-2'>
          <Search />
          <LanguageSwitcher />
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

        <div className='mb-6 grid gap-4 sm:grid-cols-3'>
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
                / {jobs?.length ?? 0} tổng số tin
              </p>
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
                {wallet?.balance ?? 0}
              </div>
              <Link
                to='/credit'
                className='text-xs text-accent-jade hover:underline'
              >
                Xem chi tiết →
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
                {pendingPaymentJobs.length}
              </div>
              <p className='text-xs text-muted-foreground'>tin chờ đối soát</p>
            </CardContent>
          </Card>
        </div>

        <Card>
          <CardHeader>
            <CardTitle className='text-base'>Tin tuyển dụng</CardTitle>
          </CardHeader>
          <CardContent className='divide-y divide-border'>
            {jobs?.length === 0 && (
              <p className='py-3 text-sm text-muted-foreground'>
                Chưa có tin tuyển dụng nào.
              </p>
            )}
            {jobs?.map((job) => (
              <div
                key={job.id}
                className='flex items-center justify-between gap-4 py-3 first:pt-0 last:pb-0'
              >
                <div>
                  <p className='text-sm font-medium'>{job.title}</p>
                  <p className='text-xs text-muted-foreground'>
                    {job.specialtyName} · {job.locationName}
                  </p>
                </div>
                <Badge variant='outline'>
                  {STATUS_LABEL[job.status] ?? job.status}
                </Badge>
              </div>
            ))}
          </CardContent>
        </Card>
      </Main>
    </>
  )
}
