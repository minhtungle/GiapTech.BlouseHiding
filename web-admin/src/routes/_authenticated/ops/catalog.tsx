import { createFileRoute } from '@tanstack/react-router'
import { OpsCatalog } from '@/features/ops-catalog'

export const Route = createFileRoute('/_authenticated/ops/catalog')({
  component: OpsCatalog,
})
