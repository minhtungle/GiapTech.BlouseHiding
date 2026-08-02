import { Link } from '@tanstack/react-router'
import { Plus } from 'lucide-react'
import { MOCK_ADMIN_JOBS, JOB_STATUS_LABEL } from '@/lib/mock-data'
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
import { ThemeSwitch } from '@/components/theme-switch'

const STATUS_BADGE_CLASS: Record<string, string> = {
  published: 'bg-accent-jade text-white',
  pending_payment: 'bg-amber-pending text-white',
  pending: 'bg-amber-pending text-white',
  rejected: 'bg-accent-seal text-white',
  suspended: 'bg-accent-seal text-white',
}

export function Jobs() {
  return (
    <>
      <Header>
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
                <TableHead>Gói</TableHead>
                <TableHead>Trạng thái</TableHead>
                <TableHead className='text-right'>Ứng viên</TableHead>
                <TableHead>Ngày đăng</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {MOCK_ADMIN_JOBS.map((job) => (
                <TableRow key={job.id}>
                  <TableCell className='font-medium'>{job.title}</TableCell>
                  <TableCell>{job.packageName}</TableCell>
                  <TableCell>
                    <Badge
                      className={STATUS_BADGE_CLASS[job.status] ?? ''}
                      variant={
                        STATUS_BADGE_CLASS[job.status] ? 'default' : 'outline'
                      }
                    >
                      {JOB_STATUS_LABEL[job.status]}
                    </Badge>
                  </TableCell>
                  <TableCell className='text-right'>
                    {job.applicantsCount > 0 ? (
                      <Link
                        to='/applications'
                        className='text-accent-jade hover:underline'
                      >
                        {job.applicantsCount}
                      </Link>
                    ) : (
                      '—'
                    )}
                  </TableCell>
                  <TableCell className='text-muted-foreground'>
                    {job.publishedAt ?? '—'}
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
