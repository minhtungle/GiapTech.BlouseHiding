import { createFileRoute } from '@tanstack/react-router'
import { OpsVerification } from '@/features/ops-verification'

export const Route = createFileRoute('/_authenticated/ops/verification')({
  component: OpsVerification,
})
