// Mock data tạm thời cho UI Shell (Giai đoạn 0.1) — thay bằng gọi API thật ở Giai đoạn 0.2.
// Xem docs/nghiep-vu/DANH-SACH-TINH-NANG.md mục "0.1 UI Shell".

export type MockJob = {
  id: string;
  title: string;
  organizationId: string;
  organizationName: string;
  organizationVerified: boolean;
  specialty: string;
  location: string;
  employmentType: string;
  salaryLabel: string;
  requiresLicense: boolean;
  publishedAt: string;
  description: string;
  requirements: string[];
  benefits: string[];
};

export const MOCK_JOBS: MockJob[] = [
  {
    id: "job-1",
    title: "Điều dưỡng ICU — Ca đêm",
    organizationId: "org-1",
    organizationName: "Bệnh viện Đa khoa Tâm Đức",
    organizationVerified: true,
    specialty: "Hồi sức cấp cứu",
    location: "TP. Hồ Chí Minh",
    employmentType: "Toàn thời gian",
    salaryLabel: "18 - 25 triệu",
    requiresLicense: true,
    publishedAt: "2026-07-28",
    description:
      "Chăm sóc bệnh nhân nặng tại khoa Hồi sức tích cực (ICU), phối hợp cùng bác sĩ trực xử lý cấp cứu, theo dõi sát các chỉ số sinh tồn ca đêm.",
    requirements: [
      "CCHN Điều dưỡng còn hiệu lực",
      "Tối thiểu 2 năm kinh nghiệm ICU/Cấp cứu",
      "Chịu được áp lực ca đêm, xoay ca",
    ],
    benefits: [
      "Phụ cấp ca đêm + độc hại",
      "BHXH đầy đủ từ ngày đầu thử việc",
      "Đào tạo hồi sức nâng cao định kỳ",
    ],
  },
  {
    id: "job-2",
    title: "Dược sĩ nhà thuốc",
    organizationId: "org-2",
    organizationName: "Nhà thuốc Long Châu Q7",
    organizationVerified: true,
    specialty: "Dược",
    location: "TP. Hồ Chí Minh",
    employmentType: "Toàn thời gian",
    salaryLabel: "12 - 16 triệu",
    requiresLicense: true,
    publishedAt: "2026-07-30",
    description:
      "Tư vấn, bán thuốc theo đơn và không đơn, quản lý tồn kho, đảm bảo tuân thủ quy định dược tại nhà thuốc.",
    requirements: [
      "CCHN Dược sĩ còn hiệu lực",
      "Ưu tiên có kinh nghiệm bán lẻ dược phẩm",
    ],
    benefits: ["Thưởng doanh số", "Đồng phục + đào tạo sản phẩm mới"],
  },
  {
    id: "job-3",
    title: "Kỹ thuật viên xét nghiệm",
    organizationId: "org-3",
    organizationName: "Phòng khám Đa khoa Việt Đức",
    organizationVerified: true,
    specialty: "Xét nghiệm",
    location: "Hà Nội",
    employmentType: "Toàn thời gian",
    salaryLabel: "10 - 14 triệu",
    requiresLicense: true,
    publishedAt: "2026-07-25",
    description:
      "Thực hiện các xét nghiệm huyết học, sinh hóa, vi sinh cơ bản; đảm bảo an toàn sinh học và trả kết quả đúng hạn.",
    requirements: [
      "CCHN Kỹ thuật viên xét nghiệm",
      "Biết vận hành máy xét nghiệm tự động cơ bản",
    ],
    benefits: ["Không trực đêm", "Nghỉ trưa 1.5 giờ", "Thưởng lễ Tết"],
  },
  {
    id: "job-4",
    title: "Bác sĩ Nội tổng quát — Locum cuối tuần",
    organizationId: "org-4",
    organizationName: "Phòng khám Quốc tế An Sinh",
    organizationVerified: false,
    specialty: "Nội tổng quát",
    location: "Đà Nẵng",
    employmentType: "Bán thời gian (Locum)",
    salaryLabel: "Thỏa thuận theo ca",
    requiresLicense: true,
    publishedAt: "2026-07-29",
    description:
      "Khám và tư vấn nội tổng quát cho bệnh nhân ngoại trú vào ca cuối tuần (thứ 7 + chủ nhật), linh hoạt số ca nhận theo lịch cá nhân.",
    requirements: [
      "CCHN Bác sĩ, chuyên khoa Nội hoặc Đa khoa",
      "Có thể nhận ca cố định cuối tuần hàng tháng",
    ],
    benefits: ["Thù lao theo ca, thanh toán ngay sau ca", "Không ràng buộc hợp đồng dài hạn"],
  },
];

export function getJobById(id: string): MockJob | undefined {
  return MOCK_JOBS.find((j) => j.id === id);
}

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
  { id: "org-4", name: "Phòng khám Quốc tế An Sinh", type: "Phòng khám", location: "Đà Nẵng", verified: false, activeJobs: 1 },
];

export const MOCK_SPECIALTIES = [
  "Hồi sức cấp cứu",
  "Dược",
  "Xét nghiệm",
  "Nội tổng quát",
  "Ngoại khoa",
  "Sản phụ khoa",
  "Nhi khoa",
  "Răng Hàm Mặt",
];

export const MOCK_LOCATIONS = [
  "TP. Hồ Chí Minh",
  "Hà Nội",
  "Đà Nẵng",
  "Cần Thơ",
  "Hải Phòng",
];

export const MOCK_EMPLOYMENT_TYPES = [
  "Toàn thời gian",
  "Bán thời gian (Locum)",
  "Theo ca",
  "Thời vụ",
];

export type MockCandidateProfile = {
  fullName: string;
  headline: string;
  specialty: string;
  yearsOfExperience: number;
  location: string;
  completionPercent: number;
  license: {
    number: string;
    issuedBy: string;
    issuedAt: string;
    expiresAt: string;
    verifyStatus: "pending" | "verified" | "rejected";
  };
};

export const MOCK_CANDIDATE: MockCandidateProfile = {
  fullName: "Nguyễn Thị Thu Hà",
  headline: "Điều dưỡng Hồi sức cấp cứu",
  specialty: "Hồi sức cấp cứu",
  yearsOfExperience: 4,
  location: "TP. Hồ Chí Minh",
  completionPercent: 72,
  license: {
    number: "CCHN-079123456",
    issuedBy: "Sở Y tế TP. Hồ Chí Minh",
    issuedAt: "2020-03-15",
    expiresAt: "2030-03-15",
    verifyStatus: "verified",
  },
};

export type MockApplication = {
  id: string;
  jobId: string;
  jobTitle: string;
  organizationName: string;
  stage: "moi" | "dang_xem" | "phu_hop" | "phong_van" | "offer" | "trung_tuyen" | "tu_choi";
  appliedAt: string;
};

export const MOCK_APPLICATIONS: MockApplication[] = [
  { id: "app-1", jobId: "job-1", jobTitle: "Điều dưỡng ICU — Ca đêm", organizationName: "Bệnh viện Đa khoa Tâm Đức", stage: "phong_van", appliedAt: "2026-07-29" },
  { id: "app-2", jobId: "job-3", jobTitle: "Kỹ thuật viên xét nghiệm", organizationName: "Phòng khám Đa khoa Việt Đức", stage: "moi", appliedAt: "2026-07-31" },
];
