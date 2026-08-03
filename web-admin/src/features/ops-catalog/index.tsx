import { useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Plus, Pencil } from 'lucide-react'
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
      toast.success(specialty ? 'Đã lưu chuyên khoa.' : 'Đã thêm chuyên khoa.')
      onOpenChange(false)
    },
    onError: () => toast.error('Không lưu được chuyên khoa.'),
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{specialty ? 'Sửa chuyên khoa' : 'Thêm chuyên khoa'}</DialogTitle>
        </DialogHeader>
        <div className='space-y-4'>
          {!specialty && (
            <div className='space-y-1.5'>
              <Label>Mã (code)</Label>
              <Input value={code} onChange={(e) => setCode(e.target.value)} placeholder='vd: noi-khoa' />
            </div>
          )}
          <div className='space-y-1.5'>
            <Label>Tên (tiếng Việt)</Label>
            <Input value={name} onChange={(e) => setName(e.target.value)} />
          </div>
          <div className='space-y-1.5'>
            <Label>Chuyên khoa cha (tuỳ chọn)</Label>
            <Select value={parentId} onValueChange={setParentId}>
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value={NO_PARENT}>Không có (cấp gốc)</SelectItem>
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
            Hủy
          </Button>
          <Button
            disabled={!name.trim() || (!specialty && !code.trim()) || mutation.isPending}
            onClick={() => mutation.mutate()}
          >
            Lưu
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
      toast.success(location ? 'Đã lưu địa điểm.' : 'Đã thêm địa điểm.')
      onOpenChange(false)
    },
    onError: () => toast.error('Không lưu được địa điểm.'),
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{location ? 'Sửa địa điểm' : 'Thêm địa điểm'}</DialogTitle>
        </DialogHeader>
        <div className='space-y-4'>
          <div className='space-y-1.5'>
            <Label>Tên</Label>
            <Input value={name} onChange={(e) => setName(e.target.value)} placeholder='vd: Hà Nội' />
          </div>
          <div className='space-y-1.5'>
            <Label>Thuộc tỉnh/thành (tuỳ chọn — để trống nếu đây là tỉnh/thành)</Label>
            <Select value={parentId} onValueChange={setParentId}>
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value={NO_PARENT}>Không có (tỉnh/thành gốc)</SelectItem>
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
            Hủy
          </Button>
          <Button disabled={!name.trim() || mutation.isPending} onClick={() => mutation.mutate()}>
            Lưu
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
      toast.success(jobPackage ? 'Đã lưu gói tin.' : 'Đã thêm gói tin.')
      onOpenChange(false)
    },
    onError: () => toast.error('Không lưu được gói tin.'),
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{jobPackage ? 'Sửa gói tin' : 'Thêm gói tin'}</DialogTitle>
        </DialogHeader>
        <div className='space-y-4'>
          {!jobPackage && (
            <div className='space-y-1.5'>
              <Label>Hạng gói (không đổi được sau khi tạo)</Label>
              <Select value={tier} onValueChange={(v) => setTier(v as typeof tier)}>
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {JOB_PACKAGE_TIERS.map((t) => (
                    <SelectItem key={t} value={t}>
                      {t}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          )}
          <div className='space-y-1.5'>
            <Label>Tên gói</Label>
            <Input value={name} onChange={(e) => setName(e.target.value)} />
          </div>
          <div className='grid grid-cols-2 gap-4'>
            <div className='space-y-1.5'>
              <Label>Giá (VNĐ)</Label>
              <Input type='number' min={0} value={price} onChange={(e) => setPrice(e.target.value)} />
            </div>
            <div className='space-y-1.5'>
              <Label>Thời hạn (ngày)</Label>
              <Input
                type='number'
                min={1}
                value={durationDays}
                onChange={(e) => setDurationDays(e.target.value)}
              />
            </div>
          </div>
          <div className='space-y-1.5'>
            <Label>Số tin tối đa đang đăng (tuỳ chọn)</Label>
            <Input
              type='number'
              min={0}
              value={maxActiveJobs}
              onChange={(e) => setMaxActiveJobs(e.target.value)}
              placeholder='Không giới hạn nếu để trống'
            />
          </div>
        </div>
        <DialogFooter>
          <Button variant='outline' onClick={() => onOpenChange(false)}>
            Hủy
          </Button>
          <Button
            disabled={!name.trim() || !durationDays || !price || mutation.isPending}
            onClick={() => mutation.mutate()}
          >
            Lưu
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}

export function OpsCatalog() {
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
        <p className='text-xs font-medium text-muted-foreground'>Vận hành</p>
        <h1 className='mb-6 text-2xl font-semibold tracking-tight'>
          Danh mục &amp; gói tin
        </h1>

        <Tabs defaultValue='specialty'>
          <TabsList>
            <TabsTrigger value='specialty'>Chuyên khoa</TabsTrigger>
            <TabsTrigger value='location'>Địa điểm</TabsTrigger>
            <TabsTrigger value='package'>Gói tin</TabsTrigger>
          </TabsList>

          <TabsContent value='specialty' className='space-y-4'>
            <div className='flex justify-end'>
              <Button size='sm' onClick={() => setSpecialtyDialog({ open: true, item: null })}>
                <Plus />
                Thêm chuyên khoa
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
                Thêm địa điểm
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
                Thêm gói tin
              </Button>
            </div>
            <Card>
              <CardHeader>
                <CardTitle className='text-base'>Gói đăng tin</CardTitle>
              </CardHeader>
              <CardContent className='p-0'>
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Hạng</TableHead>
                      <TableHead>Tên gói</TableHead>
                      <TableHead>Giá</TableHead>
                      <TableHead>Thời hạn</TableHead>
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
                        <TableCell>{pkg.durationDays} ngày</TableCell>
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
