import { useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Plus, Pencil } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { toast } from 'sonner'
import {
  catalogApi,
  opsCatalogApi,
  type ApiJobPackage,
  type ApiLocation,
  type ApiSpecialty,
} from '@/lib/api'
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
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { ConfigDrawer } from '@/components/config-drawer'
import { Header } from '@/components/layout/header'
import { Main } from '@/components/layout/main'
import { ProfileDropdown } from '@/components/profile-dropdown'
import { LanguageSwitcher } from '@/components/language-switcher'
import { ThemeSwitch } from '@/components/theme-switch'

const NO_PARENT = '__none__'
const JOB_PACKAGE_TIERS = ['Free', 'Eco', 'Pro', 'Max'] as const

function SpecialtyDialog({
  open,
  onOpenChange,
  specialty,
  specialties,
}: {
  open: boolean
  onOpenChange: (open: boolean) => void
  specialty: ApiSpecialty | null
  specialties: ApiSpecialty[]
}) {
  const { t } = useTranslation('ops')
  const queryClient = useQueryClient()
  const [code, setCode] = useState(specialty?.code ?? '')
  const [name, setName] = useState(specialty?.name ?? '')
  const [parentId, setParentId] = useState(specialty?.parentId ?? NO_PARENT)

  const mutation = useMutation({
    mutationFn: async () => {
      const parent = parentId === NO_PARENT ? null : parentId
      if (specialty) {
        await opsCatalogApi.updateSpecialty(specialty.id, { name, parentId: parent })
      } else {
        await opsCatalogApi.createSpecialty({ code, name, parentId: parent })
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['catalog', 'specialties'] })
      toast.success(
        specialty ? t('catalog.specialtyDialog.updateSuccess') : t('catalog.specialtyDialog.createSuccess')
      )
      onOpenChange(false)
    },
    onError: () => toast.error(t('catalog.specialtyDialog.failed')),
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>
            {specialty ? t('catalog.specialtyDialog.editTitle') : t('catalog.specialtyDialog.createTitle')}
          </DialogTitle>
        </DialogHeader>
        <div className='space-y-4'>
          {!specialty && (
            <div className='space-y-1.5'>
              <Label>{t('catalog.specialtyDialog.codeLabel')}</Label>
              <Input
                value={code}
                onChange={(e) => setCode(e.target.value)}
                placeholder={t('catalog.specialtyDialog.codePlaceholder')}
              />
            </div>
          )}
          <div className='space-y-1.5'>
            <Label>{t('catalog.specialtyDialog.nameLabel')}</Label>
            <Input value={name} onChange={(e) => setName(e.target.value)} />
          </div>
          <div className='space-y-1.5'>
            <Label>{t('catalog.specialtyDialog.parentLabel')}</Label>
            <Select value={parentId} onValueChange={setParentId}>
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value={NO_PARENT}>{t('catalog.specialtyDialog.noParent')}</SelectItem>
                {specialties
                  .filter((s) => s.id !== specialty?.id)
                  .map((s) => (
                    <SelectItem key={s.id} value={s.id}>
                      {s.name}
                    </SelectItem>
                  ))}
              </SelectContent>
            </Select>
          </div>
        </div>
        <DialogFooter>
          <Button variant='outline' onClick={() => onOpenChange(false)}>
            {t('catalog.specialtyDialog.cancel')}
          </Button>
          <Button
            disabled={!name.trim() || (!specialty && !code.trim()) || mutation.isPending}
            onClick={() => mutation.mutate()}
          >
            {t('catalog.specialtyDialog.save')}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}

function LocationDialog({
  open,
  onOpenChange,
  location,
  locations,
}: {
  open: boolean
  onOpenChange: (open: boolean) => void
  location: ApiLocation | null
  locations: ApiLocation[]
}) {
  const { t } = useTranslation('ops')
  const queryClient = useQueryClient()
  const [name, setName] = useState(location?.name ?? '')
  const [parentId, setParentId] = useState(location?.parentId ?? NO_PARENT)

  const mutation = useMutation({
    mutationFn: async () => {
      const parent = parentId === NO_PARENT ? null : parentId
      if (location) {
        await opsCatalogApi.updateLocation(location.id, { name, parentId: parent })
      } else {
        await opsCatalogApi.createLocation({ name, parentId: parent })
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['catalog', 'locations'] })
      toast.success(
        location ? t('catalog.locationDialog.updateSuccess') : t('catalog.locationDialog.createSuccess')
      )
      onOpenChange(false)
    },
    onError: () => toast.error(t('catalog.locationDialog.failed')),
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>
            {location ? t('catalog.locationDialog.editTitle') : t('catalog.locationDialog.createTitle')}
          </DialogTitle>
        </DialogHeader>
        <div className='space-y-4'>
          <div className='space-y-1.5'>
            <Label>{t('catalog.locationDialog.nameLabel')}</Label>
            <Input
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder={t('catalog.locationDialog.namePlaceholder')}
            />
          </div>
          <div className='space-y-1.5'>
            <Label>{t('catalog.locationDialog.parentLabel')}</Label>
            <Select value={parentId} onValueChange={setParentId}>
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value={NO_PARENT}>{t('catalog.locationDialog.noParent')}</SelectItem>
                {locations
                  .filter((l) => l.id !== location?.id)
                  .map((l) => (
                    <SelectItem key={l.id} value={l.id}>
                      {l.name}
                    </SelectItem>
                  ))}
              </SelectContent>
            </Select>
          </div>
        </div>
        <DialogFooter>
          <Button variant='outline' onClick={() => onOpenChange(false)}>
            {t('catalog.locationDialog.cancel')}
          </Button>
          <Button disabled={!name.trim() || mutation.isPending} onClick={() => mutation.mutate()}>
            {t('catalog.locationDialog.save')}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}

function JobPackageDialog({
  open,
  onOpenChange,
  jobPackage,
}: {
  open: boolean
  onOpenChange: (open: boolean) => void
  jobPackage: ApiJobPackage | null
}) {
  const { t } = useTranslation('ops')
  const queryClient = useQueryClient()
  const [tier, setTier] = useState<(typeof JOB_PACKAGE_TIERS)[number]>(
    (jobPackage?.tier as (typeof JOB_PACKAGE_TIERS)[number]) ?? 'Free'
  )
  const [name, setName] = useState(jobPackage?.name ?? '')
  const [durationDays, setDurationDays] = useState(String(jobPackage?.durationDays ?? 30))
  const [price, setPrice] = useState(String(jobPackage?.price ?? 0))
  const [maxActiveJobs, setMaxActiveJobs] = useState(
    jobPackage?.maxActiveJobs != null ? String(jobPackage.maxActiveJobs) : ''
  )

  const mutation = useMutation({
    mutationFn: async () => {
      const input = {
        name,
        durationDays: Number(durationDays),
        price: Number(price),
        maxActiveJobs: maxActiveJobs.trim() ? Number(maxActiveJobs) : null,
      }
      if (jobPackage) {
        await opsCatalogApi.updateJobPackage(jobPackage.id, input)
      } else {
        await opsCatalogApi.createJobPackage({ ...input, tier })
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['catalog', 'job-packages'] })
      toast.success(
        jobPackage ? t('catalog.packageDialog.updateSuccess') : t('catalog.packageDialog.createSuccess')
      )
      onOpenChange(false)
    },
    onError: () => toast.error(t('catalog.packageDialog.failed')),
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>
            {jobPackage ? t('catalog.packageDialog.editTitle') : t('catalog.packageDialog.createTitle')}
          </DialogTitle>
        </DialogHeader>
        <div className='space-y-4'>
          {!jobPackage && (
            <div className='space-y-1.5'>
              <Label>{t('catalog.packageDialog.tierLabel')}</Label>
              <Select value={tier} onValueChange={(v) => setTier(v as typeof tier)}>
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {JOB_PACKAGE_TIERS.map((tierOption) => (
                    <SelectItem key={tierOption} value={tierOption}>
                      {tierOption}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          )}
          <div className='space-y-1.5'>
            <Label>{t('catalog.packageDialog.nameLabel')}</Label>
            <Input value={name} onChange={(e) => setName(e.target.value)} />
          </div>
          <div className='grid grid-cols-2 gap-4'>
            <div className='space-y-1.5'>
              <Label>{t('catalog.packageDialog.priceLabel')}</Label>
              <Input type='number' min={0} value={price} onChange={(e) => setPrice(e.target.value)} />
            </div>
            <div className='space-y-1.5'>
              <Label>{t('catalog.packageDialog.durationLabel')}</Label>
              <Input
                type='number'
                min={1}
                value={durationDays}
                onChange={(e) => setDurationDays(e.target.value)}
              />
            </div>
          </div>
          <div className='space-y-1.5'>
            <Label>{t('catalog.packageDialog.maxActiveJobsLabel')}</Label>
            <Input
              type='number'
              min={0}
              value={maxActiveJobs}
              onChange={(e) => setMaxActiveJobs(e.target.value)}
              placeholder={t('catalog.packageDialog.maxActiveJobsPlaceholder')}
            />
          </div>
        </div>
        <DialogFooter>
          <Button variant='outline' onClick={() => onOpenChange(false)}>
            {t('catalog.packageDialog.cancel')}
          </Button>
          <Button
            disabled={!name.trim() || !durationDays || !price || mutation.isPending}
            onClick={() => mutation.mutate()}
          >
            {t('catalog.packageDialog.save')}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}

export function OpsCatalog() {
  const { t } = useTranslation('ops')
  const { data: specialties } = useQuery({
    queryKey: ['catalog', 'specialties'],
    queryFn: catalogApi.getSpecialties,
  })
  const { data: locations } = useQuery({
    queryKey: ['catalog', 'locations'],
    queryFn: catalogApi.getLocations,
  })
  const { data: jobPackages } = useQuery({
    queryKey: ['catalog', 'job-packages'],
    queryFn: catalogApi.getJobPackages,
  })

  const [specialtyDialog, setSpecialtyDialog] = useState<{ open: boolean; item: ApiSpecialty | null }>({
    open: false,
    item: null,
  })
  const [locationDialog, setLocationDialog] = useState<{ open: boolean; item: ApiLocation | null }>({
    open: false,
    item: null,
  })
  const [packageDialog, setPackageDialog] = useState<{ open: boolean; item: ApiJobPackage | null }>({
    open: false,
    item: null,
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
        <p className='text-xs font-medium text-muted-foreground'>{t('breadcrumb')}</p>
        <h1 className='mb-6 text-2xl font-semibold tracking-tight'>
          {t('catalog.pageTitle')}
        </h1>

        <Tabs defaultValue='specialty'>
          <TabsList>
            <TabsTrigger value='specialty'>{t('catalog.tabSpecialty')}</TabsTrigger>
            <TabsTrigger value='location'>{t('catalog.tabLocation')}</TabsTrigger>
            <TabsTrigger value='package'>{t('catalog.tabPackage')}</TabsTrigger>
          </TabsList>

          <TabsContent value='specialty' className='space-y-4'>
            <div className='flex justify-end'>
              <Button size='sm' onClick={() => setSpecialtyDialog({ open: true, item: null })}>
                <Plus />
                {t('catalog.addSpecialty')}
              </Button>
            </div>
            <div className='divide-y divide-border rounded-md border'>
              {specialties?.map((item) => (
                <div key={item.id} className='flex items-center justify-between px-4 py-2.5'>
                  <span className='text-sm'>{item.name}</span>
                  <Button
                    size='icon'
                    variant='ghost'
                    className='size-7'
                    onClick={() => setSpecialtyDialog({ open: true, item })}
                  >
                    <Pencil className='size-3.5' />
                  </Button>
                </div>
              ))}
            </div>
          </TabsContent>

          <TabsContent value='location' className='space-y-4'>
            <div className='flex justify-end'>
              <Button size='sm' onClick={() => setLocationDialog({ open: true, item: null })}>
                <Plus />
                {t('catalog.addLocation')}
              </Button>
            </div>
            <div className='divide-y divide-border rounded-md border'>
              {locations?.map((item) => (
                <div key={item.id} className='flex items-center justify-between px-4 py-2.5'>
                  <span className='text-sm'>{item.name}</span>
                  <Button
                    size='icon'
                    variant='ghost'
                    className='size-7'
                    onClick={() => setLocationDialog({ open: true, item })}
                  >
                    <Pencil className='size-3.5' />
                  </Button>
                </div>
              ))}
            </div>
          </TabsContent>

          <TabsContent value='package'>
            <div className='mb-4 flex justify-end'>
              <Button size='sm' onClick={() => setPackageDialog({ open: true, item: null })}>
                <Plus />
                {t('catalog.addPackage')}
              </Button>
            </div>
            <Card>
              <CardHeader>
                <CardTitle className='text-base'>{t('catalog.packageTableTitle')}</CardTitle>
              </CardHeader>
              <CardContent className='p-0'>
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>{t('catalog.colTier')}</TableHead>
                      <TableHead>{t('catalog.colName')}</TableHead>
                      <TableHead>{t('catalog.colPrice')}</TableHead>
                      <TableHead>{t('catalog.colDuration')}</TableHead>
                      <TableHead className='w-16' />
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {jobPackages?.map((pkg) => (
                      <TableRow key={pkg.id}>
                        <TableCell className='font-medium'>{pkg.tier}</TableCell>
                        <TableCell>{pkg.name}</TableCell>
                        <TableCell>
                          {pkg.price === 0 ? '0đ' : `${pkg.price.toLocaleString('vi-VN')}đ`}
                        </TableCell>
                        <TableCell>{t('catalog.durationDays', { count: pkg.durationDays })}</TableCell>
                        <TableCell>
                          <Button
                            size='icon'
                            variant='ghost'
                            className='size-7'
                            onClick={() => setPackageDialog({ open: true, item: pkg })}
                          >
                            <Pencil className='size-3.5' />
                          </Button>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </CardContent>
            </Card>
          </TabsContent>
        </Tabs>
      </Main>

      <SpecialtyDialog
        open={specialtyDialog.open}
        onOpenChange={(open) => setSpecialtyDialog((s) => ({ ...s, open }))}
        specialty={specialtyDialog.item}
        specialties={specialties ?? []}
      />
      <LocationDialog
        open={locationDialog.open}
        onOpenChange={(open) => setLocationDialog((s) => ({ ...s, open }))}
        location={locationDialog.item}
        locations={locations ?? []}
      />
      <JobPackageDialog
        open={packageDialog.open}
        onOpenChange={(open) => setPackageDialog((s) => ({ ...s, open }))}
        jobPackage={packageDialog.item}
      />
    </>
  )
}
