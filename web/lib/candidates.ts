import { backendFetch } from "@/lib/backend-fetch";

export type ApiLicense = {
  id: string;
  licenseNo: string;
  verifyStatus: string;
  expiredAt: string | null;
  rejectReason: string | null;
};

export type ApiProfileSpecialty = {
  specialtyId: string;
  code: string;
  name: string;
  level: string;
};

export type ApiCandidateProfile = {
  id: string;
  fullName: string;
  headline: string | null;
  summary: string | null;
  completionPct: number;
  specialties: ApiProfileSpecialty[];
  licenses: ApiLicense[];
};

export type ApiApplication = {
  id: string;
  jobId: string;
  jobTitle: string;
  candidateId: string;
  candidateFullName: string;
  coverLetter: string | null;
  stage: string;
  score: number | null;
  rejectedReason: string | null;
  appliedAt: string;
};

// Dùng trong Server Component — 401/404 (chưa tạo hồ sơ) trả null thay vì throw, để trang tự quyết
// định hiển thị "chưa có hồ sơ" thay vì crash.
export async function getMyCandidateProfile(): Promise<ApiCandidateProfile | null> {
  const res = await backendFetch("/candidates/me", { cache: "no-store" });
  if (!res.ok) return null;
  return (await res.json()) as ApiCandidateProfile;
}

export async function getMyApplications(): Promise<ApiApplication[]> {
  const res = await backendFetch("/candidates/me/applications", { cache: "no-store" });
  if (!res.ok) return [];
  return (await res.json()) as ApiApplication[];
}
