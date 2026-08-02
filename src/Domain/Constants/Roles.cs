namespace GiapTech.BlouseHiding.Domain.Constants;

// Khớp quy ước role backend ở docs/kien-truc/THUAT-NGU.md — role backend KHÔNG phải tên site hiển thị
// (vd role "admin" ở đây là đội Vận hành nội bộ, không phải trang "Admin" của Nhà tuyển dụng).
public abstract class Roles
{
    public const string Candidate = "candidate";
    public const string Employer = "employer";
    public const string Admin = "admin";
    public const string Moderator = "moderator";
}