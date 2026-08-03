// Mock data tạm thời cho các màn hình chưa có bounded context backend tương ứng (Report) — xem
// docs/nghiep-vu/TIEN-DO-DU-AN.md.

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
