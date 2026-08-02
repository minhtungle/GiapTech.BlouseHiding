import { createFileRoute } from '@tanstack/react-router'
import { OpsPayments } from '@/features/ops-payments'

export const Route = createFileRoute('/_authenticated/ops/payments')({
  component: OpsPayments,
})
