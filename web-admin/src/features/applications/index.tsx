import {
  APPLICATION_STAGES,
  APPLICATION_STAGE_LABEL,
  MOCK_APPLICATIONS,
  type ApplicationStage,
} from '@/lib/mock-data'
import { Badge } from '@/components/ui/badge'
import { Card, CardContent, CardHeader } from '@/components/ui/card'
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
import { Search } from '@/components/search'
import { ThemeSwitch } from '@/components/theme-switch'

export function Applications() {
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

      <Main fixed>
        <div className='mb-4'>
          <p className='text-xs font-medium text-muted-foreground'>
            Điều dưỡng ICU — Ca đêm
          </p>
          <h1 className='text-2xl font-semibold tracking-tight'>
            ATS — Ứng viên
          </h1>
          <p className='mt-1 text-sm text-muted-foreground'>
            Chuyển giai đoạn bằng bộ chọn trên từng thẻ — kéo-thả (dnd-kit) sẽ
            bổ sung khi nối API thật.
          </p>
        </div>

        <div className='flex gap-4 overflow-x-auto pb-4'>
          {APPLICATION_STAGES.map((stage) => {
            const items = MOCK_APPLICATIONS.filter((a) => a.stage === stage)
            return (
              <div key={stage} className='w-72 shrink-0'>
                <div className='mb-2 flex items-center justify-between px-1'>
                  <h2 className='text-sm font-semibold'>
                    {APPLICATION_STAGE_LABEL[stage]}
                  </h2>
                  <Badge variant='secondary'>{items.length}</Badge>
                </div>
                <div className='space-y-2'>
                  {items.map((app) => (
                    <Card key={app.id}>
                      <CardHeader className='pb-2'>
                        <p className='text-sm font-medium'>
                          {app.candidateName}
                        </p>
                        <p className='text-xs text-muted-foreground'>
                          {app.specialty} · {app.yearsOfExperience} năm KN
                        </p>
                      </CardHeader>
                      <CardContent className='pb-3'>
                        <Select defaultValue={app.stage}>
                          <SelectTrigger size='sm' className='w-full'>
                            <SelectValue />
                          </SelectTrigger>
                          <SelectContent>
                            {(
                              Object.keys(
                                APPLICATION_STAGE_LABEL
                              ) as ApplicationStage[]
                            ).map((s) => (
                              <SelectItem key={s} value={s}>
                                {APPLICATION_STAGE_LABEL[s]}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                      </CardContent>
                    </Card>
                  ))}
                  {items.length === 0 && (
                    <div className='rounded-md border border-dashed p-4 text-center text-xs text-muted-foreground'>
                      Chưa có ứng viên
                    </div>
                  )}
                </div>
              </div>
            )
          })}
        </div>
      </Main>
    </>
  )
}
