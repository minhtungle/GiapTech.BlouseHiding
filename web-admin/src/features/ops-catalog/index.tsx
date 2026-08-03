import { useQuery } from '@tanstack/react-query'
import { Plus, Pencil, Trash2 } from 'lucide-react'
import { catalogApi } from '@/lib/api'
import {
  MOCK_SPECIALTIES,
  MOCK_LOCATIONS,
  MOCK_JOB_PACKAGES,
} from '@/lib/mock-data'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
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
import { ThemeSwitch } from '@/components/theme-switch'

function CatalogList({ items }: { items: string[] }) {
  return (
    <div className='divide-y divide-border rounded-md border'>
      {items.map((item) => (
        <div key={item} className='flex items-center justify-between px-4 py-2.5'>
          <span className='text-sm'>{item}</span>
          <div className='flex gap-1'>
            <Button size='icon' variant='ghost' className='size-7'>
              <Pencil className='size-3.5' />
            </Button>
            <Button size='icon' variant='ghost' className='size-7'>
              <Trash2 className='size-3.5' />
            </Button>
          </div>
        </div>
      ))}
    </div>
  )
}

export function OpsCatalog() {
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
              <Button size='sm'>
                <Plus />
                Thêm chuyên khoa
              </Button>
            </div>
            <CatalogList items={specialties} />
          </TabsContent>

          <TabsContent value='location' className='space-y-4'>
            <div className='flex justify-end'>
              <Button size='sm'>
                <Plus />
                Thêm địa điểm
              </Button>
            </div>
            <CatalogList items={locations} />
          </TabsContent>

          <TabsContent value='package'>
            <div className='mb-4 flex justify-end'>
              <Button size='sm'>
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
                      <TableHead>Tên gói</TableHead>
                      <TableHead>Giá</TableHead>
                      <TableHead>Thời hạn</TableHead>
                      <TableHead>Ghi chú</TableHead>
                      <TableHead className='w-16' />
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {jobPackages.map((pkg) => (
                      <TableRow key={pkg.id}>
                        <TableCell className='font-medium'>
                          {pkg.name}
                        </TableCell>
                        <TableCell>{pkg.priceLabel}</TableCell>
                        <TableCell>{pkg.durationDays} ngày</TableCell>
                        <TableCell className='text-muted-foreground'>
                          {pkg.note}
                        </TableCell>
                        <TableCell>
                          <Button size='icon' variant='ghost' className='size-7'>
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
    </>
  )
}
