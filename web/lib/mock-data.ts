// Mock data tạm thời cho UI Shell (Giai đoạn 0.1) — thay bằng gọi API thật ở Giai đoạn 0.2.
// Xem docs/nghiep-vu/DANH-SACH-TINH-NANG.md mục "0.1 UI Shell".

export type MockJob = {
  id: string;
  title: string;
  organizationName: string;
  specialty: string;
  location: string;
  employmentType: string;
  salaryLabel: string;
  requiresLicense: boolean;
  publishedAt: string;
};

export const MOCK_JOBS: MockJob[] = [
  {
    id: "job-1",
    title: "Điều dưỡng ICU — Ca đêm",
    organizationName: "Bệnh viện Đa khoa Tâm Đức",
    specialty: "Hồi sức cấp cứu",
    location: "TP. Hồ Chí Minh",
    employmentType: "Toàn thời gian",
    salaryLabel: "18 - 25 triệu",
    requiresLicense: true,
    publishedAt: "2026-07-28",
  },
  {
    id: "job-2",
    title: "Dược sĩ nhà thuốc",
    organizationName: "Nhà thuốc Long Châu Q7",
    specialty: "Dược",
    location: "TP. Hồ Chí Minh",
    employmentType: "Toàn thời gian",
    salaryLabel: "12 - 16 triệu",
    requiresLicense: true,
    publishedAt: "2026-07-30",
  },
  {
    id: "job-3",
    title: "Kỹ thuật viên xét nghiệm",
    organizationName: "Phòng khám Đa khoa Việt Đức",
    specialty: "Xét nghiệm",
    location: "Hà Nội",
    employmentType: "Toàn thời gian",
    salaryLabel: "10 - 14 triệu",
    requiresLicense: true,
    publishedAt: "2026-07-25",
  },
  {
    id: "job-4",
    title: "Bác sĩ Nội tổng quát — Locum cuối tuần",
    organizationName: "Phòng khám Quốc tế An Sinh",
    specialty: "Nội tổng quát",
    location: "Đà Nẵng",
    employmentType: "Bán thời gian (Locum)",
    salaryLabel: "Thỏa thuận theo ca",
    requiresLicense: true,
    publishedAt: "2026-07-29",
  },
];

export type MockOrganization = {
  id: string;
  name: string;
  type: string;
  location: string;
  verified: boolean;
  activeJobs: number;
};

export const MOCK_ORGANIZATIONS: MockOrganization[] = [
  { id: "org-1", name: "Bệnh viện Đa khoa Tâm Đức", type: "Bệnh viện", location: "TP. Hồ Chí Minh", verified: true, activeJobs: 6 },
  { id: "org-2", name: "Nhà thuốc Long Châu Q7", type: "Nhà thuốc", location: "TP. Hồ Chí Minh", verified: true, activeJobs: 2 },
  { id: "org-3", name: "Phòng khám Đa khoa Việt Đức", type: "Phòng khám", location: "Hà Nội", verified: true, activeJobs: 3 },
];
