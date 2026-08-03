"use client";

import { useState } from "react";
import { useTranslations } from "next-intl";
import { useRouter } from "@/i18n/navigation";
import { Button } from "@/components/ui/button";

export function ApplyButton({
  jobId,
  isLoggedIn,
}: {
  jobId: string;
  isLoggedIn: boolean;
}) {
  const t = useTranslations("jobs");
  const router = useRouter();

  const [status, setStatus] = useState<"idle" | "submitting" | "success" | "error">("idle");

  async function handleApply() {
    if (!isLoggedIn) {
      router.push(`/auth/login?redirect=/jobs/${jobId}`);
      return;
    }

    setStatus("submitting");

    try {
      const res = await fetch(`/api/jobs/${jobId}/apply`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ coverLetter: null }),
      });

      setStatus(res.ok ? "success" : "error");
    } catch {
      setStatus("error");
    }
  }

  if (status === "success") {
    return <p className="text-sm font-medium text-accent-jade">{t("applySuccess")}</p>;
  }

  return (
    <div className="flex flex-col items-end gap-2">
      <Button
        size="lg"
        className="sm:w-auto"
        onClick={handleApply}
        disabled={status === "submitting"}
      >
        {status === "submitting" ? t("applying") : t("applyNow")}
      </Button>
      {status === "error" && (
        <p className="text-sm text-destructive">{t("applyError")}</p>
      )}
    </div>
  );
}
