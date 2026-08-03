import { Link } from '@tanstack/react-router'
import { useQuery } from '@tanstack/react-query'
import { Plus } from 'lucide-react'
import { jobsApi } from '@/lib/api'
import { useMyOrganization } from '@/hooks/use-my-organization'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
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

const STATUS_BADGE_CLASS: Record<string, string> = {
  Published: 'bg-accent-jade text-white',
  PendingPayment: 'bg-amber-pending text-white',
  Pending: 'bg-amber-pending text-white',
  Rejected: 'bg-accent-seal text-white',
  Suspended: 'bg-accent-seal text-white',
}

export function Jobs() {
  const { organization } = useMyOrganization()

  const { data: jobs } = useQuery({
    queryKey: ['jobs', 'organization', organization?.id],
    queryFn: () => jobsApi.getOrganizationJobs(organization!.id),
    enabled: !!organization,
  })

  return (
    <>
      <Header>
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
              Tin tuyển dụng
            </h1>
          </div>
          <Button asChild>
            <Link to='/jobs/new'>
              <Plus />
              Đăng tin mới
            </Link>
          </Button>
        </div>

        <div className='rounded-md border'>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Tin tuyển dụng</TableHead>
                <TableHead>Chuyên khoa · Địa điểm</TableHead>
                <TableHead>Trạng thái</TableHead>
                <TableHead>Ứng viên</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {jobs?.map((job) => (
                <TableRow key={job.id}>
                  <TableCell className='font-medium'>{job.title}</TableCell>
                  <TableCell className='text-muted-foreground'>
                    {job.specialtyName} · {job.locationName}
                  </TableCell>
                  <TableCell>
                    <Badge
                      className={STATUS_BADGE_CLASS[job.status] ?? ''}
                      variant={
                        STATUS_BADGE_CLASS[job.status] ? 'default' : 'outline'
                      }
                    >
                      {STATUS_LABEL[job.status] ?? job.status}
                    </Badge>
                  </TableCell>
                  <TableCell>
                    <Link
                      to='/applications/$jobId'
                      params={{ jobId: job.id }}
                      className='text-accent-jade hover:underline'
                    >
                      Xem ATS →
                    </Link>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </div>
      </Main>
    </>
  )
}
