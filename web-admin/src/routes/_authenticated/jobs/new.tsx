import { createFileRoute } from '@tanstack/react-router'
import { NewJob } from '@/features/jobs/new'

export const Route = createFileRoute('/_authenticated/jobs/new')({
  component: NewJob,
})
