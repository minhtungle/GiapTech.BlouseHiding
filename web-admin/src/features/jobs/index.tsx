import { Link } from '@tanstack/react-router'
import { useQuery } from '@tanstack/react-query'
import { Plus } from 'lucide-react'
import { useTranslation } from 'react-i18next'
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

const STATUS_BADGE_CLASS: Record<string, string> = {
  Published: 'bg-accent-jade text-white',
  PendingPayment: 'bg-amber-pending text-white',
  Pending: 'bg-amber-pending text-white',
  Rejected: 'bg-accent-seal text-white',
  Suspended: 'bg-accent-seal text-white',
}

export function Jobs() {
  const { t } = useTranslation('jobs')
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
              {t('breadcrumb')}
            </p>
            <h1 className='text-2xl font-semibold tracking-tight'>
              {t('list.title')}
            </h1>
          </div>
          <Button asChild>
            <Link to='/jobs/new'>
              <Plus />
              {t('list.newJob')}
            </Link>
          </Button>
        </div>

        <div className='rounded-md border'>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>{t('list.colTitle')}</TableHead>
                <TableHead>{t('list.colSpecialtyLocation')}</TableHead>
                <TableHead>{t('list.colStatus')}</TableHead>
                <TableHead>{t('list.colApplicants')}</TableHead>
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
                      {t(`status.${job.status}`, job.status)}
                    </Badge>
                  </TableCell>
                  <TableCell>
                    <Link
                      to='/applications/$jobId'
                      params={{ jobId: job.id }}
                      className='text-accent-jade hover:underline'
                    >
                      {t('list.viewAts')}
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
