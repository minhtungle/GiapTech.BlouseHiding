"use client";

import { useState } from "react";
import { ShieldCheck, ShieldAlert, ShieldQuestion } from "lucide-react";
import { useTranslations } from "next-intl";
import { useRouter } from "@/i18n/navigation";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Separator } from "@/components/ui/separator";
import type { ApiLicense } from "@/lib/candidates";

const VERIFY_META: Record<string, { icon: typeof ShieldCheck; className: string; labelKey: string }> = {
  Verified: {
    icon: ShieldCheck,
    className: "bg-accent-jade text-white",
    labelKey: "verifyStatusVerified",
  },
  Pending: {
    icon: ShieldQuestion,
    className: "bg-amber-pending text-white",
    labelKey: "verifyStatusPending",
  },
  Rejected: {
    icon: ShieldAlert,
    className: "bg-accent-seal text-white",
    labelKey: "verifyStatusRejected",
  },
  Expired: {
    icon: ShieldAlert,
    className: "bg-accent-seal text-white",
    labelKey: "verifyStatusRejected",
  },
};

export function LicenseSection({ licenses }: { licenses: ApiLicense[] }) {
  const t = useTranslations("profile");
  const router = useRouter();

  const [showForm, setShowForm] = useState(licenses.length === 0);
  const [licenseNo, setLicenseNo] = useState("");
  const [issuedBy, setIssuedBy] = useState("");
  const [issuedAt, setIssuedAt] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    setIsSubmitting(true);

    try {
      const res = await fetch("/api/candidates/me/licenses", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          licenseNo,
          issuedBy,
          scope: null,
          issuedAt,
          expiredAt: null,
          // Upload thật (MinIO/presigned URL) chưa nối — placeholder tạm để test luồng thêm CCHN.
          documentUrl: "https://placeholder.local/pending-upload",
        }),
      });

      if (res.ok) {
        setShowForm(false);
        setLicenseNo("");
        setIssuedBy("");
        setIssuedAt("");
        router.refresh();
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="space-y-4">
      {licenses.map((license) => {
        const meta = VERIFY_META[license.verifyStatus] ?? VERIFY_META.Pending;
        const VerifyIcon = meta.icon;

        return (
          <div key={license.id} className="space-y-3 rounded-md border border-line p-3">
            <div className="flex items-center justify-between">
              <span className="text-sm font-medium text-ink">{license.licenseNo}</span>
              <Badge className={`gap-1 ${meta.className}`}>
                <VerifyIcon className="size-3" />
                {t(meta.labelKey)}
              </Badge>
            </div>
            {license.rejectReason && (
              <p className="text-xs text-destructive">{license.rejectReason}</p>
            )}
          </div>
        );
      })}

      {showForm ? (
        <form className="space-y-4" onSubmit={handleSubmit}>
          <div className="space-y-1.5">
            <Label htmlFor="licenseNumber">{t("licenseNumber")}</Label>
            <Input
              id="licenseNumber"
              value={licenseNo}
              onChange={(e) => setLicenseNo(e.target.value)}
              required
            />
          </div>
          <div className="space-y-1.5">
            <Label htmlFor="issuedBy">{t("issuedBy")}</Label>
            <Input
              id="issuedBy"
              value={issuedBy}
              onChange={(e) => setIssuedBy(e.target.value)}
              required
            />
          </div>
          <div className="space-y-1.5">
            <Label htmlFor="issuedAt">{t("issuedAt")}</Label>
            <Input
              id="issuedAt"
              type="date"
              value={issuedAt}
              onChange={(e) => setIssuedAt(e.target.value)}
              required
            />
          </div>
          <Separator />
          <p className="text-xs text-ink-muted">
            Ảnh chứng chỉ sẽ được yêu cầu upload ở đợt sau (đang dùng URL tạm thời).
          </p>
          <Button type="submit" disabled={isSubmitting}>
            {isSubmitting ? "Đang gửi..." : "Thêm CCHN"}
          </Button>
        </form>
      ) : (
        <Button variant="outline" onClick={() => setShowForm(true)}>
          Thêm CCHN mới
        </Button>
      )}
    </div>
  );
}
