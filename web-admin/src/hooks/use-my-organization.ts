import { useQuery } from '@tanstack/react-query'
import { organizationsApi } from '@/lib/api'

// Employer hiện tại chỉ thuộc 1 tổ chức trong luồng MVP (mời thêm HR chưa làm — xem
// docs/nghiep-vu/TIEN-DO-DU-AN.md) — lấy tổ chức đầu tiên user là member. Vận hành (admin/moderator)
// không thuộc tổ chức nào, hook này trả undefined cho họ (đúng, vì họ không cần organizationId).
export function useMyOrganization() {
  const { data, isLoading } = useQuery({
    queryKey: ['organizations', 'mine'],
    queryFn: organizationsApi.getMine,
  })

  return { organization: data?.[0], isLoading }
}
