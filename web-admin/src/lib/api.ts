// Client gọi backend thật qua axios instance dùng chung (lib/http.ts) — tự gắn Bearer token +
// tự refresh khi hết hạn. App này chỉ tiếng Việt (ADR-0008 mục 5), http.ts đã gửi sẵn Accept-Language.
import { http } from '@/lib/http'
import type { AuthUser } from '@/stores/auth-store'

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
  getSpecialties: () => http.get<ApiSpecialty[]>('/catalog/specialties').then((r) => r.data),
  getLocations: () => http.get<ApiLocation[]>('/catalog/locations').then((r) => r.data),
  getJobPackages: () => http.get<ApiJobPackage[]>('/catalog/job-packages').then((r) => r.data),
  getEmploymentTypes: () => http.get<string[]>('/catalog/employment-types').then((r) => r.data),
}

export type LoginResult = { accessToken: string; refreshToken: string }

export const authApi = {
  login: (email: string, password: string) =>
    http.post<LoginResult>('/auth/login', { email, password }).then((r) => r.data),
  me: () => http.get<AuthUser>('/users/me').then((r) => r.data),
  logout: (refreshToken: string) => http.post('/auth/logout', { refreshToken }),
}

export type ApiJob = {
  id: string
  organizationId: string
  organizationName: string
  title: string
  specialtyName: string
  locationName: string
  employmentType: string
  salaryMin: number | null
  salaryMax: number | null
  salaryNegotiable: boolean
  requiredLicense: boolean
  minExperienceYears: number
  description: string
  requirements: string | null
  benefits: string | null
  status: string
  rejectReason: string | null
  publishedAt: string | null
  expiresAt: string | null
}

export type CreateJobInput = {
  organizationId: string
  title: string
  specialtyId: string
  employmentType: string
  salaryMin: number | null
  salaryMax: number | null
  salaryNegotiable: boolean
  locationId: string
  addressDetail: string | null
  requiredLicense: boolean
  minExperienceYears: number
  description: string
  requirements: string | null
  benefits: string | null
}

export const jobsApi = {
  getOrganizationJobs: (organizationId: string) =>
    http.get<ApiJob[]>(`/organizations/${organizationId}/jobs`).then((r) => r.data),
  getById: (jobId: string) => http.get<ApiJob>(`/jobs/${jobId}`).then((r) => r.data),
  create: (input: CreateJobInput) => http.post<string>('/jobs', input).then((r) => r.data),
  update: (jobId: string, input: Omit<CreateJobInput, 'organizationId'>) =>
    http.put(`/jobs/${jobId}`, input),
  submit: (jobId: string, packageId: string) =>
    http.post(`/jobs/${jobId}/submit`, { packageId }),
  close: (jobId: string) => http.post(`/jobs/${jobId}/close`),
  renew: (jobId: string) => http.post<string>(`/jobs/${jobId}/renew`).then((r) => r.data),
}

export type ApiApplication = {
  id: string
  jobId: string
  jobTitle: string
  candidateId: string
  candidateFullName: string
  coverLetter: string | null
  stage: string
  score: number | null
  rejectedReason: string | null
  appliedAt: string
}

export const applicationsApi = {
  getByJob: (jobId: string) =>
    http.get<ApiApplication[]>(`/jobs/${jobId}/applications`).then((r) => r.data),
  transitionStage: (
    applicationId: string,
    stage: string,
    silent: boolean,
    rejectedReason: string | null = null
  ) => http.patch(`/applications/${applicationId}/stage`, { stage, silent, rejectedReason }),
  addNote: (applicationId: string, noteText: string) =>
    http.post(`/applications/${applicationId}/notes`, { noteText }),
  score: (applicationId: string, score: number) =>
    http.patch(`/applications/${applicationId}/score`, { score }),
}

export type ApiOrganization = {
  id: string
  name: string
  orgType: string
  description: string | null
  logoUrl: string | null
  coverUrl: string | null
  address: string | null
  verifyStatus: string
}

export type ApiCreditWallet = { organizationId: string; balance: number }
export type ApiCreditTransaction = {
  id: string
  amount: number
  reason: string
  createdAt: string
}

export type MyOrganization = {
  id: string
  name: string
  orgType: string
  verifyStatus: string
  memberRole: string
}

export type OrganizationMember = {
  id: string
  userId: string
  email: string
  memberRole: 'Owner' | 'HrManager' | 'HrMember'
  joinedAt: string | null
}
export type PendingMemberInvitation = {
  id: string
  email: string
  invitedRole: 'HrManager' | 'HrMember'
  expiresAt: string
}
export type OrganizationMembers = {
  members: OrganizationMember[]
  pendingInvitations: PendingMemberInvitation[]
}

