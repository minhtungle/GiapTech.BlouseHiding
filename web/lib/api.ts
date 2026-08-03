// Client gọi backend thật — dùng ở Server Component nên có thể gửi kèm Accept-Language khớp
// locale đang render (xem docs/backend/CONG-NGHE-BACKEND.md mục Đa ngôn ngữ).
const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5100/api/v1";

async function apiGet<T>(path: string, locale: string): Promise<T | null> {
  try {
    const res = await fetch(`${API_BASE_URL}${path}`, {
      headers: { "Accept-Language": locale },
      // Danh mục ít đổi — cache ngắn thay vì no-store, tránh gọi lại mỗi request
      // (xem ERD-CHI-TIET.md mục ràng buộc nghiệp vụ điểm 12, Redis cache dời sau).
      next: { revalidate: 300 },
    });
    if (!res.ok) return null;
    return (await res.json()) as T;
  } catch {
    // Backend chưa chạy hoặc lỗi mạng — để trang gọi vẫn dùng fallback thay vì crash.
    return null;
  }
}

export type ApiSpecialty = { id: string; code: string; name: string; parentId: string | null };
export type ApiLocation = { id: string; name: string; parentId: string | null };
export type ApiJobPackage = {
  id: string;
  tier: string;
  name: string;
  durationDays: number;
  price: number;
  maxActiveJobs: number | null;
  perks: Record<string, boolean>;
};

export async function getSpecialties(locale: string): Promise<ApiSpecialty[]> {
  return (await apiGet<ApiSpecialty[]>("/catalog/specialties", locale)) ?? [];
}

export async function getLocations(locale: string): Promise<ApiLocation[]> {
  return (await apiGet<ApiLocation[]>("/catalog/locations", locale)) ?? [];
}

export async function getJobPackages(locale: string): Promise<ApiJobPackage[]> {
  return (await apiGet<ApiJobPackage[]>("/catalog/job-packages", locale)) ?? [];
}

export async function getEmploymentTypes(locale: string): Promise<string[]> {
  return (await apiGet<string[]>("/catalog/employment-types", locale)) ?? [];
}

export type ApiJob = {
  id: string;
  organizationId: string;
  organizationName: string;
  title: string;
  specialtyName: string;
  locationName: string;
  employmentType: string;
  salaryMin: number | null;
  salaryMax: number | null;
  salaryNegotiable: boolean;
  requiredLicense: boolean;
  minExperienceYears: number;
  description: string;
  requirements: string | null;
  benefits: string | null;
  status: string;
  rejectReason: string | null;
  publishedAt: string | null;
  expiresAt: string | null;
};

export type JobSearchParams = {
  specialty?: string;
  location?: string;
  organizationId?: string;
  employmentType?: string;
  salaryMin?: number;
  keyword?: string;
};

export async function getJobs(locale: string, params: JobSearchParams = {}): Promise<ApiJob[]> {
  const query = new URLSearchParams();
  if (params.specialty) query.set("specialty", params.specialty);
  if (params.location) query.set("location", params.location);
  if (params.organizationId) query.set("organizationId", params.organizationId);
  if (params.employmentType) query.set("employmentType", params.employmentType);
  if (params.salaryMin) query.set("salaryMin", String(params.salaryMin));
  if (params.keyword) query.set("keyword", params.keyword);

  const qs = query.toString();
  return (await apiGet<ApiJob[]>(`/jobs${qs ? `?${qs}` : ""}`, locale)) ?? [];
}

export async function getJobById(id: string, locale: string): Promise<ApiJob | null> {
  return apiGet<ApiJob>(`/jobs/${id}`, locale);
}

// Danh sách tin CÔNG KHAI (chỉ published) của 1 tổ chức — dùng GET /jobs?organizationId=...
// (không phải GET /organizations/{id}/jobs, endpoint đó yêu cầu member org và trả mọi trạng thái).
export async function getOrganizationJobs(organizationId: string, locale: string): Promise<ApiJob[]> {
  return getJobs(locale, { organizationId });
}

export type ApiOrganization = {
  id: string;
  name: string;
  orgType: string;
  description: string | null;
  logoUrl: string | null;
  coverUrl: string | null;
  address: string | null;
  verifyStatus: string;
};

export async function getOrganizationById(id: string, locale: string): Promise<ApiOrganization | null> {
  return apiGet<ApiOrganization>(`/organizations/${id}`, locale);
}
