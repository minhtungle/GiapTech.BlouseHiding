// Khớp Domain.Enums.ApplicationStage phía backend — xem docs/backend/API-DESIGN.md mục 8.
export type ApplicationStageValue =
  | 'New'
  | 'Reviewing'
  | 'Shortlisted'
  | 'Interview'
  | 'Offer'
  | 'Hired'
  | 'Rejected'

// Nhãn hiển thị lấy qua t(`stage.${value}`) trong namespace "applications" — xem src/messages/.

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
