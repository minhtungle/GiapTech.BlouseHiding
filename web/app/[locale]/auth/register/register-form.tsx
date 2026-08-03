"use client";

import { useState } from "react";
import { useTranslations } from "next-intl";
import { useRouter } from "@/i18n/navigation";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";

export function RegisterForm() {
  const t = useTranslations("auth");
  const router = useRouter();

  const [role, setRole] = useState<"candidate" | "employer">("candidate");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    setError(null);
    setIsSubmitting(true);

    try {
      const res = await fetch("/api/auth/register", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password, role }),
      });

      if (!res.ok) {
        setError(t("registerError"));
        return;
      }

      router.push(`/auth/verify-otp?email=${encodeURIComponent(email)}`);
    } catch {
      setError(t("registerError"));
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <form className="space-y-4" onSubmit={handleSubmit}>
      <div className="space-y-2">
        <Label>{t("registerTitle")}</Label>
        <RadioGroup
          value={role}
          onValueChange={(value) => setRole(value as "candidate" | "employer")}
          className="grid grid-cols-2 gap-2"
        >
          <Label className="flex items-center gap-2 rounded-md border border-line p-3 text-sm has-[[data-state=checked]]:border-accent-jade has-[[data-state=checked]]:bg-accent-jade/5">
            <RadioGroupItem value="candidate" />
            {t("roleCandidate")}
          </Label>
          <Label className="flex items-center gap-2 rounded-md border border-line p-3 text-sm has-[[data-state=checked]]:border-accent-jade has-[[data-state=checked]]:bg-accent-jade/5">
            <RadioGroupItem value="employer" />
            {t("roleEmployer")}
          </Label>
        </RadioGroup>
      </div>

      <div className="space-y-1.5">
        <Label htmlFor="email">{t("email")}</Label>
        <Input
          id="email"
          type="email"
          autoComplete="email"
          required
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />
      </div>
      <div className="space-y-1.5">
        <Label htmlFor="password">{t("password")}</Label>
        <Input
          id="password"
          type="password"
          autoComplete="new-password"
          required
          minLength={8}
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />
      </div>

      {error && <p className="text-sm text-destructive">{error}</p>}

      <Button type="submit" className="w-full" disabled={isSubmitting}>
        {isSubmitting ? t("verifying") : t("submitRegister")}
      </Button>
    </form>
  );
}
