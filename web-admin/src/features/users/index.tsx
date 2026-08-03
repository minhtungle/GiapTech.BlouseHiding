import { useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { AxiosError } from 'axios'
import { Mail, Plus, Trash2 } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { toast } from 'sonner'
import { organizationsApi } from '@/lib/api'
import { useMyOrganization } from '@/hooks/use-my-organization'
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
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
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
import { LanguageSwitcher } from '@/components/language-switcher'
import { ThemeSwitch } from '@/components/theme-switch'

function InviteMemberDialog({
  open,
  onOpenChange,
  organizationId,
}: {
  open: boolean
  onOpenChange: (open: boolean) => void
  organizationId: string
}) {
  const { t } = useTranslation('members')
  const queryClient = useQueryClient()
  const [email, setEmail] = useState('')
  const [invitedRole, setInvitedRole] = useState<'HrManager' | 'HrMember'>('HrMember')

  const mutation = useMutation({
    mutationFn: () => organizationsApi.inviteMember(organizationId, email, invitedRole),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['organizations', 'members', organizationId] })
      toast.success(t('inviteDialog.success'))
      setEmail('')
      onOpenChange(false)
    },
    onError: (error) => {
      if (error instanceof AxiosError && error.response?.status === 400) {
        const message = error.response.data?.errors
          ? Object.values(error.response.data.errors).flat().join(' ')
          : t('inviteDialog.genericError')
        toast.error(message)
        return
      }
      toast.error(t('inviteDialog.failed'))
    },
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{t('inviteDialog.title')}</DialogTitle>
        </DialogHeader>
        <div className='space-y-4'>
          <div className='space-y-1.5'>
            <Label>{t('inviteDialog.emailLabel')}</Label>
            <Input
              type='email'
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder={t('inviteDialog.emailPlaceholder')}
            />
            <p className='text-xs text-muted-foreground'>
              {t('inviteDialog.emailHint')}
            </p>
          </div>
          <div className='space-y-1.5'>
            <Label>{t('inviteDialog.roleLabel')}</Label>
            <Select value={invitedRole} onValueChange={(v) => setInvitedRole(v as typeof invitedRole)}>
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value='HrMember'>{t('role.HrMember')}</SelectItem>
                <SelectItem value='HrManager'>{t('role.HrManager')}</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </div>
        <DialogFooter>
          <Button variant='outline' onClick={() => onOpenChange(false)}>
            {t('inviteDialog.cancel')}
          </Button>
          <Button disabled={!email.trim() || mutation.isPending} onClick={() => mutation.mutate()}>
            {t('inviteDialog.submit')}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}

export function Users() {
  const { t } = useTranslation('members')
  const { organization } = useMyOrganization()
  const queryClient = useQueryClient()
  const [inviteOpen, setInviteOpen] = useState(false)
  const [removeTarget, setRemoveTarget] = useState<{ id: string; email: string } | null>(null)

  const { data } = useQuery({
    queryKey: ['organizations', 'members', organization?.id],
    queryFn: () => organizationsApi.getMembers(organization!.id),
    enabled: !!organization,
  })

  const removeMember = useMutation({
    mutationFn: (memberId: string) => organizationsApi.removeMember(organization!.id, memberId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['organizations', 'members', organization?.id] })
      toast.success(t('removeDialog.success'))
      setRemoveTarget(null)
    },
    onError: () => toast.error(t('removeDialog.failed')),
  })

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
        <div className='mb-6 flex items-center justify-between'>
          <div>
            <p className='text-xs font-medium text-muted-foreground'>
              {t('breadcrumb')}
            </p>
            <h1 className='text-2xl font-semibold tracking-tight'>{t('pageTitle')}</h1>
          </div>
          <Button onClick={() => setInviteOpen(true)}>
            <Plus />
            {t('inviteButton')}
          </Button>
        </div>

        <Card className='mb-6'>
          <CardHeader>
            <CardTitle className='text-base'>{t('membersCardTitle')}</CardTitle>
          </CardHeader>
          <CardContent className='p-0'>
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>{t('colEmail')}</TableHead>
                  <TableHead>{t('colRole')}</TableHead>
                  <TableHead>{t('colJoinedAt')}</TableHead>
                  <TableHead className='w-16' />
                </TableRow>
              </TableHeader>
              <TableBody>
                {data?.members.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={4} className='text-center text-muted-foreground'>
                      {t('noMembers')}
                    </TableCell>
                  </TableRow>
                )}
                {data?.members.map((member) => (
                  <TableRow key={member.id}>
                    <TableCell className='font-medium'>{member.email}</TableCell>
                    <TableCell>
                      <Badge variant='outline'>{t(`role.${member.memberRole}`, member.memberRole)}</Badge>
                    </TableCell>
                    <TableCell className='text-muted-foreground'>
                      {member.joinedAt ? new Date(member.joinedAt).toLocaleDateString('vi-VN') : '—'}
                    </TableCell>
                    <TableCell>
                      {member.memberRole !== 'Owner' && (
                        <Button
                          size='icon'
                          variant='ghost'
                          className='size-7'
                          onClick={() => setRemoveTarget({ id: member.id, email: member.email })}
                        >
                          <Trash2 className='size-3.5' />
                        </Button>
                      )}
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </CardContent>
        </Card>

        {(data?.pendingInvitations.length ?? 0) > 0 && (
          <Card>
            <CardHeader>
              <CardTitle className='text-base'>{t('pendingInvitationsCardTitle')}</CardTitle>
            </CardHeader>
            <CardContent className='p-0'>
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>{t('colEmail')}</TableHead>
                    <TableHead>{t('colRole')}</TableHead>
                    <TableHead>{t('colExpiresAt')}</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {data?.pendingInvitations.map((invitation) => (
                    <TableRow key={invitation.id}>
                      <TableCell className='flex items-center gap-2 font-medium'>
                        <Mail className='size-3.5 text-muted-foreground' />
                        {invitation.email}
                      </TableCell>
                      <TableCell>
                        <Badge variant='outline'>
                          {t(`role.${invitation.invitedRole}`, invitation.invitedRole)}
                        </Badge>
                      </TableCell>
                      <TableCell className='text-muted-foreground'>
                        {new Date(invitation.expiresAt).toLocaleDateString('vi-VN')}
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </CardContent>
          </Card>
        )}
      </Main>

      {organization && (
        <InviteMemberDialog
          open={inviteOpen}
          onOpenChange={setInviteOpen}
          organizationId={organization.id}
        />
      )}

      <Dialog open={!!removeTarget} onOpenChange={(open) => !open && setRemoveTarget(null)}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{t('removeDialog.title')}</DialogTitle>
          </DialogHeader>
          <p className='text-sm text-muted-foreground'>
            {t('removeDialog.confirmText', { email: removeTarget?.email })}
          </p>
          <DialogFooter>
            <Button variant='outline' onClick={() => setRemoveTarget(null)}>
              {t('removeDialog.cancel')}
            </Button>
            <Button
              variant='destructive'
              disabled={removeMember.isPending}
              onClick={() => removeTarget && removeMember.mutate(removeTarget.id)}
            >
              {t('removeDialog.confirm')}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </>
  )
}
