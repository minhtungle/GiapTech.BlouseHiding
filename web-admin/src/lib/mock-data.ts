// Mock data tạm thời cho các màn hình chưa có bounded context backend tương ứng
// (Payments, Report) — xem docs/nghiep-vu/TIEN-DO-DU-AN.md.

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
