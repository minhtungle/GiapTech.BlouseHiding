import { useDraggable } from '@dnd-kit/core'
import { CSS } from '@dnd-kit/utilities'
import { Card, CardContent, CardHeader } from '@/components/ui/card'
import { GripVertical } from 'lucide-react'
import type { MockApplication } from '@/lib/mock-data'

export function KanbanCard({ application }: { application: MockApplication }) {
  const { attributes, listeners, setNodeRef, transform, isDragging } =
    useDraggable({ id: application.id })

  const style = transform
    ? { transform: CSS.Translate.toString(transform) }
    : undefined

  return (
    <Card
      ref={setNodeRef}
      style={style}
      className={isDragging ? 'z-10 opacity-50' : undefined}
    >
      <CardHeader className='flex-row items-start justify-between pb-2 space-y-0'>
        <div>
          <p className='text-sm font-medium'>{application.candidateName}</p>
          <p className='text-xs text-muted-foreground'>
            {application.specialty} · {application.yearsOfExperience} năm KN
          </p>
        </div>
        <button
          type='button'
          {...attributes}
          {...listeners}
          className='cursor-grab touch-none text-muted-foreground active:cursor-grabbing'
          aria-label='Kéo để chuyển giai đoạn'
        >
          <GripVertical className='size-4' />
        </button>
      </CardHeader>
      <CardContent className='pb-3 text-xs text-muted-foreground'>
        Nộp ngày {application.appliedAt}
      </CardContent>
    </Card>
  )
}
