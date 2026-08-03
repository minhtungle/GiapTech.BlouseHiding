// Khớp Domain.Enums.ApplicationStage phía backend — xem docs/backend/API-DESIGN.md mục 8.
export type ApplicationStageValue =
  | 'New'
  | 'Reviewing'
  | 'Shortlisted'
  | 'Interview'
  | 'Offer'
  | 'Hired'
  | 'Rejected'

export const APPLICATION_STAGE_LABEL: Record<ApplicationStageValue, string> = {
  New: 'Mới',
  Reviewing: 'Đang xem',
  Shortlisted: 'Phù hợp',
  Interview: 'Hẹn phỏng vấn',
  Offer: 'Offer',
  Hired: 'Trúng tuyển',
  Rejected: 'Từ chối',
}

// Không đưa "Rejected" vào cột Kanban chính — kéo sang cột nào cũng là bước tiến, từ chối xử lý qua
// hành động riêng (không nằm trong phạm vi MVP hiện tại, để nguyên trong danh sách nhưng không cột).
export const APPLICATION_STAGES: ApplicationStageValue[] = [
  'New',
  'Reviewing',
  'Shortlisted',
  'Interview',
  'Offer',
  'Hired',
]
