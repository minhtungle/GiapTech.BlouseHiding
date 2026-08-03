import {
  DndContext,
  PointerSensor,
  useSensor,
  useSensors,
  type DragEndEvent,
} from '@dnd-kit/core'
import { useParams } from '@tanstack/react-router'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'
import { applicationsApi, jobsApi } from '@/lib/api'
import { ConfigDrawer } from '@/components/config-drawer'
import { Header } from '@/components/layout/header'
import { Main } from '@/components/layout/main'
import { ProfileDropdown } from '@/components/profile-dropdown'
import { Search } from '@/components/search'
import { ThemeSwitch } from '@/components/theme-switch'
import { APPLICATION_STAGES, APPLICATION_STAGE_LABEL, type ApplicationStageValue } from './constants'
import { KanbanCard } from './kanban-card'
import { KanbanColumn } from './kanban-column'

export function Applications() {
  const { jobId } = useParams({ from: '/_authenticated/applications/$jobId' })
  const queryClient = useQueryClient()

  const { data: job } = useQuery({
    queryKey: ['jobs', jobId],
    queryFn: () => jobsApi.getById(jobId),
  })

  const { data: applications } = useQuery({
    queryKey: ['applications', jobId],
    queryFn: () => applicationsApi.getByJob(jobId),
  })

  const transitionStage = useMutation({
    mutationFn: ({ applicationId, stage }: { applicationId: string; stage: ApplicationStageValue }) =>
      applicationsApi.transitionStage(applicationId, stage, false),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['applications', jobId] })
    },
    onError: () => {
      toast.error('Chuyển giai đoạn không thành công.')
    },
  })

  const sensors = useSensors(
    useSensor(PointerSensor, { activationConstraint: { distance: 4 } })
  )

  function handleDragEnd(event: DragEndEvent) {
    const { active, over } = event
    if (!over) return

    const targetStage = over.id as ApplicationStageValue
    const application = applications?.find((a) => a.id === active.id)
    if (!application || application.stage === targetStage) return

    transitionStage.mutate({ applicationId: application.id, stage: targetStage })
    toast.success(`${application.candidateFullName} → ${APPLICATION_STAGE_LABEL[targetStage]}`)
  }

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
            {job?.title ?? '...'}
          </p>
          <h1 className='text-2xl font-semibold tracking-tight'>
            ATS — Ứng viên
          </h1>
          <p className='mt-1 text-sm text-muted-foreground'>
            Kéo thẻ (biểu tượng ⋮⋮) sang cột khác để chuyển giai đoạn.
          </p>
        </div>

        <DndContext sensors={sensors} onDragEnd={handleDragEnd}>
          <div className='flex gap-4 overflow-x-auto pb-4'>
            {APPLICATION_STAGES.map((stage) => {
              const items = (applications ?? []).filter((a) => a.stage === stage)
              return (
                <KanbanColumn key={stage} stage={stage} count={items.length}>
                  {items.map((app) => (
                    <KanbanCard key={app.id} application={app} />
                  ))}
                </KanbanColumn>
              )
            })}
          </div>
        </DndContext>
      </Main>
    </>
  )
}
