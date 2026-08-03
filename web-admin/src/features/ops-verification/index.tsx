import { useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { ShieldCheck, ShieldX } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { toast } from 'sonner'
import { opsApi } from '@/lib/api'
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
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { Textarea } from '@/components/ui/textarea'
import { ConfigDrawer } from '@/components/config-drawer'
import { Header } from '@/components/layout/header'
import { Main } from '@/components/layout/main'
import { ProfileDropdown } from '@/components/profile-dropdown'
import { LanguageSwitcher } from '@/components/language-switcher'
import { ThemeSwitch } from '@/components/theme-switch'

type RejectTarget =
  | { kind: 'license'; id: string }
  | { kind: 'organization'; id: string }
  | { kind: 'job'; id: string }

export function OpsVerification() {
  const { t } = useTranslation('ops')
  const queryClient = useQueryClient()
  const [rejectTarget, setRejectTarget] = useState<RejectTarget | null>(null)
  const [rejectReason, setRejectReason] = useState('')

  const { data: licenses } = useQuery({
    queryKey: ['ops', 'licenses'],
    queryFn: opsApi.getPendingLicenses,
  })
  const { data: organizations } = useQuery({
    queryKey: ['ops', 'organizations'],
    queryFn: opsApi.getPendingOrganizations,
  })
  const { data: jobs } = useQuery({
    queryKey: ['ops', 'jobs'],
    queryFn: opsApi.getPendingJobs,
  })

  const verifyLicense = useMutation({
    mutationFn: ({ id, approved, reason }: { id: string; approved: boolean; reason: string | null }) =>
      opsApi.verifyLicense(id, approved, reason),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['ops', 'licenses'] })
      toast.success(t('verification.licenseSuccess'))
    },
    onError: () => toast.error(t('verification.licenseFailed')),
  })

  const verifyOrganization = useMutation({
    mutationFn: ({ id, approved, reason }: { id: string; approved: boolean; reason: string | null }) =>
      opsApi.verifyOrganization(id, approved ? 'Verify' : 'Reject', reason),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['ops', 'organizations'] })
      toast.success(t('verification.orgSuccess'))
    },
    onError: () => toast.error(t('verification.orgFailed')),
  })

  const moderateJob = useMutation({
    mutationFn: ({ id, approved, reason }: { id: string; approved: boolean; reason: string | null }) =>
      opsApi.moderateJob(id, approved, reason),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['ops', 'jobs'] })
      toast.success(t('verification.jobSuccess'))
    },
    onError: () => toast.error(t('verification.jobFailed')),
  })

  function openRejectDialog(target: RejectTarget) {
    setRejectReason('')
    setRejectTarget(target)
  }

  function confirmReject() {
    if (!rejectTarget || !rejectReason.trim()) return

    if (rejectTarget.kind === 'license') {
      verifyLicense.mutate({ id: rejectTarget.id, approved: false, reason: rejectReason })
    } else if (rejectTarget.kind === 'organization') {
      verifyOrganization.mutate({ id: rejectTarget.id, approved: false, reason: rejectReason })
    } else {
      moderateJob.mutate({ id: rejectTarget.id, approved: false, reason: rejectReason })
    }
    setRejectTarget(null)
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
        <p className='text-xs font-medium text-muted-foreground'>{t('breadcrumb')}</p>
        <h1 className='mb-6 text-2xl font-semibold tracking-tight'>
          {t('verification.pageTitle')}
        </h1>

        <Tabs defaultValue='license'>
          <TabsList>
            <TabsTrigger value='license'>
              {t('verification.tabLicense', { count: licenses?.length ?? 0 })}
            </TabsTrigger>
            <TabsTrigger value='org'>
              {t('verification.tabOrg', { count: organizations?.length ?? 0 })}
            </TabsTrigger>
            <TabsTrigger value='job'>
              {t('verification.tabJob', { count: jobs?.length ?? 0 })}
            </TabsTrigger>
          </TabsList>

          <TabsContent value='license' className='space-y-3'>
            {licenses?.length === 0 && (
              <p className='text-sm text-muted-foreground'>{t('verification.noLicenses')}</p>
            )}
            {licenses?.map((item) => (
              <Card key={item.id}>
                <CardHeader className='flex flex-row items-center justify-between'>
                  <div>
                    <CardTitle className='text-base'>
                      {item.candidateFullName}
                    </CardTitle>
                    <p className='text-sm text-muted-foreground'>
                      {t('verification.licenseNo', { no: item.licenseNo, issuedBy: item.issuedBy })}
                    </p>
                  </div>
                  <Badge className='bg-amber-pending text-white'>
                    {t('verification.pendingBadge')}
                  </Badge>
                </CardHeader>
                <CardContent className='flex items-center justify-between'>
                  <a
                    href={item.documentUrl}
                    target='_blank'
                    rel='noreferrer'
                    className='text-xs text-accent-jade hover:underline'
                  >
                    {t('verification.viewLicenseDocument')}
                  </a>
                  <div className='flex gap-2'>
                    <Button
                      size='sm'
                      variant='destructive'
                      onClick={() => openRejectDialog({ kind: 'license', id: item.id })}
                    >
                      <ShieldX />
                      {t('verification.reject')}
                    </Button>
                    <Button
                      size='sm'
                      onClick={() => verifyLicense.mutate({ id: item.id, approved: true, reason: null })}
                    >
                      <ShieldCheck />
                      {t('verification.approve')}
                    </Button>
                  </div>
                </CardContent>
              </Card>
            ))}
          </TabsContent>

          <TabsContent value='org' className='space-y-3'>
            {organizations?.length === 0 && (
              <p className='text-sm text-muted-foreground'>{t('verification.noOrgs')}</p>
            )}
            {organizations?.map((item) => (
              <Card key={item.id}>
                <CardHeader className='flex flex-row items-center justify-between'>
                  <div>
                    <CardTitle className='text-base'>{item.name}</CardTitle>
                    <p className='text-sm text-muted-foreground'>{item.orgType}</p>
                  </div>
                  <Badge className='bg-amber-pending text-white'>
                    {t('verification.pendingBadge')}
                  </Badge>
                </CardHeader>
                <CardContent className='flex items-center justify-end'>
                  <div className='flex gap-2'>
                    <Button
                      size='sm'
                      variant='destructive'
                      onClick={() => openRejectDialog({ kind: 'organization', id: item.id })}
                    >
                      <ShieldX />
                      {t('verification.reject')}
                    </Button>
                    <Button
                      size='sm'
                      onClick={() =>
                        verifyOrganization.mutate({ id: item.id, approved: true, reason: null })
                      }
                    >
                      <ShieldCheck />
                      {t('verification.approve')}
                    </Button>
                  </div>
                </CardContent>
              </Card>
            ))}
          </TabsContent>

          <TabsContent value='job' className='space-y-3'>
            {jobs?.length === 0 && (
              <p className='text-sm text-muted-foreground'>{t('verification.noJobs')}</p>
            )}
            {jobs?.map((item) => (
              <Card key={item.id}>
                <CardHeader className='flex flex-row items-center justify-between'>
                  <div>
                    <CardTitle className='text-base'>{item.title}</CardTitle>
                    <p className='text-sm text-muted-foreground'>
                      {item.organizationName}
                    </p>
                  </div>
                  <Badge className='bg-amber-pending text-white'>
                    {t('verification.pendingBadge')}
                  </Badge>
                </CardHeader>
                <CardContent className='flex items-center justify-between'>
                  <p className='max-w-md truncate text-xs text-muted-foreground'>
                    {item.description}
                  </p>
                  <div className='flex gap-2'>
                    <Button
                      size='sm'
                      variant='destructive'
                      onClick={() => openRejectDialog({ kind: 'job', id: item.id })}
                    >
                      <ShieldX />
                      {t('verification.reject')}
                    </Button>
                    <Button
                      size='sm'
                      onClick={() => moderateJob.mutate({ id: item.id, approved: true, reason: null })}
                    >
                      <ShieldCheck />
                      {t('verification.approve')}
                    </Button>
                  </div>
                </CardContent>
              </Card>
            ))}
          </TabsContent>
        </Tabs>
      </Main>

      <Dialog open={!!rejectTarget} onOpenChange={(open) => !open && setRejectTarget(null)}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{t('verification.rejectDialog.title')}</DialogTitle>
          </DialogHeader>
          <Textarea
            value={rejectReason}
            onChange={(e) => setRejectReason(e.target.value)}
            placeholder={t('verification.rejectDialog.placeholder')}
            rows={3}
          />
          <DialogFooter>
            <Button variant='outline' onClick={() => setRejectTarget(null)}>
              {t('verification.rejectDialog.cancel')}
            </Button>
            <Button variant='destructive' disabled={!rejectReason.trim()} onClick={confirmReject}>
              {t('verification.rejectDialog.confirm')}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </>
  )
}
