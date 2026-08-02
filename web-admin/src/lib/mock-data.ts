// Mock data tạm thời cho UI Shell (Giai đoạn 0.1) — thay bằng gọi API thật ở Giai đoạn 0.2.
// Xem docs/nghiep-vu/DANH-SACH-TINH-NANG.md mục "0.1 UI Shell".

export const MOCK_SPECIALTIES = [
  'Hồi sức cấp cứu',
  'Dược',
  'Xét nghiệm',
  'Nội tổng quát',
  'Ngoại khoa',
  'Sản phụ khoa',
]

export const MOCK_LOCATIONS = [
  'TP. Hồ Chí Minh',
  'Hà Nội',
  'Đà Nẵng',
  'Cần Thơ',
]

export type MockJobPackage = {
  id: string
  name: string
  priceLabel: string
  durationDays: number
  note: string
}

export const MOCK_JOB_PACKAGES: MockJobPackage[] = [
  { id: 'pkg-free', name: 'Free', priceLabel: '0đ', durationDays: 7, note: 'Giới hạn hiển thị, không ưu tiên' },
  { id: 'pkg-eco', name: 'Eco', priceLabel: '490.000đ', durationDays: 30, note: 'Ưu tiên thấp' },
  { id: 'pkg-pro', name: 'Pro', priceLabel: '990.000đ', durationDays: 30, note: 'Ưu tiên cao' },
  { id: 'pkg-max', name: 'Max', priceLabel: '1.990.000đ', durationDays: 45, note: 'Ghim đầu trang tìm kiếm' },
]

export type JobStatus =
  | 'draft'
  | 'pending_payment'
  | 'pending'
  | 'published'
  | 'rejected'
  | 'expired'
  | 'closed'
  | 'suspended'

export type MockJob = {
  id: string
  title: string
  status: JobStatus
  packageName: string
  applicantsCount: number
  publishedAt: string | null
}

export const JOB_STATUS_LABEL: Record<JobStatus, string> = {
  draft: 'Nháp',
  pending_payment: 'Chờ thanh toán',
  pending: 'Chờ duyệt',
  published: 'Đang tuyển',
  rejected: 'Bị từ chối',
  expired: 'Hết hạn',
  closed: 'Đã đóng',
  suspended: 'Bị ẩn',
}

export const MOCK_ADMIN_JOBS: MockJob[] = [
  { id: 'job-1', title: 'Điều dưỡng ICU — Ca đêm', status: 'published', packageName: 'Pro', applicantsCount: 12, publishedAt: '2026-07-28' },
  { id: 'job-2', title: 'Điều dưỡng Nội tổng quát', status: 'pending', packageName: 'Eco', applicantsCount: 0, publishedAt: null },
  { id: 'job-3', title: 'Kỹ thuật viên xét nghiệm', status: 'pending_payment', packageName: 'Pro', applicantsCount: 0, publishedAt: null },
  { id: 'job-4', title: 'Hộ lý ca ngày', status: 'draft', packageName: '—', applicantsCount: 0, publishedAt: null },
  { id: 'job-5', title: 'Dược sĩ nhà thuốc', status: 'expired', packageName: 'Free', applicantsCount: 4, publishedAt: '2026-06-10' },
]

export type ApplicationStage =
  | 'moi'
  | 'dang_xem'
  | 'phu_hop'
  | 'phong_van'
  | 'offer'
  | 'trung_tuyen'
  | 'tu_choi'

export const APPLICATION_STAGE_LABEL: Record<ApplicationStage, string> = {
  moi: 'Mới',
  dang_xem: 'Đang xem',
  phu_hop: 'Phù hợp',
  phong_van: 'Hẹn phỏng vấn',
  offer: 'Offer',
  trung_tuyen: 'Trúng tuyển',
  tu_choi: 'Từ chối',
}

export const APPLICATION_STAGES: ApplicationStage[] = [
  'moi',
  'dang_xem',
  'phu_hop',
  'phong_van',
  'offer',
  'trung_tuyen',
]

export type MockApplication = {
  id: string
  jobId: string
  jobTitle: string
  candidateName: string
  specialty: string
  yearsOfExperience: number
  stage: ApplicationStage
  appliedAt: string
}

