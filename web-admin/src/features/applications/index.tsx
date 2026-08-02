import { useState } from 'react'
import {
  DndContext,
  PointerSensor,
  useSensor,
  useSensors,
  type DragEndEvent,
} from '@dnd-kit/core'
import { toast } from 'sonner'
import {
  APPLICATION_STAGES,
  APPLICATION_STAGE_LABEL,
  MOCK_APPLICATIONS,
  type ApplicationStage,
} from '@/lib/mock-data'
import { ConfigDrawer } from '@/components/config-drawer'
import { Header } from '@/components/layout/header'
import { Main } from '@/components/layout/main'
import { ProfileDropdown } from '@/components/profile-dropdown'
import { Search } from '@/components/search'
import { ThemeSwitch } from '@/components/theme-switch'
import { KanbanCard } from './kanban-card'
import { KanbanColumn } from './kanban-column'

export function Applications() {
  const [applications, setApplications] = useState(MOCK_APPLICATIONS)

  const sensors = useSensors(
    useSensor(PointerSensor, { activationConstraint: { distance: 4 } })
  )

  function handleDragEnd(event: DragEndEvent) {
    const { active, over } = event
    if (!over) return

    const targetStage = over.id as ApplicationStage
    const application = applications.find((a) => a.id === active.id)
    if (!application || application.stage === targetStage) return

    setApplications((prev) =>
      prev.map((a) =>
        a.id === active.id ? { ...a, stage: targetStage } : a
      )
    )
    toast.success(
      `${application.candidateName} → ${APPLICATION_STAGE_LABEL[targetStage]}`
    )
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
            Điều dưỡng ICU — Ca đêm
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
              const items = applications.filter((a) => a.stage === stage)
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
