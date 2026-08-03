import { useState } from 'react'
import { Link, useNavigate } from '@tanstack/react-router'
import { useQuery } from '@tanstack/react-query'
import { ChevronLeft, Loader2 } from 'lucide-react'
import { toast } from 'sonner'
import { catalogApi, jobsApi, paymentsApi, type PaymentInstructions } from '@/lib/api'
import { useMyOrganization } from '@/hooks/use-my-organization'
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
import { Textarea } from '@/components/ui/textarea'
import { ConfigDrawer } from '@/components/config-drawer'
import { Header } from '@/components/layout/header'
import { Main } from '@/components/layout/main'
import { ProfileDropdown } from '@/components/profile-dropdown'
import { LanguageSwitcher } from '@/components/language-switcher'
import { ThemeSwitch } from '@/components/theme-switch'

export function NewJob() {
  const navigate = useNavigate()
  const { organization } = useMyOrganization()

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
  const { data: employmentTypes } = useQuery({
    queryKey: ['catalog', 'employment-types'],
    queryFn: catalogApi.getEmploymentTypes,
  })

  const [title, setTitle] = useState('')
  const [specialtyId, setSpecialtyId] = useState('')
  const [locationId, setLocationId] = useState('')
  const [employmentType, setEmploymentType] = useState('')
  const [salaryMin, setSalaryMin] = useState('')
  const [salaryMax, setSalaryMax] = useState('')
  const [description, setDescription] = useState('')
  const [requirements, setRequirements] = useState('')
  const [benefits, setBenefits] = useState('')
  const [packageId, setPackageId] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [paymentInstructions, setPaymentInstructions] = useState<PaymentInstructions | null>(null)

  const freePackage = jobPackages?.find((p) => p.tier === 'Free')

  async function handleSubmit() {
    if (!organization) {
      toast.error('Không tìm thấy tổ chức của bạn.')
      return
    }
    if (!title || !specialtyId || !locationId || !employmentType || !description) {
      toast.error('Vui lòng điền đủ thông tin bắt buộc.')
      return
    }

    setIsSubmitting(true)

    try {
      const jobId = await jobsApi.create({
        organizationId: organization.id,
        title,
        specialtyId,
        employmentType,
        salaryMin: salaryMin ? Number(salaryMin) : null,
        salaryMax: salaryMax ? Number(salaryMax) : null,
        salaryNegotiable: !salaryMin && !salaryMax,
        locationId,
        addressDetail: null,
        requiredLicense: true,
        minExperienceYears: 0,
        description,
        requirements: requirements || null,
        benefits: benefits || null,
      })

      const selectedPackage = jobPackages?.find((p) => p.id === packageId) ?? freePackage
      if (!selectedPackage) {
        toast.error('Vui lòng chọn gói đăng tin.')
        return
      }

      await jobsApi.submit(jobId, selectedPackage.id)

      if (selectedPackage.tier === 'Free') {
        toast.success('Đăng tin thành công, đang chờ duyệt nội dung.')
        navigate({ to: '/jobs' })
        return
      }

      const instructions = await paymentsApi.createJobPackagePayment(jobId, selectedPackage.id)
      setPaymentInstructions(instructions)
    } catch {
      toast.error('Đăng tin không thành công.')
    } finally {
      setIsSubmitting(false)
    }
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
        <Link
          to='/jobs'
          className='mb-4 inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground'
        >
          <ChevronLeft className='size-4' />
          Danh sách tin
        </Link>
        <h1 className='mb-6 text-2xl font-semibold tracking-tight'>
          Đăng tin tuyển dụng
        </h1>

        <div className='grid gap-6 lg:grid-cols-3'>
          <Card className='lg:col-span-2'>
            <CardHeader>
              <CardTitle className='text-base'>Thông tin tin tuyển dụng</CardTitle>
            </CardHeader>
            <CardContent className='space-y-4'>
              <div className='space-y-1.5'>
                <Label htmlFor='title'>Vị trí tuyển dụng</Label>
                <Input
                  id='title'
                  placeholder='VD: Điều dưỡng ICU — Ca đêm'
                  value={title}
                  onChange={(e) => setTitle(e.target.value)}
                />
              </div>

              <div className='grid grid-cols-2 gap-4'>
                <div className='space-y-1.5'>
                  <Label htmlFor='specialty'>Chuyên khoa</Label>
                  <Select value={specialtyId} onValueChange={setSpecialtyId}>
                    <SelectTrigger id='specialty' className='w-full'>
                      <SelectValue placeholder='Chọn chuyên khoa' />
                    </SelectTrigger>
                    <SelectContent>
                      {specialties?.map((s) => (
                        <SelectItem key={s.id} value={s.id}>
                          {s.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
                <div className='space-y-1.5'>
                  <Label htmlFor='location'>Địa điểm</Label>
                  <Select value={locationId} onValueChange={setLocationId}>
                    <SelectTrigger id='location' className='w-full'>
                      <SelectValue placeholder='Chọn địa điểm' />
                    </SelectTrigger>
                    <SelectContent>
                      {locations?.map((l) => (
                        <SelectItem key={l.id} value={l.id}>
                          {l.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
              </div>

              <div className='grid grid-cols-2 gap-4'>
                <div className='grid grid-cols-2 gap-2'>
                  <div className='space-y-1.5'>
                    <Label htmlFor='salaryMin'>Lương tối thiểu</Label>
                    <Input
                      id='salaryMin'
                      type='number'
                      placeholder='15000000'
                      value={salaryMin}
                      onChange={(e) => setSalaryMin(e.target.value)}
                    />
                  </div>
                  <div className='space-y-1.5'>
                    <Label htmlFor='salaryMax'>Lương tối đa</Label>
                    <Input
                      id='salaryMax'
                      type='number'
                      placeholder='25000000'
                      value={salaryMax}
                      onChange={(e) => setSalaryMax(e.target.value)}
                    />
                  </div>
                </div>
                <div className='space-y-1.5'>
                  <Label htmlFor='employmentType'>Loại hình</Label>
                  <Select value={employmentType} onValueChange={setEmploymentType}>
                    <SelectTrigger id='employmentType' className='w-full'>
                      <SelectValue placeholder='Chọn loại hình' />
                    </SelectTrigger>
                    <SelectContent>
                      {employmentTypes?.map((e) => (
                        <SelectItem key={e} value={e}>
                          {e}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
              </div>

              <div className='space-y-1.5'>
                <Label htmlFor='description'>Mô tả công việc</Label>
                <Textarea
                  id='description'
                  rows={4}
                  value={description}
                  onChange={(e) => setDescription(e.target.value)}
                />
              </div>
              <div className='space-y-1.5'>
                <Label htmlFor='requirements'>Yêu cầu ứng viên</Label>
                <Textarea
                  id='requirements'
                  rows={3}
                  value={requirements}
                  onChange={(e) => setRequirements(e.target.value)}
                />
              </div>
              <div className='space-y-1.5'>
                <Label htmlFor='benefits'>Quyền lợi</Label>
                <Textarea
                  id='benefits'
                  rows={3}
                  value={benefits}
                  onChange={(e) => setBenefits(e.target.value)}
                />
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle className='text-base'>Chọn gói đăng tin</CardTitle>
            </CardHeader>
            <CardContent className='space-y-3'>
              {jobPackages?.map((pkg) => (
                <label
                  key={pkg.id}
                  className='flex cursor-pointer items-center justify-between rounded-md border p-3 text-sm has-[input:checked]:border-accent-jade has-[input:checked]:bg-accent-jade/5'
                >
                  <span>
                    <input
                      type='radio'
                      name='package'
                      value={pkg.id}
                      className='sr-only'
                      checked={packageId ? packageId === pkg.id : pkg.tier === 'Free'}
                      onChange={() => setPackageId(pkg.id)}
                    />
                    <span className='font-medium'>{pkg.tier}</span>
                    <span className='block text-xs text-muted-foreground'>
                      {pkg.durationDays} ngày · {pkg.name}
                    </span>
                  </span>
                  <span className='font-medium'>
                    {pkg.price === 0 ? '0đ' : `${pkg.price.toLocaleString('vi-VN')}đ`}
                  </span>
                </label>
              ))}

              <Button className='w-full' disabled={isSubmitting} onClick={handleSubmit}>
                {isSubmitting && <Loader2 className='animate-spin' />}
                Lưu & nộp duyệt
              </Button>
              <p className='text-center text-xs text-muted-foreground'>
                Gói Free vào hàng đợi duyệt nội dung ngay. Gói trả phí cần chuyển khoản thủ công —
                tin vào hàng chờ duyệt sau khi Vận hành xác nhận đã nhận tiền.
              </p>
            </CardContent>
          </Card>
        </div>
      </Main>

      <Dialog
        open={!!paymentInstructions}
        onOpenChange={(open) => {
          if (!open) {
            setPaymentInstructions(null)
            navigate({ to: '/jobs' })
          }
        }}
      >
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Hoàn tất chuyển khoản để đăng tin</DialogTitle>
          </DialogHeader>
          <p className='text-sm text-muted-foreground'>
            Tin đã lưu và đang chờ thanh toán. Chuyển khoản theo thông tin bên dưới, ghi đúng nội
            dung để Vận hành đối soát.
          </p>
          <div className='rounded-md border p-4 text-sm'>
            <div className='flex justify-between py-1'>
              <span className='text-muted-foreground'>Số tiền</span>
              <span className='font-medium'>
                {paymentInstructions?.amount.toLocaleString('vi-VN')}đ
              </span>
            </div>
            <div className='flex justify-between py-1'>
              <span className='text-muted-foreground'>Nội dung chuyển khoản</span>
              <span className='font-mono font-medium'>{paymentInstructions?.referenceCode}</span>
            </div>
          </div>
          <p className='text-xs text-muted-foreground'>
            Tin sẽ vào hàng đợi duyệt nội dung sau khi Vận hành xác nhận đã nhận được chuyển khoản.
          </p>
          <DialogFooter>
            <Button
              onClick={() => {
                setPaymentInstructions(null)
                navigate({ to: '/jobs' })
              }}
            >
              Đã hiểu
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </>
  )
}