export const MOCK_APPLICATIONS: MockApplication[] = [
  { id: 'app-1', jobId: 'job-1', jobTitle: 'Điều dưỡng ICU — Ca đêm', candidateName: 'Nguyễn Thị Thu Hà', specialty: 'Hồi sức cấp cứu', yearsOfExperience: 4, stage: 'phong_van', appliedAt: '2026-07-29' },
  { id: 'app-2', jobId: 'job-1', jobTitle: 'Điều dưỡng ICU — Ca đêm', candidateName: 'Trần Văn Long', specialty: 'Hồi sức cấp cứu', yearsOfExperience: 2, stage: 'moi', appliedAt: '2026-07-31' },
  { id: 'app-3', jobId: 'job-1', jobTitle: 'Điều dưỡng ICU — Ca đêm', candidateName: 'Lê Thị Mai', specialty: 'Cấp cứu', yearsOfExperience: 6, stage: 'phu_hop', appliedAt: '2026-07-30' },
  { id: 'app-4', jobId: 'job-1', jobTitle: 'Điều dưỡng ICU — Ca đêm', candidateName: 'Phạm Đức Anh', specialty: 'Hồi sức cấp cứu', yearsOfExperience: 3, stage: 'dang_xem', appliedAt: '2026-08-01' },
  { id: 'app-5', jobId: 'job-1', jobTitle: 'Điều dưỡng ICU — Ca đêm', candidateName: 'Võ Thị Ngọc', specialty: 'ICU', yearsOfExperience: 5, stage: 'offer', appliedAt: '2026-07-27' },
  { id: 'app-6', jobId: 'job-1', jobTitle: 'Điều dưỡng ICU — Ca đêm', candidateName: 'Đặng Minh Quân', specialty: 'Hồi sức cấp cứu', yearsOfExperience: 8, stage: 'trung_tuyen', appliedAt: '2026-07-20' },
]

export const MOCK_CREDIT_WALLET = {
  balance: 340,
  transactions: [
    { id: 'tx-1', reason: 'purchase' as const, amount: 500, createdAt: '2026-07-15' },
    { id: 'tx-2', reason: 'unlock_profile' as const, amount: -20, createdAt: '2026-07-29' },
    { id: 'tx-3', reason: 'unlock_profile' as const, amount: -20, createdAt: '2026-07-30' },
    { id: 'tx-4', reason: 'unlock_profile' as const, amount: -20, createdAt: '2026-08-01' },
    { id: 'tx-5', reason: 'refund' as const, amount: 20, createdAt: '2026-08-01' },
  ],
}

export type VerifyStatus = 'pending' | 'verified' | 'rejected'

export type MockLicenseQueueItem = {
  id: string
  candidateName: string
  licenseNumber: string
  specialty: string
  submittedAt: string
  status: VerifyStatus
}

export const MOCK_LICENSE_QUEUE: MockLicenseQueueItem[] = [
  { id: 'lic-1', candidateName: 'Nguyễn Thị Thu Hà', licenseNumber: 'CCHN-079123456', specialty: 'Hồi sức cấp cứu', submittedAt: '2026-08-01', status: 'pending' },
  { id: 'lic-2', candidateName: 'Trần Văn Long', licenseNumber: 'CCHN-079654321', specialty: 'Hồi sức cấp cứu', submittedAt: '2026-08-02', status: 'pending' },
  { id: 'lic-3', candidateName: 'Lê Thị Mai', licenseNumber: 'CCHN-048112233', specialty: 'Cấp cứu', submittedAt: '2026-07-31', status: 'pending' },
]

export type MockOrgQueueItem = {
  id: string
  name: string
  type: string
  submittedAt: string
  status: VerifyStatus
}

export const MOCK_ORG_QUEUE: MockOrgQueueItem[] = [
  { id: 'org-5', name: 'Phòng khám Đa khoa Sài Gòn', type: 'Phòng khám', submittedAt: '2026-08-01', status: 'pending' },
  { id: 'org-6', name: 'Nhà thuốc Pharmacity Q3', type: 'Nhà thuốc', submittedAt: '2026-08-02', status: 'pending' },
]

export type MockPaymentQueueItem = {
  id: string
  referenceCode: string
  organizationName: string
  type: 'job_package' | 'credit_topup'
  amount: number
  createdAt: string
}

export const MOCK_PAYMENT_QUEUE: MockPaymentQueueItem[] = [
  { id: 'pay-1', referenceCode: 'PAY-7F3K2Q', organizationName: 'Phòng khám Đa khoa Việt Đức', type: 'job_package', amount: 990000, createdAt: '2026-08-02' },
  { id: 'pay-2', referenceCode: 'PAY-9M1L8R', organizationName: 'Bệnh viện Đa khoa Tâm Đức', type: 'credit_topup', amount: 2000000, createdAt: '2026-08-02' },
]

export type MockReportQueueItem = {
  id: string
  targetType: 'job' | 'organization' | 'profile' | 'message'
  targetLabel: string
  reason: string
  reportedAt: string
}

export const MOCK_REPORT_QUEUE: MockReportQueueItem[] = [
  { id: 'rep-1', targetType: 'job', targetLabel: 'Điều dưỡng ICU — lương 50 triệu (tin nghi ngờ)', reason: 'Mức lương không hợp lý, nghi spam', reportedAt: '2026-08-01' },
  { id: 'rep-2', targetType: 'organization', targetLabel: 'Phòng khám ABC (chưa xác thực)', reason: 'Nghi ngờ giấy phép giả', reportedAt: '2026-08-02' },
]