export const organizationsApi = {
  getMine: () => http.get<MyOrganization[]>('/organizations/mine').then((r) => r.data),
  create: (input: {
    name: string
    orgType: string
    licenseNo: string | null
    size: string | null
  }) => http.post<string>('/organizations', input).then((r) => r.data),
  getById: (organizationId: string) =>
    http.get<ApiOrganization>(`/organizations/${organizationId}`).then((r) => r.data),
  getCreditWallet: (organizationId: string) =>
    http.get<ApiCreditWallet>(`/organizations/${organizationId}/credit-wallet`).then((r) => r.data),
  getCreditTransactions: (organizationId: string) =>
    http
      .get<ApiCreditTransaction[]>(`/organizations/${organizationId}/credit-transactions`)
      .then((r) => r.data),
  getMembers: (organizationId: string) =>
    http.get<OrganizationMembers>(`/organizations/${organizationId}/members`).then((r) => r.data),
  inviteMember: (organizationId: string, email: string, invitedRole: 'HrManager' | 'HrMember') =>
    http.post<string>(`/organizations/${organizationId}/members/invite`, { email, invitedRole }),
  removeMember: (organizationId: string, memberId: string) =>
    http.delete(`/organizations/${organizationId}/members/${memberId}`),
}

export type CandidateSearchResult = {
  id: string
  headline: string | null
  isUnlocked: boolean
  unlockCost: number
  contactEmail: string | null
}

export const candidatesApi = {
  search: (organizationId: string, specialty?: string, location?: string) =>
    http
      .get<CandidateSearchResult[]>('/candidates/search', {
        params: { organizationId, specialty, location },
      })
      .then((r) => r.data),
  unlock: (candidateId: string, organizationId: string) =>
    http.post<string>(`/candidates/${candidateId}/unlock`, { organizationId }).then((r) => r.data),
}

export type PendingLicense = {
  id: string
  profileId: string
  candidateFullName: string
  licenseNo: string
  issuedBy: string
  documentUrl: string
  issuedAt: string
}

export type PendingOrganization = {
  id: string
  name: string
  orgType: string
  licenseNo: string | null
}

export type PendingJob = {
  id: string
  title: string
  organizationName: string
  description: string
}

export const opsApi = {
  getPendingLicenses: () => http.get<PendingLicense[]>('/ops/licenses').then((r) => r.data),
  verifyLicense: (licenseId: string, approved: boolean, rejectReason: string | null = null) =>
    http.post(`/ops/licenses/${licenseId}/verify`, { approved, rejectReason }),
  getPendingOrganizations: () =>
    http.get<PendingOrganization[]>('/ops/organizations').then((r) => r.data),
  verifyOrganization: (
    organizationId: string,
    action: 'Verify' | 'Reject' | 'Suspend',
    rejectReason: string | null = null
  ) => http.post(`/ops/organizations/${organizationId}/verify`, { action, rejectReason }),
  getPendingJobs: () => http.get<PendingJob[]>('/ops/jobs').then((r) => r.data),
  moderateJob: (jobId: string, approved: boolean, rejectReason: string | null = null) =>
    http.post(`/ops/jobs/${jobId}/moderate`, { approved, rejectReason }),
}

// 5 locale không phải "vi" (vi nằm ở field name gốc) — xem CatalogLocales.cs.
export type CatalogTranslations = Partial<Record<'en' | 'ja' | 'zh' | 'ko' | 'es', string>>

export type CreateSpecialtyInput = {
  code: string
  name: string
  parentId?: string | null
  translations?: CatalogTranslations
}
export type UpdateSpecialtyInput = Omit<CreateSpecialtyInput, 'code'>

export type CreateLocationInput = {
  name: string
  parentId?: string | null
  translations?: CatalogTranslations
}
export type UpdateLocationInput = CreateLocationInput

export type CreateJobPackageInput = {
  tier: 'Free' | 'Eco' | 'Pro' | 'Max'
  name: string
  durationDays: number
  price: number
  maxActiveJobs?: number | null
  perks?: Record<string, boolean>
  translations?: CatalogTranslations
}
export type UpdateJobPackageInput = Omit<CreateJobPackageInput, 'tier'>

export const opsCatalogApi = {
  createSpecialty: (input: CreateSpecialtyInput) =>
    http.post<string>('/ops/catalog/specialties', input).then((r) => r.data),
  updateSpecialty: (id: string, input: UpdateSpecialtyInput) =>
    http.put(`/ops/catalog/specialties/${id}`, input),
  createLocation: (input: CreateLocationInput) =>
    http.post<string>('/ops/catalog/locations', input).then((r) => r.data),
  updateLocation: (id: string, input: UpdateLocationInput) =>
    http.put(`/ops/catalog/locations/${id}`, input),
  createJobPackage: (input: CreateJobPackageInput) =>
    http.post<string>('/ops/job-packages', input).then((r) => r.data),
  updateJobPackage: (id: string, input: UpdateJobPackageInput) =>
    http.put(`/ops/job-packages/${id}`, input),
}
