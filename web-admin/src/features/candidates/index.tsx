import { useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { AxiosError } from 'axios'
import { Lock, Search, Unlock } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { toast } from 'sonner'
import { candidatesApi, catalogApi } from '@/lib/api'
import { useMyOrganization } from '@/hooks/use-my-organization'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { ConfigDrawer } from '@/components/config-drawer'
import { Header } from '@/components/layout/header'
import { Main } from '@/components/layout/main'
import { ProfileDropdown } from '@/components/profile-dropdown'
import { LanguageSwitcher } from '@/components/language-switcher'
import { ThemeSwitch } from '@/components/theme-switch'

const ALL = '__all__'

export function Candidates() {
  const { t } = useTranslation('candidates')
  const { organization } = useMyOrganization()
  const queryClient = useQueryClient()
  const [specialtyId, setSpecialtyId] = useState<string>(ALL)
  const [locationId, setLocationId] = useState<string>(ALL)

  const { data: specialties } = useQuery({
    queryKey: ['catalog', 'specialties'],
    queryFn: catalogApi.getSpecialties,
  })
  const { data: locations } = useQuery({
    queryKey: ['catalog', 'locations'],
    queryFn: catalogApi.getLocations,
  })

  const { data: candidates, isLoading } = useQuery({
    queryKey: ['candidates', 'search', organization?.id, specialtyId, locationId],
    queryFn: () =>
      candidatesApi.search(
        organization!.id,
        specialtyId === ALL ? undefined : specialtyId,
        locationId === ALL ? undefined : locationId
      ),
    enabled: !!organization,
  })

  const unlock = useMutation({
    mutationFn: (candidateId: string) => candidatesApi.unlock(candidateId, organization!.id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['candidates', 'search'] })
      queryClient.invalidateQueries({ queryKey: ['credit-wallet', organization?.id] })
      toast.success(t('unlockSuccess'))
    },
    onError: (error) => {
      if (error instanceof AxiosError && error.response?.status === 400) {
        toast.error(t('unlockInsufficientCredit'))
        return
      }
      toast.error(t('unlockFailed'))
    },
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
        <p className='text-xs font-medium text-muted-foreground'>
          {t('breadcrumb')}
        </p>
        <h1 className='mb-6 text-2xl font-semibold tracking-tight'>
          {t('pageTitle')}
        </h1>

        <div className='mb-6 flex flex-wrap gap-3'>
          <Select value={specialtyId} onValueChange={setSpecialtyId}>
            <SelectTrigger className='w-56'>
              <SelectValue placeholder={t('specialtyPlaceholder')} />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value={ALL}>{t('allSpecialties')}</SelectItem>
              {specialties?.map((s) => (
                <SelectItem key={s.id} value={s.id}>
                  {s.name}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>

          <Select value={locationId} onValueChange={setLocationId}>
            <SelectTrigger className='w-56'>
              <SelectValue placeholder={t('locationPlaceholder')} />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value={ALL}>{t('allLocations')}</SelectItem>
              {locations?.map((l) => (
                <SelectItem key={l.id} value={l.id}>
                  {l.name}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>

        {isLoading && (
          <p className='text-sm text-muted-foreground'>{t('loading')}</p>
        )}

        {!isLoading && candidates?.length === 0 && (
          <div className='flex flex-col items-center gap-2 py-16 text-center'>
            <Search className='size-8 text-muted-foreground' />
            <p className='text-sm text-muted-foreground'>
              {t('noResults')}
            </p>
          </div>
        )}

        <div className='grid gap-3 sm:grid-cols-2 lg:grid-cols-3'>
          {candidates?.map((candidate) => (
            <Card key={candidate.id}>
              <CardHeader className='pb-2'>
                <CardTitle className='text-base'>
                  {candidate.headline ?? t('noHeadline')}
                </CardTitle>
              </CardHeader>
              <CardContent className='space-y-3'>
                {candidate.isUnlocked ? (
                  <div className='flex items-center gap-2 text-sm'>
                    <Unlock className='size-4 text-accent-jade' />
                    <span>{candidate.contactEmail}</span>
                  </div>
                ) : (
                  <div className='flex items-center justify-between'>
                    <div className='flex items-center gap-2 text-sm text-muted-foreground'>
                      <Lock className='size-4' />
                      <span>{t('contactHidden')}</span>
                    </div>
                    <Badge variant='outline'>{t('creditUnit', { count: candidate.unlockCost })}</Badge>
                  </div>
                )}

                {!candidate.isUnlocked && (
                  <Button
                    size='sm'
                    className='w-full'
                    disabled={unlock.isPending}
                    onClick={() => unlock.mutate(candidate.id)}
                  >
                    {t('unlockButton', { cost: candidate.unlockCost })}
                  </Button>
                )}
              </CardContent>
            </Card>
          ))}
        </div>
      </Main>
    </>
  )
}
