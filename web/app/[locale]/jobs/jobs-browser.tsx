"use client";

import { useMemo, useState } from "react";
import { useTranslations } from "next-intl";
import { ShieldCheck, Stethoscope, MapPin, Briefcase } from "lucide-react";
import { Link } from "@/i18n/navigation";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  MOCK_JOBS,
  MOCK_SPECIALTIES,
  MOCK_LOCATIONS,
  MOCK_EMPLOYMENT_TYPES,
} from "@/lib/mock-data";

const ALL = "all";

export function JobsBrowser() {
  const t = useTranslations("jobs");
  const [specialty, setSpecialty] = useState(ALL);
  const [location, setLocation] = useState(ALL);
  const [employmentType, setEmploymentType] = useState(ALL);

  const filtered = useMemo(() => {
    return MOCK_JOBS.filter((job) => {
      if (specialty !== ALL && job.specialty !== specialty) return false;
      if (location !== ALL && job.location !== location) return false;
      if (employmentType !== ALL && job.employmentType !== employmentType)
        return false;
      return true;
    });
  }, [specialty, location, employmentType]);

  const hasActiveFilter =
    specialty !== ALL || location !== ALL || employmentType !== ALL;

  return (
    <div className="grid gap-8 lg:grid-cols-[260px_1fr]">
      <aside className="space-y-6">
        <div className="rounded-lg border border-border bg-paper-raised p-4">
          <div className="mb-4 flex items-center justify-between">
            <h2 className="font-heading text-base font-semibold text-ink">
              {t("filters")}
            </h2>
            {hasActiveFilter && (
              <button
                type="button"
                className="text-xs text-accent-jade underline-offset-2 hover:underline"
                onClick={() => {
                  setSpecialty(ALL);
                  setLocation(ALL);
                  setEmploymentType(ALL);
                }}
              >
                {t("clearFilters")}
              </button>
            )}
          </div>

          <div className="space-y-4">
            <div>
              <label className="mb-1.5 block text-sm font-medium text-ink">
                {t("specialty")}
              </label>
              <Select value={specialty} onValueChange={setSpecialty}>
                <SelectTrigger className="w-full">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value={ALL}>{t("specialty")} — Tất cả</SelectItem>
                  {MOCK_SPECIALTIES.map((s) => (
                    <SelectItem key={s} value={s}>
                      {s}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div>
              <label className="mb-1.5 block text-sm font-medium text-ink">
                {t("location")}
              </label>
              <Select value={location} onValueChange={setLocation}>
                <SelectTrigger className="w-full">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value={ALL}>{t("location")} — Tất cả</SelectItem>
                  {MOCK_LOCATIONS.map((l) => (
                    <SelectItem key={l} value={l}>
                      {l}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div>
              <label className="mb-1.5 block text-sm font-medium text-ink">
                {t("employmentType")}
              </label>
              <Select
                value={employmentType}
                onValueChange={setEmploymentType}
              >
                <SelectTrigger className="w-full">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value={ALL}>
                    {t("employmentType")} — Tất cả
                  </SelectItem>
                  {MOCK_EMPLOYMENT_TYPES.map((e) => (
                    <SelectItem key={e} value={e}>
                      {e}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>
        </div>
      </aside>

      <div>
        <p className="mb-4 text-sm text-ink-muted">
          {t("resultsCount", { count: filtered.length })}
        </p>

        {filtered.length === 0 ? (
          <div className="rounded-lg border border-dashed border-line bg-paper-raised p-10 text-center text-ink-muted">
            {t("noResults")}
          </div>
        ) : (
          <div className="grid gap-4 sm:grid-cols-2">
            {filtered.map((job) => (
              <Link key={job.id} href={`/jobs/${job.id}`}>
                <Card className="h-full transition-shadow hover:shadow-md">
                  <CardHeader>
                    <div className="flex items-start justify-between gap-2">
                      <CardTitle className="text-base leading-snug">
                        {job.title}
                      </CardTitle>
                      {job.requiresLicense && (
                        <Badge
                          variant="outline"
                          className="shrink-0 gap-1 border-accent-jade/40 text-accent-jade"
                        >
                          <ShieldCheck className="size-3" />
                          CCHN
                        </Badge>
                      )}
                    </div>
                    <p className="text-sm text-ink-muted">
                      {job.organizationName}
                    </p>
                  </CardHeader>
                  <CardContent className="flex flex-col gap-2 text-sm text-ink-muted">
                    <span className="inline-flex items-center gap-1.5">
                      <Stethoscope className="size-3.5" />
                      {job.specialty}
                    </span>
                    <span className="inline-flex items-center gap-1.5">
                      <MapPin className="size-3.5" />
                      {job.location}
                    </span>
                    <span className="inline-flex items-center gap-1.5">
                      <Briefcase className="size-3.5" />
                      {job.employmentType}
                    </span>
                    <div className="mt-1 font-medium text-ink">
                      {job.salaryLabel}
                    </div>
                  </CardContent>
                </Card>
              </Link>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
