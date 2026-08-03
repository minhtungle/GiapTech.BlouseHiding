import { Link } from '@tanstack/react-router'
import { useQuery } from '@tanstack/react-query'
import { ChevronLeft } from 'lucide-react'
import { catalogApi } from '@/lib/api'
import {
  MOCK_SPECIALTIES,
  MOCK_LOCATIONS,
  MOCK_JOB_PACKAGES,
} from '@/lib/mock-data'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
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
import { ThemeSwitch } from '@/components/theme-switch'

export function NewJob() {
  const { data: apiSpecialties } = useQuery({
    queryKey: ['catalog', 'specialties'],
    queryFn: catalogApi.getSpecialties,
  })
  const { data: apiLocations } = useQuery({
    queryKey: ['catalog', 'locations'],
    queryFn: catalogApi.getLocations,
  })
  const { data: apiJobPackages } = useQuery({
    queryKey: ['catalog', 'job-packages'],
    queryFn: catalogApi.getJobPackages,
  })

  const specialties = apiSpecialties?.length
    ? apiSpecialties.map((s) => s.name)
    : MOCK_SPECIALTIES
  const locations = apiLocations?.length
    ? apiLocations.map((l) => l.name)
    : MOCK_LOCATIONS
  const jobPackages = apiJobPackages?.length
    ? apiJobPackages.map((pkg) => ({
        id: pkg.id,
        name: pkg.tier,
        priceLabel: pkg.price === 0 ? '0đ' : `${pkg.price.toLocaleString('vi-VN')}đ`,
        durationDays: pkg.durationDays,
        note: pkg.name,
      }))
    : MOCK_JOB_PACKAGES

  return (
    <>
      <Header>
        <div className='ms-auto flex items-center gap-2'>
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
                <Input id='title' placeholder='VD: Điều dưỡng ICU — Ca đêm' />
              </div>

              <div className='grid grid-cols-2 gap-4'>
                <div className='space-y-1.5'>
                  <Label htmlFor='specialty'>Chuyên khoa</Label>
                  <Select>
                    <SelectTrigger id='specialty' className='w-full'>
                      <SelectValue placeholder='Chọn chuyên khoa' />
                    </SelectTrigger>
                    <SelectContent>
                      {specialties.map((s) => (
                        <SelectItem key={s} value={s}>
                          {s}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
                <div className='space-y-1.5'>
                  <Label htmlFor='location'>Địa điểm</Label>
                  <Select>
                    <SelectTrigger id='location' className='w-full'>
                      <SelectValue placeholder='Chọn địa điểm' />
                    </SelectTrigger>
                    <SelectContent>
                      {locations.map((l) => (
                        <SelectItem key={l} value={l}>
                          {l}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
              </div>

              <div className='grid grid-cols-2 gap-4'>
                <div className='space-y-1.5'>
                  <Label htmlFor='salary'>Mức lương</Label>
                  <Input id='salary' placeholder='VD: 18 - 25 triệu' />
                </div>
                <div className='space-y-1.5'>
                  <Label htmlFor='employmentType'>Loại hình</Label>
                  <Select>
                    <SelectTrigger id='employmentType' className='w-full'>
                      <SelectValue placeholder='Chọn loại hình' />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value='full-time'>Toàn thời gian</SelectItem>
                      <SelectItem value='locum'>Bán thời gian (Locum)</SelectItem>
                      <SelectItem value='shift'>Theo ca</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              </div>

              <div className='space-y-1.5'>
                <Label htmlFor='description'>Mô tả công việc</Label>
                <Textarea id='description' rows={4} />
              </div>
              <div className='space-y-1.5'>
                <Label htmlFor='requirements'>Yêu cầu ứng viên</Label>
                <Textarea id='requirements' rows={3} />
              </div>
              <div className='space-y-1.5'>
                <Label htmlFor='benefits'>Quyền lợi</Label>
                <Textarea id='benefits' rows={3} />
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle className='text-base'>Chọn gói đăng tin</CardTitle>
            </CardHeader>
            <CardContent className='space-y-3'>
              {jobPackages.map((pkg) => (
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
                      defaultChecked={pkg.name === 'Pro'}
                    />
                    <span className='font-medium'>{pkg.name}</span>
                    <span className='block text-xs text-muted-foreground'>
                      {pkg.durationDays} ngày · {pkg.note}
                    </span>
                  </span>
                  <span className='font-medium'>{pkg.priceLabel}</span>
                </label>
              ))}

              <Button className='w-full'>Lưu nháp & chọn thanh toán</Button>
              <p className='text-center text-xs text-muted-foreground'>
                Gói trả phí chuyển sang trạng thái &quot;Chờ thanh toán&quot; —
                chuyển khoản thủ công theo mã tham chiếu.
              </p>
            </CardContent>
          </Card>
        </div>
      </Main>
    </>
  )
}
