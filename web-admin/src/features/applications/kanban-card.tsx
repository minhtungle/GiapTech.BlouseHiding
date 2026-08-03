import { useDraggable } from '@dnd-kit/core'
import { CSS } from '@dnd-kit/utilities'
import { GripVertical } from 'lucide-react'
import { Card, CardContent, CardHeader } from '@/components/ui/card'
import type { ApiApplication } from '@/lib/api'

export function KanbanCard({ application }: { application: ApiApplication }) {
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
          <p className='text-sm font-medium'>{application.candidateFullName}</p>
          {application.score !== null && (
            <p className='text-xs text-muted-foreground'>
              Điểm phù hợp: {application.score}
            </p>
          )}
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
        Nộp ngày {new Date(application.appliedAt).toLocaleDateString('vi-VN')}
      </CardContent>
    </Card>
  )
}
