"use client";

import { useState } from "react";
import { useRouter } from "@/i18n/navigation";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import type { ApiCandidateProfile } from "@/lib/candidates";

export function ProfileForm({ profile }: { profile: ApiCandidateProfile | null }) {
  const router = useRouter();

  const [fullName, setFullName] = useState(profile?.fullName ?? "");
  const [headline, setHeadline] = useState(profile?.headline ?? "");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [saved, setSaved] = useState(false);

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    setIsSubmitting(true);
    setSaved(false);

    try {
      const res = await fetch("/api/candidates/me", {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          fullName,
          headline: headline || null,
          summary: profile?.summary ?? null,
          dob: null,
          gender: null,
          address: null,
        }),
      });

      if (res.ok) {
        setSaved(true);
        router.refresh();
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <form className="space-y-4" onSubmit={handleSubmit}>
      <div className="space-y-1.5">
        <Label htmlFor="fullName">Họ và tên</Label>
        <Input
          id="fullName"
          value={fullName}
          onChange={(e) => setFullName(e.target.value)}
          required
        />
      </div>
      <div className="space-y-1.5">
        <Label htmlFor="headline">Chức danh</Label>
        <Input
          id="headline"
          value={headline}
          onChange={(e) => setHeadline(e.target.value)}
        />
      </div>

      {saved && (
        <p className="text-sm text-accent-jade">Đã lưu thay đổi.</p>
      )}

      <Button type="submit" disabled={isSubmitting}>
        {isSubmitting ? "Đang lưu..." : "Lưu thay đổi"}
      </Button>
    </form>
  );
}
