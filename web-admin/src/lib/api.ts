// Client gọi backend thật — app này chỉ tiếng Việt (ADR-0008 mục 5), luôn gửi Accept-Language: vi.
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5100/api/v1'

async function apiGet<T>(path: string): Promise<T> {
  const res = await fetch(`${API_BASE_URL}${path}`, {
    headers: { 'Accept-Language': 'vi' },
  })
  if (!res.ok) {
    throw new Error(`GET ${path} thất bại: ${res.status}`)
  }
  return (await res.json()) as T
}

export type ApiSpecialty = { id: string; code: string; name: string; parentId: string | null }
export type ApiLocation = { id: string; name: string; parentId: string | null }
export type ApiJobPackage = {
  id: string
  tier: string
  name: string
  durationDays: number
  price: number
  maxActiveJobs: number | null
  perks: Record<string, boolean>
}

export const catalogApi = {
  getSpecialties: () => apiGet<ApiSpecialty[]>('/catalog/specialties'),
  getLocations: () => apiGet<ApiLocation[]>('/catalog/locations'),
  getJobPackages: () => apiGet<ApiJobPackage[]>('/catalog/job-packages'),
}
