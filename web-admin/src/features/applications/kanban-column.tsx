import { useDroppable } from '@dnd-kit/core'
import { useTranslation } from 'react-i18next'
import { Badge } from '@/components/ui/badge'
import { cn } from '@/lib/utils'
import { type ApplicationStageValue } from './constants'

export function KanbanColumn({
  stage,
  count,
  children,
}: {
  stage: ApplicationStageValue
  count: number
  children: React.ReactNode
}) {
  const { t } = useTranslation('applications')
  const { setNodeRef, isOver } = useDroppable({ id: stage })

  return (
    <div className='w-72 shrink-0'>
      <div className='mb-2 flex items-center justify-between px-1'>
        <h2 className='text-sm font-semibold'>
          {t(`stage.${stage}`)}
        </h2>
        <Badge variant='secondary'>{count}</Badge>
      </div>
      <div
        ref={setNodeRef}
        className={cn(
          'min-h-24 space-y-2 rounded-md p-1 transition-colors',
          isOver && 'bg-accent-jade/10 outline-2 outline-dashed outline-accent-jade/40'
        )}
      >
        {children}
        {count === 0 && (
          <div className='rounded-md border border-dashed p-4 text-center text-xs text-muted-foreground'>
            {t('emptyColumn')}
          </div>
        )}
      </div>
    </div>
  )
}
