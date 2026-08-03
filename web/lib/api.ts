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
