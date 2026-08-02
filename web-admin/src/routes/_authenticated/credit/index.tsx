import { createFileRoute } from '@tanstack/react-router'
import { Credit } from '@/features/credit'

export const Route = createFileRoute('/_authenticated/credit/')({
  component: Credit,
})
