"use client";

import { useState } from "react";
import { useTranslations } from "next-intl";
import { useRouter } from "@/i18n/navigation";
import { Button } from "@/components/ui/button";

export function DeleteAccountButton() {
  const t = useTranslations("profile");
  const router = useRouter();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [confirming, setConfirming] = useState(false);

  async function handleDelete() {
    if (!confirming) {
      setConfirming(true);
      return;
    }

    setIsSubmitting(true);
    try {
      const res = await fetch("/api/users/me", { method: "DELETE" });
      if (res.ok) {
        router.push("/");
        router.refresh();
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <Button variant="destructive" onClick={handleDelete} disabled={isSubmitting}>
      {confirming ? "Xác nhận xóa tài khoản?" : t("deleteAccount")}
    </Button>
  );
}
