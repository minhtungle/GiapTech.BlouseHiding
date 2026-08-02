import { getTranslations } from "next-intl/server";
import { ShieldCheck, ShieldAlert, ShieldQuestion } from "lucide-react";
import { SiteHeader } from "@/components/site-header";
import { SiteFooter } from "@/components/site-footer";
import { AccountNav } from "@/components/account-nav";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import { MOCK_CANDIDATE } from "@/lib/mock-data";

const VERIFY_META = {
  verified: {
    icon: ShieldCheck,
    className: "bg-accent-jade text-white",
    labelKey: "verifyStatusVerified",
  },
  pending: {
    icon: ShieldQuestion,
    className: "bg-amber-pending text-white",
    labelKey: "verifyStatusPending",
  },
  rejected: {
    icon: ShieldAlert,
    className: "bg-accent-seal text-white",
    labelKey: "verifyStatusRejected",
  },
} as const;

export default async function ProfilePage() {
  const t = await getTranslations("profile");
  const { license } = MOCK_CANDIDATE;
  const verifyMeta = VERIFY_META[license.verifyStatus];
  const VerifyIcon = verifyMeta.icon;

  return (
    <>
      <SiteHeader />
      <main className="flex-1">
        <div className="mx-auto max-w-5xl px-4 py-8 sm:px-6">
          <h1 className="mb-6 font-heading text-2xl font-semibold text-ink">
            {t("title")}
          </h1>

          <AccountNav active="profile" />

          <div className="mt-6 grid gap-6 sm:grid-cols-2">
            <Card>
              <CardHeader>
                <CardTitle className="text-base">Thông tin chung</CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="space-y-1.5">
                  <Label htmlFor="fullName">Họ và tên</Label>
                  <Input id="fullName" defaultValue={MOCK_CANDIDATE.fullName} />
                </div>
                <div className="space-y-1.5">
                  <Label htmlFor="headline">Chức danh</Label>
                  <Input id="headline" defaultValue={MOCK_CANDIDATE.headline} />
                </div>
                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-1.5">
                    <Label htmlFor="years">{t("yearsExperience")}</Label>
                    <Input
                      id="years"
                      type="number"
                      defaultValue={MOCK_CANDIDATE.yearsOfExperience}
                    />
                  </div>
                  <div className="space-y-1.5">
                    <Label htmlFor="location">Địa điểm</Label>
                    <Input id="location" defaultValue={MOCK_CANDIDATE.location} />
                  </div>
                </div>
                <Button>Lưu thay đổi</Button>
              </CardContent>
            </Card>

            <Card>
              <CardHeader className="flex flex-row items-center justify-between">
                <CardTitle className="text-base">
                  Chứng chỉ hành nghề (CCHN)
                </CardTitle>
                <Badge className={`gap-1 ${verifyMeta.className}`}>
                  <VerifyIcon className="size-3" />
                  {t(verifyMeta.labelKey)}
                </Badge>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="space-y-1.5">
                  <Label htmlFor="licenseNumber">{t("licenseNumber")}</Label>
                  <Input id="licenseNumber" defaultValue={license.number} disabled />
                </div>
                <div className="space-y-1.5">
                  <Label htmlFor="issuedBy">{t("issuedBy")}</Label>
                  <Input id="issuedBy" defaultValue={license.issuedBy} disabled />
                </div>
                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-1.5">
                    <Label htmlFor="issuedAt">{t("issuedAt")}</Label>
                    <Input id="issuedAt" defaultValue={license.issuedAt} disabled />
                  </div>
                  <div className="space-y-1.5">
                    <Label htmlFor="expiresAt">{t("expiresAt")}</Label>
                    <Input id="expiresAt" defaultValue={license.expiresAt} disabled />
                  </div>
                </div>
                <Separator />
                <p className="text-xs text-ink-muted">
                  Ảnh chứng chỉ đã tải lên — chờ đội Vận hành đối chiếu khi có
                  thay đổi. Không thể tự sửa các trường CCHN sau khi đã xác
                  thực.
                </p>
                <Button variant="outline">Tải lại ảnh CCHN</Button>
              </CardContent>
            </Card>
          </div>
        </div>
      </main>
      <SiteFooter />
    </>
  );
}
