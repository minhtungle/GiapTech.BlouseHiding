import { createFileRoute } from '@tanstack/react-router'
import { OpsReports } from '@/features/ops-reports'

export const Route = createFileRoute('/_authenticated/ops/reports')({
  component: OpsReports,
})
