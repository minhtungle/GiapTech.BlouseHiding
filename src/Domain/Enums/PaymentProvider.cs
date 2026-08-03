namespace GiapTech.BlouseHiding.Domain.Enums;

// manual_transfer là provider duy nhất hoạt động ở MVP — vnpay/momo/zalopay dự phòng cho khi có
// cổng tự động (xem ADR-0003), chưa có implementation nào dùng tới.
public enum PaymentProvider
{
    ManualTransfer,
    Vnpay,
    Momo,
    Zalopay,
}
